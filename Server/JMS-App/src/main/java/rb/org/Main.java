package rb.org;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

public class Main extends Application {

    @Override
    public void start(Stage primaryStage) throws Exception{
        Parent root = FXMLLoader.load(getClass().getResource("/sample.fxml"));
        root.getStylesheets().add(this.getClass() .getResource("/Console.css").toExternalForm());
        primaryStage.initStyle(StageStyle.DECORATED);
        primaryStage.setResizable(true);
        primaryStage.setTitle("Chess Server");
        primaryStage.setScene(new Scene(root, 300, 450));

        primaryStage.show();
    }


    public static void main(String[] args) {
        launch(args);
    }
}
