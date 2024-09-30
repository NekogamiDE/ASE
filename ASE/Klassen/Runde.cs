using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class Runde
    {
        private string datum;
        private string uhrzeit;
        private string map;
        private string id;
        private string laenge;
        private string modus;
        private int runden;
        List<List<int>> teamwins;
        List<Player> player;

        public Runde(string datum, string uhrzeit, string map, string modus)
        {
            this.datum = datum;
            this.uhrzeit = uhrzeit;
            this.map = map;
            this.id = "None";
            this.laenge = "0";
            this.modus = modus;
            this.runden = 0;
            this.teamwins = new List<List<int>>();
            this.player = new List<Player>();
        }
        public string GetDatum()
        {
            return this.datum;
        }

        public string GetUhrzeit()
        {
            return this.uhrzeit;
        }

        public string GetMap()
        {
            return this.map;
        }
        public string GetId()
        {
            return this.id;
        }
        public string GetLaenge()
        {
            return this.laenge;
        }
        public int GetRunden()
        {
            return this.runden;
        }

        public void AddTeamWins(int team, int val)
        {
            for (int i = 0; i < teamwins.Count; i++)
            {
                if (teamwins[i][0] == team)
                {
                    teamwins[i][1] += val;
                    return;
                }
            }
            teamwins.Add(new List<int>{ team, val });
        }
        public void SortTeamWins()
        {
            //Nach Häufigkeit sortieren und als String ausgeben
            
            for(int i = 0; i < teamwins.Count; i++)
            {
                for(int j = 1; j < teamwins.Count; j++)
                {
                    if (teamwins[j - 1][1] < teamwins[j][1])
                    {
                        int temp = teamwins[j][1];
                        teamwins[j][1] = teamwins[j - 1][1];
                        teamwins[j - 1][1] = temp;
                    }
                }
            }
        }
        public bool TeamWon(int team)
        {
            if (teamwins[0][0] == team)
            {
                return true;
            }

            return false;        
        }
        public List<List<int>> GetTeamWins()
        {
            return teamwins;
        }
        
        public Player GetPlayer(int key)
        {
            return this.player[key];
        }
        public void SetLaenge(string laenge)
        {
            this.laenge = laenge;
        }
        public void AddRunden()
        {
            this.runden ++;
        }
        public void SetPlayer(int key, Player value)
        {
            this.player[key] = value;
        }
        public void AddPlayer(Player value)
        {
            this.player.Add(value);
        }
        public void SetID(string id)
        {
            this.id = id;
        }
        public int CountPlayer()
        {
            return this.player.Count();
        }
    }
}
