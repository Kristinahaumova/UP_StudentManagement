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

namespace UP_Student_Management.Pages
{
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }

        private void Regin(object sender, RoutedEventArgs e)
        {

        }
    }
}
