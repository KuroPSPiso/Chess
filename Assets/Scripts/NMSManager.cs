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
using Assets.Scripts;

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

    [Header("GameInformation")]
    public GameManager gameManager; //Gamelogic
    public Text gameInfo;
    public LookAt checkPositionTarget; //Mouse controlled position finder
    private CheckSpace selectedCheckspace = null;
    public Material highlightMaterial;
    private Material oldMaterial = null;

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
    IMessageProducer consumer;

    ITemporaryQueue tempQueue;
    IDestination replyToDestination;
    IMessageConsumer responseConsumer;

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
        this.connection.ClientId = this.gameID + "/" + this.userID;
        this.connection.RedeliveryPolicy = new Apache.NMS.Policies.RedeliveryPolicy() { MaximumRedeliveries = 0 }; //TODO: remove?

        this.ReloadData();

        session = connection.CreateSession();
        destination = SessionUtil.GetDestination(session, "client.messages");
        consumer = session.CreateProducer(destination);
        // Create a consumer and producer
        this.tempQueue = session.CreateTemporaryQueue();
        this.replyToDestination = SessionUtil.GetDestination(session, this.tempQueue.QueueName, this.tempQueue.DestinationType);
        this.responseConsumer = session.CreateConsumer(this.replyToDestination);

        this.responseConsumer.Listener += new MessageListener(OnMessage);
        connection.Start();
        selfReference = this;
    }

    void UpdateText(String text)
    {
        if(debugMessages.Length == 0)
        {
            debugMessages = ":{" + text + "}:";
        }
        else if(debugMessages.Length < 250 && debugMessages.Length > 0)
        {
            debugMessages = ":{" + text + "}:" + debugMessages;
        }
        else
        {
            debugMessages = ":{" + text + "}::{" + debugMessages.Substring(0, 250 - text.Length);
        }

        //reformat again
        if (debugMessages.Length > 250)
        {
            debugMessages = debugMessages.Substring(0, 250);
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

        ExecuteGameLogic();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(this.isStopped)
        {
            //return to menu
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player.gameObject);
            }
            SceneManager.LoadScene(0);
            return;
        }
        if (this.isStopping)
        {
            if (this._AsyncReadThread != null)
            {
                if (!this._AsyncReadThread.IsAlive)
                {
                    this.isStopped = true;
                }
            } else
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
                    if(message.NMSCorrelationID != this.gameID + "/" + this.userID)
                    {
                        //message is not ment for you;
                        return;
                    }
                    if (lastMessage != null)
                    {
                        if (lastMessage.NMSMessageId == message.NMSMessageId)
                        {
                            return; //sameMessage
                        }
                        else
                        {
                            this.lastMessage = message;
                        }
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
                        case GameplayTypes.Initialised:
                            HandleInitialisedMessage();
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

        //if (this.processor.ReceiveMessage(textMessage))
        //{
        //    this.session.Commit();
        //}
        //else
        //{
        //    Console.WriteLine("Error - returning message to queue.");
        //    this.session.Rollback();
        //}
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
            request.NMSReplyTo = this.replyToDestination;
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
            request.NMSReplyTo = this.replyToDestination;
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

    public void HandleInitialisedMessage()
    {
        if (!gameManager.isInitialised)
        {
            if (gameManager.isReadyToOperate)
            {
                gameManager.isReadyToOperate = false; //disable
            }
            gameManager.isInitialised = true;

            if(!gameManager.isBlacksTurn && !gameManager.isWhitesTurn)
            {
                //first turn, white goes first always
                gameManager.isWhitesTurn = true; //Not required to send a message as this get's auto set after initialisation.
            }
        }
    }

    public void HandleDisconnectMessage()
    {
        isStopping = true;
        if (_AsyncReadThread != null)
        {
            while (_AsyncReadThread.IsAlive)
            {
                try
                {
                    selfReference.UpdateText("Disconnecting");
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

    private void ExecuteGameLogic()
    {
        if(!gameManager.isInitialised)
        {
            //wait for ready state of operation so it's interactable
            gameInfo.text = "Setting up";
            return;
        }

        //check if can move
        if (mTurnJSON.GetColour(this.userID).Equals(Color.white))
        {
            if(!gameManager.isWhitesTurn)
            {
                gameInfo.text = "It's your opponents turn (Black)";
                return; //not your turn;
            }
            else
            {
                gameInfo.text = "It's Your turn (White)";
            }
        }
        else
        {
            if (!gameManager.isBlacksTurn)
            {
                gameInfo.text = "It's your opponents turn (White)";
                return; //not your turn;
            }
            else
            {
                gameInfo.text = "It's Your turn (Black)";
            }
        }

        if(Input.GetMouseButtonUp(0) && this.checkPositionTarget.targetCheckSpace != null)
        {
            if(this.selectedCheckspace != null)
            {
                this.selectedCheckspace.gameObject.GetComponent<Renderer>().material = oldMaterial;
                this.selectedCheckspace = null;
            }
            this.selectedCheckspace = this.checkPositionTarget.targetCheckSpace;
            this.oldMaterial = this.selectedCheckspace.gameObject.GetComponent<Renderer>().material;
            this.selectedCheckspace.gameObject.GetComponent<Renderer>().material = highlightMaterial;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            if (this.selectedCheckspace != null)
            {
                this.selectedCheckspace.gameObject.GetComponent<Renderer>().material = oldMaterial;
                this.selectedCheckspace = null;
            }
        }
    }
}
