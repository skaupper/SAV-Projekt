using CoronaTracker.Infrastructure;
using System.ComponentModel;

namespace CoronaTracker.Models
{
    class GraphSelection : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;




        private string _name = null;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public GraphSelection()
        {

        }
        public GraphSelection(string name)
        {
            Name = name;
        }
    }
}
