output "manager_public_ip" {
  value = digitalocean_droplet.manager.ipv4_address
}

output "manager_private_ip" {
  value = digitalocean_droplet.manager.ipv4_address_private
}

output "manager_token" {
  value = data.external.swarm_join_token.result.manager
}

output "worker_token" {
  value = data.external.swarm_join_token.result.worker
}
