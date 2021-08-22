using LightBlueFox.Util.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public static class SettingsManager
    {
        private static string SettingsXML = "settings.xml";

        private static string tokenPath;
        private static string _token = null;
        private static bool isInitialized = false;

        private static XmlDocument doc;

        public static string Token { get {
                if(_token == null)
                {
                    updateTokenPath();
                    while (true)
                    {
                        try
                        {
                            _token = File.ReadAllText(tokenPath).Trim();
                            break;
                        }
                        catch (Exception)
                        {
                            Logger.Log(LogLevel.ERROR, "Token not at file path! Please enter the path to the token:");
                            ConsoleLogWriter.ConsoleAvailable.WaitOne();
                            Console.Write("Path: ");
                            tokenPath = Console.ReadLine();
                            ConsoleLogWriter.ConsoleAvailable.Set();
                        }
                    }
                    doc.GetElementsByTagName("tokenPath")[0].InnerText = tokenPath;
                    writeSettings();
                }
                return _token;           
            }
        }

        private static void writeSettings()
        {
            doc.Save(File.OpenWrite(SettingsXML));
        }

        private static void updateTokenPath()
        {
            if (!isInitialized)
                Initialize();
            tokenPath = doc.GetElementsByTagName("tokenPath")[0].InnerText;
        }

        private static void Initialize()
        {
            
            doc = new XmlDocument();
            try
            {
                doc.Load(SettingsXML);
            }catch(Exception)
            {
                SettingsXML = "../../../settings.xml";
                try
                {
                    doc.Load(SettingsXML);
                }
                catch (FileNotFoundException)
                {
                    Logger.Log(LogLevel.FATAL, "Settings cannot be found! Exiting....");
                    Environment.Exit(1);
                }catch(XmlException e)
                {
                    Logger.Log(LogLevel.FATAL, "Settings XML is invalid! Message: '" + e.Message + "'. Exiting....");
                    Environment.Exit(1);
                }
                
            }
            isInitialized = true;

        }
        private static List<ulong> _channelID;
        public static List<ulong> ChannelIDs
        {
            get {
                if(_channelID == null)
                {

                    _channelID = new List<ulong>();
                    foreach (XmlNode x in doc.GetElementsByTagName("channel"))
                    {
                        if (x.NodeType == XmlNodeType.Element)
                        {
                            _channelID.Add(Convert.ToUInt64(x.InnerXml));
                        }
                    }
                }
                return _channelID;
            }
        }
    }
}
