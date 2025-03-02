Sample application show casing both Modular Monolith and Microservie architecture

Domains are loosley coupled with own schema and process changes from other domains using event model

Communication done through MediatR

Outbox Pattern is used for sending messages and domain events

# Outbox Dispatcher Service
Outbox dispatcher service written to process both messages to external services using Azure service bus or RabbitMQ and also send internal domain events

# Notification Service
This service look for messages (Email, SMS) in RabbitMQ or Azure Service Bus and process them