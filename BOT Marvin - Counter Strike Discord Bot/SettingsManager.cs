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
        private static string _token = null;
        private static bool isInitialized = false;

        private static XmlDocument doc;

        public static string Token { get {
                if(_token == null)
                {
                    Logger.Log(LogLevel.ERROR, "Token has not been set yet! Please manually enter the discord API token.");
                    ConsoleLogWriter.ConsoleAvailable.WaitOne();
                    Console.Write("Token: ");
                    _token = Console.ReadLine().Trim();
                }
                return _token; 
                
            }
        }

        private static List<string> _mongoips;
        public static List<string> MongoIPs
        {
            get
            {
                if (_mongoips == null)
                {
                    if (!isInitialized)
                        Initialize();
                    _mongoips = new List<string>();
                    foreach (XmlNode x in doc.GetElementsByTagName("mongo-ip"))
                    {
                        if (x.NodeType == XmlNodeType.Element)
                        {
                            _mongoips.Add(x.InnerXml);
                        }
                    }
                }
                return _mongoips;
            }
        }

        private static void writeSettings()
        {
            doc.Save(File.OpenWrite(SettingsXML));
        }

        public static void Initialize(string tkn = null, string settingsPath = null, string mip = null)
        {
            _token = tkn;
            SettingsXML = settingsPath == null ? SettingsXML : settingsPath;
            if(mip != null)
                _mongoips.Insert(0, mip);
            doc = new XmlDocument();
            try
            {
                doc.Load(SettingsXML);
            }catch(Exception)
            {
                SettingsXML = "publish/settings.xml";
                try
                {
                    doc.Load(SettingsXML);
                }
                catch (Exception)
                {
                    SettingsXML = "../settings.xml";
                    try
                    {
                        doc.Load(SettingsXML);
                    }
                    catch (Exception)
                    {
                        Logger.Log(LogLevel.ERROR, "Settings cannot be found! Please manually enter the path....");
                        while (true)
                        {
                            Console.Write("Path: ");
                            SettingsXML = Console.ReadLine();
                            try
                            {
                                doc.Load(SettingsXML);
                                break;
                            }
                            catch (XmlException e)
                            {
                                Logger.Log(LogLevel.FATAL, "Settings XML is invalid! Message: '" + e.Message + "'. Exiting....");
                                Environment.Exit(1);
                            }
                            catch (Exception)
                            {
                                Logger.Log(LogLevel.ERROR, "Invalid Path! Try again:");
                            }
                        }
                    }
                    
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

        public static bool IsDevEnv = false;
    }
}
