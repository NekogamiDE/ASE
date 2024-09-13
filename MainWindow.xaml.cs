using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LogReader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Translate> translations = new List<Translate>();
        private List<Player> avg_player = new List<Player>(); //alle
        private List<Runde> allRounds = new List<Runde>();
        private List<Runde> ausgabe_m = new List<Runde>(); //ausgabe current
        private List<string> ausgabe_m_t = new List<string>(); //ausgabe current -> Typ

        private List<string> liste_m = new List<string>(); //speichern
        private List<int> liste_d = new List<int>(); //offene detailansichten
        private int akt_zeile = -1;
        private int gesamtzeilen = 0;
        private int index = -1;
        private List<int> selection = new List<int>();
        private bool autoreader = false;
        
        private string logdatum = "";
        private string filepath = "";
        private string self_name = "";
        private string cur_user = ""; //weiß noch nicht -> würde über den game_log ausgelesen

        private List<string> log = new List<string>();

        private static Timer aTimer;

        public MainWindow()
        {
            InitializeComponent();

            this.Tag = "Main";

            Load();

            //Name_Combobox füllen mit zuletzt verwendeten namen (bis zu 5 oder so)

            //Save();

            //TestObjekte();

            Startup();

            NeuerLog();
            Auslesen("", -1);
        }

        //Das ist halt nicht notwendig bisher
        //Alle Runden halt abspeichern und überprüfen ob in der SafeFile die ID bereits vorhanden ist
        private void Save()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string localFolder = System.IO.Path.Combine(localAppData, "LogReader");

            if (!Directory.Exists(localFolder))
            {
                Directory.CreateDirectory(localFolder);
            }
            if(!File.Exists(System.IO.Path.Combine(localFolder, "Config.txt")))
            {
                File.Create(System.IO.Path.Combine(localFolder, "Config.txt"));
            }
            if (!File.Exists(System.IO.Path.Combine(localFolder, "Data_Translations.txt")))
            {
                //Hier wird im Normalfall nichts hinein geschrieben. Dazu gibt es meines Wissens keine Datenbank.
                File.Create(System.IO.Path.Combine(localFolder, "Data_Translations.txt"));
            }
            if (!File.Exists(System.IO.Path.Combine(localFolder, "Data_Rounds.txt")))
            {
                File.Create(System.IO.Path.Combine(localFolder, "Data_Rounds.txt"));
            }

            /*
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(localFolder, "Data_Translations.txt"), false))
            {
                for(int i = 0; i < translations.Count; i++)
                {
                    writer.WriteLine(translations[i].GetTranslation(0) + "," + translations[i].GetLogname());
                }
            }
            */

            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(localFolder, "Data_Rounds.txt"), false))
            {
                for (int i = 0; i < avg_player.Count; i++)
                {
                    //writer.WriteLine(liste_t[i]. + "," + translations[i].); Klasse noch nicht fertig
                }
            }
        }

        //kleiner Zusatz: einfach alle Runden speichern und wieder einlesen. dann braucht man keine extra klasse für alle Spieler
        private void Load()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string localFolder = System.IO.Path.Combine(localAppData, "LogReader");

            if (Directory.Exists(localFolder))
            {
                if (File.Exists(System.IO.Path.Combine(localFolder, "Config.txt")))
                {
                    using (StreamReader reader = new StreamReader(System.IO.Path.Combine(localFolder, "Config.txt"), Encoding.Default))
                    {
                        //hier muss immer was stehen, sonst startet das programm ohne zusätzliche einstellungen

                        if (!reader.EndOfStream)
                        {
                            string zeile = reader.ReadLine();
                        }
                        else
                        {
                            //standard start
                        }
                    }
                }
                else
                {
                    //standard start
                }

                if (File.Exists(System.IO.Path.Combine(localFolder, "Data_Translations.txt")))
                {
                    using (StreamReader reader = new StreamReader(System.IO.Path.Combine(localFolder, "Data_Translations.txt"), Encoding.Default))
                    {
                        while (!reader.EndOfStream)
                        {
                            string[] zeile = reader.ReadLine().Split(',');
                            List<string> translation = new List<string>();

                            for(int i = 1; i < zeile.Length; i++) //evtl. Length - 1
                            {
                                translation.Add(zeile[i]);
                            }

                            Translate p = new Translate(zeile[0], translation);

                            translations.Add(p);
                        }
                    }
                }

                if (File.Exists(System.IO.Path.Combine(localFolder, "Data_Player.txt")))
                {
                    using (StreamReader reader = new StreamReader(System.IO.Path.Combine(localFolder, "Data_Player.txt"), Encoding.Default))
                    {
                        while (!reader.EndOfStream)
                        {
                            string[] zeile = reader.ReadLine().Split(',');

                            Player t = new Player(zeile[0], zeile[1], Convert.ToInt32(zeile[2]));

                            avg_player.Add(t);
                        }
                    }
                }
            }
        }

        private void TestObjekte()
        {
            for (int i = 0; i < 50; i++)
            {
                Runde neum = new Runde("30.08.2024", (DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()), "TestMap: " + i.ToString(), "CW");

                //Player in der Runde anlegen

                ausgabe_m.Add(neum);
            }

            while(index < ausgabe_m.Count - 1)
            {
                ListeHinzufuegen();
            }

            return;
        }

        private void Startup()
        {
            //Params auffüllen

            Session session = new Session();
            
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

            Liste_Grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            Liste_Grid.VerticalAlignment = VerticalAlignment.Top;
            Liste_Grid.Background = new SolidColorBrush(Colors.Black);

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(140, GridUnitType.Star);
            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column4 = new ColumnDefinition();
            column4.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column5 = new ColumnDefinition();
            column5.Width = new GridLength(80, GridUnitType.Star);
            ColumnDefinition column6 = new ColumnDefinition();
            column6.Width = new GridLength(80, GridUnitType.Star);

            Liste_Grid.ColumnDefinitions.Add(column1);
            Liste_Grid.ColumnDefinitions.Add(column2);
            Liste_Grid.ColumnDefinitions.Add(column3);
            Liste_Grid.ColumnDefinitions.Add(column4);
            Liste_Grid.ColumnDefinitions.Add(column5);
            Liste_Grid.ColumnDefinitions.Add(column6);

            //6 Sekunden Interval. Kein Autostart -> Automatisches auslesen der Log-Datei
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 6000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            return;
        }

        private void ToggleAutoRead()
        {
            aTimer.Enabled = !aTimer.Enabled;
        }

        //"Invoke" Ansonsten gehts erstmal nicht...GUI macht Thread-Probleme
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if(CheckForLogUpdate())
            {
                Einlesen();
                Auslesen("", akt_zeile);
            }

            return;
        }
        private void NeuerLog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Reset();
            openFileDialog.Filter = "Log Dateien (*.log)|*.log|Alle Dateien (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                ListeLoeschen();

                gesamtzeilen = 0;
                log.Clear();

                filepath = openFileDialog.FileName;

                Einlesen();
            }

            return;
        }

        private bool CheckForLogUpdate()
        {
            if (File.Exists(filepath))
            {
                string temp_logdatum = File.GetLastWriteTime(filepath).ToString();

                if (temp_logdatum != logdatum)
                    return true;
            }

            return false;
        }

        private void Einlesen()
        {
            if (File.Exists(filepath))
            {
                int cur_line = -1;
                logdatum = File.GetLastWriteTime(filepath).ToString();

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

            return;
        }

        private void GetAusgabeMT()
        {
            List<string> ausgabetname = new List<string>();
            List<int> ausgabetcount = new List<int>();

            for (int i = 0; i < ausgabe_m.Count; i++)
            {
                for(int j = 0; j < ausgabe_m[i].CountPlayer(); j++)
                {
                    bool found = false;

                    for(int k = 0; k < ausgabetname.Count; k++)
                    {
                        if(ausgabe_m[i].GetPlayer(j).GetTypName() == ausgabetname[k])
                        {
                            found = true;

                            ausgabetcount[k] += 1;

                            break;
                        }

                    }
                    
                    if(!found)
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

        private void Auslesen(string type, int start_zeile) 
        {
            akt_zeile = start_zeile;

            while (akt_zeile < gesamtzeilen - 1)
            {
                akt_zeile += 1;
                string temphead = ZeileLesen(log, akt_zeile);

                if (temphead.IndexOf("BestOf3") != -1) //Start
                {
                    Runde runde = new Runde(logdatum, temphead.Substring(0, temphead.IndexOf("|")).Remove(5), temphead.Substring((temphead.IndexOf("'") + 13), temphead.LastIndexOf("'") - (temphead.IndexOf("'") + 13)), "CW");

                    //Runde match = new Runde(logdatum, temphead.Substring(0, temphead.IndexOf("|")).Remove(5), new MatchInfo(temphead.Substring((temphead.IndexOf("'") + 13), temphead.LastIndexOf("'") - (temphead.IndexOf("'") + 13)), ""));

                    bool important;

                    //bool leave = false;

                    //Laufparams
                    while (akt_zeile < gesamtzeilen - 1)
                    {
                        akt_zeile += 1;
                        string temp = ZeileLesen(log, akt_zeile);

                        temp = temp.Replace(" ", "");
                        temp = temp.Replace("\t", "");

                        string timestamp = temp;
                        timestamp = timestamp.Remove(temp.IndexOf("|"));
                        temp = temp.Remove(0, temp.IndexOf("|") + 1);

                        if (temp.IndexOf("startinglevel") != -1)
                        {
                            break;
                        }

                        //anweisung
                        if (temp.StartsWith("player") && runde.GetRunden() == 0)
                        {
                            temp = temp.Remove(0, 6);

                            string[] pl = temp.Split(',');

                            runde.AddPlayer(new Player(pl[1].Remove(0, 3), pl[3].Remove(0, 9), Convert.ToInt32(pl[4].Remove(0, 5))));

                            runde.GetPlayer(runde.CountPlayer() - 1).AddRundenstat(new Statistik());
                        }
                        else if (temp.StartsWith("Damage."))
                        {
                            temp = temp.Remove(0, 14);

                            string[] dmg = temp.Split(',');

                            dmg[3].Remove(0, dmg[3].IndexOf(":") + 1).Substring(0, dmg[3].IndexOf("DMG"));

                            if (dmg[3].IndexOf("HUD_IMPORTANT") != -1)
                                important = true;
                            else
                                important = false;

                            dmg[3] = dmg[3].Remove(0, dmg[3].IndexOf(":") + 1);
                            dmg[3] = dmg[3].Substring(0, dmg[3].IndexOf("DMG"));

                            for (int i = 0; i < runde.CountPlayer(); i++)
                            {
                                if (dmg[0] == runde.GetPlayer(i).GetTypName())
                                {
                                    dmg[3] = dmg[3].Replace(".", ",");

                                    runde.GetPlayer(i).GetLastRundenstat().AddTake(new DamageType(timestamp, dmg[1].Remove(0, 9), dmg[0], dmg[2], important, Convert.ToDouble(dmg[3])));
                                }

                                if (dmg[1].Remove(0, 9) == runde.GetPlayer(i).GetTypName())
                                {
                                    dmg[3] = dmg[3].Replace(".", ",");

                                    runde.GetPlayer(i).GetLastRundenstat().AddDeal(new DamageType(timestamp, dmg[1].Remove(0, 9), dmg[0], dmg[2], important, Convert.ToDouble(dmg[3])));
                                }
                            }
                        }
                        else if (temp.StartsWith("Score:"))
                        {
                            temp = temp.Remove(0, 6);

                            string[] score = temp.Split(',');

                            runde.GetPlayer(Convert.ToInt32(score[0].Remove(0, 7))).GetLastRundenstat().AddPTS(Convert.ToInt32(score[2].Remove(0, 4)));
                        }
                        else if (temp.StartsWith("Stripe"))
                        {
                            //nö, kein bock
                        }
                        else if (temp.StartsWith("Kill."))
                        {
                            temp = temp.Remove(0, 12);

                            string[] kill = temp.Split(':');

                            if (kill[0].IndexOf("killer") != -1)
                            {
                                for (int i = 0; i < runde.CountPlayer(); i++)
                                {
                                    if (kill[0].Remove(kill[0].IndexOf("killer"), 6) == runde.GetPlayer(i).GetTypName())
                                    {
                                        runde.GetPlayer(i).GetLastRundenstat().AddD(1);
                                    }

                                    if (kill[1] == runde.GetPlayer(i).GetTypName())
                                    {
                                        runde.GetPlayer(i).GetLastRundenstat().AddK(1);
                                    }
                                }
                            }
                        }
                        else if (temp.IndexOf("BestOfNround") != -1)
                        {
                            temp = temp.Remove(0, 12);

                            string[] round = temp.Split(',');

                            if (round[2].Remove(0, 10) == "1")
                            {
                                runde.AddTeamWins(1, 1);
                                runde.AddTeamWins(2, 0);
                            }
                            else if (round[2].Remove(0, 10) == "2")
                            {
                                runde.AddTeamWins(2, 1);
                                runde.AddTeamWins(1, 0);
                            }
                            else
                            {
                                runde.AddTeamWins(1, 1);
                                runde.AddTeamWins(2, 1);
                            }
                            for (int i = 0; i < runde.CountPlayer(); i++)
                            {
                                runde.GetPlayer(i).AddPRounds();
                            }

                            runde.AddRunden();
                            //runde ende
                        }
                        else if (temp.IndexOf("Gameplayfinish") != -1)
                        {
                            temp = temp.Remove(0, 20);

                            string[] laenge = temp.Split(',');

                            laenge[3] = laenge[3].Remove(0, 11);

                            var temp21312 = laenge[3].IndexOf("sec");

                            runde.SetLaenge(laenge[3].Substring(0, laenge[3].IndexOf("sec")));

                            //double match_value = 0.0; //In Zukunft evtl. die Gesamtschäden (Deal + Take) und die Spieler IDs zusammen zählen. sollte deutlich seltener vorkommen, als die gleiche Rundenlänge zur gleichen Zeit

                            runde.SortTeamWins();

                            for (int i = 0; i < runde.CountPlayer(); i++)
                            {
                                //runde.GetPlayer(i).AddOverall();

                                runde.GetPlayer(i).AddPMatches();

                                if (runde.TeamWon(runde.GetPlayer(i).GetTeam()))
                                    runde.GetPlayer(i).SetErg(true);
                                else
                                    runde.GetPlayer(i).SetErg(false);
                            }

                            runde.SetID(/*match_value.ToString().Replace(",", "") + */runde.GetLaenge().Replace(".", "") + runde.GetUhrzeit().Replace(":", "")); //Naja

                            ausgabe_m.Add(runde);

                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ListeHinzufuegen();
                            }));
                            UpdateDurchschnittStats(runde);
                            GetAusgabeMT();

                            break;
                        }
                    }
                }
                else if(false /* 8v8 */)
                {

                }

                //hier durchschnitt für jeden spieler berechnen. (modus mit einbeziehen. von daher in den eigenen if-anweisungen der speziellen modi)
                //falls noch nicht vorhanden: neuen spieler erstellen.
            }

            //selection.Add(0);

            return;
        }

        private string ZeileLesen(List<string> log, int akt_zeile)
        {
            return log[akt_zeile];
        }
        
        //Veraltet, wird nicht genutzt
        private void AddGesamtStats()
        {
            bool mVorhanden = false;
            bool tVorhanden = false;

            for (int i = 0; i < selection.Count; i++)
            {
                for (int f = 0; f < liste_m.Count; f++)
                {
                    if (liste_m[f] == ausgabe_m[selection[i]].GetId())
                    {
                        mVorhanden = true;

                        break; //schon vorhanden
                    }
                }

                if (!mVorhanden)
                {
                    liste_m.Add(ausgabe_m[selection[i]].GetId());

                    for (int j = 0; j < ausgabe_m[selection[i]].CountPlayer(); j++)
                    {
                        for (int k = 0; k < avg_player.Count; k++)
                        {
                            if (ausgabe_m[selection[i]].GetPlayer(j).GetTypName() == avg_player[k].GetTypName())
                            {
                                tVorhanden = true;

                                //liste_t[k].AddOverall(); //ehhh dazu muss ne liste mit allen matches erstellt werden
                                /*
                                liste_t[k].GetOverall().AddPlayedRounds(ausgabe_m[selection[i]].GetInfo().GetRunden());
                                liste_t[k].GetOverall().AddK(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetK());
                                liste_t[k].GetOverall().AddD(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetD());
                                liste_t[k].GetOverall().AddPTS(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetPTS());
                                liste_t[k].GetOverall().GetAVGD().AddN(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGD().GetN());
                                liste_t[k].GetOverall().GetAVGD().AddI(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGD().GetI());
                                liste_t[k].GetStatistik().GetAVGT().AddN(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGT().GetN());
                                liste_t[k].GetStatistik().GetAVGT().AddI(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGT().GetI());
                                */
                                break;
                            }
                        }

                        if (!tVorhanden)
                        {
                            Player neu = new Player(ausgabe_m[i].GetPlayer(j).GetTypID(), ausgabe_m[i].GetPlayer(j).GetTypName(), -1);

                            //neu.AddOverall();
                            /*
                            neu.GetStatistik().AddPlayedRounds(ausgabe_m[selection[i]].GetInfo().GetRunden());
                            neu.GetStatistik().AddK(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetK());
                            neu.GetStatistik().AddD(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetD());
                            neu.GetStatistik().AddPTS(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetPTS());
                            neu.GetStatistik().GetAVGD().AddN(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGD().GetN());
                            neu.GetStatistik().GetAVGD().AddI(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGD().GetI());
                            neu.GetStatistik().GetAVGT().AddN(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGT().GetN());
                            neu.GetStatistik().GetAVGT().AddI(ausgabe_m[selection[i]].GetInfo().GetPStats(j).GetStatistik().GetAVGT().GetI());
                            */
                            avg_player.Add(neu);
                        }

                        tVorhanden = false;
                    }
                }

                mVorhanden = false;
            }
            return;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }//Für alle Buttons in der Motorenliste.

        private void ErstelleDurchschnittStats()
        {
            for (int r = 0; r < allRounds.Count; r++)
            {
                for(int i = 0; i < allRounds[r].CountPlayer(); i++)
                {
                    bool playerExist = false;

                    for(int j = 0; j < avg_player.Count; j++)
                    {
                        if(allRounds[r].GetPlayer(i).GetTypName() == avg_player[j].GetTypName())
                        {
                            playerExist = true;
                            
                            Statistik old_avg = avg_player[j].GetLastRundenstat();
                            Statistik new_avg = new Statistik();

                            List<Statistik> temp = allRounds[r].GetPlayer(i).GetAllRundenstats();

                            for(int s = 0; s < temp.Count; s++)
                            {
                                //muss noch implementiert werden.
                            }

                            new_avg.AddPTS(0);

                            avg_player[j].AddRundenstat(new_avg);

                            break;
                        }
                    }

                    if(!playerExist)
                    {
                        avg_player.Add(allRounds[r].GetPlayer(i));
                    }
                }
            }

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

        private void ListeLoeschen()
        {
            Liste_Grid.RowDefinitions.Clear();
            Liste_Grid.Children.Clear();

            //akt_zeile = -1;
            index = -1;
            selection.Clear();

            return;
        }

        private void UpdateMostFrequentlyPlayers()
        {
            //dropdown clearen;

            for (int i = 0; i < ausgabe_m_t.Count; i++)
            {
                //dropdown add item (ausgabe_m_t[i]);
            }
        }

        private void ListeNeuErstellen()
        {
            ListeLoeschen();
            UpdateMostFrequentlyPlayers();

            for (int i = 0; i < ausgabe_m.Count; i++)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ListeHinzufuegen();
                }));
            }

            return;
        }

        /*
        private void UpdateButton()
        {
            foreach (Button b in FindVisualChildren<Button>(Liste_Grid))
            {
                b.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            for (int i = 0; i < selection.Count; i++)
            {
                foreach (Button b in FindVisualChildren<Button>(Liste_Grid))
                {
                    if (selection[i] == Convert.ToInt32(b.Tag))
                    {
                        b.BorderBrush = new SolidColorBrush(Colors.Green);
                    }
                }
            }

            return;
        }
        */

        private void ListeHinzufuegen()
        {
            bool self_found = false;
            
            index++;

            for (int i = 0; i < ausgabe_m[index].CountPlayer(); i++)
            {
                if (ausgabe_m[index].GetPlayer(i).GetTypName() == self_name)
                {
                    self_found = true;

                    Header_Label.Content = ausgabe_m[index].GetDatum();

                    #region Button

                    Button button = new Button
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(2, 1, 2, 1),
                        Tag = index
                    };


                    /*
                    button.MouseEnter += (s, e) =>
                    {
                        button.Background = new SolidColorBrush(Colors.Transparent);


                        return;
                    };
                    button.MouseLeave += (s, e) =>
                    {
                        button.Background = new SolidColorBrush(Colors.Transparent);

                        return;
                    };
                    */
                    button.Click += (s, e) =>
                    {
                        for (int j = 0; j < selection.Count; j++)
                        {
                            if (selection[j] == Convert.ToInt32(button.Tag))
                            {
                                selection.RemoveAt(j);
                                button.BorderThickness = new Thickness(2, 1, 2, 1);
                                button.BorderBrush = new SolidColorBrush(Colors.Transparent);

                                return;
                            }
                        }

                        selection.Add(Convert.ToInt32(button.Tag));
                        button.BorderThickness = new Thickness(4, 2, 4, 2);
                        button.BorderBrush = new SolidColorBrush(Colors.Yellow);

                        return;
                    };
                    button.MouseDoubleClick += (s, e) =>
                    {
                        DetailFensterOpen(Convert.ToInt32(button.Tag));

                        return;
                    };

                    


                    Grid.SetRow(button, index);
                    Grid.SetColumnSpan(button, 6);
                    Grid.SetColumn(button, 0);
                    Liste_Grid.Children.Add(button);
                    #endregion

                    #region Uhrzeit
                    Label Uhrzeit = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(1, 1, 1, 1),
                        IsHitTestVisible = false,
                        Content = ausgabe_m[index].GetUhrzeit(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Uhrzeit, index);
                    Grid.SetColumn(Uhrzeit, 0);
                    Liste_Grid.Children.Add(Uhrzeit);
                    #endregion

                    Label Map = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(1, 1, 0, 1),
                        IsHitTestVisible = false,
                        Content = ausgabe_m[index].GetMap(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Map, index);
                    Grid.SetColumn(Map, 1);
                    Liste_Grid.Children.Add(Map);

                    Label Laenge = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(0, 1, 0, 1),
                        IsHitTestVisible = false,
                        Content = ausgabe_m[index].GetLaenge(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Laenge, index);
                    Grid.SetColumn(Laenge, 4);
                    Liste_Grid.Children.Add(Laenge);

                    string runden = "";
                    /*
                    for (int j = 0; j < ausgabe_m[index].GetRunden(); j++)
                    {
                        if (j != ausgabe_m[index].GetInfo().CountTeamRounds() - 1)
                            runden += ausgabe_m[index].GetInfo().GetTeamRounds(ausgabe_m[index].GetInfo().GetTeam(j)) + " / ";
                        else
                            runden += ausgabe_m[index].GetInfo().GetTeamRounds(ausgabe_m[index].GetInfo().GetTeam(j));
                    }
                    */

                    Label Runden = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(0, 1, 2, 1),
                        IsHitTestVisible = false,
                        Content = runden,
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Runden, index);
                    Grid.SetColumn(Runden, 5);
                    Liste_Grid.Children.Add(Runden);

                    /*
                    Label Id = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(0, 1, 0, 1),
                        IsHitTestVisible = false,
                        Content = liste_m[index].GetInfo().GetId(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Id, index);
                    Grid.SetColumn(Id, 3);
                    Liste_Grid.Children.Add(Id);
                    */

                    List<List<int>> temprounds = new List<List<int>>();
                    ausgabe_m[index].SortTeamWins();
                    temprounds = ausgabe_m[index].GetTeamWins();

                    for (int j = 0; j < temprounds.Count; j++)
                    {
                        if (temprounds[j][0] == ausgabe_m[index].GetPlayer(i).GetTeam())
                        {
                            Runden.Content += "Team: " + temprounds[j][0] + " - (" + temprounds[j][1] + ")";
                        }
                    }


                    double damageDealGeneral = 0;
                    double damageDealImportant = 0;

                    double damageTakeGeneral = 0;
                    double damageTakeImportant = 0;

                    int kills = 0;
                    int deaths = 0;

                    //Bei Player Als Funktion ermöglichen
                    foreach (Statistik s in ausgabe_m[index].GetPlayer(i).GetAllRundenstats())
                    {
                        string[] tempDeal = s.GetDealAusgabe().Split('/');
                        damageDealGeneral += Convert.ToDouble(tempDeal[0]) + Convert.ToDouble(tempDeal[1]);
                        damageDealImportant += Convert.ToDouble(tempDeal[1]);

                        string[] temptake = s.GetTakeAusgabe().Split('/');

                        damageTakeGeneral += Convert.ToDouble(temptake[0]) + Convert.ToDouble(temptake[1]);
                        damageTakeImportant += Convert.ToDouble(temptake[1]);

                        kills += s.GetK();
                        deaths += s.GetD();
                    }

                    Label DG = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(0, 1, 0, 1),
                        IsHitTestVisible = false,
                        Content = Convert.ToInt32(damageDealGeneral).ToString(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(DG, index);
                    Grid.SetColumn(DG, 2);
                    Liste_Grid.Children.Add(DG);

                    Label TG = new Label
                    {
                        Height = 44,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = new SolidColorBrush(Colors.BurlyWood),
                        BorderThickness = new Thickness(0, 1, 0, 1),
                        IsHitTestVisible = false,
                        Content = Convert.ToInt32(damageTakeGeneral).ToString(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(TG, index);
                    Grid.SetColumn(TG, 3);
                    Liste_Grid.Children.Add(TG);

                    if (ausgabe_m[index].GetPlayer(i).GetErg())
                    {
                        Uhrzeit.Background = new SolidColorBrush(Colors.Green);
                        Map.Background = new SolidColorBrush(Colors.Green);
                        Runden.Background = new SolidColorBrush(Colors.Green);
                        Laenge.Background = new SolidColorBrush(Colors.Green);
                        DG.Background = new SolidColorBrush(Colors.Green);
                        TG.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        Uhrzeit.Background = new SolidColorBrush(Colors.Red);
                        Map.Background = new SolidColorBrush(Colors.Red);
                        Runden.Background = new SolidColorBrush(Colors.Red);
                        Laenge.Background = new SolidColorBrush(Colors.Red);
                        DG.Background = new SolidColorBrush(Colors.Red);
                        TG.Background = new SolidColorBrush(Colors.Red);
                    }

                    RowDefinition gridRow = new RowDefinition
                    {
                        Height = new GridLength(48, GridUnitType.Pixel)
                    };
                    Liste_Grid.RowDefinitions.Add(gridRow);

                    break;
                }
            }

            if(!self_found)
            {
                Header_Label.Content = "";
            }

            return;
        }

        private void DetailFensterOpen(int auswahl)
        {
            for(int i = 0; i < liste_d.Count; i++)
            {
                if (liste_d[i] == auswahl)
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Tag != null && liste_d[i].ToString() == window.Tag.ToString())
                        {
                            window.Activate();
                        }
                    }

                    return;
                }
            }

            liste_d.Add(auswahl);

            Window Detail = new Detail(ausgabe_m[auswahl], self_name);

            //Detail.ShowInTaskbar = false;

            Detail.Owner = this;

            Detail.Tag = auswahl;

            Detail.Show();

            return;
        }

        private void Beenden()
        {
            //SessionInfo + Programmparameter(.ini) schreiben
            //filepath

            Main.Close();

            return;
        }

        public void DetailfensterClose(int tag)
        {
            for (int i = 0; i < liste_d.Count; i++)
            {
                if (liste_d[i] == tag)
                {
                    liste_d.RemoveAt(i);
                    break;
                }
            }

            return;
        }

        private void MainWindowSchließen_Btn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < liste_d.Count(); i++)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Tag != null)
                    {
                        if (liste_d[i].ToString() == window.Tag.ToString())
                        {
                            window.Close();
                        }
                    }
                }
            }
            Beenden();

            return;
        }

        private void MainWindowMinimieren_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            return;
        }

        private void MainWindowMinMax_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;

            return;
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

        private void Self_Name_Accept(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && sender is TextBox box)
            {
                self_name = box.Text;

                ListeNeuErstellen();
            }
        }
    }
}
