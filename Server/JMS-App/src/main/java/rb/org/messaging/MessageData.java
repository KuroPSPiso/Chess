package rb.org.messaging;

import rb.org.domain.GameplayTypes;

import javax.jms.TextMessage;

/**
 * Created by bogaa on 6/4/2017.
 */
public class MessageData {
    String messageString;
    GameplayTypes gameplayTypes;
    String gameRoom;

    TextMessage textMessage;

    public MessageData(String messageString, String gameRoom, GameplayTypes gameplayTypes) {
        this.messageString = messageString;
        this.gameRoom = gameRoom;
        this.gameplayTypes = gameplayTypes;
        this.textMessage = null;
    }

    public MessageData(String messageString, String gameRoom, GameplayTypes gameplayTypes, TextMessage textMessage) {
        this.messageString = messageString;
        this.gameRoom = gameRoom;
        this.gameplayTypes = gameplayTypes;
        this.textMessage = textMessage;
    }

    public MessageData(TextMessage textMessage)
    {
        this.messageString = null;
        this.gameRoom = null;
        this.gameplayTypes = null;
        this.textMessage = textMessage;
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

    public TextMessage getTextMessage() { return this.textMessage; }
}
