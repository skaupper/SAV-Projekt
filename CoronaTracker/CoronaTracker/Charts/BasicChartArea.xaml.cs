﻿using CoronaTracker.Charts.Types;
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
    public partial class BasicChartArea : UserControl, INotifyPropertyChanged
    {

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Constants

        private const int LogBase = 10;

        #endregion


        #region Private Members

        private CartesianMapper<DataElement> mapper;

        #endregion


        #region Properties

        private SeriesCollection seriesCollection;
        private bool disableAnimations;
        private AxesCollection axisX;
        private AxesCollection axisY;



        public static readonly DependencyProperty DataSetsProperty =
            DependencyProperty.Register(
                "DataSets", typeof(DataSetsType),
                typeof(BasicChartArea),
                new PropertyMetadata(null, Static_DataSets_Changed)
            );

        public static readonly DependencyProperty UsedChartTypeProperty =
            DependencyProperty.Register(
                "UsedChartType", typeof(ChartType),
                typeof(BasicChartArea),
                new PropertyMetadata(ChartType.Lines, Static_ChartType_Changed)
            );

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(
                "ScaleX", typeof(AxisScale),
                typeof(BasicChartArea),
                new PropertyMetadata(AxisScale.Linear, Static_AxisX_Changed)
            );

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(
                "ScaleY", typeof(AxisScale),
                typeof(BasicChartArea),
                new PropertyMetadata(AxisScale.Linear, Static_AxisY_Changed)
            );

        public static readonly DependencyProperty TitleXProperty =
            DependencyProperty.Register(
                "TitleX", typeof(string),
                typeof(BasicChartArea),
                new PropertyMetadata("X", Static_TitleX_Changed)
            );

        public static readonly DependencyProperty TitleYProperty =
            DependencyProperty.Register(
                "TitleY", typeof(string),
                typeof(BasicChartArea),
                new PropertyMetadata("Y", Static_TitleY_Changed)
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

        #endregion


        #region Static Callbacks

        private static void Static_ChartType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as BasicChartArea;
            chartArea.ChartType_Changed(d, e);
        }


        private static void Static_DataSets_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as BasicChartArea;

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

        private static void Static_AxisX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as BasicChartArea;
            chartArea.AxisXScale_Changed(d, e);
        }

        private static void Static_AxisY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as BasicChartArea;
            chartArea.AxisYScale_Changed(d, e);
        }

        private static void Static_TitleX_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as BasicChartArea;
            if (chartArea?.Chart?.AxisX == null)
            {
                return;
            }

            if (chartArea.Chart.AxisX.Count > 0)
            {
                chartArea.Chart.AxisX[0].Title = chartArea.TitleX;
            }
        }

        private static void Static_TitleY_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as BasicChartArea;
            if (chartArea?.Chart?.AxisY == null)
            {
                return;
            }

            if (chartArea.Chart.AxisY.Count > 0)
            {
                chartArea.Chart.AxisY[0].Title = chartArea.TitleY;
            }
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

        private void AxisXScale_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CallCreateAxisX();
        }

        private void AxisYScale_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CallCreateAxisY();
        }

        #endregion


        #region Public Methods

        public BasicChartArea()
        {
            mapper = new CartesianMapper<DataElement>();

            SeriesCollection = new SeriesCollection(mapper);
            AxisX = new AxesCollection();
            AxisY = new AxesCollection();

            InitializeComponent();

            Loaded += (s, e) => CallCreateAllAxes();
        }


        /// <summary>
        /// TODO: How can these methods be dispatched from the UI thread?
        /// </summary>

        public void CallUpdateDataSets()
        {
            UpdateDataSets();
        }

        public void CallCreateAllAxes()
        {
            CreateAllAxes();
        }

        public void CallCreateAxisX()
        {
            CreateAxis(AxisX, ScaleX, TitleX);
        }

        public void CallCreateAxisY()
        {
            CreateAxis(AxisY, ScaleY, TitleY);
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
                    // Per default take points 1:1 as they are
                    if (coll == AxisY)
                    {
                        mapper.Y(p => p.Y);
                    }
                    else if (coll == AxisX)
                    {
                        mapper.X(p => p.X.Ticks / TimeSpan.FromDays(1).Ticks);
                        ax.LabelFormatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");
                    }
                    else
                    {
                        Debug.Fail("Unknown axis given");
                    }
                    break;

                case AxisScale.Logarithmic:
                    var logAx = new LogarithmicAxis
                    {
                        // Use a constant base and set the minimum value to 1 (base^0)
                        Base = LogBase,
                        MinValue = 0
                    };

                    // The values are plotted as the log => the labels therefore need to be Base^value.
                    logAx.LabelFormatter = value => Math.Pow(LogBase, value).ToString("N");
                    ax = logAx;


                    if (coll == AxisY)
                    {
                        if (UsedChartType == ChartType.StackedBars)
                        {
                            throw new ArgumentException("The logarithmic Y axis is not compatible with the chart type StackedBars!");
                        }
                        mapper.Y(p => (p.Y > 0) ? Math.Log(p.Y, LogBase) : 0);
                    }
                    else if (coll == AxisX)
                    {
                        throw new ArgumentException("Logarithmic X axes are not supported!");
                    }
                    else
                    {
                        Debug.Fail("Unknown axis given");
                    }
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
                    case ChartType.Bars: series = new ColumnSeries(); break;
                    case ChartType.StackedBars: series = new StackedColumnSeries(); break;
                    case ChartType.Lines: series = new LineSeries(); break;
                }
                
                series.Title = dataSet.Name;
                series.Values = new ChartValues<DataElement>(dataSet.Values);

                // Add the series to the chart
                SeriesCollection.Add(series);
            }
        }

        #endregion 
    }
}