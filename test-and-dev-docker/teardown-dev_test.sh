source ~/.bash_profile

docker compose -f docker-compose.postgres.yml down --volumes --remove-orphans

docker compose -f ../remote_files/docker-compose.yml down --volumes --remove-orphans

docker volume prune -f

docker network prune -f
