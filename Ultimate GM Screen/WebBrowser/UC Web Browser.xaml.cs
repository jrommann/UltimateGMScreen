﻿using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ultimate_GM_Screen.WebBrowser
{
    /// <summary>
    /// Interaction logic for UC_Web_Browser.xaml
    /// </summary>
    public partial class UC_Web_Browser : UserControl
    {
        string _startingAddresss = string.Empty;
        public string StartingAddress 
        {
            get 
            {
                if (browser.IsInitialized)
                    return browser.Source.ToString();
                else
                    return _startingAddresss;
            }
            set
            {
                if (browser.IsInitialized && !string.IsNullOrEmpty(value))
                    browser.Source = new Uri(value);

                addressbox.Text = value;
                _startingAddresss = value;
            }
        }        

        public UC_Web_Browser()
        {
            InitializeComponent();           
            InitializeAsync();                        
        }

        async void InitializeAsync()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            var browserEnviorment = await CoreWebView2Environment.CreateAsync(null, Common.UserDataFolder);
            await browser.EnsureCoreWebView2Async(browserEnviorment);            
            
            browser.DefaultBackgroundColor = System.Drawing.Color.FromArgb(61, 61, 76);
            if (!string.IsNullOrEmpty(StartingAddress))
            {
                browser.Source = new Uri(StartingAddress);
                addressbox.Text = StartingAddress;
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if(browser.Source != null)
                browser.Reload();
        }

        private void addressbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return)
                browser.Source = new Uri(addressbox.Text, UriKind.Absolute);
        }

        private void goBtn_Click(object sender, RoutedEventArgs e)
        {
            browser.Source = new Uri(addressbox.Text, UriKind.Absolute);
        }

        private async void clearCacheBtn_Click(object sender, RoutedEventArgs e)
        {
            await browser.CoreWebView2.Profile.ClearBrowsingDataAsync();
        }
    }
}
