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
using UnityEngine.SceneManagement;

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
    public Image connectionIndicator;

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
    ITextMessage lastMessage = null;

    int iMsgCount = 0;
    bool isStopping = false;
    bool isStopped = false;

    // Use this for initialization
    void Start()
    {
        //Allow application to run in the background
        Application.runInBackground = true;
        connectionIndicator.color = Color.yellow;

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

        this.connection.ClientId = this.userID;

        this.ReloadData();

        session = connection.CreateSession();
        destination = SessionUtil.GetDestination(session, queue + queueVar);
        consumer = session.CreateConsumer(destination);
        // Create a consumer and producer
        consumer.Listener += new MessageListener(OnMessage);
        connection.Start();
        selfReference = this;
    }

    void UpdateText(String text)
    {
        if(debugMessages.Length == 0)
        {
            debugMessages = ":{" + text + "}:";
        }
        if (debugMessages.Length < 250 && debugMessages.Length > 0)
        {
            debugMessages = ":{" + text + "}:" + debugMessages.Substring(0, debugMessages.Length - text.Length);
        }
        else
        {
            debugMessages = ":{" + text + "}::{" + debugMessages.Substring(0, 250 - text.Length);
        }
    }

    private void Update()
    {
        this.debugText.text = this.debugMessages;

        if(isStopping)
        {
            connectionIndicator.color = Color.red;
        }
        else if(connectionConfirmed)
        {
            connectionIndicator.color = Color.green;
        }
        else
        {
            connectionIndicator.color = Color.yellow;
        }

    }

    // Update is called once per frame
    void FixedUpdate() {
        if(this.isStopped)
        {
            //return to menu
            SceneManager.LoadScene(0);
            return;
        }
        if (this.isStopping)
        {
            if (!this._AsyncReadThread.IsAlive)
            {
                this.isStopped = true;
            }
        }
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
            if(isStopping)
            {
                return;
            }

            try
            {
                // Consume a message
                semaphore.WaitOne((int)receiveTimeout.TotalMilliseconds, true);
                if (message == null)
                {
                    //selfReference.UpdateText("No message received!" + "\n");
                }
                else
                {
                    if(lastMessage == message)
                    {
                        return; //sameMessage
                    }
                    else
                    {
                        this.lastMessage = message;
                    }
                    message.Acknowledge();

                    if (debug)
                    {
                        selfReference.UpdateText("Received message with text: " + message.Text);
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
                        case GameplayTypes.Disconnect:
                            HandleDisconnectMessage();
                            break;
                        case GameplayTypes.Movement:
                            break;
                        case GameplayTypes.Turn:
                            break;
                        case GameplayTypes.Surrender:
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
        if (connection == null)
        {
            return;
        }

        try
        {
            IMessageProducer producer = session.CreateProducer(destination);
            //connection.Start();

            ITextMessage request = session.CreateTextMessage("SampleMessage" + iMsgCount.ToString());
            request.Properties["FromUser"] = this.userID;

            if (userID.Equals(mTurnJSON.GetColourString(Color.black)))
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.white);
            }
            else
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.black);
            }
            request.Properties["GameCode"] = this.gameID;
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
        if(connection == null)
        {
            return;
        }

        try
        {
            IMessageProducer producer = session.CreateProducer(destination);
            //connection.Start();

            ITextMessage request = session.CreateTextMessage(message);
            request.Properties["FromUser"] = this.userID;

            if (userID.Equals(mTurnJSON.GetColourString(Color.black)))
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.white);
            }
            else
            {
                request.Properties["ToUser"] = mTurnJSON.GetColourString(Color.black);
            }
            request.Properties["GameCode"] = this.gameID;
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

    public void SendDisconnectMessage()
    {
        SendMessage("Disconnect", GameplayTypes.Disconnect.ToString());
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

    public void SendFinishMessage()
    {
        SendMessage("Finished", GameplayTypes.Finish.ToString());
    }
    public void SendSurrenderMessage()
    {
        SendMessage("Surrender", GameplayTypes.Surrender.ToString());
    }

    public void HandleConnectionMessage()
    {
        //normally load history
        connectionConfirmed = true; //Finally confirm connection and use of previously saved data if available.
    }

    public void HandleDisconnectMessage()
    {
        if (_AsyncReadThread != null)
        {
            while (_AsyncReadThread.IsAlive)
            {
                try
                {
                    selfReference.UpdateText("Very Funny");
                    isStopping = true;
                    message = null;
                    semaphore.Set();
                    connection.Stop();
                    connection.Dispose();
                    connection = null;
                }
                catch (Exception ex)
                {
                    //interupt caught
                    selfReference.UpdateText(ex.Message);
                }


                try
                {
                    _AsyncReadThread.Abort();
                    _AsyncReadThread = null;
                }
                catch (Exception ex)
                {
                    //interupt caught
                    selfReference.UpdateText(ex.Message);
                }
            }
        }
    }
}
