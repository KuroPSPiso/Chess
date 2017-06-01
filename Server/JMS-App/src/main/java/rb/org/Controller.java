package rb.org;

import javafx.fxml.Initializable;
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

    public void initialize(URL location, ResourceBundle resources) {
        System.out.print("Opening broker");

        TransportConnector connector = new TransportConnector();
        try {
            connector.setUri(new URI("tcp://localhost:61616"));
            broker.addConnector(connector);
            broker.start();
        } catch (Exception e) {
            e.printStackTrace();
        }

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
        Thread launchServer = new Thread(new Runnable() {
            public void run() {
                while(!broker.isStarted())
                {
                    try {
                        Thread.sleep(100); // lessen load
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
                System.out.print("Starting chessServer");
                chessServer = new ChessServer();
            }
        });
        launchServer.setDaemon(true);
        launchServer.start();
    }
}
