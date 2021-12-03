import socket
import random
from time import sleep

def RandomPosition():
    s = socket.socket()

    # connect to the server on local computer
    s.connect(('192.168.0.20', 3000))
    s.send(
        (
            str(random.uniform(0,1)) + "," +
            str(random.uniform(0,1)) + "," +
            str(random.uniform(0,1)) + ","
        ).encode()
    )
    s.close()

for i in range(500):
    RandomPosition()
    sleep(1)