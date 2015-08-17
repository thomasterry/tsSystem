using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TSTrade
{   
    class Program
    {
        static List<string> shIDList = new List<string>();
        static List<string> shNameList = new List<string>();
        static List<string> szIDList = new List<string>();
        static List<string> szNameList = new List<string>();
        static List<Stock> stockList = new List<Stock>();
        static List<Stock> ztbList = new List<Stock>();
        static List<Stock> buyList = new List<Stock>();
        static List<Stock> prebuyList = new List<Stock>();

        static string connectionString = "Server=localhost;Initial Catalog=stock;integrated security = sspi";

        static void Main(string[] args)
        {
            initList();

            getZTB();

            getBuy();

            getCurrentVal();

            buy();

            Console.WriteLine("init finish");
        }

        static void getCurrentVal()
        {
            WebRequest myReq;
            WebResponse myRes;
            string result;


            //sh
            for (int i = 0; i < shIDList.Count; i += 10)
            {
                string url = "http://hq.sinajs.cn/list=";
                for (int j = 0; j < 9; j++)
                {
                    if (j + i >= shIDList.Count) break;
                    url = url + "sh" + shIDList[j + i] + ",";
                }
                myReq = WebRequest.Create(url);
                myRes = myReq.GetResponse();
                Stream resStream = myRes.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, Encoding.Default);

                int index = i;
                while ((result = sr.ReadLine()) != null)
                {
                    int start = result.IndexOf("\"");
                    int end = result.LastIndexOf("\"");
                    string[] val = result.Substring(start + 1, end - start - 1).Split(',');

                    if (val.Length < 5)
                    {
                        continue;  //tuishi
                    }
                    Console.WriteLine(val[0]);
                    Stock stock = new Stock();
                    stock.ID = shIDList[index];
                    stock.Name = val[0];
                    stock.Price = val[3];
                    stock.SPJ = val[2];
                    stock.CJL = Math.Ceiling((double.Parse(val[9]) / 10000)).ToString();
                    stock.Buy1 = val[10];
                    stock.Sale1 = val[20];
                    stock.Time = val[31];
                    stock.ZF = (double.Parse(stock.Price) - double.Parse(stock.SPJ)) * 100 / double.Parse(stock.SPJ);
                    stockList.Add(stock);
                    index += 1;

                }

            }


            //SZ
            for (int i = 0; i < szIDList.Count; i += 10)
            {
                string url = "http://hq.sinajs.cn/list=";
                for (int j = 0; j < 9; j++)
                {
                    if (j + i >= szIDList.Count) break;
                    url = url + "sz" + szIDList[j + i] + ",";
                }
                myReq = WebRequest.Create(url);
                myRes = myReq.GetResponse();
                Stream resStream = myRes.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, Encoding.Default);

                int index = i;
                while ((result = sr.ReadLine()) != null)
                {
                    int start = result.IndexOf("\"");
                    int end = result.LastIndexOf("\"");
                    string[] val = result.Substring(start + 1, end - start - 1).Split(',');

                    if (val.Length < 5)
                    {
                        continue;  //tuishi
                    }
                    Console.WriteLine(val[0]);
                    Stock stock = new Stock();
                    stock.ID = szIDList[index];
                    stock.Name = val[0];
                    stock.Price = val[3];
                    stock.SPJ = val[2];
                    stock.CJL = Math.Ceiling((double.Parse(val[9]) / 10000)).ToString();
                    stock.Buy1 = val[10];
                    stock.Sale1 = val[20];
                    stock.Time = val[31];

                    stock.ZF = (double.Parse(stock.Price) - double.Parse(stock.SPJ)) * 100 / double.Parse(stock.SPJ);
                    stockList.Add(stock);


                    index += 1;

                }

            }

            Console.WriteLine("Get d-val finish");

        }
        static void getZTB()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand command = connection.CreateCommand();

                command.Connection = connection;

                string cmd = "SELECT * from [ZTB]";
                command.CommandText = cmd;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Stock stock = new Stock();
                    stock.ID = reader["ID"].ToString().Trim();
                    stock.Name = reader["Name"].ToString().Trim();
                    stock.Price = reader["Price"].ToString().Trim();
                    stock.SPJ = reader["spj"].ToString().Trim();
                    stock.CJL = reader["cjl"].ToString().Trim();
                    stock.Buy1 = reader["buy1"].ToString().Trim();
                    stock.Sale1 = reader["sale1"].ToString().Trim();
                    stock.Time = reader["time"].ToString().Trim();
                    stock.ZF = 10;
                    ztbList.Add(stock);
                }
            }
        }

        static void getBuy()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand command = connection.CreateCommand();

                command.Connection = connection;

                string cmd = "SELECT * from [Trade]";
                command.CommandText = cmd;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Stock stock = new Stock();
                    stock.ID = reader["ID"].ToString().Trim();
                    stock.Name = reader["Name"].ToString().Trim();   
                    stock.Price = reader["Price"].ToString().Trim();
                    stock.Time = reader["time"].ToString().Trim();
                    prebuyList.Add(stock);
                }
            }
        }

        static void buy()
        {
            foreach(Stock stock in stockList)
            {
                if(stock.ZF > -4 && stock.ZF < -2 && !prebuyList.Exists(s => s.ID.Equals(stock.ID)))    //<-5%
                {
                    if(ztbList.Exists(s => s.ID.Equals(stock.ID)))
                    {
                        //buy
                        Console.WriteLine(stock.Name);
                        buyList.Add(stock);
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand command = connection.CreateCommand();

                command.Connection = connection;

                try
                {
                    foreach (Stock stock in buyList)
                    { 
                        string cmd = "INSERT INTO [Trade]([ID],Name,Price,[Day],[time]) " +
                            "VALUES('" + stock.ID + "'" +
                            ",'" + stock.Name + "'" +
                            ",'" + stock.Price + "'" +
                            ",'" + DateTime.Now.ToString("yyyyMMdd") + "'" +                           
                            ",'" + stock.Time + "')";

                        command.CommandText = cmd;
                        command.ExecuteNonQuery();                        
                    }                   
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error" + ex.Message);
                }
            }


        }
        static void initList()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlCommand command = connection.CreateCommand();

                command.Connection = connection;

                string cmd = "SELECT * from stList";
                command.CommandText = cmd;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader["ID"].ToString().Trim();
                    string name = reader["Name"].ToString().Trim();
                    if (id.StartsWith("6"))
                    {
                        shIDList.Add(id);
                        shNameList.Add(name);
                    }
                    else if (id.StartsWith("0") || id.StartsWith("3"))
                    {
                        szIDList.Add(id);
                        szNameList.Add(name);
                    }

                }

            }
        }
    }
}
