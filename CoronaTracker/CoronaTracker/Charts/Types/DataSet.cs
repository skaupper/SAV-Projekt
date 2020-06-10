using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CoronaTracker.Charts.Types
{
    public class DataSet : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private ObservableCollection<DataElement> values = new ObservableCollection<DataElement>();
        private string name = "<anonymous>";

        public ObservableCollection<DataElement> Values
        {
            get => values;
            set
            {
                values = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
    }
}
