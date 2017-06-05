package rb.org.messaging;

import rb.org.Console;
import rb.org.domain.GameplayTypes;

import javax.jms.*;

/**
 * Created by bogaa on 6/4/2017.
 */
public interface IMessageReply {

    TextMessage sendMessage(
            Session session,
            Message messageReceived,
            TextMessage mutatedMessageReceived,
            MessageProducer messageProducer,
            Destination destination
    );

    TextMessage sendMessage(
            Session session,
            Message messageReceived,
            TextMessage mutatedMessageReceived,
            String newMessage,
            GameplayTypes newGameType,
            MessageProducer messageProducer,
            Destination destination
    );
}
