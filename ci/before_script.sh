#!/usr/bin/env bash

set -e
set -x
mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
set +x
echo 'Writing $UNITY_LICENSE to license file /root/.local/share/unity3d/Unity/Unity_lic.ulf'
cp $UNITY_LICENSE /root/.local/share/unity3d/Unity/Unity_lic.ulf