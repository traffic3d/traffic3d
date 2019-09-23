from abc import ABC, abstractmethod
import socket
import cv2
import os
import tempfile
from datetime import datetime

class ModelGenerator(ABC):
    PORT = 13000
    @abstractmethod
    def __init__(self, port=PORT):
        self.port = port
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
        self.send_data(self.images_path)
        print("tcpConnection established")

    def get_data(self):
        return self.client_socket.recv(1024)

    def send_data(self, data_to_send):
        action_bytes = bytes(str(data_to_send), "ascii")
        self.client_socket.send(action_bytes)

    def receive_image(self):
        data_string = (self.get_data().decode('utf-8'))
        print(data_string)
        img_path = os.path.join(self.images_path, data_string)
        img = cv2.imread(img_path)
        if img is None:
            raise Exception("Image cannot be found, image path may be incorrect.")
        return img

    def send_action(self, action):
        self.send_data(action)
        print("action sent")

    def receive_rewards(self):
        data = (self.get_data().decode('utf-8'))
        data_float = float(data)
        print("rewards received")
        return data_float
