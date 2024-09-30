using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class Player
    {
        private string id;
        private string name;
        private int playedMatches;
        private int playedRounds;
        private int team;
        private bool ergebnis;
        private List<Statistik> cur_rundenstats;
        //private Statistik overall;
        //private List<Anderes> anderes;
        private double damageDealGeneral;
        private double damageDealImportant;

        private double damageTakeGeneral;
        private double damageTakeImportant;

        private int kills;
        private int deaths;
        
        public Player(string id, string name, int team)
        {
            this.id = id;
            this.name = name;
            this.playedMatches = 0;
            this.playedRounds = 0;
            //this.overall = new Statistik();
            this.team = team;
            this.ergebnis = false;
            this.cur_rundenstats = new List<Statistik>();
            this.team = team;
        }
        public bool GetErg()
        {
            return this.ergebnis;
        }
        public void SetErg(bool erg)
        {
            this.ergebnis = erg;
        }
        public int GetTeam()
        {
            return this.team;
        }
        public string GetTypID()
        {
            return this.id;
        }
        public string GetTypName()
        {
            return this.name;
        }
        public void AddPMatches()
        {
            this.playedMatches ++;
        }
        public int GetPMatches()
        {
            return this.playedMatches;
        }
        public void AddPRounds()
        {
            this.playedRounds++;
        }
        public int GetPRounds()
        {
            return this.playedRounds;
        }
        public void AddRundenstat(Statistik stat)
        {
            this.cur_rundenstats.Add(stat);
        }
        public void ClearRundenstats()
        {
            this.cur_rundenstats.Clear();
        }
        public Statistik GetLastRundenstat()
        {
            return this.cur_rundenstats[cur_rundenstats.Count - 1];
        }
        public List<Statistik> GetAllRundenstats()
        {
            return this.cur_rundenstats;
        }
        public void BerechneStatistik()
        {
            this.damageDealGeneral = 0;
            this.damageDealImportant = 0;

            this.damageTakeGeneral = 0;
            this.damageTakeImportant = 0;

            this.kills = 0;
            this.deaths = 0;

            foreach (Statistik s in cur_rundenstats)
            {
                string[] tempDeal = s.GetDealAusgabe().Split('/');
                this.damageDealGeneral += Convert.ToDouble(tempDeal[0]) + Convert.ToDouble(tempDeal[1]);
                this.damageDealImportant += Convert.ToDouble(tempDeal[1]);

                string[] temptake = s.GetTakeAusgabe().Split('/');

                this.damageTakeGeneral += Convert.ToDouble(temptake[0]) + Convert.ToDouble(temptake[1]);
                this.damageTakeImportant += Convert.ToDouble(temptake[1]);

                this.kills += s.GetK();
                this.deaths += s.GetD();
            }
        }
        
        public double GetDamageDealGeneral()
        {
            return this.damageDealGeneral;
        }
        public double GetDamageDealImportant()
        {
            return this.damageDealImportant;
        }
        public double GetDamageTakeGeneral()
        {
            return this.damageTakeGeneral;
        }
        public double GetDamageTakeImportant()
        {
            return this.damageTakeImportant;
        }
        public int GetKills()
        {
            return this.kills;
        }
        public int GetDeaths()
        {
            return this.deaths;
        }



        /*
        public Statistik AddOverall()
        {
            Statistik overall = new Statistik();

            for(int i = 0; i < cur_rundenstats.Count; i++)
            {
                overall.AddPTS(cur_rundenstats[i].GetPTS());
                overall.AddK(cur_rundenstats[i].GetPTS());
                overall.AddD(cur_rundenstats[i].GetPTS());

                List<DType> dtl = cur_rundenstats[i].GetDeal();
                for (int j = 0; j < dtl.Count; j++)
                {
                    overall.AddDeal(dtl[j]);
                }
                dtl.Clear();
                dtl = cur_rundenstats[i].GetTake();
                for (int j = 0; j < dtl.Count; j++)
                {
                    overall.AddTake(dtl[j]);
                }
            }
            
            return overall;
        }
        */
    }
}
