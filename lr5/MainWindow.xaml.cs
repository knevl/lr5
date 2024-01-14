using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lr5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void b_customer_Click(object sender, RoutedEventArgs e)
        {
            var customerWin = new customer_win();
            Close();
            customerWin.Show();
        }

        private void b_order_Click(object sender, RoutedEventArgs e)
        {
            var orderWin = new order_win();
            Close();
            orderWin.Show();
        }

        private void b_positions_Click(object sender, RoutedEventArgs e)
        {
            var positionsWin = new positions_win();
            Close(); 
            positionsWin.Show();
        }

        private void b_product_Click(object sender, RoutedEventArgs e)
        {
            var productWin = new product_win();
            Close();
            productWin.Show();
        }

        private void b_confectioner_Click(object sender, RoutedEventArgs e)
        {
            var confectionerWin = new confectioner_win();
            Close();
            confectionerWin.Show();
        }
    }
}