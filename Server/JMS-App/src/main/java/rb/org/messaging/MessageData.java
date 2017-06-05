package rb.org.messaging;

import rb.org.domain.GameplayTypes;

/**
 * Created by bogaa on 6/4/2017.
 */
public class MessageData {
    String messageString;
    GameplayTypes gameplayTypes;
    String gameRoom;

    public MessageData(String messageString, String gameRoom, GameplayTypes gameplayTypes) {
        this.messageString = messageString;
        this.gameRoom = gameRoom;
        this.gameplayTypes = gameplayTypes;
    }

    public String getMessageString() {
        return this.messageString;
    }

    public String getGameRoom() {
        return this.gameRoom;
    }

    public GameplayTypes getGameplayTypes()
    {
        return this.gameplayTypes;
    }

}
