
using System.Windows;

namespace CoronaTracker.Infrastructure
{
    interface IPageViewModel
    {
        string Name { get; }

        bool IsEnabled { get; set; }

        bool IsSelected { get; set; }

        void SetupPage();
    }
}
