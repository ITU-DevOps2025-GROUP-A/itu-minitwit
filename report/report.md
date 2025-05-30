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
## Current Deployment
![Current deployment of _itu_minitwit_ ](images/CurrentDeployment.png)

\pagebreak

## Swarm Deployment
![Docker swarm deployment of _itu_minitwit_ ](images/DockerDeployment.png)

\pagebreak

The system is in the process of being converted to use Docker Swarm instead of a docker network. 
This was to increase the crash resilience by replicating the services, so that if the application ever did crash, it would still function and be up and running. Currently, the Swarm can be set up by using terraform, but it only deploys an empty Swarm. 
The idea was to populate the Swarm as a step in the deployment chain, where we would SSH into the manager node and then deploy it from there. 
However, there were complications with the Prometheus config file. It can't mount to the swarm in the same way as it can to a single machine with docker compose.

One thing missing in our Terraform configuration is to correctly open for the firewalls.
We are currently not opening the correct ports for the internal DNS network to route between docker services.

## Used Technologies
* Programmed in C# .NET 9 with ASP.NET as web framework and Blazor frontend.
* Containerized with Docker.
* Onion architecture for code structure
* Dependencies:
  * EFCore
  * PostgreSQL
  * Digital Ocean
  * Docker
  * Prometheus
  * Grafana
  * Serilog
  * Seq
  * Terraform

## Interactions of subsystems
Below is a sequence diagram, showing the flow of a user posting a message.
It shows the sequence of calls for a message being posted on our application through the web.
The simulator calls the API directly, bypassing the web part of the sequence diagram.

![alt text](images/sequence-user.png)

## Current state of System
As seen on the image below, the reliability and maintainability score is as high as can be. The same goes for code-duplication.
For explanation of security, see the security section in the process overview.

![Static analysis on SonarQube](images/sonarqube.png)

# Process

## Provisioning
Vagrant was used to provision virtual machines, specified with a Vagrantfile. In the Vagrantfile, you're able to provision several virtual machines 
at the same time (fx the web app and the database), define and install their dependencies. This allows for an easy, streamlined way to always provision
VM's without having to rely on configuration a specific user interface from various VM providers. This means that we are able to use the Vagrantfile with several providers,
only having to change the vm.provider.

We are changing from Vagrant to Terraform. Terraform is an Infrastructure as Code. 
This allows us to define how we want our infrastructure to look, 
and let Terraform figure out how to get there.
Terraform is currently configured to provision DigitalOcean, 
having 1 manager and 2 workers connected in a Docker Swarm.

We have a folder called remote_files which contains files that need to be pushed to the server. 
This folder contains:

* Deploy script
* Docker compose
* Prometheus configuration

The folder also contains the Grafana Dashboards. These Were added during report writing and are currently not being used.
The dashboards are also stored in a volume on the server for persistence.  

## Workflow
For our entire developing process, we've used trunk-based development with each feature being developed in a separate branch. 
We use GitHub actions for CI/CD and GitHub issues for task management. Our workflows include building, testing and deploying the code.
On each pull request to the main branch, we first run the 'changes-to-pr-to-main' that checks if the pull request has a label, followed by 'commit-pr-to-main'
which runs a handful of jobs:

* Check-for-warnings
* Build
* Test
* Run-simulation test script
* Sonarqube-analysis

These jobs are there to ensure that the codebase still works as intended on the branch that the developer has worked on.
The important note is that the "run-simulation test script" could have http requests that could time out, however, we ensured that if
there were only a couple of timeouts, we could deduce that the codebase still worked as intended. This was primarily to confirm
that if we had tens or hundreds of timeouts, we could be sure that the codebase was broken.

Below is a diagram showing the flow of our release-and-deploy workflow and the other workflows it calls.

![Release And Deploy Workflow](images/ReleaseAndDeployWorkflow.png)

## Monitoring
We monitor through the use of Prometheus and Grafana.
Our application exposes an endpoint using the OpenTelemetry nuget package for exporting telemetry data that Prometheus can understand.
Prometheus then scrapes the endpoint with an interval of 5 seconds, configured in the Prometheus.yaml file.
Prometheus saves the data in a time series database. This database is queried by Grafana, which visualizes the data in a custom dashboard.
Our custom dashboard has been built on top of the "ASP.NET Core" dashboard published by the .NET Team (https://grafana.com/grafana/dashboards/19924-asp-net-core/).
We have added a few custom panels, the most interesting being, a table that shows the total amount of request per status code for each endpoint. 
Another useful panel we made, plots the request duration of different endpoints.
(The queries for the panels can be found in the appendix)

## Logging
We rely on serilog for generating and sending logs to our log visualiser Seq. 
Our logging strategy is quite extensive, since we have had a lot of trouble with our application. We thought it was better to log more, and then not keep them for as long, to see if they could help us sort out our errors/bugs. Our logging goes as follows:

- We log when we raise exceptions and when exceptions are caught, this is to help us see how errors were propagated through the system.
- We log execution time of methods called. This was to help us see if there were any methods that was bottle necking us.
- We log the input and output of methods called, this way we can observe if they behave like we expect them too.

## Security
We had set SonarQube up to comment on every pull-request we had made, to ensure that the pull-request had passed the quality gate.
A segment of this quality gate was to ensure that there were no security hotspots. Not only did SonarQube show where the hotspots were,
but it also explained why this is a hotspot, and how to fix it. In our last release, we still had security hotspot, more specifically an
"Open Redirect Vulnerability". This vulnerability results in an "E-score", but the rest is B rated or above. 
This could have been fixed by creating an "allowlist" of safe relative paths to redirect to.

## Scaling
Regarding the scaling of our application, we are in the transition of moving from docker compose to docker swarm. However, we are using docker compose that composes
an API and MiniTwit dockerfile. Our intentions are to set up a declarative IaC using Terraform, and with that, set up a docker swarm cluster that can handle the scaling of our application.
Unfortunately, as of now, we haven't fully integrated this structure because of some complications with the implementation.

## AI-assistant
The use of ChatGPT has been used to some extent. For example, to more easily understand (at the time) complex notions in the topic of operations.
Various Docker files were created with the assistance of AI to more effectively "learn-by-doing". 
Furthermore, the assistance of AI proved efficient when having to translate the python API into C#. Problems did arise from this, though.
As an example, the AI rewrote the returned status codes, which meant that it wasn't compliant with the simulator. This ended up being an Achilles
heel, since we spent a lot of time trying to diagnose the problem with the simulator.

# Reflections

## Evolution

### Technical Debt 
The group experienced first hand, how much technical debt can slow down the development process. 
When we first shipped our code, we had not made sure that all the simulator tests passed. Because of this, 
our database was missing initial users, which gave us simulator errors. This was likely due to our implementation of the simulator API that would handle the data to insert into our database.
Because of these errors, we later attempted to insert the missing users into the database. However, this created a new error where our VM would crash and was never resolved.
As of now (28-05-2025), the API will still create any users who aren't registered in the database.

### 'Dev' and 'Ops'
At the beginning of the project, we had a lot of work that needed to be done. This was, for instance, translating the simulation whilst
also setting up the CI/CD pipelines. We also had to start using containerization with Docker and Vagrant. This led to a split in the group
between the developers, who started translating the simulation to C#, and the operators, who started setting up the CI/CD and containerization.
The difficulties lied in the communication between these two groups and how both parties could get up to speed with their respective work they've made.
A logbook was created to keep track of any work that has been done, but was unfortunately disregarded by other tasks and hasn't been updated since 07/02/25 (according to the log.md file).

### Refactoring
When rewriting the code to C# and adhering to the "minitwit_sim_api.py" from session3, we weren't thorough enough when analyzing the specs.
This resulted in us pushing code to production, which "seemingly" followed specs from the aforementioned file.
After a long contemplation on why it didn't work, we took a step back and properly analyzed the API specs.
Lesson: Do it right the first time.

## Operation and Maintenance

### Logging
We had an experience before we were introduced to logging in class, where our VM crashed due to extensive (and redundant) logging. We logged to console, Docker logs, and files.
This resulted in the bloating our Digital Ocean droplet with sizable logs. What could be done differently was to only log
once and automatically delete old ones which were no longer required.

### CPU overload
We experienced a CPU overload in our droplet. The CPU would spike to 100% and sometimes exceed that (due to Digital Ocean limiting the CPU size of the droplet).
This resulted in a crash of the droplet. Unfortunately, as of now (9/5/2025) we haven't found the reason for why this is happening.
For future reference, we should have a more thorough testing suite.

![CPU Spike on Digital Ocean](images/Digital_Ocean_2025-04-05.png)
![Spikes on Grafana](images/Grafana_Issues%202025-04-04.png)

## Grafana
During session6 we were tasked to add monitoring to our application through Prometheus using Grafana for visualization. 
This included monitoring over the requests for different endpoints in the application. While we did add monitoring to the application, we didn't use it as much as we should have. 
There wasn't much time to monitor the application with our schedule this semester. This was made worse by having to use digitalocean's dashboard as well, 
as we struggled to add CPU usage to Grafana. One thing that could have ameliorated this issue was alerts for any odd behavior with the application.

## DevOps Reflections
Despite the fact the group did not solve all issues, we still learned a lot about the DevOps work. 
When comparing the workflow to other projects like "_Chirp!_" from BDSA, a significant difference was the deployment strategy. 
Using Docker Images to deploy instead of just copying the entire codebase was a great learning experience. 
We could have used the built-in versioning in Docker Hub to better be able to roll back to working versions of the codebase. 
This would have been helpful when faulty code was deployed to production.

### Workflow Learnings
The group also learned a lot working with workflows. In previous projects, most of the logic was in one large workflow. 
In this course, we worked with splitting up the responsibility into smaller workflows, then these where called by composing workflows. 
This way of working made it easier to find errors and read the intended functionality of the workflows.

Another learning was incorporating static analysis into the integration chain. 
This gave better clarity of the code quality and helped reduce errors being merged with the main deployment.

# Appendix

## Status code panel
Query for the status code panel:
```
label_join(
  http_server_request_duration_seconds_count{
    job="$job", 
    instance="$instance", 
    http_route!=""
  }, 
  "method_route", 
  " ", 
  "http_request_method", 
  "http_route"
)
```

## Endpoint duration panel
Query for the endpoint duration panel:
```
rate(
  http_server_request_duration_seconds_sum{
    job="$job", 
    instance="$instance", http_route!=""
  }[5m]
) / rate(
  http_server_request_duration_seconds_count{
    job="$job", 
    instance="$instance", 
    http_route!=""
    } [5m]
)
```