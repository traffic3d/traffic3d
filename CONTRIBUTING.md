# Contributing to the Traffic 3D project

## Technologies

This project is based on the [Unity 3d games engine](https://unity3d.com/unity).
The AI is written in [Python3](https://www.python.org/) with [PyTorch](https://pytorch.org/).

Traffic3D is tested and supported on 64-bit Windows and Linux.

## Getting started

To get the Traffic3D source code, from the command line, run:

```sh
git clone git@gitlab.com:traffic3d/traffic3d.git
cd traffic3d
```

## Installation and set-up

The project runs on Unity 2018.3.11f1 and up.
Download the latest of Unity from the following link: [https://unity3d.com/get-unity/download/](https://unity3d.com/get-unity/download/)
Or download Unity 2018.3.11f1 from the following link: [https://unity3d.com/get-unity/download/archive](https://unity3d.com/get-unity/download/archive)

Use a preferred C# IDE or download Visual Studio using the following link: [https://visualstudio.microsoft.com/vs/](https://visualstudio.microsoft.com/vs/)

## Density Measurements

On new scenes, the density measure points need setting up to allow for correct density per km calculations.
To set these points up, on the paths, 
select the node that needs to be used as the density measure point (normally just after exiting a junction) and add the `DensityMeasurePoint.cs` script to the node.
Then add a `BoxCollider` to the node, check `Is Trigger` and resize where vehicles on that path will pass through the box.

## Further documentation

Detailed documentation, including how to use the Unity editor, how to extend Traffic3D with new assets and scenes, can be found in the [Traffic3D documentation](https://traffic3d.org).
