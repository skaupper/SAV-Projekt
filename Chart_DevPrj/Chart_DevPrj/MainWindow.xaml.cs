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
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion



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

        BindingList<HeatMapElement> heatMap;
        public BindingList<HeatMapElement> HeatMap
        {
            get => heatMap;
            set
            {
                heatMap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeatMap"));
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

        string yTitle;
        public string YTitle
        {
            get => yTitle;
            set
            {
                yTitle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("YTitle"));
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



            YTitle = "Y";

            DataSets = new BindingList<ObservableCollection<DataElement>>{
                new ObservableCollection<DataElement>()
                {
                    new DataElement
                    {
                        X=1,
                        Y=2
                    },
                    new DataElement
                    {
                        X=10,
                        Y=4
                    },
                    new DataElement
                    {
                        X=100,
                        Y=40
                    }
                }
            };

            HeatMap = new BindingList<HeatMapElement>{
                new HeatMapElement
                {
                    Country = "US",
                    Value = 10
                }
            };
        }

        static int nextX = 5;
        static Random rand = new Random();

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {/*
            if (YScale == AxisScale.Linear)
            {
                YScale = AxisScale.Logarithmic;
            }
            else
            {
                YScale = AxisScale.Linear;
            }
            */
            YTitle += "T";
            /*
            DataSets[0].Add(new DataElement { X = nextX++, Y = rand.Next(0, 100) });



            HeatMap.Add(new HeatMapElement
            {
                Country = "DE",
                Value = 100
            });
            */
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (UsedChartType)
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
