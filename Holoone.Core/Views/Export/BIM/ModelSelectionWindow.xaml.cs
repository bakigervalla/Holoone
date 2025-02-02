﻿using Autodesk.Navisworks.Api;
using HolooneNavis.ViewModels.Export.BIM.Existing;
using HolooneNavis.ViewModels.Export.BIM.New;
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
using System.Windows.Shapes;

namespace HolooneNavis.Views.Export.BIM
{
    /// <summary>
    /// Interaction logic for ModelSelectionWindow.xaml
    /// </summary>
    public partial class ModelSelectionWindow : Window
    {
        private bool _isNewBIM;
        private ExportBIMNewViewModel BIMNewViewModel { get; set; }
        private ExportBIMExistingViewModel BIMExistingViewModel { get; set; }

        public ModelSelectionWindow(bool IsNewBIM)
        {
            InitializeComponent();

            _isNewBIM = IsNewBIM;
            this.Loaded += ModelSelectionWindow_Loaded;
        }

        private void ModelSelectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isNewBIM)
                BIMNewViewModel = this.DataContext as ExportBIMNewViewModel;
            else
                BIMExistingViewModel = this.DataContext as ExportBIMExistingViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BIMNewViewModel?.SelectedModelItem == null && BIMExistingViewModel?.SelectedModelItem == null)
            {
                MessageBox.Show("Model item was not selected. Please select a model");
                return;
            }
            this.DialogResult = true;
            this.Close();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedModel = (ModelItem)((TextBlock)sender).Tag;

            if (_isNewBIM)
                BIMNewViewModel.SelectedModelItem = selectedModel;
            else
                BIMExistingViewModel.SelectedModelItem = selectedModel;
            ;
        }
    }
}
