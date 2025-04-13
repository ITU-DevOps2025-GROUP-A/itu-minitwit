source ~/.bash_profile

if ! docker network ls --format '{{.Name}}' | grep -w shared_network; then
  docker network create shared_network
fi

docker compose -f docker-compose.postgres.yml up --build -d

docker compose -f ../remote_files/docker-compose.yml up --build -d
