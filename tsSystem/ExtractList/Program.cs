using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExtractList
{
    class Program
    {
        static List<string> shList = new List<string>();
        static List<string> szList = new List<string>();
        static void Main(string[] args)
        {
            string url = "http://bbs.10jqka.com.cn/codelist.html";
            readUri(url);
            export();
        }

        static void readUri(string url)
        {
            string rl;
            WebRequest myReq = WebRequest.Create(url);
            WebResponse myRes = myReq.GetResponse();
            Stream resStream = myRes.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, Encoding.Default);
            StringBuilder sb = new StringBuilder();
            int flag = 0;
            while ((rl = sr.ReadLine()) != null)
            {
                if(flag == 0 && rl.IndexOf("<a target=\"_blank\" id=\"sh\" name=\"sh\"></a>") >= 0)
                {
                    flag = 1;
                    continue;
                }

                if(rl.IndexOf("<a target=\"_blank\" id=\"sz\" name=\"sz\"></a>") >= 0)
                {
                    flag = 2;
                    continue;
                }
                if (flag == 1)
                {
                    if (rl.IndexOf("<li><a href=\"http://bbs.10jqka.com.cn/sh") >= 0)
                    {
                        int start = rl.IndexOf("\">");
                        int end = rl.IndexOf("</a>");
                        string text = rl.Substring(start + 2, end - start - 2);
                        shList.Add(text);
                    }
                }
                else if(flag ==2)
                {
                    if (rl.IndexOf("<li><a href=\"http://bbs.10jqka.com.cn/sz") >= 0)
                    {
                        int start = rl.IndexOf("\">");
                        int end = rl.IndexOf("</a>");
                        string text = rl.Substring(start + 2, end - start - 2);
                        szList.Add(text);
                    }
                }
                                  
            }
            
            myRes.Close();
        }

        static void export()
        {
            string connectionString = "Server=localhost;Initial Catalog=stock;integrated security = sspi";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;               

                transaction = connection.BeginTransaction("CreateStockListTransaction");

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    // sh 
                    for (int i = 0; i < shList.Count; i++)
                    {
                        string[] val = shList[i].Split(' ');
                        string cmd = "INSERT INTO stList (ID,name) VALUES('" + val[1] + "','" + val[0] + "')";
                        command.CommandText = cmd;
                        command.ExecuteNonQuery();
                    }

                    // sz
                    for (int i = 0; i < szList.Count; i++)
                    {
                        string[] val = szList[i].Split(' ');
                        string cmd = "INSERT INTO stList (ID,name) VALUES('" + val[1] + "','" + val[0] + "')";
                        command.CommandText = cmd;                        
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    Console.WriteLine(" OK");
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error" + ex.Message);
                }

            }

        }
    }
}
