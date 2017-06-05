package rb.org.messaging;

import rb.org.Console;

import javax.jms.*;

/**
 * Created by bogaa on 6/4/2017.
 */
public interface IMessageReply {

    void sendMessage(
            Session session,
            Message messageReceived,
            TextMessage mutatedMessageReceived,
            MessageProducer messageProducer,
            Destination destination
    );
}
