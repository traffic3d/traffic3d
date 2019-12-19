#!/usr/bin/env bash

set -e
set -x

echo "Testing for $TEST_PLATFORM"

touch $(pwd)/$TEST_PLATFORM-results.xml

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
  -projectPath $(pwd)/Traffic3D \
  -runTests \
  -testCategory "Tests" \
  -testPlatform $TEST_PLATFORM \
  -testResults $(pwd)/$TEST_PLATFORM-results.xml \
  -CacheServerIPAddress 172.17.0.1:8126 \
  -logFile \
  -batchmode | ts '[%Y-%m-%d %H:%M:%S]'

UNITY_EXIT_CODE=$?

set +x
echo "$(<$(pwd)/$TEST_PLATFORM-results.xml)"
set -x

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

cat $(pwd)/$TEST_PLATFORM-results.xml | grep test-run | grep Passed
exit $UNITY_TEST_EXIT_CODE
