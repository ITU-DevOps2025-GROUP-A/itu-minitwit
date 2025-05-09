---
title: ITU DevOps Project Report
subtitle: ITU DevOps 2025 Group a
author:
- "Christoffer Grünberg <gryn@itu.dk>"
- "Rasmus Rosenmejer Larsen <rarl@itu.dk>"
- "Mathias Labori Olsen <mlao@itu.dk>"
- "Anthon Castillo Hertzum <acah@itu.dk>"
- "Jacques Puvis de Chavannes <japu@itu.dk>"
numbersections: true
header-includes:
  - \usepackage{caption}
  - \usepackage{graphicx}
---
\begin{center}
\includegraphics[width=0.5\textwidth]{images/icon1.png}
\end{center}

\pagebreak

\tableofcontents

\pagebreak

# System
* programmed in C# .NET 9 with ASP.NET as web framework and Blazor frontend.
* Containerised with docker.
* Onion architecture for code structure
* dependencies:
  * API
  * PostgreSQL
  * Digital Ocean
  * Prometheus
  * Grafana
# Process
For our entire developing process we've used trunk-based development with each feature being developed in a separate branch. 
We use GitHub actions for CI/CD and GitHub issues for task management. So you have your standard workflows for building, testing and deploying the code.
On each pull request to the main branch, we run first run the 'changes-to-pr-to-main' that checks if the pull request has a label followed by 'commit-pr-to-main'
which runs a handful of jobs:
* check-for-warnings
* build
* test
* run-simulation
* sonarqube-analysis

These jobs are there to ensure that the codebase still works as intended on the branch that the developer has worked on.
The important note is that the run-simulation could have http requests that could time out, however, we ensured that if
there was only a couple of timouts we could deduce that the codebase still worked as intended. This was primarily to confirm
that if we had 10's or 100's of timeouts, we could be sure that the codebase was broken.

'***add section about what and how we monitor here'

'***add section about what and how we log here'

'***add section about our security assessment results here'

Regarding the scaling of our application, we are in the transition of moving from docker compose to docker swarm. However, we are using docker compose that composes
a api and minitwit dockerfile. Our intentions are to set up a declarative IaC using Terraform and with that set up a docker swarm cluster that can handle the scaling of our application.
Unfortunately as of now, we haven't fully integrated this structure because of some complications with the implementation.

sticky-notes:
* trunk-based development with feature branches
* GitHub actions for CI/CD
* GitHub issues for task management

# Reflections

## Evolution
Throughout the project there has been a division in the group between developing the new implementations to the web app
and operating these feature with CI/CD pipelines. 
* Split between 'Dev' and 'Ops'.
* Technical dept

## Refactoring

When rewriting the code to C# and adhering to the 'minitwit_sim_api.py' from session3, we weren't thorough enough when analyzing the specs.
This resulted in us pushing code to production, which 'seemingly' followed specs from the aforementioned file.
After long contemplation on why it didn't work, we took a step back and properly analyzed the api specs.
Lesson: Do it right the first time.


## Operation

Our VM crashed due to extensive (and redundant) logging. We logged to console, to docker logs and to files. This resulted in the logs using all the storage in the VM
What could be done, is to make sure only to log once and automatically delete old logs which are not needed anymore


* CPU overload on our droplet (VM) on Digital Ocean

We experienced CPU overload on our droplet (VM) on Digital ocean. This resulted in the droplet crashing.
Unfortunately, we have no idea why this is happening, even after extensive diagnosis
## Maintenance
