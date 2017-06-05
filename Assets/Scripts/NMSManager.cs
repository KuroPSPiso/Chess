using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Util;
using System.Threading;
using Apache.NMS.Util;
using Assets.Scripts.Messages;

public class NMSManager : MonoBehaviour {

    //Debug
    [Header("Debug")]
    public bool debug = false;
    public Text debugText;
    private String debugMessages = String.Empty;
    NMSManager selfReference;

    [Header("Settings")]
    Uri connectUri;
    public String queue = "queue://";
    public String queueVar = "";
    public String gameID = "";
    public String userID = "";
    private String connectionString;

    Player player;
    bool connectionConfirmed;
    bool connectionConfirmationSend;

    IConnectionFactory factory;
    IConnection connection;
    protected static AutoResetEvent semaphore = new AutoResetEvent(false);
    protected static TimeSpan receiveTimeout = TimeSpan.FromSeconds(10);
    float reloadTimer;
    protected static ITextMessage message = null;

    public String connectionUri = "tcp://localhost:61616";
    ISession session;
    IDestination destination;
    IMessageConsumer consumer;

    Thread _AsyncReadThread;

    int iMsgCount = 0;

    // Use this for initialization
    void Start()
    {
        this.player = GameObject.FindObjectOfType<Player>();
        this.connectionConfirmed = false;
        if (this.player == null)
        {
            //return to menu
        }
        else
        {
            this.gameID = this.player.GameCode;
            this.userID = mTurnJSON.GetColourString(this.player.PlayerColour);
        }

        this.connectUri = new Uri("activemq:" + connectionUri);

        this.factory = new NMSConnectionFactory(connectUri);
        this.connection = factory.CreateConnection();
        this.connection.ClientId = mTurnJSON.GetColourString(player.PlayerColour);

        this.ReloadData();

        session = connection.CreateSession();
        destination = SessionUtil.GetDestination(session, queue + queueVar);
        consumer = session.CreateConsumer(destination);
        connection.Start();
        selfReference = this;
    }

    void UpdateText(String text)
    {
        debugMessages += text;
    }

    private void Update()
    {
        this.debugText.text = this.debugMessages;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(this.connection == null)
        {
            return; //connection created
        }
        if(!this.connection.IsStarted)
        {
            return; //connection established
        }

        reloadTimer += Time.fixedDeltaTime;
        if (reloadTimer > 0.4)
        {
            reloadTimer = 0;
            ReloadData();
        }

        if (!this.connectionConfirmationSend)
        {
            this.SendConnectMessage(); //publicate CUSTOM connection details
            this.connectionConfirmationSend = true;
        }
        if(!this.connectionConfirmed)
        {
            return; //Final wait for data collection
        }
    }

    public void ReloadData()
    {
        if (_AsyncReadThread != null)
        {
            if (_AsyncReadThread.IsAlive)
            {
                return; // Already fetching data
            }
        }
        _AsyncReadThread = new Thread(() =>
        {
            try
            {
                // Create a consumer and producer
                consumer.Listener += new MessageListener(OnMessage);
                // Consume a message
                semaphore.WaitOne((int)receiveTimeout.TotalMilliseconds, true);
                if (message == null)
                {
                    //selfReference.UpdateText("No message received!" + "\n");
                }
                else
                {
                    message.Acknowledge();

                    if (debug)
                    {
                        selfReference.UpdateText("Received message with text: " + message.Text + "\n");
                    }

                    GameplayTypes gameTypes = GameplayTypes.NONE;

                    if (message.Properties.Contains("GameType"))
                    {
                        gameTypes = (GameplayTypes)Enum.Parse(typeof(GameplayTypes), message.Properties.GetString("GameType"));
                    }
                    switch (gameTypes)
                    {
                        case GameplayTypes.Connect:
                            HandleConnectionMessage();
                            break;

                        default:
                            //No Default Handler
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                //selfReference.UpdateText(e.Message + " -" + e.GetBaseException().ToString() + "\n");
            }
        });

        _AsyncReadThread.Start();
    }

    protected static void OnMessage(IMessage receivedMsg)
    {
        message = receivedMsg as ITextMessage;
        semaphore.Set();
    }

    [ContextMenu("TestMessage")]
    public void SendTestMessage()
    {
        try
        {
            IMessageProducer producer = session.CreateProducer(destination);
            //connection.Start();

            ITextMessage request = session.CreateTextMessage("SampleMessage" + iMsgCount.ToString());
            request.Properties["FromUser"] = mTurnJSON.GetColourString(player.PlayerColour);

            if(player.PlayerColour.Equals(Color.black))
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.white);
            }
            else
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.black);
            }
            request.NMSCorrelationID = this.gameID + "/" + this.userID;
            request.NMSReplyTo = destination;
            producer.Send(request);

            iMsgCount++;
        }
        catch (NMSConnectionException ex)
        {
            string exception = ex.Message;
        }
    }

    public void SendMessage(String message, String gameplayType)
    {
        try
        {
            IMessageProducer producer = session.CreateProducer(destination);
            //connection.Start();

            ITextMessage request = session.CreateTextMessage(message);
            request.Properties["FromUser"] = mTurnJSON.GetColourString(player.PlayerColour);

            if (player.PlayerColour.Equals(Color.black))
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.white);
            }
            else
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.black);
            }
            request.Properties["GameType"] = gameplayType;
            request.NMSCorrelationID = this.gameID + "/" + this.userID;
            request.NMSReplyTo = this.destination;
            producer.Send(request);
        }
        catch (NMSConnectionException ex)
        {
            string exception = ex.Message;
        }
    }

    public void SendConnectMessage()
    {
        SendMessage("Connect", GameplayTypes.Connect.ToString());
    }

    public void SendMovementMessage(mMovementJSON movement)
    {
        SendMessage(movement.toJson(), GameplayTypes.Movement.ToString());
    }

    public void SendPromotionMessage(mPromotionJSON promotion)
    {
        SendMessage(promotion.toJson(), GameplayTypes.Promotion.ToString());
    }

    public void SendTurnMessage(mTurnJSON turn)
    {
        SendMessage(turn.toJson(), GameplayTypes.Turn.ToString());
    }

    public void SendFinish()
    {
        SendMessage("Finished", GameplayTypes.Finish.ToString());
    }
    public void SendSurrender()
    {
        SendMessage("Surrender", GameplayTypes.Surrender.ToString());
    }

    public void HandleConnectionMessage()
    {
        //normally load history
        connectionConfirmed = true; //Finally confirm connection and use of previously saved data if available.
    }
}
