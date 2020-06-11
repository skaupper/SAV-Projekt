using CoronaTracker.Infrastructure;

namespace CoronaTracker.Models
{
    class GraphSelection : NotifyBase
    {
        private string _name = null;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
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
