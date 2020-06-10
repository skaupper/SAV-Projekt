using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CoronaTracker.Infrastructure
{
    /// <summary>
    /// The default DataGrid did not offer any possibility to bind to all selected items. It was only possible to bind to one of the selected items. This was caused by the
    /// fact that the items property is a readonly property. So no binding can be applied to it. Added additional code to this defualt DataGrid so it is now possible to bind 
    /// on the new selecteditems property.
    /// </summary>
    public class CustomDataGrid : DataGrid
    {
        #region SelectedItemsList Dependecy Property
        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGrid), new PropertyMetadata(null));
        #endregion

        #region CTOR
        public CustomDataGrid()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }
        #endregion CTOR

        #region selectionChanged event
        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        #endregion selectionChanged event
    }
}
