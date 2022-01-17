using Autodesk.Navisworks.Api;
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
        private ExportBIMNewViewModel DX { get; set; }

        public ModelSelectionWindow()
        {
            InitializeComponent();
            this.Loaded += ModelSelectionWindow_Loaded;
        }

        private void ModelSelectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DX = this.DataContext as ExportBIMNewViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedModel = (ModelItem)((TextBlock)sender).Tag;
            DX.SelectedModelItem = selectedModel;
            ;
        }
    }
}
