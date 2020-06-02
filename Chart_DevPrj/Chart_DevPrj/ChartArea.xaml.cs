using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
    using DataSetsType = BindingList<ObservableCollection<DataElement>>;


    public enum AxisScale
    {
        Linear,
        Logarithmic
    }

    public enum ChartType
    {
        Bars,
        StackedBars,
        Lines
    }

    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class ChartArea : UserControl, INotifyPropertyChanged
    {

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Constants

        private const int LogBase = 10;

        #endregion


        #region Private Members

        private AutoResetEvent updatingChartEvent;
        private CartesianMapper<ObservablePoint> mapper;

        #endregion


        #region Properties

        private SeriesCollection seriesCollection;
        private bool updatingChart;
        private bool disableAnimations;
        private AxesCollection axisX;
        private AxesCollection axisY;



        public static readonly DependencyProperty DataSetsProperty =
            DependencyProperty.Register(
                "DataSets", typeof(DataSetsType),
                typeof(ChartArea),
                new PropertyMetadata(null, Static_DataSets_Changed)
            );

        public static readonly DependencyProperty UsedChartTypeProperty =
            DependencyProperty.Register(
                "UsedChartType", typeof(ChartType),
                typeof(ChartArea),
                new PropertyMetadata(ChartType.Lines, Static_ChartType_Changed)
            );

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(
                "ScaleX", typeof(AxisScale),
                typeof(ChartArea),
                new PropertyMetadata(AxisScale.Linear, Static_AxisScale_Changed)
            );

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(
                "ScaleY", typeof(AxisScale),
                typeof(ChartArea),
                new PropertyMetadata(AxisScale.Linear, Static_AxisScale_Changed)
            );

        public static readonly DependencyProperty TitleXProperty =
            DependencyProperty.Register(
                "TitleX", typeof(string),
                typeof(ChartArea),
                new PropertyMetadata("X")
            );

        public static readonly DependencyProperty TitleYProperty =
            DependencyProperty.Register(
                "TitleY", typeof(string),
                typeof(ChartArea),
                new PropertyMetadata("Y")
            );



        public DataSetsType DataSets
        {
            get => (DataSetsType)GetValue(DataSetsProperty);
            set => SetValue(DataSetsProperty, value);
        }
        public ChartType UsedChartType
        {
            get => (ChartType)GetValue(UsedChartTypeProperty);
            set => SetValue(UsedChartTypeProperty, value);
        }
        public AxisScale ScaleX
        {
            get => (AxisScale)GetValue(ScaleXProperty);
            set => SetValue(ScaleXProperty, value);
        }
        public AxisScale ScaleY
        {
            get => (AxisScale)GetValue(ScaleYProperty);
            set => SetValue(ScaleYProperty, value);
        }
        public string TitleX
        {
            get => (string)GetValue(TitleXProperty);
            set => SetValue(TitleXProperty, value);
        }
        public string TitleY
        {
            get => (string)GetValue(TitleYProperty);
            set => SetValue(TitleYProperty, value);
        }







        /// <summary>
        /// Internal properties. Should not be used from the outside!
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set
            {
                seriesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeriesCollection"));
            }
        }
        public bool UpdatingChart
        {
            get => updatingChart;
            set
            {
                if (value)
                {
                    updatingChartEvent.WaitOne();
                }
                else
                {
                    updatingChartEvent.Set();
                }
                updatingChart = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UpdatingChart"));
            }
        }
        public bool DisableAnimations
        {
            get => disableAnimations;
            set
            {
                disableAnimations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableAnimations"));
            }
        }
        public AxesCollection AxisX
        {
            get => axisX;
            set
            {
                axisX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisX"));
            }
        }
        public AxesCollection AxisY
        {
            get => axisY;
            set
            {
                axisY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisY"));
            }
        }

        #endregion


        #region Static Callbacks

        private static void Static_ChartType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as ChartArea;
            chartArea.ChartType_Changed(d, e);
        }


        private static void Static_DataSets_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as ChartArea;

            if (e.OldValue != null)
            {
                var coll = (IBindingList)e.OldValue;
                coll.ListChanged -= chartArea.DataSets_ListChanged;
            }

            if (e.NewValue != null)
            {
                var coll = (IBindingList)e.NewValue;
                coll.ListChanged += chartArea.DataSets_ListChanged;
            }

            chartArea.CallUpdateDataSets();
        }

        private static void Static_AxisScale_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as ChartArea;
            chartArea.AxisScale_Changed(d, e);
        }

        #endregion


        #region Non-static Callbacks

        private void ChartType_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            CallUpdateDataSets();
        }

        private void DataSets_ListChanged(object sender, ListChangedEventArgs e)
        {
            CallUpdateDataSets();
        }

        private void AxisScale_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CallCreateAllAxes();
        }

        #endregion


        #region Public Methods

        public ChartArea()
        {
            updatingChartEvent = new AutoResetEvent(true);
            mapper = new CartesianMapper<ObservablePoint>();

            SeriesCollection = new SeriesCollection(mapper);
            AxisX = new AxesCollection();
            AxisY = new AxesCollection();

            InitializeComponent();

            //Chart.DisableAnimations = true;
        }


        /// <summary>
        /// TODO: How can these methods be dispatched from the UI thread?
        /// </summary>

        public void CallUpdateDataSets()
        {
            UpdatingChart = true;
            UpdateDataSets();
            UpdatingChart = false;
        }

        public void CallCreateAllAxes()
        {
            UpdatingChart = true;
            CreateAllAxes();
            UpdatingChart = false;
        }

        #endregion


        #region Private Methods

        private void CreateAllAxes()
        {
            CreateAxis(AxisX, ScaleX, TitleX);
            CreateAxis(AxisY, ScaleY, TitleY);
        }

        private void CreateAxis(AxesCollection coll, AxisScale scale, string title)
        {
            Axis ax = null;

            // Create the axis depending on the choses scale
            switch (scale)
            {
                case AxisScale.Linear:
                    ax = new Axis();

                    // Take points 1:1 as they are
                    mapper.X(p => p.X).Y(p => p.Y);
                    break;

                case AxisScale.Logarithmic:
                    var logAx = new LogarithmicAxis
                    {
                        // Use a constant base and set the minimum value to 1 (base^0)
                        Base = LogBase,
                        MinValue = 0
                    };

                    // The Y values are plotted as the log => the labels therefore need to be Base^value.
                    mapper.X(p => p.X).Y(p => (p.Y > 0) ? Math.Log(p.Y, LogBase) : 0);
                    logAx.LabelFormatter = value => Math.Pow(LogBase, value).ToString("N");

                    ax = logAx;
                    break;

                default:
                    Debug.Fail("An unknown axis scale has been encountered.");
                    break;
            }

            ax.Title = title;

            coll.Clear();
            coll.Add(ax);
        }

        private void UpdateDataSets()
        {
            SeriesCollection.Clear();

            foreach (var dataSet in DataSets)
            {
                Series series = null;

                // Depending on the chart type create different series object
                switch (UsedChartType)
                {
                    case ChartType.Bars: series = new ColumnSeries(); break;
                    case ChartType.StackedBars: series = new StackedColumnSeries(); break;
                    case ChartType.Lines: series = new LineSeries(); break;
                }


                // Transform DataElements into ObservablePoints
                var chartValues = new ChartValues<ObservablePoint>();
                foreach (var p in dataSet)
                {
                    chartValues.Add(new ObservablePoint
                    {
                        X = p.X,
                        Y = p.Y
                    });
                }
                series.Values = chartValues;

                // Add the series to the chart
                SeriesCollection.Add(series);
            }

            //CreateAllAxes();
        }

        #endregion 
    }
}
