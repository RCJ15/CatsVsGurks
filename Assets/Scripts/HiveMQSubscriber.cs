using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;

public class HiveMQSubscriber : MonoBehaviour
{
    [Header("Broker Settings")]
    public string brokerHost = "broker.hivemq.com";
    public int brokerPort = 1883;

    [Header("Subscribe Topics")]
    public string topic_beamMode = "ludvig/beamMode";
    public string topic_button = "ludvig/button";
    public string topic_selected = "ludvig/selected";
    public string topic_pot = "ludvig/pot";

    [Header("Publish Topic")]
    public string topic_publish = "ludvig/output";

    private IMqttClient client;
    private MqttFactory factory;

    public Menu menu;

    private bool toggleMenuRequested = false;

    async void Start()
    {
        factory = new MqttFactory();
        client = factory.CreateMqttClient();

        client.ApplicationMessageReceivedAsync += OnMessageReceived;

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerHost, brokerPort)
            .Build();

        try
        {
            await client.ConnectAsync(options);
            Debug.Log("Connected to broker");

            await SubscribeToTopic(topic_beamMode);
            await SubscribeToTopic(topic_button);
            await SubscribeToTopic(topic_selected);
            await SubscribeToTopic(topic_pot);
        }
        catch (Exception ex)
        {
            Debug.LogError("Connection failed: " + ex.Message);
        }
    }

    void Update()
    {
        if (toggleMenuRequested)
        {
            toggleMenuRequested = false;

            if (menu != null)
            {
                menu.OpenCloseMenu();
            }
            else
            {
                Debug.LogError("Menu reference is not assigned in Inspector!");
            }
        }
    }

    async Task SubscribeToTopic(string topic)
    {
        var subscribeOptions = factory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic(topic); })
            .Build();

        await client.SubscribeAsync(subscribeOptions);
        Debug.Log("Subscribed to: " + topic);
    }

    private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        string topic = e.ApplicationMessage?.Topic ?? "";
        string payload = "";

        if (e.ApplicationMessage?.Payload != null)
        {
            payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        }

        Debug.Log("Received from topic [" + topic + "]: " + payload);

        HandleMessage(topic, payload);

        return Task.CompletedTask;
    }

    void HandleMessage(string topic, string payload)
    {
        payload = payload.Trim();

        if (topic == topic_beamMode)
        {
            Debug.Log("BeamMode signal: " + payload);

            if (payload == "1")
            {
                Debug.Log("Beam mode = 1");
            }
            else if (payload == "2")
            {
                Debug.Log("Beam mode = 2");
            }
        }
        else if (topic == topic_button)
        {

            if (payload == "1")
            {
                toggleMenuRequested = true;
            }
        }
        else if (topic == topic_selected)
        {

            if (payload == "3")
            {

            }
            else if (payload == "4")
            {

            }
        }
        else if (topic == topic_pot)
        {
            Debug.Log("Pot value: " + payload);

            if (float.TryParse(payload, out float potValue))
            {
                Debug.Log("Parsed pot value: " + potValue);
            }
        }
    }

    public async void PublishMessage(string message)
    {
        if (client == null || !client.IsConnected)
        {
            Debug.LogWarning("Cannot publish, client is not connected.");
            return;
        }

        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic_publish)
            .WithPayload(message)
            .Build();

        try
        {
            await client.PublishAsync(mqttMessage);
            Debug.Log("Published to [" + topic_publish + "]: " + message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Publish failed: " + ex.Message);
        }
    }

    async void OnApplicationQuit()
    {
        if (client != null && client.IsConnected)
        {
            await client.DisconnectAsync();
        }
    }
}