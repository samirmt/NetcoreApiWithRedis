# NetcoreApiWithRedisInDocker
Simple example Netcore 3.1 API using Redis in Docker

download redis to docker:
docker pull redis:6.2.4-alpine

start container:
docker run -d -p 6379:6379 -i -t redis:6.2.4-alpine
