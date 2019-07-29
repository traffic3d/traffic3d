from abc import ABC, abstractmethod
import socket
import cv2
import os


class Traffic3DInterface(ABC):
    @abstractmethod
    def __init__(self, port, images_path):
        self.port = port
        self.images_path = images_path
        self.client_socket = None
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

    def get_data(self):
        return self.client_socket.recv(1024)

    def receive_image(self):
        data_string = (self.get_data().decode('utf-8'))
        print(data_string)
        img_path = os.path.join(self.images_path, data_string)
        img = cv2.imread(img_path)
        return img

    def send_action(self, action):
        action_bytes = bytes(str(action), "ascii")
        print(action)
        self.client_socket.send(action_bytes)
        print("action sent")

    def receive_rewards(self):
        data = (self.get_data().decode('utf-8'))
        data_float = float(data)
        print("rewards received")
        return data_float
