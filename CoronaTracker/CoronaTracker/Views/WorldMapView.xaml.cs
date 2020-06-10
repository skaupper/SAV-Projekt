using CoronaTracker.Charts.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for WorldmapView.xaml
    /// </summary>
    public partial class WorldMapView : UserControl, INotifyPropertyChanged
    {
        public WorldMapView()
        {
            HeatMap = new BindingList<HeatMapElement>()
            {
                new HeatMapElement
                {
                    Country = "US",
                    Value = 123
                }
            };
            DataContext = this;
            InitializeComponent();

        }




        BindingList<HeatMapElement> heatMap;

        public event PropertyChangedEventHandler PropertyChanged;

        public BindingList<HeatMapElement> HeatMap
        {
            get => heatMap;
            set
            {
                heatMap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeatMap"));
            }
        }
    }
}
