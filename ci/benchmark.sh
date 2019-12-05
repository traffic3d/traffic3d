#!/usr/bin/env bash
echo "Running Benchmark"
chmod +x ./ci/benchmark.sh
/usr/local/bin/multitime -b ./ci/benchmark.sh -n 5
