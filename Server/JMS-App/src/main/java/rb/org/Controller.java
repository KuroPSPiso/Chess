package rb.org;

import javafx.application.Platform;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.TextArea;
import org.apache.activemq.ActiveMQConnectionFactory;
import org.apache.activemq.broker.BrokerService;
import org.apache.activemq.broker.TransportConnector;
import rb.org.messaging.ChessServer;

import java.net.URI;
import java.net.URL;
import java.util.ResourceBundle;

public class Controller implements Initializable{

    ActiveMQConnectionFactory connectionFactory; //Waits for CORE, MQTT, AMQP, STOMP, HORNETQ AND OPENWIRE (like nms)

    BrokerService broker = new BrokerService();
    ChessServer chessServer;
    Thread launchServer;

    @FXML
    TextArea taConsole;

    Console console;

    public void initialize(URL location, ResourceBundle resources) {
        console = new Console(taConsole);

        console.println("Opening broker");

        /*
        Thread launchFactory = new Thread(new Runnable() {
            public void run() {
                while(!broker.isStarted())
                {
                    try {
                        Thread.sleep(100); // lessen load
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
                System.out.print("Starting connectionFactory");
                connectionFactory = new ActiveMQConnectionFactory("tcp://localhost:61616");
            }
        });
        launchFactory.setDaemon(true);
        launchFactory.start();*/
        launchServer = new Thread(new Runnable() {
            public void run() {
                console.println("Starting chessServer");
                chessServer = new ChessServer(broker, console);
                while(!broker.isStarted())
                {
                    try {
                        Thread.sleep(100); // lessen load
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
                console.println("ChessServer started.");
            }
        });
        launchServer.setDaemon(true);
        launchServer.start();
    }

    public void OnClick_CloseApplication()
    {
        this.chessServer.Close();

        try {
            this.broker.stop();
        } catch (Exception e) {
            e.printStackTrace();
        }

        this.broker.waitUntilStopped();

        this.launchServer.interrupt();

        Platform.exit();
    }
}
