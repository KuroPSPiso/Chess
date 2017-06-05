package rb.org.messaging;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by bogaa on 6/4/2017.
 */
public class GameRoomMessages {

    boolean isFinished;
    List<MessageData> messages;

    public GameRoomMessages()
    {
        this.isFinished = false;
        this.messages = new ArrayList<MessageData>();
    }

    public boolean getIsFinished() {
        return isFinished;
    }

    public List<MessageData> getMessages() {
        return messages;
    }

    public void addMessage(MessageData messageData, boolean... gameStatus)
    {
        this.messages.add(messageData);
        if(gameStatus != null)
        {
            if(gameStatus.length > 0)
            {
                this.isFinished = gameStatus[0];
            }
        }
    }
}
