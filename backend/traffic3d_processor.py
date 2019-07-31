#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Wed Apr 10 11:46:42 2019

@author: deepekagarg
"""
import model_generator

import numpy as np
import cv2
import matplotlib
import matplotlib.pyplot as plt

import torch
from torch import *
import torch.nn as nn
import torch.optim as optim
import torch.nn.functional as F
from torch.autograd import Variable
from torch.distributions import Categorical

IMAGES_PATH = "../Traffic3D/Assets/Screenshots"


class Traffic3DProcessor(model_generator.ModelGenerator):
    def __init__(self, images_path):
        super().__init__(images_path)

    def enable(self):
        # Using CUDA GPU
        use_cuda = torch.cuda.is_available()

        FloatTensor = torch.cuda.FloatTensor if use_cuda else torch.FloatTensor
        self.Tensor = FloatTensor

        self.is_ipython = 'inline' in matplotlib.get_backend()
        plt.ion()

        PATHmodel2 = 'emerMFD2.pt'
        # hyperparameters
        self.gamma = 0.99
        self.policy = PG()
        if use_cuda:
            self.policy.cuda()
        optimizer = optim.RMSprop(self.policy.parameters(), lr=0.001)
        eps = np.finfo(np.float32).eps
        for u in torch.arange(1, 501):
            for v in torch.arange(1, 11):
                self.run()
            optimizer.step()
            print("optim step up")
            torch.save(self.policy, PATHmodel2)
            optimizer.zero_grad()
            print("gradients zeroed")
            del self.policy.basic_rewards[:]
            del self.policy.saved_log_probs[:]
            print("things deleted")

    def run(self):
        R = 0
        policy_loss = []
        discounted_rewards = []
        # One episode is made of 100 timesteps, get 100 rewards after an episode.
        for i in torch.arange(1, 11):
            img = self.receive_image()
            img1 = self.prepro(img)
            act = self.select_action(img1)
            self.send_action(act)
            rew = self.receive_rewards()
            self.policy.basic_rewards.append(rew)

        for r in self.policy.basic_rewards[::-1]:
            R = r + self.gamma * R
            discounted_rewards.insert(0, R)
        discounted_rewards = torch.FloatTensor(discounted_rewards)
        discounted_rewards = (discounted_rewards - discounted_rewards.mean()) / (
                discounted_rewards.std() + np.finfo(np.float32).eps)
        for log_prob, reward in zip(self.policy.saved_log_probs, discounted_rewards):
            policy_loss.append(-log_prob * reward)
        policy_loss = torch.cat(policy_loss).sum()
        # As long as you have retain_raph=True, you can do backprop at any time.
        policy_loss.backward(retain_graph=True)
        print("done backprop")

    def select_action(self, state):
        probs1 = self.policy(Variable(state))
        m = Categorical(probs1)
        action = m.sample()
        self.policy.saved_log_probs.append(m.log_prob(action))
        print(action.data[0].tolist())
        return action.data[0].tolist()

    def prepro(self, x):
        x = cv2.resize(x, (100, 100))
        x = x.transpose(2, 0, 1)
        x = np.ascontiguousarray(x, dtype=np.float32) / 255
        x = torch.from_numpy(x)
        return (x.unsqueeze(0).type(self.Tensor))


class PG(nn.Module):
    def __init__(self):
        super(PG, self).__init__()
        self.conv1 = nn.Conv2d(3, 16, kernel_size=5, stride=2)
        self.bn1 = nn.BatchNorm2d(16)
        self.conv2 = nn.Conv2d(16, 32, kernel_size=5, stride=2)
        self.bn2 = nn.BatchNorm2d(32)
        self.conv3 = nn.Conv2d(32, 32, kernel_size=5, stride=2)
        self.bn3 = nn.BatchNorm2d(32)
        self.conv4 = nn.Conv2d(32, 64, kernel_size=5, stride=2)
        self.bn4 = nn.BatchNorm2d(64)
        self.head = nn.Linear(576, 2)

        self.saved_log_probs = []
        self.basic_rewards = []

    def forward(self, x):
        x = F.relu(self.bn1(self.conv1(x)))
        x = F.relu(self.bn2(self.conv2(x)))
        x = F.relu(self.bn3(self.conv3(x)))
        x = F.relu(self.bn4(self.conv4(x)))
        x = self.head(x.view(x.size(0), -1))
        return F.softmax(x, dim=1)


Traffic3DProcessor(IMAGES_PATH)
