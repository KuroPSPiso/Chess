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
            return;
        }
        if(!this.connection.IsStarted)
        {
            return;
        }

        reloadTimer += Time.fixedDeltaTime;
        if (reloadTimer > 0.4)
        {
            reloadTimer = 0;
            ReloadData();
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
                    selfReference.UpdateText("Received message with text: " + message.Text + "\n");
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
    public void SendMessage()
    {
        try
        {
            IMessageProducer producer = session.CreateProducer(destination);
            //connection.Start();

            ITextMessage request = session.CreateTextMessage("SampleMessage" + iMsgCount.ToString());
            request.Properties["User"] = mTurnJSON.GetColourString(player.PlayerColour);

            if(player.PlayerColour.Equals(Color.black))
            {
                request.Properties["AltUser"] = mTurnJSON.GetColourString(Color.white);
            }
            else
            {
                request.Properties["AltUser"] = mTurnJSON.GetColourString(Color.black);
            }
            request.NMSCorrelationID = gameID;
            request.NMSReplyTo = destination;
            producer.Send(request);

            iMsgCount++;
        }
        catch (NMSConnectionException ex)
        {
            string exception = ex.Message;
        }
    }
}
