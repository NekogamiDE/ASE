using System.Numerics;
using LogReader;

namespace LogReader_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNeuerSpieler()
        {
            //arrange
            string zeile = "player  7, uid 4654375@live, party 62160332, nickname: NekoLP              , team: 1, bot: 0, ur: 14841, mmHash: 5ab70afa";
            Runde r = new Runde("", "", "", "");

            string ergebnis_id = "4654375@live";
            string ergebnis_name = "NekoLP";
            int ergebnis_team = 1;
            int ergebnis_punkte = 0;

            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            zeile = zeile.Remove(0, 6);

            string[] pl = zeile.Split(',');

            r.AddPlayer(new Player(pl[1].Remove(0, 3), pl[3].Remove(0, 9), Convert.ToInt32(pl[4].Remove(0, 5))));

            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());

            //assert
            Assert.AreEqual(r.GetPlayer(r.CountPlayer() - 1).GetTypID(), ergebnis_id);
            Assert.AreEqual(r.GetPlayer(r.CountPlayer() - 1).GetTypName(), ergebnis_name);
            Assert.AreEqual(r.GetPlayer(r.CountPlayer() - 1).GetTeam(), ergebnis_team);
            Assert.AreEqual(r.GetPlayer(r.CountPlayer() - 1).GetLastRundenstat().GetPTS(), ergebnis_punkte); 
        }

        [TestMethod]
        public void TestSchaden1()
        {
            //arrange
            string zeile = " Damage. Victim: Player1       , attacker: Player2     , weapon 'CarPart_Gun_MineLauncher', damage: 21.87 DMG_DIRECT|CONTINUOUS";
            Runde r = new Runde("", "", "", "");
            r.AddPlayer(new Player("Test1@live", "Player1", 1));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            r.AddPlayer(new Player("Test2@live", "Player2", 2));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            string timestamp = "Gestern glaube ich";

            double ergebnis_schadenNone = 21.87;
            double ergebnis_schadenImportant = 0;
            string ergebnis_weaponName = "weapon'CarPart_Gun_MineLauncher'";
            bool ergebnis_important = false;
            string ergebnis_attackerName = "Player2";


            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            bool important;

            zeile = zeile.Remove(0, 14);

            string[] dmg = zeile.Split(',');

            dmg[3].Remove(0, dmg[3].IndexOf(":") + 1).Substring(0, dmg[3].IndexOf("DMG"));

            if (dmg[3].IndexOf("HUD_IMPORTANT") != -1)
                important = true;
            else
                important = false;

            dmg[3] = dmg[3].Remove(0, dmg[3].IndexOf(":") + 1);
            dmg[3] = dmg[3].Substring(0, dmg[3].IndexOf("DMG"));

            for (int i = 0; i < r.CountPlayer(); i++)
            {
                if (dmg[0] == r.GetPlayer(i).GetTypName())
                {
                    dmg[3] = dmg[3].Replace(".", ",");

                    r.GetPlayer(i).GetLastRundenstat().AddTake(new DamageType(timestamp, dmg[1].Remove(0, 9), dmg[0], dmg[2], important, Convert.ToDouble(dmg[3])));
                }

                if (dmg[1].Remove(0, 9) == r.GetPlayer(i).GetTypName())
                {
                    dmg[3] = dmg[3].Replace(".", ",");

                    r.GetPlayer(i).GetLastRundenstat().AddDeal(new DamageType(timestamp, dmg[1].Remove(0, 9), dmg[0], dmg[2], important, Convert.ToDouble(dmg[3])));
                }
            }

            //assert
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetTake()[0].GetN(), r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetN());
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetTake()[0].GetI(), r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetI());
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetTake()[0].GetAttackerName(), ergebnis_attackerName); //(ergebnis_attackerName == r.GetPlayer(1).GetTypName())
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetAttackerName(), ergebnis_attackerName); //(ergebnis_attackerName == r.GetPlayer(1).GetTypName())

            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetN(), ergebnis_schadenNone);
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetI(), ergebnis_schadenImportant);
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetWeaponName(), ergebnis_weaponName);
            Assert.AreEqual(important, ergebnis_important);
        }

        [TestMethod]
        public void TestSchaden2()
        {
            //arrange
            string zeile = " Damage. Victim: Player1       , attacker: Player2     , weapon 'CarPart_Gun_MineLauncher', damage: 3.56 DMG_FLAME|CONTINUOUS|HUD_IMPORTANT";
            Runde r = new Runde("", "", "", "");
            r.AddPlayer(new Player("Test1@live", "Player1", 1));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            r.AddPlayer(new Player("Test2@live", "Player2", 2));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            string timestamp = "Gestern glaube ich";

            double ergebnis_schadenNone = 0;
            double ergebnis_schadenImportant = 3.56;
            string ergebnis_weaponName = "weapon'CarPart_Gun_MineLauncher'";
            bool ergebnis_important = true;
            string ergebnis_attackerName = "Player2";

            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            bool important;

            zeile = zeile.Remove(0, 14);

            string[] dmg = zeile.Split(',');

            dmg[3].Remove(0, dmg[3].IndexOf(":") + 1).Substring(0, dmg[3].IndexOf("DMG"));

            if (dmg[3].IndexOf("HUD_IMPORTANT") != -1)
                important = true;
            else
                important = false;

            dmg[3] = dmg[3].Remove(0, dmg[3].IndexOf(":") + 1);
            dmg[3] = dmg[3].Substring(0, dmg[3].IndexOf("DMG"));

            for (int i = 0; i < r.CountPlayer(); i++)
            {
                if (dmg[0] == r.GetPlayer(i).GetTypName())
                {
                    dmg[3] = dmg[3].Replace(".", ",");

                    r.GetPlayer(i).GetLastRundenstat().AddTake(new DamageType(timestamp, dmg[1].Remove(0, 9), dmg[0], dmg[2], important, Convert.ToDouble(dmg[3])));
                }

                if (dmg[1].Remove(0, 9) == r.GetPlayer(i).GetTypName())
                {
                    dmg[3] = dmg[3].Replace(".", ",");

                    r.GetPlayer(i).GetLastRundenstat().AddDeal(new DamageType(timestamp, dmg[1].Remove(0, 9), dmg[0], dmg[2], important, Convert.ToDouble(dmg[3])));
                }
            }

            //assert
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetTake()[0].GetN(), r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetN());
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetTake()[0].GetI(), r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetI());
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetTake()[0].GetAttackerName(), ergebnis_attackerName); //(ergebnis_attackerName == r.GetPlayer(1).GetTypName())
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetAttackerName(), ergebnis_attackerName); //(ergebnis_attackerName == r.GetPlayer(1).GetTypName())

            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetN(), ergebnis_schadenNone);
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetI(), ergebnis_schadenImportant);
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetDeal()[0].GetWeaponName(), ergebnis_weaponName);
            Assert.AreEqual(important, ergebnis_important);
        }

        [TestMethod]
        public void TestScore()
        {
            //arrange
            string zeile = " Score:\t\tplayer:  1,\t\tnick:           Player2,\t\tGot:  13,\t\treason: PART_DETACH";
            Runde r = new Runde("", "", "", "");
            r.AddPlayer(new Player("Test1@live", "Player1", 1));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            r.AddPlayer(new Player("Test2@live", "Player2", 2));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());

            string erbenis_name = "Player2";
            int ergebnis_points = 13;

            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            zeile = zeile.Remove(0, 6);

            string[] score = zeile.Split(',');

            r.GetPlayer(Convert.ToInt32(score[0].Remove(0, 7))).GetLastRundenstat().AddPTS(Convert.ToInt32(score[2].Remove(0, 4)));

            //assert
            Assert.AreEqual(r.GetPlayer(1).GetTypName(), erbenis_name);
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetPTS(), ergebnis_points);
        }

        [TestMethod]
        public void TestKill()
        {
            //arrange
            string zeile = "Kill. Victim: Player2              killer: Player1     ";
            Runde r = new Runde("", "", "", "");
            r.AddPlayer(new Player("Test1@live", "Player1", 1));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            r.AddPlayer(new Player("Test2@live", "Player2", 2));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());

            int erbenis_victimKills = 0;
            int erbenis_killerKills = 1;
            int erbenis_victimDeaths = 1;
            int erbenis_killerDeaths = 0;

            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            zeile = zeile.Remove(0, 12);

            string[] kill = zeile.Split(':');

            if (kill[0].IndexOf("killer") != -1)
            {
                for (int i = 0; i < r.CountPlayer(); i++)
                {
                    string temp = kill[0].Remove(kill[0].IndexOf("killer"), 6);
                    if (kill[0].Remove(kill[0].IndexOf("killer"), 6) == r.GetPlayer(i).GetTypName())
                    {
                        r.GetPlayer(i).GetLastRundenstat().AddD(1);
                    }

                    if (kill[1] == r.GetPlayer(i).GetTypName())
                    {
                        r.GetPlayer(i).GetLastRundenstat().AddK(1);
                    }
                }
            }

            //assert
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetD(), erbenis_killerDeaths);
            Assert.AreEqual(r.GetPlayer(0).GetLastRundenstat().GetK(), erbenis_killerKills);

            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetD(), erbenis_victimDeaths);
            Assert.AreEqual(r.GetPlayer(1).GetLastRundenstat().GetK(), erbenis_victimKills);
        }

        [TestMethod]
        public void EndeRunde()
        {
            //arrange
            string zeile = " ===== Best Of N round 1 finish, reason: no_cars, winner team 1, win reason: MORE_CARS_LEFT, battle time: 93.9 sec =====";
            Runde r = new Runde("", "", "", "");
            r.AddPlayer(new Player("Test1@live", "Player1", 1));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            r.AddPlayer(new Player("Test2@live", "Player2", 2));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());

            int erbenis_player0Rounds = 1;
            int erbenis_player1Rounds = 1;
            List<List<int>> ergebnis_teamWins = new List<List<int>>();
            int ergebnis_runden = 1;

            int ergebnis_teamwin1 = 1;
            int ergebnis_teamwin2 = 0;

            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            zeile = zeile.Remove(0, 12);

            string[] round = zeile.Split(',');

            if (round[2].Remove(0, 10) == "1")
            {
                r.AddTeamWins(1, 1);
                r.AddTeamWins(2, 0);
            }
            else if (round[2].Remove(0, 10) == "2")
            {
                r.AddTeamWins(2, 1);
                r.AddTeamWins(1, 0);
            }
            else
            {
                r.AddTeamWins(1, 1);
                r.AddTeamWins(2, 1);
            }
            for (int i = 0; i < r.CountPlayer(); i++)
            {
                r.GetPlayer(i).AddPRounds();
            }

            r.AddRunden();

            //assert
            Assert.AreEqual(r.GetPlayer(0).GetPRounds(), erbenis_player0Rounds);
            Assert.AreEqual(r.GetPlayer(1).GetPRounds(), erbenis_player1Rounds);
            Assert.AreEqual(r.GetRunden(), ergebnis_runden);
            
            Assert.AreEqual(r.GetTeamWins()[0][1], ergebnis_teamwin1);
            Assert.AreEqual(r.GetTeamWins()[1][1], ergebnis_teamwin2);
        }

        [TestMethod]
        public void EndeMatch()
        {
            //arrange
            string zeile = " ===== Gameplay finish, reason: no_cars, winner team 1, win reason: BEST_OF_THREE, battle time: 263.5 sec =====";
            Runde r = new Runde("", "99:99", "Map", "BestOf3");
            r.AddPlayer(new Player("Test1@live", "Player1", 1));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
            r.AddPlayer(new Player("Test2@live", "Player2", 2));
            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());

            bool ergebnis_ergebnisTeam1 = true;
            bool ergebnis_ergebnisTeam2 = false;
            string ergebnis_laengeMatch = "263.5";
            string ergebnis_uhrzeit = "99:99";
            string ergebnis_matchId = "26359999";

            //act (noch beim Einlesen)
            zeile = zeile.Replace(" ", "");
            zeile = zeile.Replace("\t", "");

            //act
            //einschub wegen den bisher gelaufenen Runden (ergebnis war 2:0 für Team 1)
            r.AddTeamWins(1, 1);
            r.AddTeamWins(2, 0);
            r.AddTeamWins(1, 1);
            r.AddTeamWins(2, 0);

            zeile = zeile.Remove(0, 20);

            string[] laenge = zeile.Split(',');

            laenge[3] = laenge[3].Remove(0, 11);

            var temp21312 = laenge[3].IndexOf("sec");

            r.SetLaenge(laenge[3].Substring(0, laenge[3].IndexOf("sec")));

            r.SortTeamWins();

            for (int i = 0; i < r.CountPlayer(); i++)
            {
                r.GetPlayer(i).AddPMatches();

                if (r.TeamWon(r.GetPlayer(i).GetTeam()))
                    r.GetPlayer(i).SetErg(true);
                else
                    r.GetPlayer(i).SetErg(false);
            }

            r.SetID(r.GetLaenge().Replace(".", "") + r.GetUhrzeit().Replace(":", ""));

            //assert
            Assert.AreEqual(r.GetPlayer(0).GetErg(), ergebnis_ergebnisTeam1);
            Assert.AreEqual(r.GetPlayer(1).GetErg(), ergebnis_ergebnisTeam2);
            Assert.AreEqual(r.GetLaenge(), ergebnis_laengeMatch);
            Assert.AreEqual(r.GetUhrzeit(), ergebnis_uhrzeit);
            Assert.AreEqual(r.GetId(), ergebnis_matchId);
        }
    }
}