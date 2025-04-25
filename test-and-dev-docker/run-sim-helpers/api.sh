#!/bin/bash
clear
. setup-dev_test.sh

#Wait for api to have been started
until curl -k --silent http://localhost:5000/health > /dev/null; do
  sleep 1
done

docker logs -f minitwitapi
