package rb.org.domain;

/**
 * Created by bogaa on 6/4/2017.
 */
public enum GameplayTypes {
    Connect("Connect"),
    Movement("Movement"),
    Promotion("Promotion"),
    Turn("Turn"),
    Finish("Finish"),
    Surrender("Surrender"),
    None("None");

    private final String gameplayType;

    private GameplayTypes(final String gameplayType) {
        this.gameplayType = gameplayType;
    }

    @Override
    public String toString() {
        return gameplayType;
    }
}
