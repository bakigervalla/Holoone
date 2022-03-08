﻿using System;
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

namespace HolooneNavis.Views.Anchors.Anchors
{
    /// <summary>
    /// Interaction logic for AddAnchor.xaml
    /// </summary>
    public partial class AddAnchor : UserControl
    {
        public AddAnchor()
        {
            InitializeComponent();
            this.Loaded += AddAnchor_Loaded;
        }

        private void AddAnchor_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(AnchorNameTextBox);
        }
    }
}
