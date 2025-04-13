source ~/.bash_profile

cd /minitwit

sudo docker compose -f docker-compose.yml pull
sudo docker compose -f docker-compose.yml up -d
