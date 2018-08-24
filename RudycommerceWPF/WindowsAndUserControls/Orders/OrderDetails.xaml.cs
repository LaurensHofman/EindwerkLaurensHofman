using RudycommerceData.Entities.Orders;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
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
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls.Orders
{
    /// <summary>
    /// Interaction logic for OrderDetails.xaml
    /// </summary>
    public partial class OrderDetails : WindowsAndUserControls.Abstracts.MultilingualWindow
    {
        /// <summary>
        /// Submit event from the order details. Allows to let its status be updated
        /// </summary>
        /// <param name="id"></param>
        public delegate void OrderReady(int id);
        public event OrderReady OrderReadyEvent;

        private IIncOrderRepository _orderRepo;
        private IProductRepository _prodRepo;

        public IncomingOrder OrderModel { get; set; }

        private List<Product> ProductList;

        public OrderDetails()
        {
            InitializeComponent();
        }

        public OrderDetails(int id)
        {
            InitializeComponent();

            _orderRepo = new IncOrderRepository();
            _prodRepo = new ProductRepository();

            // Gets the order
            OrderModel = _orderRepo.Get(id);

            // Sets the display language
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            // Gets all the product IDs from the incoming order lines, belonging to the incoming order.
            List<int> productIDs = OrderModel.IncomingOrderLines.Select(c => c.ProductID).Distinct().ToList();
            // Gets all the products that match the list of product IDs from the order
            ProductList = _prodRepo.GetAllQueryable().Where(x => productIDs.Contains(x.ID)).ToList();

            DisplayOrderLines();
        }

        private void DisplayOrderLines()
        {
            foreach (var orderLine in OrderModel.IncomingOrderLines)
            {
                // Gets the product belonging to a product line
                var product = ProductList.FirstOrDefault(x => x.ID == orderLine.ProductID);
                // Gets the localized product, based on the preferred display language of the user.
                var locProduct = product.LocalizedProducts.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID);
                
                // Creates a label for the ProductCode and puts it in the designated left stack panel
                Label lblProductCode = new Label
                {
                    Margin = new Thickness(0, 75, 0, 0),
                    Content = LangResource.ProductCode + ": ",
                    Style = Application.Current.Resources["FormLabel"] as Style
                };
                stackLeft.Children.Add(lblProductCode);

                // Creates a label that shows the value of the ProductCode and puts it in the designated right stack panel
                Label lblProductCodeVal = new Label
                {
                    Margin = new Thickness(0, 75, 0, 0),
                    Content = product.ID,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Style = Application.Current.Resources["FormLabel"] as Style
                };
                stackRight.Children.Add(lblProductCodeVal);

                // Creates a label for the Quantity and Product Name and puts it in the designated left stack panel
                Label lblProductName = new Label
                {
                    Content =  LangResource.Product + ": ",
                    Style = Application.Current.Resources["FormLabel"] as Style
                };
                stackLeft.Children.Add(lblProductName);

                // Creates a label that shows the values of the Quantity and the Product Name and puts it in the designated right stack panel
                Label lblProductNameVal = new Label
                {
                    Content = orderLine.ProductQuantity + " x " + locProduct.Name,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Style = Application.Current.Resources["FormLabel"] as Style
                };
                stackRight.Children.Add(lblProductNameVal);
            }            
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            OrderReadyEvent(OrderModel.ID);
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
