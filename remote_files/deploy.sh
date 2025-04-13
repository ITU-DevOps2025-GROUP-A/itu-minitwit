source ~/.bash_profile

if ! docker network ls --format '{{.Name}}' | grep -w shared_network; then
  sudo docker network create shared_network
fi

sudo docker compose -f docker-compose.yml pull
sudo docker compose -f docker-compose.yml up -d
