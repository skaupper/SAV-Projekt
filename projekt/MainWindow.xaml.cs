using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string ResponseRaw { get; private set; }
        public List<CountryData> ResponseList { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CallAPI()
        {
            DataDownloader dl = new DataDownloader();

            string url = "https://api.thevirustracker.com/free-api?countryTotals=ALL";

            ResponseRaw = dl.GetDataRaw(url);
            ResponseList = dl.GetDataDeserialized(url);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResponseRaw"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResponseList"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CallAPI();
        }
    }
}
