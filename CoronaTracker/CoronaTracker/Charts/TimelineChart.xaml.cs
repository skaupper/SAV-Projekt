using CoronaTracker.Charts.Helper;
using CoronaTracker.Charts.Types;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
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

namespace CoronaTracker.Charts
{
    using DataSetsType = BindingList<Types.DataSet>;

    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class TimelineChart : UserControl, INotifyPropertyChanged
    {

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Constants

        public int LogBase { get; } = 10;

        #endregion


        #region Private Members

        private CartesianMapper<DataElement> mapper;
        private DateHelper dateHelper;

        #endregion


        #region Properties

        private SeriesCollection seriesCollection;
        private Func<double, string> formatterX;
        private Func<double, string> formatterY;
        private Func<double, string> formatterYLog;



        public static readonly DependencyProperty DataSetsProperty =
            DependencyProperty.Register(
                "DataSets", typeof(DataSetsType),
                typeof(TimelineChart),
                new PropertyMetadata(null, Static_DataSets_Changed)
            );

        public static readonly DependencyProperty UsedChartTypeProperty =
            DependencyProperty.Register(
                "UsedChartType", typeof(ChartType),
                typeof(TimelineChart),
                new PropertyMetadata(ChartType.Lines, Static_ChartType_Changed)
            );

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(
                "ScaleY", typeof(AxisScale),
                typeof(TimelineChart),
                new PropertyMetadata(AxisScale.Linear, Static_AxisY_Changed)
            );

        public static readonly DependencyProperty TitleXProperty =
            DependencyProperty.Register(
                "TitleX", typeof(string),
                typeof(TimelineChart),
                new PropertyMetadata("X")
            );

        public static readonly DependencyProperty TitleYProperty =
            DependencyProperty.Register(
                "TitleY", typeof(string),
                typeof(TimelineChart),
                new PropertyMetadata("Y")
            );

        public static readonly DependencyProperty LegendLocationProperty =
            DependencyProperty.Register(
                "LegendLocation", typeof(LegendLocation),
                typeof(TimelineChart),
                new PropertyMetadata(LiveCharts.LegendLocation.Top)
            );

        public static readonly DependencyProperty IsChartEnabledProperty =
            DependencyProperty.Register(
                "IsChartEnabled", typeof(bool),
                typeof(TimelineChart),
                new PropertyMetadata(true)
            );

        public static readonly DependencyProperty DisableAnimationsProperty =
            DependencyProperty.Register(
                "DisableAnimations", typeof(bool),
                typeof(TimelineChart),
                new PropertyMetadata(true)
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
        public LegendLocation LegendLocation
        {
            get => (LegendLocation)GetValue(LegendLocationProperty);
            set => SetValue(LegendLocationProperty, value);
        }
        public bool IsChartEnabled
        {
            get => (bool)GetValue(IsChartEnabledProperty);
            set => SetValue(IsChartEnabledProperty, value);
        }
        public bool DisableAnimations
        {
            get => (bool)GetValue(DisableAnimationsProperty);
            set => SetValue(DisableAnimationsProperty, value);
        }





        #region Internal Properties

        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set
            {
                seriesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeriesCollection"));
            }
        }

        public Func<double, string> FormatterX
        {
            get => formatterX;
            set
            {
                formatterX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormatterX"));
            }
        }

        public Func<double, string> FormatterYLog
        {
            get => formatterYLog;
            set
            {
                formatterYLog = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormatterYLog"));
            }
        }

        public Func<double, string> FormatterY
        {
            get => formatterY;
            set
            {
                formatterY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormatterY"));
            }
        }

        #endregion

        #endregion


        #region Static Callbacks

        private static void Static_ChartType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as TimelineChart;
            chartArea.ChartType_Changed(d, e);
        }


        private static void Static_DataSets_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as TimelineChart;

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

        private static void Static_AxisY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as TimelineChart;
            chartArea.AxisYScale_Changed(d, e);
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

        private void AxisYScale_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CallCreateAxisY();
        }

        #endregion


        #region Public Methods

        public TimelineChart()
        {
            InitializeComponent();
            Chart.DataContext = this;

            // Initialize private members
            dateHelper = new DateHelper(TimeSpan.FromDays(1));
            mapper = new CartesianMapper<DataElement>();

            mapper.X(p => dateHelper.ToDouble(p.Date));
            mapper.Y(p => p.Value);

            // Internal properties
            SeriesCollection = new SeriesCollection(mapper);
            FormatterX = val => dateHelper.FromDouble(val).ToString("d");
            FormatterY = val => val.ToString("N0");
            FormatterYLog = val => Math.Pow(LogBase, val).ToString("N0");

            UpdateYAxis();
        }


        // TODO: How can these methods be dispatched from the UI thread?

        public void CallUpdateDataSets()
        {
            UpdateDataSets();
        }

        public void CallCreateAxisY()
        {
            UpdateYAxis();
        }

        #endregion


        #region Private Methods

        private void UpdateDataSets()
        {
            SeriesCollection.Clear();

            if (DataSets == null)
            {
                return;
            }

            foreach (var dataSet in DataSets)
            {
                Series series = null;

                // Depending on the chart type create different series object
                switch (UsedChartType)
                {
                    case ChartType.Bars: series = new ColumnSeries { ColumnPadding = 1 }; break;
                    case ChartType.StackedBars: series = new StackedColumnSeries { ColumnPadding = 1 }; break;
                    case ChartType.Lines: series = new LineSeries(); break;
                }

                series.Title = dataSet.Name;
                series.Values = new ChartValues<DataElement>(dataSet.Values);

                // Add the series to the chart
                SeriesCollection.Add(series);
            }
        }

        private void UpdateYAxis()
        {
            Chart.AxisY.Clear();

            if (ScaleY == AxisScale.Linear)
            {
                Axis ax = new Axis();
                ax.SetBinding(Axis.LabelFormatterProperty, "FormatterY");
                ax.SetBinding(Axis.TitleProperty, "TitleY");
                Chart.AxisY.Add(ax);

                mapper.Y(p => p.Value);
            }
            else if (ScaleY == AxisScale.Logarithmic)
            {
                LogarithmicAxis ax = new LogarithmicAxis { MinValue = 0 };
                ax.SetBinding(LogarithmicAxis.BaseProperty, "LogBase");
                ax.SetBinding(Axis.LabelFormatterProperty, "FormatterYLog");
                ax.SetBinding(Axis.TitleProperty, "TitleY");
                Chart.AxisY.Add(ax);

                mapper.Y(p => (p.Value > 0) ? Math.Log(p.Value, LogBase) : double.NaN);
            }
        }

        #endregion 
    }
}
