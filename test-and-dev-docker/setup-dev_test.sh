source ~/.bash_profile

if ! docker network ls --format '{{.Name}}' | grep -w shared_network; then
  docker network create shared_network
fi

export DB_CONNECTION_STRING="Host=postgres;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword"

docker compose -f docker-compose.postgres.yml up --build -d

docker compose -f ../remote_files/docker-compose.yml up --build -d
