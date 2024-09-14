using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace LogReader
{
    public partial class Detail : Window
    {
        public Detail(Runde m, string self_name)
        {
            InitializeComponent();

            Detail1_Grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            Detail1_Grid.VerticalAlignment = VerticalAlignment.Top;
            Detail1_Grid.Background = new SolidColorBrush(Colors.Transparent);
            Detail2_Grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            Detail2_Grid.VerticalAlignment = VerticalAlignment.Top;
            Detail2_Grid.Background = new SolidColorBrush(Colors.Transparent);

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column4 = new ColumnDefinition();
            column4.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column5 = new ColumnDefinition();
            column5.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column6 = new ColumnDefinition();
            column6.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column7 = new ColumnDefinition();
            column7.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column8 = new ColumnDefinition();
            column8.Width = new GridLength(100, GridUnitType.Star);

            Detail1_Grid.ColumnDefinitions.Add(column1);
            Detail1_Grid.ColumnDefinitions.Add(column2);
            Detail1_Grid.ColumnDefinitions.Add(column3);
            Detail1_Grid.ColumnDefinitions.Add(column4);
            Detail1_Grid.ColumnDefinitions.Add(column5);
            Detail1_Grid.ColumnDefinitions.Add(column6);
            Detail1_Grid.ColumnDefinitions.Add(column7);
            Detail1_Grid.ColumnDefinitions.Add(column8);

            ColumnDefinition column21 = new ColumnDefinition();
            column1.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column22 = new ColumnDefinition();
            column2.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column23 = new ColumnDefinition();
            column3.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column24 = new ColumnDefinition();
            column4.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column25 = new ColumnDefinition();
            column5.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column26 = new ColumnDefinition();
            column6.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column27 = new ColumnDefinition();
            column7.Width = new GridLength(100, GridUnitType.Star);
            ColumnDefinition column28 = new ColumnDefinition();
            column8.Width = new GridLength(100, GridUnitType.Star);

            Detail2_Grid.ColumnDefinitions.Add(column21);
            Detail2_Grid.ColumnDefinitions.Add(column22);
            Detail2_Grid.ColumnDefinitions.Add(column23);
            Detail2_Grid.ColumnDefinitions.Add(column24);
            Detail2_Grid.ColumnDefinitions.Add(column25);
            Detail2_Grid.ColumnDefinitions.Add(column26);
            Detail2_Grid.ColumnDefinitions.Add(column27);
            Detail2_Grid.ColumnDefinitions.Add(column28);

            DatenEinlesen(m, self_name);
        }

        public void DatenEinlesen(Runde m, string self_name)
        {
            //Header
            RowDefinition gridRowHead = new RowDefinition
            {
                Height = new GridLength(25.75, GridUnitType.Pixel),
            };
            Detail1_Grid.RowDefinitions.Add(gridRowHead);

            #region Datum
            TextBlock Datum = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Text = m.GetDatum(),
                FontFamily = new FontFamily("Arial"),
                FontSize = 24
            };

            Grid.SetRow(Datum, 0);
            Grid.SetColumn(Datum, 0);
            Detail1_Grid.Children.Add(Datum);
            #endregion

            #region Uhrzeit
            TextBlock Uhrzeit = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Text = m.GetUhrzeit(),
                FontFamily = new FontFamily("Arial"),
                FontSize = 24
            };

            Grid.SetRow(Uhrzeit, 0);
            Grid.SetColumnSpan(Uhrzeit, 2);
            Grid.SetColumn(Uhrzeit, 1);
            Detail1_Grid.Children.Add(Uhrzeit);
            #endregion

            #region Laenge
            TextBlock Laenge = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Text = m.GetLaenge(),
                FontFamily = new FontFamily("Arial"),
                FontSize = 24
            };

            Grid.SetRow(Laenge, 0);
            Grid.SetColumnSpan(Laenge, 2);
            Grid.SetColumn(Laenge, 3);
            Detail1_Grid.Children.Add(Laenge);
            #endregion

            #region Info
            TextBlock Map = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Text = m.GetMap(),
                FontFamily = new FontFamily("Arial"),
                FontSize = 24
            };

            Grid.SetRow(Map, 0);
            Grid.SetColumn(Map, 5);
            Detail1_Grid.Children.Add(Map);

            TextBlock Id = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Text = m.GetId(),
                FontFamily = new FontFamily("Arial"),
                FontSize = 24
            };

            Grid.SetRow(Id, 0);
            Grid.SetColumn(Id, 6);
            Detail1_Grid.Children.Add(Id);

            TextBlock Runden = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Text = "",
                FontFamily = new FontFamily("Arial"),
                FontSize = 24
            };

            Grid.SetRow(Runden, 0);
            Grid.SetColumn(Runden, 7);
            Detail1_Grid.Children.Add(Runden);
            #endregion

            for (int i = 0; i < m.CountPlayer(); i++)
            {
                if (m.GetPlayer(i).GetTypName() == self_name)
                {
                    if (m.GetPlayer(i).GetErg())
                    {
                        Datum.Background = new SolidColorBrush(Colors.Green);
                        Uhrzeit.Background = new SolidColorBrush(Colors.Green);
                        Map.Background = new SolidColorBrush(Colors.Green);
                        Id.Background = new SolidColorBrush(Colors.Green);
                        Laenge.Background = new SolidColorBrush(Colors.Green);
                        Runden.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        Datum.Background = new SolidColorBrush(Colors.Red);
                        Uhrzeit.Background = new SolidColorBrush(Colors.Red);
                        Map.Background = new SolidColorBrush(Colors.Red);
                        Id.Background = new SolidColorBrush(Colors.Red);
                        Laenge.Background = new SolidColorBrush(Colors.Red);
                        Runden.Background = new SolidColorBrush(Colors.Red);
                    }

                    break;
                }
            }

            for (int i = 0; i < m.CountPlayer(); i++)
            {
                RowDefinition gridRowBody = new RowDefinition
                {
                    Height = new GridLength(25.75, GridUnitType.Pixel),
                };
                Detail2_Grid.RowDefinitions.Add(gridRowBody);

                #region Name
                TextBlock TempName = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = m.GetPlayer(i).GetTypName(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempName, i);
                Grid.SetColumn(TempName, 0);
                Detail2_Grid.Children.Add(TempName);
                #endregion

                #region Stat

                double damageDealGeneral = 0;
                double damageDealImportant = 0;

                double damageTakeGeneral = 0;
                double damageTakeImportant = 0;

                int kills = 0;
                int deaths = 0;

                //Bei Player Als Funktion ermöglichen
                foreach (Statistik s in m.GetPlayer(i).GetAllRundenstats())
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

                TextBlock TempStatDDG = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = damageDealGeneral.ToString(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempStatDDG, i);
                Grid.SetColumn(TempStatDDG, 1);
                Detail2_Grid.Children.Add(TempStatDDG);

                TextBlock TempStatDDI = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = damageDealImportant.ToString(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempStatDDI, i);
                Grid.SetColumn(TempStatDDI, 2);
                Detail2_Grid.Children.Add(TempStatDDI);

                TextBlock TempStatDTG = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = damageTakeGeneral.ToString(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempStatDTG, i);
                Grid.SetColumn(TempStatDTG, 3);
                Detail2_Grid.Children.Add(TempStatDTG);

                TextBlock TempStatDTI = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = damageTakeImportant.ToString(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempStatDTI, i);
                Grid.SetColumn(TempStatDTI, 4);
                Detail2_Grid.Children.Add(TempStatDTI);

                TextBlock TempStatK = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = kills.ToString(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempStatK, i);
                Grid.SetColumn(TempStatK, 5);
                Detail2_Grid.Children.Add(TempStatK);

                TextBlock TempStatD = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsHitTestVisible = false,
                    Text = deaths.ToString(),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 24
                };

                Grid.SetRow(TempStatD, i);
                Grid.SetColumn(TempStatD, 6);
                Detail2_Grid.Children.Add(TempStatD);
                #endregion
            }

            return;
        }

        private void MainWindowSchließen_Btn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).DetailfensterClose(Convert.ToInt32(this.Tag));

            this.Close();

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
    }
}
