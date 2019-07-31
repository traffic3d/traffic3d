# Contributing to the Traffic 3D project

## Technologies

This project is based on the [Unity 3d games engine](https://unity3d.com/unity).

## Getting started

### Clone this repository

From the command line, run:

```sh
git clone git@gitlab.com:beautifulcanoe/developers/traffic3d.git
cd traffic3d
```

### Installation and set-up

The project runs on Unity 2017.2.0f1 and up.
Download the latest of Unity from the following link: [https://unity3d.com/get-unity/download/](https://unity3d.com/get-unity/download/)
Or download Unity 2017.2.0f1 from the following link: [https://unity3d.com/get-unity/download/archive](https://unity3d.com/get-unity/download/archive)

Use a preferred C# IDE or download Visual Studio using the following link: [https://visualstudio.microsoft.com/vs/](https://visualstudio.microsoft.com/vs/)

## Working with Unity

### Open the project

Open Unity and press the open button as shown below.

![Open Project Image](./docs/OpenProject.png)

Navigate to the location of the project select the folder that is the parent folder of both the `Assets` and `ProjectSettings` folder.

### Quick Unity Overview

#### Project Window

All `Assets` can be access in this area.
This includes scripts and models for the simulation.

![Project Window Image](./docs/ProjectWindow.png)

#### Hierarchy Window

The scenes `assets` are listed here, each object may have a child object depending on the `Asset`.

![Hierarchy Window Image](./docs/HierarchyWindow.png)

#### Scene Window

Displays the scene and objects within that scene.
Moving around the scene can be done by holding right-click, turning the mouse and using `W` (forward) `S` (backward) `A` (left) and `D` (right).

![Scene Window Image](./docs/SceneWindow.png)

#### Inspector Window

Once an object has been clicked from either the scene or the hierarchy window, the **Inspector** window will show all the properties of that object.

![Inspector Window Image](./docs/InspectorWindow.png)

### Open a scene

In the navigation bar click on **File > Open Scene** and then click on the scene to open.
Scene files have the extension `.unity`.
The scene should then open into the **Scene** window.

![Open Scene Image](./docs/OpenScene.png)

### Edit Properties

Navigate to the **Hierarchy** window and click on the object that needs to have its values changed.
As an example, car has been selected below.

![Select Car Image](./docs/SelectCar.png)

The **inspector** window then has the object’s properties which can be changed by clicking on the field and typing in the new value.
For fields with an object as a value, click on the circle icon to the right of the field and select the new object for the field.

![Select Car Image](./docs/EditCar.png)

## Working with the Python3 model generator

This code comes with a model generator, which generates stochastic events for the simulation.
The model generator is written in Python3 and can be found in the [backend](/backend) directory.

To use it, first install the requirements in a [Python virtual environment](https://docs.python-guide.org/dev/virtualenvs/).

```sh
sudo apt-get install python-virtualenv
cd backend
virtualenv --python=/usr/bin/python3.7 venv
. venv/bin/activate
pip install -r requirements.txt
```

If **Windows** is being used then use the following link to download **PyTorch**:
[https://pytorch.org/](https://pytorch.org/)

Otherwise run the following command:

```sh
pip install torch==1.1.0
```

Then run the code, which will listen for a socket from the Unity application:

```sh
$ python traffic3d_processor.py
waiting for tcpConnection
```

### Using PyCharm

[PyCharm](https://www.jetbrains.com/pycharm/) is an IDE for Python can be used to setup the backend easily.
It automatically installs the virtual environment.

1. Open up the `backend` folder as a project.
1. In settings `Ctrl+Alt+S`, `project: backend` > `project interpreter`, press the cog and press `Add...`
1. Press `Ok` and `Apply`
1. By using the `Terminal` in the bottom left of the screen, install requirements.txt and torch.
1. The Terminal can then be used to run `python traffic3d_processor.py`.

PyCharm can also be used to debug the script by using breakpoints if needed.

More information about PyCharm can be found here: [https://www.jetbrains.com/help/pycharm/meet-pycharm.html](https://www.jetbrains.com/help/pycharm/meet-pycharm.html)

## Writing a custom model generator

This code comes with a model generator, which generates stochastic events for the simulation.
To use your own model generator, you need to extend the `ModelGenerator` class, which can be found in the [backend/model_generator](/backend/model_generator) file:

```python
import model_generator

class Traffic3DProcessor(model_generator.ModelGenerator):
```

Next, add the constructor and call the constructor in the superclass:

```python
def __init__(self, images_path):
    super().__init__(images_path)
```

The constructor can have as many or as little parameters as needed but the `super().__init__` function needs to have the `image path` of the screenshots that are generated by Unity (normally `../Traffic3D/Assets/Screenshots`).

`enable()` is an abstract method which needs to be implemented into the new class.
It is called once the socket is setup and a connection has been made between Unity and the model generator.

Within the `ModelGenerator` class there are multiple methods that interact with the Traffic Simulation

* `receive_image()` - used when the script needs to grab a screen shot image from the system. This method blocks the main thread.
* `send_action(action)` - used to send the action to the simulation, usually a number to the lights that need changing.
* `receive_rewards()` - used to receive the rewards from the simulation. This method blocks the main thread.

At the end of the script, be sure to create an instance of the class:

```python
Traffic3DProcessor(IMAGES_PATH)
```

To see an example of this look at the [Traffic3DProcessor](/backend/traffic3d_processor.py) within the `backend` folder.

## Testing

### Creating or Editing Tests

For Unity, there are two testing methods; *Play Mode* and *Edit Mode*.
Edit mode tests are in the following folder:

```sh
/Assets/Scripts/Editor/EditModeTests/
```

Play mode tests are in the following folder:

```sh
/Assets/Tests/
```

To create a test script, navigate to the corresponding folder and right-click in the project window (normally at the bottom of the screen).
Go to **Create > Testing > (Test Mode) Test C# Script**.
This will create script in that directory and simply double click to edit the script.

## Running Tests

Tests can be run within the Unity UI directly.

1. Click **Window** on the top navigation bar.
1. Click **Test Runner**.
1. Click on **PlayMode** and then **Run All**.
1. Click on **EditMode** and then **Run All**.

## Building the Project

It is possible to quickly build the project from the UI if needed.
If a preview is needed, click on the Play button at the top of the screen.
Make sure to un-click the **Play** button if any more edits are needed,
when the play button is selected all changes during that time are reverted.

For a full build, navigate to the top bar and click **File > Build & Run**,
select the folder for the build.
This should create a `.exe` and will automatically execute the `.exe` to play the simulation.
