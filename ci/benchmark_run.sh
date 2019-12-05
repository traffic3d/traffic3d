#!/usr/bin/env bash
set -e
set -x
echo "Running batch file"
${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $(pwd)/Traffic3D \
  -executeMethod Benchmark.RunBenchmark \
  -CacheServerIPAddress 172.17.0.1:8126 \
  -logFile \
  -batchmode | ts '[%Y-%m-%d %H:%M:%S]'
