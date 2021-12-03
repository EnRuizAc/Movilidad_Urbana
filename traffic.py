from flask import Flask, render_template, request, jsonify
import json, logging, os, atexit
#from model import run_model
from flask_socketio import SocketIO, send

####### Model #######
# Model design
import agentpy as ap

# Visualization
import seaborn as sns
import IPython
import IPython.display
from random import randrange, uniform
import matplotlib
from matplotlib import pyplot as plt
from matplotlib.animation import FuncAnimation

# Connection
import UdpComms as U
import json
import time
#####################

app = Flask(__name__, static_url_path='')
socketio = SocketIO(app)

# On IBM Cloud Cloud Foundry, get the port number from the environment variable PORT
# When running this app on the local machine, default the port to 8000
port = int(os.getenv('PORT', 8000))


#@app.route('/')
#def root():
#    run_model()
#    return jsonify([{"message":"Pruebas Tec, from IBM Cloud!"}])

@socketio.on('connect')
def start_model():
    run_model()

if __name__ == '__main__':
    #app.run(host='0.0.0.0', port=port, debug=True)
    socketio.run(app)


############################################################
# Model

# Create UDP socket to use for sending (and receiving)
socket = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001,
                  enableRX=True, suppressWarnings=True)


class CarBot(ap.Agent):
    
    def setup(self):
        # Inititate agent attributes
        self.grid = self.model.grid
        self.random = self.model.random
        self.typeColor = randrange(0,2)
        self.status = 0
        self.direction = ""

    def set_direction(self, direction):
      self.direction = direction

    def handle_intersection(self, intersection):
        # change direction randomly according to intersection options
        new_direction = randrange(0,2)
        self.direction = intersection.options[new_direction]
        return new_direction

    def find_new_cell(self, road, intersections):
        # get current position
        i,j = self.grid.positions[self]
        new_pos = (i,j)
        new_direction = self.direction
        # check if car is at an intersection
        if (i,j) in intersections:
            new_direction = self.handle_intersection(intersections[(i,j)])
        # move to the right
        if self.direction == "right":
            # check if road is open
            if (i,j+1) in road and (i,j+1) in self.grid.empty and \
                    ((i,j+2) not in road or (i,j+2) in self.grid.empty):
                self.grid.move_by(self, (0, 1))
                new_pos = (i,j+1)
        # move to the left
        elif self.direction == "left":
            # check if road is open
            if (i,j-1) in road and (i,j-1) in self.grid.empty and \
                    ((i,j-2) not in road or (i,j-2) in self.grid.empty):
                self.grid.move_by(self, (0, -1))
                new_pos = (i,j-1)
        # move down
        elif self.direction == "down":
            # check if road is open
            if (i+1,j) in road and (i+1,j) in self.grid.empty and \
                    ((i+2,j) not in road or (i+2,j) in self.grid.empty):
                self.grid.move_by(self, (1, 0))
                new_pos = (i+1,j)
        # move up
        elif self.direction == "up":
            # check if road is open
            if (i-1,j) in road and (i-1,j) in self.grid.empty and \
                    ((i-2,j) not in road or (i-2,j) in self.grid.empty):
                self.grid.move_by(self, (-1, 0))
                new_pos = (i-1,j)

        return new_pos, new_direction


class Cell(ap.Agent):
    def setup(self):
        # Initiate agent attributes
        self.grid = self.model.grid
        self.random = self.model.random
        self.clean = False
        self.typeColor = 4
        self.status = 0

    def find_new_cell(self):
        if self.status == 1:
            num1 = 0
            num2 = 0
            while (num1 == 0 and num2 == 0):
                num1 = randrange(-1, 0)
                num2 = randrange(-1, 0)
            self.grid.move_by(self, (num1, num2))

class Intersection:
  def __init__(self, pos, type, options):
    self.pos = pos
    self.type = type
    self.options = options

class StreetModel(ap.Model):

    def setup_street(self):
      self.street_coords = set()
      for i in range(20):
        self.street_coords.add((9,i))
        self.street_coords.add((10,i))
        self.street_coords.add((i,9))
        self.street_coords.add((i,10))
      
      self.intersections = {}
      self.intersections[(9,9)] = Intersection((9,9),"upper-left",["down","left"])
      self.intersections[(9,10)] = Intersection((9,10),"upper-right",["up","left"])
      self.intersections[(10,9)] = Intersection((10,9),"lower-left",["down","right"])
      self.intersections[(10,10)] = Intersection((10,10),"lower-left",["up","right"])
      #self.cellAgents = ap.AgentList(self, 76, Cell)
      #self.grid.add_agents(self.cellAgents, empty=True, positions=list(self.street_coords))

    def setup_cars(self):
      car_init_coords = [(10,0),(10,0),(0,9),(19,10),(19,10)]
      self.robotAgents = ap.AgentList(self, 5, CarBot)
      self.grid.add_agents(self.robotAgents, positions=car_init_coords, empty=True)
      for a in self.grid.agents:
          i,j = self.grid.positions[a]
          if i == 10:
              a.set_direction("right")
          elif i == 0 and j == 9:
              a.set_direction("down")
          elif i == 19 and j == 10:
              a.set_direction("up")
      #self.robotAgents = ap.AgentList(self, 1, CarBot(direction="down"))
      #self.grid.add_agents(self.robotAgents, positions=[(0,9)], empty=True)

    def setup_agents_status(self):
        # Build initial status dictionary
        self.agents_status = {"Cars": []}
        for a in self.grid.agents:
            start_pos =  self.grid.positions[a]
            new_car_dict = {
                "CarId": a.id,
                "Position":{
                    "x": start_pos[0],
                    "y": 0,
                    "z": start_pos[1]
                },
                "Direction": a.direction
            }
            self.agents_status["Cars"].append(new_car_dict)

        # Send initial status
        status_json = json.dumps(self.agents_status)
        socket.SendData(status_json)
    
    def setup(self):
        #Parameters
        h = self.p.height 
        w = self.p.width
        # d = self.d = int(self.p.density * (w * h))
        n = self.n = self.p.n_agents

        self.num_moves = 0

        #Create grid agents
        self.grid = ap.Grid(self, (w, h), track_empty=True, check_border=True)
        #self.robotAgents = ap.AgentList(self, n, CarBot)
        self.setup_street()
        #self.cellAgents = ap.AgentList(self, d, Cell)
        #self.grid.add_agents(self.cellAgents, empty=True, random=True)
        #self.grid.add_agents(self.robotAgents, positions = None, empty=True, random=False)
        self.setup_cars()

        self.setup_agents_status()

    #def update(self):
    #    #move unhappy people to new location
    #    self.allRobots = self.robotAgents.select(self.robotAgents.id > 0)
    #    #self.allCells = self.cellAgents.select(self.cellAgents.typeColor == 1)
#
    #    #Stop simulation if no fire is left
    #    # if self.d == 0:
    #    #    self.stop()

    def step(self):
        #self.allRobots.find_new_cell(self.street_coords)
        #for a in self.allRobots:
          #if a.type == "Car":
            #key = "Car (Obj " + str(a.id) + ")"
            #print(a, ":", a.id)
          #print(self.grid.positions[a])
        i = 0
        for a in self.grid.agents:
            new_pos, new_direction = a.find_new_cell(self.street_coords, self.intersections)
            self.agents_status["Cars"][i]["Position"]["x"] = new_pos[0]
            self.agents_status["Cars"][i]["Position"]["z"] = new_pos[1]
            self.agents_status["Cars"][i]["Direction"] = new_direction
            i += 1

        # Send new status
        status_json = json.dumps(self.agents_status)
        socket.SendData(status_json)
        print(self.agents_status)
        ####### Socket #######
        send('message', status_json)
        
        self.num_moves += self.p.n_agents# print(self.num_moves)
        time.sleep(0.2)


parameters = {
    'n_agents': 1,
    # 'density': 0.3,
    'height': 20,
    'width': 20,
    'steps': 20
}

def animation_plot(model, ax):
    gridPosition = model.grid.attr_grid('typeColor')
    color_dict = {0:'#FFFF00', 1:'#0000FF', 2:'#FFA500', 3:'#b3b3b3', 4: '#808080', None:'#ffffff'}
    ap.gridplot(gridPosition, ax=ax, color_dict=color_dict, convert=True)
    total = model.p.height = model.p.width
    # percent = (total -model.d) * 100 / (total)
    ax.set_title(f"Movilidad Urbana \n Tiempo-Paso: {model.t}, # de Movimientos: {model.num_moves}")

def run_model():
    fig, ax = plt.subplots()
    model = StreetModel(parameters)
    animation = ap.animate(model, fig, ax, animation_plot)
    IPython.display.HTML(animation.to_jshtml())