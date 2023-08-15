using System;

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

                    //string username = "Campus360dev";
                    //string apiKey = "6645c5ec3750b1cd435459e36073c95c736586ed7b7ccdcf18c4cfd5cc5ebc9c";

                    string msgid = dr["ID"].ToString().Trim();
                    string telNo = dr["telephone"].ToString();
                    string recipients = "+" + telNo;
                    string message = dr["content"].ToString();

                    AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);
                    try
                    {

                        dynamic results = gateway.sendMessage(recipients, message, username);

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
                dr.Close(); dr.Dispose(); dr = null;
            }
        }
    }
}
