# itu-minitwit
This project is part of the ['DevOps'](https://github.com/itu-devops/lecture_notes) course at the IT-University of Copenhagen in 2025

## Members
Co-authored-by: Anthon \<acah@itu.dk>\
Co-authored-by: Christoffer \<gryn@itu.dk>\
Co-authored-by: Jacques \<japu@itu.dk>\
Co-authored-by: Rasmus \<rarl@itu.dk>\
Co-authored-by: Mathias \<mlao@itu.dk>

## Issue Template
### Title
As a \<who>, I want to \<What>, in order to \<Why>

### Description
\## Acceptance criteria\
\- [ ] Criteria1\
\- [ ] Criteria2

\Timespent Xh Ym
\Time estimated Xh Ym

## Branch named
All lowercap with - fx update-readme

## Useful commands
### Docker
Get container names \
`docker ps --format "{{.Names}}"`

Build then run in the background \
`docker compose up --build --detach`

Open a shell in a container \
`docker exec -it <container-name> sh`
