﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace net.derpaul.tf
{
    public class MQTT : IDataSink
    {
        /// <summary>
        /// Instance of M2Mqtt client
        /// </summary>
        private MqttClient MqttClient;

        /// <summary>
        /// Flags successful initialization
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Queue with measurement values to publish
        /// </summary>
        private Queue<MeasurementValue> DataQueue { get; set; }

        /// <summary>
        /// Published measurement values waiting for acknowledge
        /// </summary>
        private Dictionary<string, MeasurementValue> AcknowledgeList { get; set; }

        /// <summary>
        /// Disconnect from MQTT broker
        /// </summary>
        public void Shutdown()
        {
            MqttClient.Disconnect();
        }

        /// <summary>
        /// Initialize MQTT client and connect to broker
        /// </summary>
        /// <returns>true on success otherwise false</returns>
        public bool Init()
        {
            bool success = false;

            try
            {
                DataQueue = new Queue<MeasurementValue>();
                AcknowledgeList = new Dictionary<string, MeasurementValue>();

                MqttClient = new MqttClient(MQTTConfig.Instance.BrokerIP);
                MqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                MqttClient.Connect(MQTTConfig.Instance.MQTTClientIDClient);
                MqttClient.Subscribe(new string[] { MQTTConfig.Instance.MQTTTopicHandshake }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                success = MqttClient.IsConnected;
                IsInitialized = success;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Cannot connect to broker [{MQTTConfig.Instance.BrokerIP}] => [{e.Message}]");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Transform each result in a JSON string and publish string to topic
        /// </summary>
        /// <param name="SensorValues">List of all sensor values</param>
        public void HandleValues(List<MeasurementValue> SensorValues)
        {
            foreach (var currentMeasurementValue in SensorValues)
            {
                PublishSingleValue(currentMeasurementValue);
            }
        }

        /// <summary>
        /// Publish single measurement data
        /// </summary>
        /// <param name="dataToPublish"></param>
        private void PublishSingleValue(MeasurementValue dataToPublish)
        {
            var dataJSON = dataToPublish.ToJSON();
            MqttClient.Publish(MQTTConfig.Instance.MQTTTopicData, Encoding.ASCII.GetBytes(dataJSON));

            lock (AcknowledgeList)
            {
                AcknowledgeList.Add(dataToPublish.ToHash(), dataToPublish);
            }
        }

        /// <summary>
        /// Handles response of server and removes measurement values from internal acknowledge list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string messageHash = Encoding.UTF8.GetString(e.Message);

            lock (AcknowledgeList)
            {
                if (AcknowledgeList.ContainsKey(messageHash))
                {
                    AcknowledgeList.Remove(messageHash);
                }
                System.Console.WriteLine($"values not acknowledged: {AcknowledgeList.Count}");
            }
        }
    }
}