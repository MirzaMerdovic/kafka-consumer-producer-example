# kafka-consumer-producer-example
An example of Kafka's consumer producer in .Net Core

# How to use the code

To try out th code first run `docker-compose up` to get kafka broker and zookeeper running.  
Compose file will also start `kafdrop` a Kafka UI from which you can view and create topics and view messages and other interesting information about your Kafka cluster.  

Note: You don't have to create topics via `kafdrop` because they will be auto-created by the producer app.

Now that you have infrastructure running you can first run the consumer app, and once it is running and waiting for messages you can start the producer app.  
Reason behind the ordering is that if you run the producer app it will send messages and if you open `kafdrop` by going to `http://localhost:9000` you will see them,  
once you run your consumer it will not pick up those message because it only receives message once it starts running.