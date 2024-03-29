﻿using POsSMS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace POsSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            sqlconnectionclass sqlConnector = new sqlconnectionclass();
            var query = "SELECT * FROM MessageConfigs WHERE CLOSED = 0";
            SqlDataReader configReader = sqlConnector.ReadDB(query);
            var configs = new List<MessageConfig>();
            if(configReader.HasRows)
            {
                while(configReader.Read())
                {
                    string username = configReader["username"].ToString();
                    string apiKey = configReader["apiKey"].ToString();
                    string senderId = configReader["SenderId"].ToString();
                    string saccocode = configReader["saccocode"].ToString();

                    configs.Add(new MessageConfig
                    {
                        Username = username,
                        ApiKey = apiKey,
                        SenderId = senderId,
                        Saccocode = saccocode,
                    });
                }

                configReader.Close(); configReader.Dispose(); configReader = null;
            }

            foreach(var config in configs)
            {
                config.SenderId = string.IsNullOrEmpty(config.SenderId) ? null : config.SenderId;
                query = "SELECT * FROM Messages WHERE MsgType = 'Outbox' AND Code='" + config.Saccocode + "'";
                sqlConnector = new sqlconnectionclass();
                SqlDataReader messageReader = sqlConnector.ReadDB(query);
                if (messageReader.HasRows)
                {
                    while (messageReader.Read())
                    {
                        //string username = "AMTECH_TECH";
                        //string apiKey = "752ae8eed0ea2e06bd5c56a75d5c2ab3e9c5f86ab590fc27b6e1059c6e156665";

                        string msgid = messageReader["ID"].ToString().Trim();
                        string telNo = messageReader["telephone"].ToString();
                        string recipients = "+" + telNo;
                        string message = messageReader["content"].ToString();

                        AfricasTalkingGateway gateway = new AfricasTalkingGateway(config.Username, config.ApiKey);
                        try
                        {
                            dynamic results = gateway.sendMessage(recipients, message, config.SenderId);
                            foreach (dynamic result in results)
                            {
                                if ((string)result["status"] == "Success")
                                {
                                    string updatestatus = "update Messages set msgtype='Inbox' where ID='" + msgid + "'";
                                    new sqlconnectionclass().WriteDB(updatestatus);
                                    Console.WriteLine("message Send to '" + recipients + "'");
                                }
                            }
                        }
                        catch (AfricasTalkingGatewayException e)
                        {
                            //
                        }
                    }
                    messageReader.Close(); messageReader.Dispose(); messageReader = null;
                }
            }
        }
    }
}
