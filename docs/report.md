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

* Refactoring from Python to C# and incorporating Helge's API specifications. Specifically 'minitwit_sim_api.py'

## Operation

* Logging hell. Logging so much that our VM crashed.

* CPU overload on our droplet (VM) on Digital Ocean

## Maintenance