using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace signonsms
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Data.SqlClient.SqlDataReader dr;
            sqlconnectionclass rd = new sqlconnectionclass();
            dr = rd.ReadDB("SELECT * FROM Messages WHERE MsgType = 'Outbox'");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string username = "AMTECH_TECH";
                    string apiKey = "752ae8eed0ea2e06bd5c56a75d5c2ab3e9c5f86ab590fc27b6e1059c6e156665";

                    string msgid = dr["ID"].ToString().Trim();
                    string phon = dr["telephone"].ToString();
                   // string recipients = "+254" + dr["Telephone"].ToString().Substring(1);
                    string recipients = "+" + dr["Telephone"].ToString();

                    string message = dr["content"].ToString();

                    // Specify your AfricasTalking shortCode or sender id
                    string from = "AMTECH_TECH";

                    AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);
                    try
                    {

                        dynamic results = gateway.sendMessage(recipients, message, from);

                        foreach (dynamic result in results)
                        {
                            if ((string)result["status"] == "Success")
                            {
                                string updatestatus = "update Messages set msgtype='Inbox' where ID='" + msgid + "'";
                                new sqlconnectionclass().WriteDB(updatestatus);
                                Console.WriteLine("message Send to '"+ recipients +"'");
                            }
                        }
                    }
                    catch (AfricasTalkingGatewayException)
                    {
                        //
                    }
                }
                dr.Close(); dr.Dispose(); dr = null;
            }
        }
    }
}
