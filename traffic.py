from flask import Flask, render_template, request, jsonify
import json, logging, os, atexit
from model import run_model

app = Flask(__name__, static_url_path='')

# On IBM Cloud Cloud Foundry, get the port number from the environment variable PORT
# When running this app on the local machine, default the port to 8000
port = int(os.getenv('PORT', 8000))


@app.route('/')
def root():
    steps_data = run_model()
    #os.system('python model.py')
    return jsonify(steps_data)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=port, debug=True)