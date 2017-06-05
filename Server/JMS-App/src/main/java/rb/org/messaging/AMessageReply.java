package rb.org.messaging;

import rb.org.Console;

import javax.jms.*;

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

    public void sendMessage(Session session, Message messageReceived, TextMessage mutatedMessageReceived, MessageProducer messageProducer, Destination destination) {
        try {
            TextMessage response = session.createTextMessage();
            String messageText = mutatedMessageReceived.getText();

            if(console != null)
            {
                console.println(String.format("[%1s]::'%2s'", messageReceived.getStringProperty("FromUser"), executionType));
            }
            response.setText(messageText);

            response.setJMSCorrelationID(messageReceived.getJMSCorrelationID());

            messageProducer.send(destination, response);
        } catch (JMSException e) {
            //Handle the exception appropriately
        }

    }
}
