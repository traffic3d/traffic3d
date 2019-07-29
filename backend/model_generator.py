#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Wed Apr 10 11:46:42 2019

@author: deepekagarg
"""

import socket
import numpy as np
import cv2
import os
import matplotlib
import matplotlib.pyplot as plt


import torch
from torch import *
import torch.nn as nn
import torch.optim as optim
import torch.nn.functional as F
from torch.autograd import Variable
from torch.distributions import Categorical

#Using CUDA GPU
use_cuda = torch.cuda.is_available()

FloatTensor = torch.cuda.FloatTensor if use_cuda else torch.FloatTensor
LongTensor = torch.cuda.LongTensor if use_cuda else torch.LongTensor
ByteTensor = torch.cuda.ByteTensor if use_cuda else torch.ByteTensor
Tensor = FloatTensor

#if use_cuda:
#    net.cuda()
#    net = torch.nn.DataParallel(net, device_ids=range(torch.cuda.device_count()))
#    cudnn.benchmark = True

is_ipython = 'inline' in matplotlib.get_backend()
plt.ion()

#path for pulling the image
paths = '/Users/deepekagarg/Downloads/MFDemer/'
filePath = os.path.abspath(paths)
filePathWithSlash = filePath + "/"

PATHmodel = '/media/gargd/Fast/emermodeltrained.pt'

PATHmodel1 = '/media/gargd/Fast/emerMFD.pt'

PATHmodel2 = '/media/gargd/Fast/emerMFD2.pt'
#hyperparameters
gamma = 0.99

#setting up the socket
ss = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
ss.bind(("0.0.0.0", 13336))
ss.listen()
print("waiting for tcpConnection")
(clientsocket, address) = ss.accept()
print("tcpConnection established")


def receiveImage():
    Data = clientsocket.recv(1024)
    data = (Data.decode('utf-8'))
    print(data)
    filenameWithPath = os.path.join(filePathWithSlash, data)
    img = cv2.imread(filenameWithPath)
    return(img)

def prepro(x):
    #x = x[310:490, 20:600]
    x = cv2.resize(x,(100,100))
    x = x.transpose(2,0,1)
    x = np.ascontiguousarray(x, dtype=np.float32)/255
    x = torch.from_numpy(x)
    return(x.unsqueeze(0).type(Tensor))

class PG(nn.Module):
    def __init__(self):
        super(PG, self).__init__()
        self.conv1 = nn.Conv2d(3,16, kernel_size=5, stride=2)
        self.bn1 = nn.BatchNorm2d(16)
        self.conv2 = nn.Conv2d(16,32, kernel_size=5, stride=2)
        self.bn2 = nn.BatchNorm2d(32)
        self.conv3 = nn.Conv2d(32,32, kernel_size=5, stride=2)
        self.bn3 = nn.BatchNorm2d(32)
        self.conv4 = nn.Conv2d(32,64, kernel_size=5, stride=2)
        self.bn4 = nn.BatchNorm2d(64)
        self.head = nn.Linear(576,2)

        self.saved_log_probs = []
        self.basic_rewards = []

    def forward(self, x):
        x = F.relu(self.bn1(self.conv1(x)))
        x = F.relu(self.bn2(self.conv2(x)))
        x = F.relu(self.bn3(self.conv3(x)))
        x = F.relu(self.bn4(self.conv4(x)))
        x = self.head(x.view(x.size(0), -1))
        return F.softmax(x, dim=1)

#call the neural network
policy = PG()
#policy = torch.load(PATHmodel1)
if use_cuda:
    policy.cuda()
optimizer = optim.RMSprop(policy.parameters(), lr = 0.001)
eps = np.finfo(np.float32).eps



def select_action(state):
    probs1 = policy(Variable(state))
    m = Categorical(probs1)
    action = m.sample()
    policy.saved_log_probs.append(m.log_prob(action))
    print(action.data[0])
    return action.data[0]

def send_action(y):
    y = bytes(str(y), "ascii")
    print(y)
    clientsocket.send(y)
    print("action sent")

def receive_rewards():
    Data1 = clientsocket.recv(1024)
    data1 = (Data1.decode('utf-8'))
    dataa = float(data1)
    print("rewards received")
    return dataa

def run():
    R = 0
    policy_loss = []
    discounted_rewards = []
    for i in torch.arange(1, 11):     #one episode is made of 100 timesteps, get 100 rewards after an episode
        img = receiveImage()
        img1 = prepro(img)
        act = select_action(img1)
        send_action(act)
        rew = receive_rewards()
        policy.basic_rewards.append(rew)

    for r in policy.basic_rewards[::-1]:
        R = r + gamma * R
        discounted_rewards.insert(0,R)
    discounted_rewards = torch.FloatTensor(discounted_rewards)
    discounted_rewards = (discounted_rewards - discounted_rewards.mean()) / (discounted_rewards.std() + np.finfo(np.float32).eps)
    #return ds_rewards
    #return policy.rewards
    for log_prob, reward in zip(policy.saved_log_probs, discounted_rewards):
        policy_loss.append(-log_prob * reward)
    #optimizer.zero_grad()
    policy_loss = torch.cat(policy_loss).sum()
    policy_loss.backward(retain_graph=True) #as lon as you have retain_raph=True, you can do backprop at any time at all..
    print("done backprop")

for u in torch.arange(1, 501):
    for v in torch.arange(1, 11):
        run()
    optimizer.step()
    print("optim step up")
    torch.save(policy, PATHmodel2)
    optimizer.zero_grad()
    print("gradients zeroed")
    del policy.basic_rewards[:]
    del policy.saved_log_probs[:]
    print("things deleted")
