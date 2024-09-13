using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    internal class TR
    {
        private int team;
        private int win;

        public TR(int team)
        {
            this.team = team;
            this.win = 0;
        }
        public void AddWin(int win)
        {
            this.win += win;
        }
        public int GetTeam()
        {
            return this.team;
        }
        public int GetWin()
        {
            return this.win;
        }
    }
}
