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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using CoronaTracker.Charts.Types;

namespace CoronaTracker.Charts
{
    using HeatMapValueType = BindingList<HeatMapElement>;


    public partial class BasicGeoMap : UserControl, INotifyPropertyChanged
    {

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Properties

        private bool hoverable;
        private Dictionary<string, double> internalHeatMap;
        private Dictionary<string, string> langPack;
        private string mapSource;



        public static readonly DependencyProperty HeatMapProperty =
            DependencyProperty.Register(
                "HeatMap", typeof(HeatMapValueType),
                typeof(BasicGeoMap),
                new PropertyMetadata(new HeatMapValueType(), Static_HeatMap_Changed)
            );




        public HeatMapValueType HeatMap
        {
            get => (HeatMapValueType)GetValue(HeatMapProperty);
            set => SetValue(HeatMapProperty, value);
        }


        #region Internal Properties

        public bool Hoverable
        {
            get => hoverable;
            set
            {
                hoverable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hoverable"));
            }
        }

        public Dictionary<string, double> InternalHeatMap
        {
            get => internalHeatMap;
            set
            {
                internalHeatMap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InternalHeatMap"));
            }
        }

        public Dictionary<string, string> LanguagePack
        {
            get => langPack;
            set
            {
                langPack = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LanguagePack"));
            }
        }

        public string SourceFile
        {
            get => mapSource;
            set
            {
                mapSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceFile"));
            }
        }

        #endregion

        #endregion


        #region Static Callbacks

        private static void Static_HeatMap_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = d as BasicGeoMap;

            if (e.OldValue != null)
            {
                var coll = (IBindingList)e.OldValue;
                coll.ListChanged -= inst.HeatMap_ListChanged;
            }

            if (e.NewValue != null)
            {
                var coll = (IBindingList)e.NewValue;
                coll.ListChanged += inst.HeatMap_ListChanged;
            }

            inst.HeatMap_Changed(d, e);
        }

        #endregion


        #region Non-static Callbacks

        private void HeatMap_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateHeatMap();
        }

        private void HeatMap_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateHeatMap();
        }

        #endregion


        #region Public Methods

        public BasicGeoMap()
        {
            InternalHeatMap = new Dictionary<string, double>();
            LanguagePack = new Dictionary<string, string>();
            Hoverable = true;


            try
            {
                string tempPath = System.IO.Path.GetTempFileName();
                File.WriteAllText(tempPath, Maps.World);
                SourceFile = tempPath;
            }
            catch (IOException)
            {
                Chart.Visibility = Visibility.Collapsed;
                ErrorTextBlock.Visibility = Visibility.Visible;
            }

            InitializeComponent();
        }

        #endregion


        #region Private Methods

        private void UpdateHeatMap()
        {
            Dictionary<string, double> tmp = new Dictionary<string, double>();

            foreach (var item in HeatMap)
            {
                tmp[item.Country] = item.Value;
            }

            InternalHeatMap = tmp;
        }

        #endregion


        #region UI Event Handlers

        private void Chart_LandClick(object sender, LiveCharts.Maps.MapData e)
        {
            // TODO: do we need this event?
        }

        #endregion
    }
}
