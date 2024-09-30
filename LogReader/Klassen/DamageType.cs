using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class DamageType
    {
        private string timestamp;
        private string attackerName;
        private string victimName;
        private string weaponName;
        private double none;
        private double important;

        // + was, wie viel

        //private int sd;
        //private int collision;
        //private int weakpoint;
        //private int ignore;

        public DamageType(string timestamp, string attackerName, string victimName, string weaponName, bool important, double val)
        {
            this.timestamp = timestamp;
            this.attackerName = attackerName;
            this.victimName = victimName;
            this.weaponName = weaponName;

            if (important)
            {
                this.important = val;
                this.none = 0.0;
            }
            else
            {
                this.none = val;
                this.important = 0.0;
            }
        }

        public DamageType()
        {
            //Leer

            this.attackerName = "";
            this.weaponName = "";
            this.important = 0.0;
            this.none = 0.0;
        }

        /*
        public void AVGDMG(int rounds)
        {
            this.dNone = this.dNone / rounds;
            this.dImportant = this.dImportant / rounds;
            this.tNone = this.tNone / rounds;
            this.tImportant = this.tImportant / rounds;
        }
        */
        public string GetAttackerName()
        {
            return this.attackerName;
        }
        public string GetWeaponName()
        {
            return this.weaponName;
        }
        public double GetN()
        {
            return this.none;
        }
        public double GetI()
        {
            return this.important;
        }
        /*
        public double GetTN()
        {
            return this.tNone;
        }
        public double GetTI()
        {
            return this.tImportant;
        }
        public void AddDN (double value)
        {
            this.dNone += value;
        }
        public void AddDI(double value)
        {
            this.dImportant += value;
        }
        */
        public void AddN(double value)
        {
            this.none += value;
        }
        public void AddI(double value)
        {
            this.important += value;
        }
    }
}
