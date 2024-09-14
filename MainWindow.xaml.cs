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
    public partial class MainWindow : Window
    {
        Parser parser = new Parser();
        private List<int> liste_d = new List<int>(); //offene detailansichten
        private int index = -1;
        private List<int> selection = new List<int>();

        public MainWindow()
        {
            InitializeComponent();

            this.Tag = "Main";

            Session session = new Session();

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

            if(parser.NeuerLog())
            {
                ListeNeuErstellen();
            }
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
        

        private void ListeLoeschen()
        {
            Liste_Grid.RowDefinitions.Clear();
            Liste_Grid.Children.Clear();
            
            index = -1;
            selection.Clear();

            return;
        }

        private void ListeNeuErstellen()
        {
            ListeLoeschen();
            parser.UpdateMostFrequentlyPlayers();

            for (int i = 0; i < parser.ausgabe_m.Count; i++)
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

            for (int i = 0; i < parser.GetRunde(index).CountPlayer(); i++)
            {
                if (parser.GetRunde(index).GetPlayer(i).GetTypName() == parser.self_name)
                {
                    self_found = true;

                    Header_Label.Content = parser.GetRunde(index).GetDatum();

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
                        Content = parser.GetRunde(index).GetUhrzeit(),
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
                        Content = parser.GetRunde(index).GetMap(),
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
                        Content = parser.GetRunde(index).GetLaenge(),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Laenge, index);
                    Grid.SetColumn(Laenge, 4);
                    Liste_Grid.Children.Add(Laenge);

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
                        Content = "",
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 24
                    };

                    Grid.SetRow(Runden, index);
                    Grid.SetColumn(Runden, 5);
                    Liste_Grid.Children.Add(Runden);

                    List<List<int>> temprounds = new List<List<int>>();
                    parser.GetRunde(index).SortTeamWins();
                    temprounds = parser.GetRunde(index).GetTeamWins();

                    for (int j = 0; j < temprounds.Count; j++)
                    {
                        if (temprounds[j][0] == parser.GetRunde(index).GetPlayer(i).GetTeam())
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
                    foreach (Statistik s in parser.GetRunde(index).GetPlayer(i).GetAllRundenstats())
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

                    if (parser.GetRunde(index).GetPlayer(i).GetErg())
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

            Window Detail = new Detail(parser.GetRunde(auswahl), parser.self_name);

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

        private void Self_Name_Accept(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && sender is TextBox box)
            {
                parser.self_name = box.Text;

                ListeNeuErstellen();
            }
        }
    }
}
