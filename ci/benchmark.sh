#!/usr/bin/env bash
echo "Running Benchmark"
chmod +x ./ci/benchmark_run.sh
/usr/local/bin/multitime -b ./ci/benchmark_run.sh -n 30
