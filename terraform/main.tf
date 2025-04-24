data "digitalocean_ssh_key" "terraform" {
  name = "Mathias_Laptop"
}

data "external" "swarm_join_token" {
  program = ["./get-join-tokens.sh"]
  query = {
    host = "${digitalocean_droplet.manager.ipv4_address}"
  }
}

# Create a new Web Droplet in the nyc2 region
resource "digitalocean_droplet" "manager" {
  tags   = ["manager", "terraform"]
  image  = "docker-20-04"
  name   = "manager"
  region = "fra1"
  size   = "s-1vcpu-1gb"

  ssh_keys = [data.digitalocean_ssh_key.terraform.id]

  connection {
    type        = "ssh"
    user        = "root"
    private_key = file(var.path_to_private_key)
    host        = self.ipv4_address
  }

  provisioner "remote-exec" {
    inline = [
      "ufw allow 2377/tcp",
      "ufw allow 7946/tcp",
      "ufw allow 7946/udp",
      "ufw allow 4789/udp",
      "docker swarm init --advertise-addr ${digitalocean_droplet.manager.ipv4_address_private}",
      "sleep 5"
    ]
  }
}

resource "digitalocean_droplet" "worker" {
  count = 2

  depends_on = [digitalocean_droplet.manager]

  tags   = ["worker", "terraform"]
  image  = "docker-20-04"
  name   = "worker-${count.index}"
  region = "fra1"
  size   = "s-1vcpu-1gb"

  ssh_keys = [data.digitalocean_ssh_key.terraform.id]

  connection {
    type        = "ssh"
    user        = "root"
    private_key = file(var.path_to_private_key)
    host        = self.ipv4_address
  }

  provisioner "remote-exec" {
    inline = [
      "for i in {1..10}; do docker swarm join --token ${data.external.swarm_join_token.result.worker} ${digitalocean_droplet.manager.ipv4_address_private}:2377 && break || sleep 5; done"
    ]
  }
}
