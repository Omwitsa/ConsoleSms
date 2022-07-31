using System;
using System.IO;
using System.Data;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Security.Cryptography;
using System.Threading;
using SQL_KCA = System.Data.SqlClient;
using System.Data.SqlClient;
using System.Configuration;



namespace signonsms
{
    class sqlconnectionclass:IDisposable
    {
        private string mConnectionString = "";
        private SQL_KCA.SqlConnection mDB;
        private string inserttoministries;
        // ModifyRegistry Reg = new ModifyRegistry();

        /// <summary>
        /// Class Constructor
        /// initializes the connection [opens the database].
        /// </summary>
        public sqlconnectionclass()
        {
            try
            {
                this.mConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectme"].ConnectionString;
                this.mDB = new SQL_KCA.SqlConnection(this.mConnectionString);
                this.mDB.Open();
            }
            catch (Exception)
            {
                throw; /* bubble the error to the active document,
* where the error is caught and resolved */
            }
        }

        public sqlconnectionclass(string inserttoministries)
        {
            // TODO: Complete member initialization
            this.inserttoministries = inserttoministries;
        }
        public SQL_KCA.SqlDataReader ReadDB(string vSQL)
        {
            SQL_KCA.SqlDataReader r = null;

            try
            {
                SQL_KCA.SqlCommand vCMD = new System.Data.SqlClient.SqlCommand(vSQL, this.mDB);
                r = vCMD.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                //string s = ex.Message;
                throw; /* bubble the error to the active document,
* where the error is caught and resolved */
            }
            return r;
        }

        /// <summary>
        /// DA: This method is used for reading purposes only.
        /// NB: Only for reading NOT writing.
        /// The database will have a shared lock.
        /// </summary> 
        /// <param name="vSQL">SQL statement 2B executed.</param>m>
        /// <returns>
        /// returns a data adapter containing the execution
        /// results of the sql select statement
        /// </returns>
        public SQL_KCA.SqlDataAdapter ReadDB2(string vSQL)
        {
            SQL_KCA.SqlDataAdapter r = null;

            try
            {
                r = new SQL_KCA.SqlDataAdapter(vSQL, this.mDB);
                r.AcceptChangesDuringFill = false;
                r.AcceptChangesDuringUpdate = false;

            }
            catch (Exception)
            {
                //string s = ex.Message;
                throw; /* bubble the error to the active document,
* where the error is caught and resolved */
            }
            return r;
        }

        /// <summary>
        /// This method is used to update/insert/delete
        /// records using the appropriate SQL Statements.
        /// The database will have an exclusive lock.
        ///
        ///
        ///
        /// </summary>
        ///
        /// <param name="vSQL">SQL Statement 2B executed</param>
        /// <param name="vCryptographyDetails">
        /// the parameters used to encrypt the sql statement</param>
        public void WriteDB(string vSQL)
        {
            DataSet vDS = new DataSet();
            try
            {
                vDS.EnforceConstraints = true;

                SQL_KCA.SqlDataAdapter vDA = new SQL_KCA.SqlDataAdapter
                (vSQL, this.mConnectionString);

                vDA.AcceptChangesDuringFill = true;
                vDA.Fill(vDS);
            }
            catch (Exception)
            {
                vDS.RejectChanges();
                vDS.Dispose();
                throw;/*  bubble the error to the active document,
  * where the error is caught and resolved */
            }
            finally
            {
                this.mDB.Close();
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.mDB != null)
                    if (this.mDB.State != ConnectionState.Open)
                        this.mDB.Close();

                this.mDB.Dispose();
                this.mDB = null;
            }
            catch (Exception ex)
            { ex.Data.Clear(); }
        }//end of sqlconnectionclass

    }
}
