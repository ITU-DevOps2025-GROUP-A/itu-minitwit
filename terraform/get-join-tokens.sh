#!/usr/bin/env bash

# wait 10 seconds for SSH to be ready
sleep 10

# Exit if any of the intermediate steps fail
set -e

# Extract input variables
eval "$(jq -r '@sh "HOST=\(.host)"')"

# Get worker join token
WORKER=$(ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null root@$HOST docker swarm join-token worker -q)
MANAGER=$(ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null root@$HOST docker swarm join-token manager -q)

# Pass back a JSON object
jq -n --arg worker $WORKER --arg manager $MANAGER '{"worker":$worker,"manager":$manager}'