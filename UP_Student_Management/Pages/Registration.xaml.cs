using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages
{
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Regin(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = pwdPassword.Password.Trim();
            string retryPassword = pwdRetryPassword.Password.Trim();

            if (string.IsNullOrEmpty(login)) 
            {
                MessageBox.Show("Заполните логин");
            }
            if (string.IsNullOrEmpty(password)) 
            {
                MessageBox.Show("Заполните пароль");
            }
            if (string.IsNullOrEmpty(retryPassword)) 
            {
                MessageBox.Show("Повторите пароль");
            }
            if (password != retryPassword) 
            {
                MessageBox.Show("Пароли должны совпадать");
            }
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Логин должен содержать только латинские буквы и цифры.");
                return;
            }
            if (password.Length < 6  || Regex.IsMatch(password, @"[А-Яа-яЁё]"))
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов, без кириллицы.");
                return;
            }
            try 
            {
                UserContext userContext = new UserContext();

                var allUsers = userContext.AllUsers();
                bool loginExists = allUsers.Exists(u => u.Name.Equals(login, StringComparison.OrdinalIgnoreCase));
                if (loginExists)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }
                UserContext newUser = new UserContext
                {
                    Name = login,
                    Password = password,
                    isAdmin = 0
                };
                newUser.Save(false);

                MainWindow.init.OpenPage(new Pages.Login());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex}");
            }
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }
    }
}
