package rb.org.messaging;

import org.apache.activemq.ActiveMQConnectionFactory;

import javax.jms.*;

/**
 * Created by bogaa on 5/8/2017.
 */
public class QueueProducer implements Runnable {

    ActiveMQConnectionFactory connectionFactory;

    public QueueProducer(ActiveMQConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public void run() {

        try {
            // First create a connection

            Connection connection =
                    connectionFactory.createConnection();
            connection.start();

            // Now create a Session
            Session session = connection.createSession(false,
                    Session.AUTO_ACKNOWLEDGE);

            // Let's create a topic. If the topic exist,
            //it will return that
            Destination destination = session.createQueue("GAME");

            // Create a MessageProducer from
            //the Session to the Topic or Queue
            MessageProducer producer =
                    session.createProducer(destination);
            producer.setDeliveryMode(DeliveryMode.PERSISTENT);

            // Create a messages for the current climate
            String text = "Today is Hot";
            TextMessage message = session.createTextMessage(text);

            // Send the message to topic
            producer.send(message);

            // Do the cleanup
            session.close();
            connection.close();
        } catch (JMSException jmse) {
            System.out.println("Exception: " + jmse.getMessage());
        }

    }
}
