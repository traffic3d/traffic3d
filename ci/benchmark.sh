#!/usr/bin/sh
set -e
set -x
echo "Running Benchmark"
/usr/local/bin/multitime -n 5 sh benchmarkrun.sh