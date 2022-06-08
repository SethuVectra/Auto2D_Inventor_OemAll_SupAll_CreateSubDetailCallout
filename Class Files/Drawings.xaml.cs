using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{

    public partial class Drawings : Window
    {
        private ViewModel viewModel;
        public Drawings(ViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            ListBox.ItemsSource = this.viewModel.Items;
        }
        
    }
    public class ViewModel
    {
        public ObservableCollection<Drawingitems> Items;
        public ViewModel(List<string> drawingList)
        {
            Items = new ObservableCollection<Drawingitems>();
            foreach (string path in drawingList)
            {
                Items.Add(new Drawingitems() { Item = System.IO.Path.GetFileNameWithoutExtension(path), Tooltip = path });
            }
        }

    }

    public class Drawingitems
    {
        public bool IsSelected { get; set; }
        public string Item { get; set; }
        public string Tooltip { get; set; }
    }
}
