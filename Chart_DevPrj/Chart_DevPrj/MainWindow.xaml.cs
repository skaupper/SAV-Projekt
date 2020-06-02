using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Chart_DevPrj
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        BindingList<ObservableCollection<DataElement>> dataSets;
        public BindingList<ObservableCollection<DataElement>> DataSets
        {
            get => dataSets;
            set
            {
                dataSets = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DataSets"));
            }
        }


        AxisScale yScale;
        public AxisScale YScale
        {
            get => yScale;
            set
            {
                yScale = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("YScale"));
            }
        }

        ChartType chartType;
        public ChartType UsedChartType
        {
            get => chartType;
            set
            {
                chartType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UsedChartType"));
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;


            DataSets = new BindingList<ObservableCollection<DataElement>>{
                new ObservableCollection<DataElement>()
                {
                    new DataElement
                    {
                        X=0,
                        Y=2
                    },
                    new DataElement
                    {
                        X=2,
                        Y=4
                    },
                    new DataElement
                    {
                        X=3,
                        Y=40
                    }
                }
            };
        }

        static int nextX = 5;
        static Random rand = new Random();

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*
            if (YScale == AxisScale.Linear)
            {
                YScale = AxisScale.Logarithmic;
            }
            else
            {
                YScale = AxisScale.Linear;
            }
            */

            DataSets[0].Add(new DataElement { X = nextX++, Y = rand.Next(0, 100) });
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch(UsedChartType)
            {
                case ChartType.Bars:
                    UsedChartType = ChartType.Lines;
                    break;

                case ChartType.Lines:
                    UsedChartType = ChartType.Bars;
                    break;

                default:
                    UsedChartType = ChartType.Lines;
                    break;
            }
        }
    }
}
