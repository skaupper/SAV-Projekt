using Microsoft.Win32;
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
        DataLoader dataLoader;

        public MainWindow()
        {
            InitializeComponent();

            ResponseRaw = "(currently not in use)";
            dataLoader = new DataLoader();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void LoadAPIButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await dataLoader.LoadAllDataAsync(DataLoader.Source.API);
            }
            catch (FieldAccessException faex)
            {
                // TODO: Handle exception (need to load data first)
                MessageBox.Show("FieldAccessException: \n" + faex.Message);
            }
            catch (ArgumentException argex)
            {
                // TODO: Handle exception (maybe an implementation error?)
                MessageBox.Show("ArgumentException: \n" + argex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Handle exception (need to load data first)
                MessageBox.Show("Unhandled exception: \n" + ex.Message);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResponseList"));
        }

        private async void LoadLocalButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Catch and handle exception(s)

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.Filter = "JSON file (*.json)|*.json";

            if (ofd.ShowDialog() == true)
            {
                await dataLoader.LoadAllDataAsync(DataLoader.Source.LOCALFILE, ofd.FileName);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.Filter = "JSON file (*.json)|*.json";

            if (sfd.ShowDialog() == true)
            {
                // TODO: catch exception(s)
                dataLoader.SaveAllData(sfd.FileName);
            }
        }

        private void UpdateViewButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseList = dataLoader.GetAllCountryCurrentData();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResponseList"));
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseList.Clear();
            ResponseList = null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResponseList"));
        }
    }
}
