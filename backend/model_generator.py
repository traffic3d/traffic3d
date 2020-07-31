from abc import ABC, abstractmethod
import socket
import cv2
import os
import tempfile
import json
from datetime import datetime

class ModelGenerator(ABC):
    PORT = 13000
    @abstractmethod
    def __init__(self, port=PORT):
        self.port = port
        self.max_number_of_junction_states = 0
        self.client_socket = None
        self.images_path = os.path.join(tempfile.gettempdir(), "Traffic3D_Screenshots", datetime.now().strftime("%Y-%m-%d_%H_%M_%S_%f"))
        os.makedirs(self.images_path, exist_ok=True)
        print("Screenshots are located at: " + self.images_path)
        self.setup_socket()
        self.enable()

    @classmethod
    @abstractmethod
    def enable(cls):
        pass

    def setup_socket(self):
        ss = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        ss.bind(("0.0.0.0", self.port))
        ss.listen()
        print("waiting for tcpConnection")
        (self.client_socket, address) = ss.accept()
        print("tcpConnection established")
        self.send_data(self.images_path)
        self.max_number_of_junction_states = int(self.get_data().decode('utf-8'))
        if self.max_number_of_junction_states == 0:
            raise ValueError("The Max Number of Junction States is 0. It is possible that Traffic3D "
                             "never sent the number in the first place or there are no Junction States in Traffic3D.")
        print("Max Junction State Size: " + str(self.max_number_of_junction_states))

    def get_data(self):
        return self.client_socket.recv(1024)

    def send_data(self, data_to_send):
        action_bytes = bytes(str(data_to_send), "ascii")
        self.client_socket.send(action_bytes)

    def receive_images(self):
        data_string = (self.get_data().decode('utf-8'))
        print(data_string)
        screenshots = json.loads(data_string)
        screenshots = screenshots["screenshots"]
        imgs = {}
        for item in screenshots:
            img_path = os.path.join(self.images_path, item["screenshotPath"])
            imgs[item["junctionId"]] = cv2.imread(img_path)
        return imgs

    def send_action(self, action):
        self.send_data(action)
        print("action sent")

    def receive_rewards(self):
        data = (self.get_data().decode('utf-8'))
        data_float = float(data)
        print("rewards received")
        return data_float
