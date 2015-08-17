using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSTrade
{   
    public class Stock
    {
        private string m_id;  //daima
        private string m_name;
        private string m_price;
        private string m_cjl;  //chen jiao liang
        private string m_spj; // yesterday shou pan jia
        private string m_buy1; //mai 1 jia
        private string m_sale1; //mai 1 jia
        private double m_zf;    //zhang fu
        private string m_time;

        public string ID
        {
            get
            {
                return m_id;
            }

            set
            {
                m_id = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }

            set
            {
                m_name = value;
            }
        }
        public string Price
        {
            get
            {
                return m_price;
            }

            set
            {
                m_price = value;
            }
        }

        public string CJL
        {
            get
            {
                return m_cjl;
            }

            set
            {
                m_cjl = value;
            }
        }
        public string SPJ
        {
            get
            {
                return m_spj;
            }

            set
            {
                m_spj = value;
            }
        }
        public string Buy1
        {
            get
            {
                return m_buy1;
            }

            set
            {
                m_buy1 = value;
            }
        }
        public string Sale1
        {
            get
            {
                return m_sale1;
            }

            set
            {
                m_sale1 = value;
            }
        }
        public double ZF
        {
            get
            {
                return m_zf;
            }

            set
            {
                m_zf = value;
            }
        }

        public string Time
        {
            get
            {
                return m_time;
            }

            set
            {
                m_time = value;
            }
        }
    }
}
