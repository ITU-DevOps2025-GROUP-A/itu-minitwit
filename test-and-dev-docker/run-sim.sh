#!/bin/bash

# Start a new tmux session named 'my_session'
tmux new-session -d -s my_session

# Run the first command in the initial pane
tmux send-keys -t my_session 'bash ./run-sim-helpers/api.sh' C-m

# Split the window vertically (creates a pane on the right)
tmux split-window -h -t my_session

# Run the second command in the new pane
tmux send-keys -t my_session 'bash ./run-sim-helpers/sim.sh' C-m

# Attach to the tmux session
tmux attach -t my_session
