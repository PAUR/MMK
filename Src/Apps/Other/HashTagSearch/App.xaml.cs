﻿using MMK.HashTagSearch.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MMK.HashTagSearch
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainViewModel viewModel = null;


        public static MainViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new MainViewModel();
                }
                return viewModel;
            }
        }
    }
}
