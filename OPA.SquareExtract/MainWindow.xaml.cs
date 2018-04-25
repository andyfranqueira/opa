// <copyright file="MainWindow.xaml.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using CsvHelper;
using Microsoft.Win32;
using OPA.Payments;
using System.Configuration;

namespace OPA.SquareExtract
{
    public partial class MainWindow : Window
    {
        private static readonly string SquareKey = ConfigurationManager.AppSettings["key:Square"];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetData(object sender, RoutedEventArgs eventArgs)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "SquareExtract",
                DefaultExt = ".csv",
                Filter = "CSV file (*.csv)|*.csv"
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                SquareTransactions.IsEnabled = false;
                BusyImage.Visibility = Visibility.Visible;

                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += CreateExtract;
                backgroundWorker.RunWorkerCompleted += DisplayComplete;
                backgroundWorker.RunWorkerAsync(new[] {dialog.FileName});
            }
        }

        private static void CreateExtract(object sender, DoWorkEventArgs eventArgs)
        {
            var param = eventArgs.Argument as string[];
            var startDate = DateTime.Now.AddDays(-14);
            var endDate = DateTime.Now.AddDays(1);
            var transactions = PaymentManager.GetSquareTransactions(SquareKey, startDate, endDate);

            using (var textWriter = File.CreateText(param != null ? param[0] : string.Empty))
            {
                var csv = new CsvWriter(textWriter);
                csv.WriteRecords(transactions);
            }
        }

        private void DisplayComplete(object sender, RunWorkerCompletedEventArgs eventArgs)
        {
            SquareTransactions.Content = "All done!";
            BusyImage.Visibility = Visibility.Hidden;
            DoneImage.Visibility = Visibility.Visible;
        }
    }
}
