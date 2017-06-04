package rb.org.messaging;

import javafx.fxml.FXML;
import javafx.scene.control.TextArea;
import javafx.scene.control.TextAreaBuilder;
import org.apache.activemq.broker.BrokerService;
import org.apache.activemq.ActiveMQConnectionFactory;
import rb.org.Console;

import javax.jms.*;

/**
 * Created by bogaa on 6/1/2017.
 */
public class ChessServer implements MessageListener {

    private static int ackMode;
    private static String messageQueueName;
    private static String messageBrokerUrl;

    private Session session;
    private boolean transacted = false;
    private MessageProducer replyProducer;

    BrokerService broker;
    Console console;
    Connection connection;

    static {
        messageBrokerUrl = "tcp://localhost:61616";
        messageQueueName = "client.messages";
        ackMode = Session.AUTO_ACKNOWLEDGE;
    }

    public ChessServer(BrokerService broker, Console console) {
        this.console = console;
        try {
            //This message broker is embedded
            //BrokerService broker = new BrokerService();
            broker.setPersistent(false);
            broker.setUseJmx(false);
            broker.addConnector(messageBrokerUrl);
            broker.start();
            this.broker = broker;
        } catch (Exception e) {
            //Handle the exception appropriately
        }

        this.setupMessageQueueConsumer();
    }

    private void setupMessageQueueConsumer() {
        ActiveMQConnectionFactory connectionFactory = new ActiveMQConnectionFactory(messageBrokerUrl);

        try {
            connection = connectionFactory.createConnection();
            connection.start();
            this.session = connection.createSession(this.transacted, ackMode);
            Destination chessQueue = this.session.createQueue(messageQueueName);

            //Setup a message producer to respond to messages from clients, we will get the destination
            //to send to from the JMSReplyTo header field from a Message
            this.replyProducer = this.session.createProducer(null);
            this.replyProducer.setDeliveryMode(DeliveryMode.NON_PERSISTENT);

            //Set up a consumer to consume messages off of the admin queue
            MessageConsumer consumer = this.session.createConsumer(chessQueue);
            consumer.setMessageListener(this);
        } catch (JMSException e) {
            //Handle the exception appropriately
        }
    }

    public void onMessage(Message message) {
        try {
            TextMessage response = this.session.createTextMessage();
            if (message instanceof TextMessage) {
                TextMessage txtMsg = (TextMessage) message;
                String messageText = txtMsg.getText();
                response.setText(messageText);
            }
            response.setJMSCorrelationID(message.getJMSCorrelationID());

            this.replyProducer.send(message.getJMSReplyTo(), response);
        } catch (JMSException e) {
            //Handle the exception appropriately
        }
    }

    public BrokerService getBroker()
    {
        return this.broker;
    }

    public void Close()
    {
        try {
            this.connection.close();
        } catch (JMSException e) {
            e.printStackTrace();
        }
    }
}
