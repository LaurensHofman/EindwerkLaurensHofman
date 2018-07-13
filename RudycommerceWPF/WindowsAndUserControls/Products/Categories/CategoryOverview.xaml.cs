using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Categories
{
    /// <summary>
    /// Interaction logic for CategoryOverview.xaml
    /// </summary>
    public partial class CategoryOverview : OverviewUserControl
    {
        public CategoryOverview()
        {
            InitializeComponent();
        }

        public override void LoadDataGridData()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void Update(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
