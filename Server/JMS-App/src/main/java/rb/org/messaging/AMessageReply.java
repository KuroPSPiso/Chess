package rb.org.messaging;

import rb.org.Console;
import rb.org.domain.GameplayTypes;

import javax.jms.*;
import java.util.Date;

/**
 * Created by bogaa on 6/4/2017.
 */
public class AMessageReply implements IMessageReply {

    public Console console;
    public String executionType;

    public AMessageReply(String executionType, Console... console)
    {
        if(console != null) {
            if (console.length > 0) {
                this.console = console[0];
            }
        }

        this.executionType = executionType;
    }

    public TextMessage sendMessage(Session session, Message messageReceived, TextMessage mutatedMessageReceived, MessageProducer messageProducer, Destination destination) {
        TextMessage toSend = null;
        try {
            TextMessage response = session.createTextMessage();
            String messageText = mutatedMessageReceived.getText();

            if(console != null)
            {
                console.println(String.format("[%1s]:[%2s]:'%3s'", messageReceived.getStringProperty("FromUser"), new Date(), executionType));
            }
            try {
                response.setStringProperty("FromUser", messageReceived.getStringProperty("FromUser"));
                response.setStringProperty("ToUser", messageReceived.getStringProperty("FromUser"));
                response.setStringProperty("GameCode", messageReceived.getStringProperty("GameCode"));
                response.setStringProperty("GameType", messageReceived.getStringProperty("GameType"));
            } catch (Exception ex)
            {
                //When available
            }
            response.setText(messageText);

            response.setJMSCorrelationID(messageReceived.getJMSCorrelationID());

            toSend = response;
            messageProducer.setDeliveryMode(DeliveryMode.PERSISTENT);
            messageProducer.send(destination, response);

//            try {
//                Thread.sleep(1000); //Send Safety (?)
//            } catch (InterruptedException e) {
//                e.printStackTrace();
//            }
        } catch (JMSException e) {
            //Handle the exception appropriately
        }
        return toSend;
    }

    public TextMessage sendMessage(Session session, Message messageReceived, TextMessage mutatedMessageReceived, String newMessage, GameplayTypes newGameType, MessageProducer messageProducer, Destination destination) {

        TextMessage toSend = null;
        try {
            TextMessage response = session.createTextMessage();

            if(console != null)
            {
                console.println(String.format("[%1s]:[%2s]:'%3s'", messageReceived.getStringProperty("FromUser"), new Date(), executionType));
            }
            response.setText(newMessage);
            try {
                response.setStringProperty("FromUser", messageReceived.getStringProperty("FromUser"));
                response.setStringProperty("ToUser", messageReceived.getStringProperty("FromUser"));
                response.setStringProperty("GameCode", messageReceived.getStringProperty("GameCode"));
            } catch (Exception ex)
            {
                //When available
            }
            response.setStringProperty("GameType", newGameType.toString());

            response.setJMSCorrelationID(messageReceived.getJMSCorrelationID());

            toSend = response;
            messageProducer.setDeliveryMode(DeliveryMode.PERSISTENT);
            messageProducer.send(destination, response);

//            try {
//                Thread.sleep(1000); //Send Safety (?)
//            } catch (InterruptedException e) {
//                e.printStackTrace();
//            }
        } catch (JMSException e) {
            //Nothing to handle at this point
        }

        return toSend;
    }
}
