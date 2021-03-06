﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tinkerforge;

namespace net.derpaul.tf
{
    /// <summary>
    /// Class to deal with collection of data sink plugins
    /// </summary>
    internal class PluginHandler
    {
        /// <summary>
        /// The path to the plugins
        /// </summary>
        private string _PluginPath { get; set; }

        /// <summary>
        /// List of data sink plugins
        /// </summary>
        private List<IDataSink> _DataSinkPlugins { get; set; }

        /// <summary>
        /// Constructor of TF handler
        /// </summary>
        /// <param name="pluginPath">Path to plugins</param>
        internal PluginHandler(string pluginPath)
        {
            _PluginPath = pluginPath;
        }

        /// <summary>
        /// Initialize datasink plugins
        /// </summary>
        /// <returns>true on success, otherwise false</returns>
        private bool InitDataSinkPlugins()
        {
            _DataSinkPlugins = PluginLoader<IDataSink>.TFPluginsLoad(_PluginPath, ServerConfig.Instance.PluginProductName);
            if (_DataSinkPlugins.Count == 0)
            {
                System.Console.WriteLine($"{nameof(InitDataSinkPlugins)}: No datasink plugins found in [{_PluginPath}].");
                return false;
            }

            foreach (var currentPlugin in _DataSinkPlugins)
            {
                try
                {
                    currentPlugin.IsInitialized = currentPlugin.Init();
                    System.Console.WriteLine($"{nameof(InitDataSinkPlugins)}: Initialized [{currentPlugin.Name}] plugin.");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"{nameof(InitDataSinkPlugins)}: Cannot init plugin [{currentPlugin.Name}] => [{e.Message}]");
                    System.Console.WriteLine($"{nameof(InitDataSinkPlugins)}: InnerException => [{e.InnerException}]");
                }
            }

            return true;
        }

        /// <summary>
        /// Initialize all plugins
        /// </summary>
        /// <returns>true on success, otherwise false</returns>
        internal bool Init()
        {
            return InitDataSinkPlugins();
        }

        /// <summary>
        /// Feed each datasink plugin with sensor data
        /// </summary>
        /// <param name="SensorValue"></param>
        internal void HandleValue(MeasurementValue SensorValue)
        {
            foreach (var currentPlugin in _DataSinkPlugins)
            {
                if (!currentPlugin.IsInitialized)
                {
                    continue;
                }

                currentPlugin.HandleValue(SensorValue);
            }
        }

        /// <summary>
        /// Shutdown all datasink plugins
        /// </summary>
        internal void Shutdown()
        {
            foreach (var currentPlugin in _DataSinkPlugins)
            {
                if (!currentPlugin.IsInitialized)
                {
                    continue;
                }

                currentPlugin.Shutdown();
            }
        }
    }
}