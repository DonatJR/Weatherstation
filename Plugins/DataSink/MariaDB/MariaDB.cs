﻿using System;

namespace net.derpaul.tf
{
    /// <summary>
    /// Data sink - sending data using SQLite
    /// </summary>
    public class MariaDB : IDataSink
    {
        /// <summary>
        /// Get the name of class
        /// </summary>
        public string Name { get { return this.GetType().Name; } }

        /// <summary>
        /// Flags successful initialization
        /// </summary>
        public bool IsInitialized { get; set; } = false;

        /// <summary>
        /// Disconnect from MQTT broker
        /// </summary>
        public void Shutdown()
        {
        }

        /// <summary>
        /// Initialize MQTT client and connect to broker
        /// </summary>
        /// <returns>true on success otherwise false</returns>
        public bool Init()
        {
            bool success = false;

            return success;
        }

        /// <summary>
        /// Transform each result in a JSON string and publish string to topic
        /// </summary>
        /// <param name="SensorValue">Sensor value</param>
        public void HandleValue(MeasurementValue SensorValue)
        {
            HandleValueData(SensorValue);
        }

        /// <summary>
        /// Deals with measurement value
        /// </summary>
        /// <param name="sensorValue"></param>
        private void HandleValueData(MeasurementValue sensorValue)
        {
        }
    }
}