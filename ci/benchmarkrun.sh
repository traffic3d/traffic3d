#!/usr/bin/env sh
set -e
set -x
echo "Running Unity"
${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity}  -projectPath $(pwd)/Traffic3D  -executeMethod CustomCommandLineArguments.Run  -OpenScene "Scenes/DayDemo.unity"  -RunBenchmark true  -logFile  -batchmode
