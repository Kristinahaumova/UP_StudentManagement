using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }
        private void Auth(object sender, RoutedEventArgs e)
        {
            //string login = txtLogin.Text.Trim();
            //string password = pwdPassword.Password.Trim();

            //UserContext userContext = new UserContext();
            //var allUsers = userContext.AllUsers();

            //var user = allUsers.FirstOrDefault(u => u.Name == login && u.Password == password);

            //if (user == null)
            //{
            //    MessageBox.Show("Неверный логин или пароль.");
            //    return;
            //}
            //if (user.isAdmin == 0)
            //{
            //    MainWindow.init.OpenPage(new Pages.User.Main());
            //}
            //if (user.isAdmin == 1)
            //{
            //    MainWindow.init.OpenPage(new Pages.Admin.Main());
            //}
            MainWindow.init.OpenPage(new Pages.Admin.Main());
        }
        private void OpenRegin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Registration());
        }
    }
}
