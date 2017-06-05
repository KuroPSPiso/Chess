package rb.org.messaging.Impl;

import rb.org.Console;
import rb.org.messaging.AMessageReply;

/**
 * Created by bogaa on 6/5/2017.
 */
public class ConnectMessage extends AMessageReply{

    public ConnectMessage(String executionType, Console... console) {
        super(executionType, console);
    }
}
