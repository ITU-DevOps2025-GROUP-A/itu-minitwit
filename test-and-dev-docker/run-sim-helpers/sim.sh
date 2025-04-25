#!/bin/bash
clear

#Wait for api to have been started
echo "waiting for api to start"
until curl -k --silent http://localhost:5000/health > /dev/null; do
  printf '.'
  sleep 1
done

echo "api, is ready, preparing to starting simulation"
sleep 2
python ./run-sim-helpers/minitwit_simulator.py "http://localhost:5000/api"
