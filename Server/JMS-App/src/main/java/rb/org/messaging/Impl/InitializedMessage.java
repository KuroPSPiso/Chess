package rb.org.messaging.Impl;

import rb.org.Console;
import rb.org.messaging.AMessageReply;

/**
 * Created by bogaa on 6/6/2017.
 */
public class InitializedMessage extends AMessageReply {
    public InitializedMessage(String executionType, Console... console) {
        super(executionType, console);
    }
}
