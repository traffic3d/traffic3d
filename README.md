# Traffic 3D

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.3460497.svg)](https://doi.org/10.5281/zenodo.3460497)
[![pipeline status](https://gitlab.com/traffic3d/traffic3d/badges/develop/pipeline.svg)](https://gitlab.com/traffic3d/traffic3d/commits/develop)

Traffic3D is a new traffic simulation paradigm, built to push forward research in human-like learning (for example, based on photo-realistic visual input).
It provides a fast, cheap and scalable proxy for real-world traffic environments, based on the [Unity 3d](https://unity3d.com/unity) platform.
This implies effective simulation of diverse and dynamic 3D-road traffic scenarios, closely mimicking real-world traffic characteristics such as faithful simulation of individual vehicle behaviour, their precise physics of movement and photo-realism.
Traffic3D can facilitate research across multiple domains, including reinforcement learning, object detection and segmentation, unsupervised representation learning and visual question answering.

For more details please see [https://traffic3d.org](https://traffic3d.org).

## Bugs and feature requests

Please report issues via the [issue tracker](https://gitlab.com/traffic3d/traffic3d/issues).

## License

This software is licensed under the [Mozilla Public License Version 2.0](/LICENSE).
Copies of the license can also be obtained [directly from Mozilla](https://mozilla.org/MPL/2.0/).

## Getting Started with Development

Please read [CONTRIBUTING.md](/CONTRIBUTING.md) before you start working on this repository.

## CLI Options

Traffic3D has custom command line options using following flag:
`-executeMethod CustomCommandLineArguments.Run`

The custom options are:

*  `-JSONConfigFile "filename.json"` - Import settings from a JSON file (see below for config details)
*  `-OpenScene "Scenes/sceneName.unity"` - Open a scene
*  `-RunHeadless true|false` - Run in headless mode

### Examples

```
# Using all custom options.
${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $(pwd)/Traffic3D \
  -testPlatform playmode \
  -testResults $(pwd)/playmode-results.xml \
  -CacheServerIPAddress 172.17.0.1:8126 \
  -executeMethod CustomCommandLineArguments.Run \
  -JSONConfigFile "config.json" \
  -OpenScene "Scenes/NightDemo.unity" \
  -RunHeadless true \
  -logFile \
  -batchmode
```

## Config

Configuration is used to bulk import initialisation values into the Traffic3D system.
Normally the values are direct mappings of the values that can be changed within the Unity UI.
For example the `vehicleFactoryConfig` object within the config example below allows us to pass data directly to the specified fields.
e.g. The `highRangeRespawnTime` field in the config maps to the `highRangeRespawnTime` field in the game object.

### Examples

See [test_config.json](/Traffic3D/Assets/Tests/TestFiles/test_config.json) for an example of how to structure the file.

## Citing the Traffi3D software

Please see the [CITATION](/CITATION) file.

## Papers that use Traffic3D

Garg, D., Chli, M. and Vogiatzis, G., 2019. [A Deep Reinforcement Learning Agent for Traffic Intersection Control Optimization.](http://maria-chli.org/ITSC19deep.html)
In Proceedings of the IEEE Intelligent Transportation Systems Conference (ITSC) IEEE.

Garg, D., Chli, M. and Vogiatzis, G., 2018, September. [Deep reinforcement learning for autonomous traffic light control.](http://www.george-vogiatzis.org/publications/ICITE2018.pdf)
In 2018 3rd IEEE International Conference on Intelligent Transportation Engineering (ICITE) (pp. 214-218). IEEE.

## Bibliography

Garg, D., Chli, M. and Vogiatzis, G., 2019, June. Traffic3D: A Rich 3D-Traffic Environment to Train Intelligent Agents. In International Conference on Computational Science (pp. 749-755). Springer, Cham.

Garg, D., Chli, M. and Vogiatzis, G., 2019, May. Traffic3D: A New Traffic Simulation Paradigm. In Proceedings of the 18th International Conference on Autonomous Agents and MultiAgent Systems (pp. 2354-2356). International Foundation for Autonomous Agents and Multiagent Systems.
