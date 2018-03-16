﻿using System;
using System.IO;

namespace net.derpaul.tf
{
    /// <summary>
    /// Main program to handle data of Tinkerforge weather station
    /// </summary>
    internal class Client
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        private static void Main()
        {
            var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ClientConfig.Instance.PluginPath);
            PluginHandler pluginHandler = new PluginHandler(pluginPath, ClientConfig.Instance.Host, ClientConfig.Instance.Port);

            if (pluginHandler.Init() == false)
            {
                return;
            }

            UtilsTF.WaitUntilStart();
            do
            {
                while (!Console.KeyAvailable)
                {
                    pluginHandler.HandleValues(pluginHandler.ValuesRead());
                    UtilsTF.WaitNMilliseconds(ClientConfig.Instance.Delay);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            pluginHandler.Shutdown();

            Environment.Exit(0);
        }
    }
}