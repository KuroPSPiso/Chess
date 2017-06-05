package rb.org.messaging.Impl;

import rb.org.Console;
import rb.org.messaging.AMessageReply;
import rb.org.messaging.IMessageReply;

import javax.jms.*;

/**
 * Created by bogaa on 6/4/2017.
 */
public class PingMessage extends AMessageReply {

    public PingMessage(String executionType, Console... console) {
        super(executionType, console);
    }
}
