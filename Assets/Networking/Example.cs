//The script below shows how to set up connections ready to use NetworkTransport.Send

//To make this work, create an empty GameObject and attach this script

//Make sure there is a NetworkManager in your Scene. To add one, click on a GameObject and click the Add Component Button in the Inspector window. Go to Network>Network Manager.

//Create 2 Buttons (Create>UI>Button), one for a client and the other a server.

//Create an Input Field (Create>UI>Input Field).

//Attach all of these in the Inspector of your GameObject.

//Build and run a version of your Scene. Press the “server” button on this side.

//In the Unity Editor, press the Play button to have a second version running. Click the client Button on this side. A text field should appear that allows you to type a message if your connection worked. Press the return or enter key to send this message from the “client” side using NetworkTransport.Send

//The script then receives the message using NetworkTransport.Receive, and outputs the message on the “server” side. Go to your built version of the Application (server side) to see the message in your Input Field.

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Example : MonoBehaviour
{
    int m_ServerSocket;
    int m_ClientSocket;
    int m_ConnectionID;
    byte m_ChannelID;

    HostTopology m_HostTopology;
    bool m_ClientsActive;
    string myText;

    //These are the Buttons that start the client and server, and the Button for sending messages
    //Assure that you assign these in the Inspector before testing
    public Button m_ClientButton, m_ServerButton;
    //This is the Input Field for writing messages. Assign it in the Inspector.
    public InputField m_InputField;

    void Start()
    {
        //Set this to false to indicate that the client hasn't connected yet
        m_ClientsActive = false;
        //Set the Text in the text field to this
        myText = "Please Type Message Here...";

        //Set up the Connection Configuration which holds channel information
        ConnectionConfig config = new ConnectionConfig();
        //Add a reliable channel mode to the configuration (all messages delivered, not particularly in order)
        m_ChannelID = config.AddChannel(QosType.Reliable);
        //Create a new Host information based on the configuration created, and the maximum connections allowed (20)
        m_HostTopology = new HostTopology(config, 20);
        //Initialise the NetworkTransport
        NetworkTransport.Init();

        //Call the ClientButton function when you click the Button
        m_ClientButton.onClick.AddListener(ClientButton);
        //Call the ServerButton function when you click the server Button
        m_ServerButton.onClick.AddListener(ServerButton);
        //Call the SendMessageField function when you press the return/enter key on the Input Field
        m_InputField.onEndEdit.AddListener(delegate {SendMessageField(); });
    }

    //This is the function that serializes the message before sending it
    void SendMyMessage(string textInput)
    {
        byte error;
        byte[] buffer = new byte[1024];
        Stream message = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        //Serialize the message
        formatter.Serialize(message , textInput);

        //Send the message from the "client" with the serialized message and the connection information
        NetworkTransport.Send(m_ClientSocket , m_ConnectionID , m_ChannelID , buffer , (int)message.Position , out error);

        //If there is an error, output message error to the console
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("Message send error: " + (NetworkError)error);
        }
    }

    void Update()
    {
        //These are the variables that are replaced by the incoming message
        int outHostId;
        int outConnectionId;
        int outChannelId;
        byte[] buffer = new byte[1024];
        int receivedSize;
        byte error;

        //Set up the Network Transport to receive the incoming message, and decide what type of event
        NetworkEventType eventType = NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId, buffer, buffer.Length, out receivedSize, out error);

        switch (eventType)
        {
            //Use this case when there is a connection detected
            case NetworkEventType.ConnectEvent:
            {
                //Call the function to deal with the received information
                OnConnect(outHostId, outConnectionId, (NetworkError)error);
                break;
            }

            //This case is called if the event type is a data event, like the serialized message
            case NetworkEventType.DataEvent:
            {
                //Call the function to deal with the received data
                OnData(outHostId, outConnectionId, outChannelId, buffer, receivedSize, (NetworkError)error);
                break;
            }

            case NetworkEventType.Nothing:
                break;

            default:
                //Output the error
                Debug.LogError("Unknown network message type received: " + eventType);
                break;
        }

        //Make sure there is a connection first before showing the Input Field
        m_InputField.gameObject.SetActive(m_ClientsActive);
        //Remove Buttons when there is a connection
        if (m_ClientsActive)
        {
            m_ClientButton.gameObject.SetActive(false);
            m_ServerButton.gameObject.SetActive(false);
        }
    }

    //This function is called when a connection is detected
    void OnConnect(int hostID, int connectionID, NetworkError error)
    {
        //Output the given information to the console
        Debug.Log("OnConnect(hostId = " + hostID + ", connectionId = "
            + connectionID + ", error = " + error.ToString() + ")");
        //There was a connection, so make this return true
        m_ClientsActive = true;
    }

    //This function is called when data is sent
    void OnData(int hostId, int connectionId, int channelId, byte[] data, int size, NetworkError error)
    {
        //Here the message being received is deserialized and output to the console
        Stream serializedMessage = new MemoryStream(data);
        BinaryFormatter formatter = new BinaryFormatter();
        string message = formatter.Deserialize(serializedMessage).ToString();

        //Output the deserialized message as well as the connection information to the console
        Debug.Log("OnData(hostId = " + hostId + ", connectionId = "
            + connectionId + ", channelId = " + channelId + ", data = "
            + message + ", size = " + size + ", error = " + error.ToString() + ")");

        m_InputField.text = "data = " + message;
    }

    void ClientButton()
    {
        byte error;
        m_ClientSocket = NetworkTransport.AddHost(m_HostTopology);
        //Connect the hostID to the port
        m_ConnectionID = NetworkTransport.Connect(m_ClientSocket, "127.0.0.1", 54321, 0, out error);
        //If any errors arise from the connection, output it in the console
        if ((NetworkError)error != NetworkError.Ok)
        {
            Debug.Log("Error: " + (NetworkError)error);
        }
    }

    void ServerButton()
    {
        byte error;
        //Open the sockets for sending and receiving the messages on port 54321
        m_ServerSocket = NetworkTransport.AddHost(m_HostTopology, 54321);
        //Connect the "server"
        NetworkTransport.Connect(m_ServerSocket, "127.0.0.1", 54321, 0, out error);
    }

    void SendMessageField()
    {
        //Check to see if a client has been chosen yet before showing the TextField and the Button
        //The Text from the InputField is stored here
        myText = m_InputField.text;

        //Send the data message
        SendMyMessage(myText);
    }
}