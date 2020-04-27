## RabbitMQ Examples

This project created for basic RabbitMQ scenarios. It contains several messaging types based on RabbitMQ server. These are

- Publist/Subscribe
- Direct Routing
- Topic Exchange
- RPC

It has four project folder for each messaging type and you can start multiple instances of subscriber projects. Than send messages from publisher projects.

## Development

Install RabbitMQ server from official site or run the
`docker run -d --hostname my-rabbit --name rabbitmq-test -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=123456 -p 15672:15672 -p 5672:5672 rabbitmq:3-management` command. 

After running command RabbitMQ server will be ready and RabbitMQ management will be available on http://localhost:15672/ and from management screen queues and exchanges will be trackable and you can create and send messages to queues.
