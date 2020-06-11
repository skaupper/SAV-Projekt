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
    public partial class RatioBar : UserControl, INotifyPropertyChanged
    {

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Private Members

        private CartesianMapper<DataElement> mapper;
        private DateHelper dateHelper;

        #endregion


        #region Properties

        private SeriesCollection seriesCollection;
        private bool disableAnimations;
        private Func<double, string> formatterX;
        private Func<double, string> formatterY;



        public static readonly DependencyProperty DataSetsProperty =
            DependencyProperty.Register(
                "DataSets", typeof(DataSetsType),
                typeof(RatioBar),
                new PropertyMetadata(null, Static_DataSets_Changed)
            );

        public static readonly DependencyProperty TitleXProperty =
            DependencyProperty.Register(
                "TitleX", typeof(string),
                typeof(RatioBar),
                new PropertyMetadata("X")
            );

        public static readonly DependencyProperty TitleYProperty =
            DependencyProperty.Register(
                "TitleY", typeof(string),
                typeof(RatioBar),
                new PropertyMetadata("Y")
            );

        public static readonly DependencyProperty LegendLocationProperty =
            DependencyProperty.Register(
                "LegendLocation", typeof(LegendLocation),
                typeof(RatioBar),
                new PropertyMetadata(LiveCharts.LegendLocation.Top)
            );




        public DataSetsType DataSets
        {
            get => (DataSetsType)GetValue(DataSetsProperty);
            set => SetValue(DataSetsProperty, value);
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
        public Func<double, string> FormatterX
        {
            get => formatterX;
            set
            {
                formatterX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormatterX"));
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

        private static void Static_DataSets_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = d as RatioBar;

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

        #endregion


        #region Non-static Callbacks

        private void DataSets_ListChanged(object sender, ListChangedEventArgs e)
        {
            CallUpdateDataSets();
        }

        #endregion


        #region Public Methods

        public RatioBar()
        {
            InitializeComponent();

            dateHelper = new DateHelper(TimeSpan.FromDays(1));

            FormatterX = val => val.ToString("P0");
            FormatterY = val => dateHelper.FromDouble(val).ToString("d");

            mapper = new CartesianMapper<DataElement>();
            mapper.X(p => Math.Round(p.Y, 0)).Y(p => dateHelper.ToDouble(p.X));
            SeriesCollection = new SeriesCollection(mapper);

            Chart.DataContext = this;
        }


        // TODO: How can this method be dispatched from the UI thread?
        public void CallUpdateDataSets()
        {
            UpdateDataSets();
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
                SeriesCollection.Add(new StackedRowSeries
                {
                    Values = new ChartValues<DataElement> { dataSet.Values.Last() },
                    StackMode = StackMode.Percentage,
                    DataLabels = true,
                    LabelPoint = p => p.X.ToString(),
                    Title = dataSet.Name
                });
            }
        }

        #endregion 
    }
}
