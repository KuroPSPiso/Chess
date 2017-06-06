package rb.org.messaging;

import javafx.fxml.FXML;
import javafx.scene.control.TextArea;
import javafx.scene.control.TextAreaBuilder;
import org.apache.activemq.broker.BrokerService;
import org.apache.activemq.ActiveMQConnectionFactory;
import rb.org.Console;
import rb.org.domain.GameplayTypes;
import rb.org.domain.Identity;
import rb.org.domain.IdentityHolder;
import rb.org.messaging.Impl.ConnectMessage;
import rb.org.messaging.Impl.InitializedMessage;
import rb.org.messaging.Impl.PingMessage;

import javax.jms.*;
import java.util.HashMap;

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

    HashMap<String, GameRoomMessages> roomMessagesHashMap = new HashMap<String, GameRoomMessages>();
    IdentityHolder identityHolder;

    static {
        messageBrokerUrl = "tcp://localhost:61616";
        messageQueueName = "client.messages";
        ackMode = Session.CLIENT_ACKNOWLEDGE;
    }

    public ChessServer(BrokerService broker, Console console) {
        this.console = console;
        this.identityHolder = new IdentityHolder();

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

    public BrokerService getBroker()
    {
        return this.broker;
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
            this.replyProducer.setDeliveryMode(DeliveryMode.PERSISTENT);

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

                GameplayTypes gameplayType = GameplayTypes.None;

                if(message.propertyExists("GameType")) {
                    gameplayType = GameplayTypes.valueOf(message.getStringProperty("GameType"));
                }

                AMessageReply reply;

                switch (gameplayType)
                {
                    case Connect:
                        reply = new ConnectMessage(gameplayType.toString(), console);
                        reply.sendMessage(this.session, message, txtMsg, this.replyProducer, message.getJMSDestination());
                        Identity identityFound = this.identityHolder.putIdentity(message.getJMSCorrelationID(), new Identity(message.getJMSDestination()));

                        //disconnect old user if data is still available
                        if(identityFound != null) {
                            //TODO: Check if this is root of issues
                            //reply.sendMessage(session, message, txtMsg, "Disconnect", GameplayTypes.Disconnect, this.replyProducer, identityFound.getDestination());
                        }
                        //TODO:resend all data to new user
                        MessageProducer initProducer = this.session.createProducer(null);
                        initProducer.setDeliveryMode(DeliveryMode.PERSISTENT);
                        InitializedMessage sendInit = new InitializedMessage(GameplayTypes.Initialised.toString(), console);
                        sendInit.sendMessage(this.session, message, txtMsg, "Initialised", GameplayTypes.Initialised, initProducer, message.getJMSDestination());
                        break;
                    case Disconnect:
                        reply = new PingMessage(gameplayType.toString(), console);
                        reply.sendMessage(this.session, message, txtMsg, this.replyProducer, message.getJMSDestination());

                        this.identityHolder.removeIdentity(message.getJMSCorrelationID());
                        break;
                    case Movement:
                        break;
                    case Promotion:
                        break;
                    case Turn:
                        break;
                    case Finish:
                        break;
                    case Surrender:
                        break;
                    default: //the following tasks rely on direct postback messages:
                        reply = new PingMessage(gameplayType.toString(), console);
                        reply.sendMessage(this.session, message, txtMsg, this.replyProducer, message.getJMSDestination());
                        break;
                }
            }
        } catch (JMSException e) {
            //Handle the exception appropriately
        }
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
