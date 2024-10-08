﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;

namespace LogReader
{
    public class Parser_StartRunde
    {
        public Parser_StartRunde()
        {
            
        }

        public void StartRunde()
        {
            //noch nicht implementiert
        }
    }
    public class Parser_NeuerSpieler
    {
        public Parser_NeuerSpieler()
        {

        }

        public void NeuerSpieler(string zeile, Runde r)
        {
            zeile = zeile.Remove(0, 6);

            string[] pl = zeile.Split(',');

            r.AddPlayer(new Player(pl[1].Remove(0, 3), pl[3].Remove(0, 9), Convert.ToInt32(pl[4].Remove(0, 5))));

            r.GetPlayer(r.CountPlayer() - 1).AddRundenstat(new Statistik());
        }
    }
    public class Parser_Schaden
    {
        public Parser_Schaden()
        {

        }

        public void Schaden(string zeile, string timestamp, Runde r)
        {
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
        }
    }
    public class Parser_Score
    {
        public Parser_Score()
        {

        }

        public void Score(string zeile, Runde r)
        {
            zeile = zeile.Remove(0, 6);

            string[] score = zeile.Split(',');

            int temp = Convert.ToInt32(score[2].Remove(0, 4));
            r.GetPlayer(Convert.ToInt32(score[0].Remove(0, 7))).GetLastRundenstat().AddPTS(temp);
        }
    }
    public class Parser_Stripe
    {
        public Parser_Stripe()
        {

        }

        public void Stripe()
        {
            //noch nicht implementiert
        }
    }
    public class Parser_Kill
    {
        public Parser_Kill()
        {

        }

        public void Kill(string zeile, Runde r)
        {
            zeile = zeile.Remove(0, 12);

            string[] kill = zeile.Split(':');

            if (kill[0].IndexOf("killer") != -1)
            {
                for (int i = 0; i < r.CountPlayer(); i++)
                {
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
        }
    }
    public class Parser_EndeRunde
    {
        public Parser_EndeRunde()
        {

        }

        public void EndeRunde(string zeile, Runde r)
        {
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
        }
    }
    public class Parser_EndeMatch
    {
        public Parser_EndeMatch()
        {

        }

        public void EndeMatch(string zeile, Runde r)
        {
            zeile = zeile.Remove(0, 20);

            string[] laenge = zeile.Split(',');

            laenge[3] = laenge[3].Remove(0, 11);

            var temp21312 = laenge[3].IndexOf("sec");

            r.SetLaenge(laenge[3].Substring(0, laenge[3].IndexOf("sec")));

            //double match_value = 0.0; //In Zukunft evtl. die Gesamtschäden (Deal + Take) und die Spieler IDs zusammen zählen. sollte deutlich seltener vorkommen, als die gleiche Rundenlänge zur gleichen Zeit

            r.SortTeamWins();

            for (int i = 0; i < r.CountPlayer(); i++)
            {
                //r.GetPlayer(i).AddOverall();

                r.GetPlayer(i).BerechneStatistik();

                r.GetPlayer(i).AddPMatches();

                if (r.TeamWon(r.GetPlayer(i).GetTeam()))
                    r.GetPlayer(i).SetErg(true);
                else
                    r.GetPlayer(i).SetErg(false);
            }

            r.SetID(/*match_value.ToString().Replace(",", "") + */r.GetLaenge().Replace(".", "") + r.GetUhrzeit().Replace(":", "")); //Naja
        }
    }

    public class Parser
    {
        private Parser_StartRunde parser_StartRunde = new Parser_StartRunde();
        private Parser_NeuerSpieler parser_NeuerSpieler = new Parser_NeuerSpieler();
        private Parser_Schaden parser_Schaden = new Parser_Schaden();
        private Parser_Score parser_Score = new Parser_Score();
        private Parser_Stripe parser_Stripe = new Parser_Stripe();
        private Parser_Kill parser_Kill = new Parser_Kill();
        private Parser_EndeRunde parser_EndeRunde = new Parser_EndeRunde();
        private Parser_EndeMatch parser_EndeMatch = new Parser_EndeMatch();

        private List<Translate> translations = new List<Translate>();
        private int akt_zeile = -1;
        private int gesamtzeilen = 0;
        private bool autoreader = false;
        private List<string> log = new List<string>();
        private static Timer aTimer;

        public string logdatum = "";
        public string filepath = "";
        public string self_name = "";
        public List<Runde> ausgabe_m = new List<Runde>(); //ausgabe current
        public List<string> ausgabe_m_t = new List<string>(); //ausgabe current -> Typ

        public Parser()
        {
            try
            {
                WebClient webClient = new WebClient();
                string page = webClient.DownloadString("https://crossoutdb.com/export?showtable=true&name=true&rarity=true&faction=true&category=true&type=true&popularity=true&sellprice=true&selloffers=true&buyprice=true&buyorders=true&margin=true&lastupdate=true&craftingcostsell=true&craftingcostbuy=true&craftingmargin=true&craftvsbuy=true&id=true&removedItems=true");

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page);

                List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table[@class='table']")
                            .Descendants("tr")
                            .Skip(1)
                            .Where(tr => tr.Elements("td").Count() > 1)
                            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                            .ToList();

                //nur zum Bilder herausfinden
                bool vorhanden = false;

                for (int i = 0; i < table.Count; i++)
                {
                    for (int j = 0; j < translations.Count; j++)
                    {
                        if (translations[j].GetTranslation(0) == table[i][0])
                        {
                            vorhanden = true;

                            //an table den lognamen dran schreiben

                            break;
                        }

                        vorhanden = false;
                    }

                    if (!vorhanden)
                    {
                        //als fehlend dran schreiben
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
            //eigene Translations laden

            //6 Sekunden Interval. Kein Autostart -> Automatisches auslesen der Log-Datei
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 6000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (CheckForLogUpdate())
            {
                Einlesen();
                Auslesen("", akt_zeile);

                //Update an UI
            }

            return;
        }

        public void ToggleAutoRead()
        {
            aTimer.Enabled = !aTimer.Enabled;
        }
        
        public bool NeuerLog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Reset();
            openFileDialog.Filter = "Log Dateien (*.log)|*.log|Alle Dateien (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                akt_zeile = -1;
                gesamtzeilen = 0;
                log.Clear();

                filepath = openFileDialog.FileName;

                Einlesen();

                return true;
            }

            return false;
        }
        private bool CheckForLogUpdate()
        {
            if (System.IO.File.Exists(filepath))
            {
                string temp_logdatum = System.IO.File.GetLastWriteTime(filepath).ToString();

                if (temp_logdatum != logdatum)
                    return true;
            }

            return false;
        }
        public void Einlesen()
        {
            if (System.IO.File.Exists(filepath))
            {
                int cur_line = -1;
                logdatum = System.IO.File.GetLastWriteTime(filepath).ToString();

                using (StreamReader reader = new StreamReader(filepath, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        cur_line++;

                        if (cur_line > akt_zeile)
                        {
                            string zeile = reader.ReadLine();

                            gesamtzeilen += 1;

                            zeile = zeile.Replace(" ", "");
                            zeile = zeile.Replace("\t", "");

                            log.Add(zeile);
                        }
                        else
                        {
                            reader.ReadLine();
                        }
                    }
                }
            }

            Auslesen("", akt_zeile);

            return;
        }
        public Runde GetRunde(int index)
        {
            return this.ausgabe_m[index];
        }

        private void GetAusgabeMT()
        {
            List<string> ausgabetname = new List<string>();
            List<int> ausgabetcount = new List<int>();

            for (int i = 0; i < ausgabe_m.Count; i++)
            {
                for (int j = 0; j < ausgabe_m[i].CountPlayer(); j++)
                {
                    bool found = false;

                    for (int k = 0; k < ausgabetname.Count; k++)
                    {
                        if (ausgabe_m[i].GetPlayer(j).GetTypName() == ausgabetname[k])
                        {
                            found = true;

                            ausgabetcount[k] += 1;

                            break;
                        }

                    }

                    if (!found)
                    {
                        ausgabetname.Add(ausgabe_m[i].GetPlayer(j).GetTypName());
                        ausgabetcount.Add(1);
                    }
                }
            }

            ausgabe_m_t.Clear();

            while (ausgabetname.Count > 0)
            {
                int maxIndex = 0;

                // Finde den Index des häufigsten Typnamens
                for (int i = 1; i < ausgabetname.Count; i++)
                {
                    if (ausgabetcount[i] > ausgabetcount[maxIndex])
                    {
                        maxIndex = i;
                    }
                }

                // Füge den häufigsten Typnamen zur Liste hinzu
                ausgabe_m_t.Add(ausgabetname[maxIndex]);

                // Entferne den hinzugefügten Typnamen aus den Listen
                ausgabetname.RemoveAt(maxIndex);
                ausgabetcount.RemoveAt(maxIndex);
            }

            return;
        }
        public void Auslesen(string type, int start_zeile)
        {
            akt_zeile = start_zeile;

            while (akt_zeile < gesamtzeilen - 1)
            {
                akt_zeile += 1;
                string temphead = log[akt_zeile];

                if (temphead.IndexOf("BestOf3") != -1/* && temphead.IndexOf("startinglevel") == -1*/) //Start
                {
                    /*
                    string map = temphead.Remove(0, temphead.IndexOf("map"));
                    map = map.Substring(map.IndexOf("'") + 1, map.LastIndexOf("'") - (map.IndexOf("'") + 1));
                    Runde runde = new Runde(logdatum, temphead.Substring(0, temphead.IndexOf("|")).Remove(5), map, "CW");
                    */
                    Runde runde = new Runde(logdatum, temphead.Substring(0, temphead.IndexOf("|")).Remove(5), temphead.Substring((temphead.IndexOf("'") + 13), temphead.LastIndexOf("'") - (temphead.IndexOf("'") + 13)), "CW"); //=> Alte Logs

                    //Laufparams
                    while (akt_zeile < gesamtzeilen - 1)
                    {
                        akt_zeile += 1;
                        string temp = log[akt_zeile];
                        string timestamp = temp;

                        if (temp.IndexOf("startinglevel") != -1)
                        {
                            break; //Spieler mittendrin rausgegangen => Daten werden verworfen
                        }
                        else
                        {
                            if (temp != "")
                            {
                                timestamp = timestamp.Remove(temp.IndexOf("|"));
                                temp = temp.Remove(0, temp.IndexOf("|") + 1);
                            }
                        }

                        //anweisung
                        if (temp.StartsWith("player") && runde.GetRunden() == 0)
                        {
                            parser_NeuerSpieler.NeuerSpieler(temp, runde);
                        }
                        else if (temp.StartsWith("Damage."))
                        {
                            parser_Schaden.Schaden(temp, timestamp, runde);
                        }
                        else if (temp.StartsWith("Score:"))
                        {
                            parser_Score.Score(temp, runde);
                        }
                        else if (temp.StartsWith("Stripe"))
                        {
                            parser_Stripe.Stripe();
                        }
                        else if (temp.StartsWith("Kill."))
                        {
                            parser_Kill.Kill(temp, runde);
                        }
                        else if (temp.IndexOf("BestOfNround") != -1)
                        {
                            parser_EndeRunde.EndeRunde(temp, runde);
                        }
                        else if (temp.IndexOf("Gameplayfinish") != -1)
                        {
                            parser_EndeMatch.EndeMatch(temp, runde);

                            ausgabe_m.Add(runde);
                            
                            UpdateDurchschnittStats(runde);
                            GetAusgabeMT();

                            break;
                        }
                    }
                }
                else if (false /* 8v8 */)
                {

                }

                //hier durchschnitt für jeden spieler berechnen. (modus mit einbeziehen. von daher in den eigenen if-anweisungen der speziellen modi)
                //falls noch nicht vorhanden: neuen spieler erstellen.
            }

            //selection.Add(0);

            return;
        }

        private void UpdateDurchschnittStats(Runde round)
        {
            for (int i = 0; i < round.CountPlayer(); i++)
            {
                //for gesamtspieler, die gelistet sind
                if (round.GetPlayer(i).GetTypName() == self_name)
                {
                    //muss noch implementiert werden.
                }
            }

            return;
        }

        public void UpdateMostFrequentlyPlayers()
        {
            //dropdown clearen;

            for (int i = 0; i < ausgabe_m_t.Count; i++)
            {
                //dropdown add item (ausgabe_m_t[i]);
            }
        }

        private string GetNameByLogname(string logname)
        {
            for(int i = 0; i < translations.Count(); i++)
            {
                if(logname == translations[i].GetLogname())
                {
                    return translations[i].GetTranslation(0);
                }
            }

            return "Not Found";
        }
    }
}
