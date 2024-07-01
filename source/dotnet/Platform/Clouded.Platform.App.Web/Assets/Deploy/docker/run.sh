#!/bin/bash

docker pull $DOCKER_IMAGE

docker run --detach \
--restart always \
--name clouded-admin-provider \
--publish 80:80 \
--publish 443:443 \
--env-file ./env \
$DOCKER_IMAGE







