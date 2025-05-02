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

\pagebreak
# Process

\pagebreak

# Reflections

## Evolution

## Refactoring

When rewriting the code to C# and adhering to the 'minitwit_sim_api.py' from session3, we weren't thorough enough when analyzing the specs.
This resulted in us pushing code to production, which 'seemingly' followed specs from the latter mentioned file.
After long contemplation on why it didn't work, we took a step back and properly analyzed the api specs.
Lesson: Do it right the first time.


## Operation

Our VM crashed due to extensive (and redundant) logging. We logged to console, to docker logs and to files. This resulted in the logs using all the storage in the VM
What could be done, is to make sure only to log once and automatically delete old logs which are not needed anymore


* CPU overload on our droplet (VM) on Digital Ocean

We experienced CPU overload on our droplet (VM) on Digital ocean. This resulted in the droplet crashing.
Unfortunately, we have no idea why this is happening, even after extensive diagnosis
## Maintenance