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

namespace Holoone.Core.Views.Export.Default.ExportDefault
{
    /// <summary>
    /// Interaction logic for SelectionView.xaml
    /// </summary>
    public partial class Selection : UserControl
    {
        public Selection()
        {
            InitializeComponent();
            //txtNum.Text = "1";
        }

        private int _numValue = 1;

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            if(NumValue == 0)
                return;
            NumValue--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();

        }

    }
}
