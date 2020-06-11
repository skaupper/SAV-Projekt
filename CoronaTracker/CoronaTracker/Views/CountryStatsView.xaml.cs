using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace CoronaTracker.Views
{
    /// <summary>
    /// Interaction logic for CountryStatsView.xaml
    /// </summary>
    public partial class CountryStatsView : UserControl
    {
        public CountryStatsView()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Assert(e.AddedItems.Count <= 1);
            if (e.AddedItems.Count == 0)
            {
                return;
            }


        }
    }
}
