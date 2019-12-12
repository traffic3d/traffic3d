#!/usr/bin/env bash

set -e
set -x

chmod +x ./ci/benchmarkrun.sh

echo "Running Benchmark"
multitime -n 15 ./ci//benchmarkrun.sh
