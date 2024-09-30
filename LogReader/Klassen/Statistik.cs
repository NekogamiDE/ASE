using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class Statistik
    {
        private List<DamageType> deal;
        private List<DamageType> take;
        private int k;
        private int d;
        private int pts;

        public Statistik()
        {
            deal = new List<DamageType>();
            take = new List<DamageType>();
            k = 0;
            d = 0;
            pts = 0;
        }
        public void AddDeal(DamageType dtype)
        {
            this.deal.Add(dtype);
        }
        public void AddTake(DamageType dtype)
        {
            this.take.Add(dtype);
        }
        public List<DamageType> GetDeal()
        {
            return this.deal;
        }
        public List<DamageType> GetTake()
        {
            return this.take;
        }
        public string GetDealAusgabe() //Standard, ohne zusätzlichen Infos
        {
            double dn = 0;
            double di = 0;
            for(int i = 0; i < deal.Count; i++)
            {
                dn += deal[i].GetN();
                di += deal[i].GetI();
            }
            return dn.ToString() + "/" + di.ToString();
        }
        public string GetTakeAusgabe() //Standard, ohne zusätzlichen Infos
        {
            double tn = 0;
            double ti = 0;
            for (int i = 0; i < take.Count; i++)
            {
                tn += take[i].GetN();
                ti += take[i].GetI();
            }
            return tn.ToString() + "/" + ti.ToString();
        }

        public int GetK()
        {
            return this.k;
        }
        public int GetD()
        {
            return this.d;
        }
        public int GetPTS()
        {
            return this.pts;
        }
        public void AddK(int k)
        {
            this.k += k;
        }
        public void AddD(int d)
        {
            this.d += d;
        }
        public void AddPTS(int pts)
        {
            this.pts += pts;
        }
    }
}
