using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


namespace Eros
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int eyes = 0;
        int image_show = 0;
        Regex regex = new Regex("^[a-zA-Z0-9_]+$");

        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(img_transition);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
            dispatcherTimer.Start();
        }
        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            WindowProducts wp= new WindowProducts();
            wp.Show();
        }
        */
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (password.Password.Length == 0 && tb_pass.Text.Length == 0)
            {
                btn_login.IsEnabled = false;
                pass_placeholder.Visibility = Visibility.Visible;
            }
            else if ((password.Password.Length > 3 || tb_pass.Text.Length > 3) && user.Text.Length > 3 && regex.IsMatch(user.Text))
            {

                btn_login.IsEnabled = true;
            }
            else
            {
                pass_placeholder.Visibility = Visibility.Hidden;
                btn_login.IsEnabled = false;
            }
        }

        private void btn_eye_Click(object sender, RoutedEventArgs e)
        {
            if (eyes == 0)
            {
                img_eye.Source = new BitmapImage(new Uri(@"/Eros;component/ShowPass.png", UriKind.Relative));
                password.Visibility = Visibility.Hidden;
                tb_pass.Text = password.Password;
                tb_pass.Visibility = Visibility.Visible;
                password.Password = "";
                eyes = 1;
            }
            else
            {
                img_eye.Source = new BitmapImage(new Uri(@"/Eros;component/NoShowPass.png", UriKind.Relative));
                tb_pass.Visibility = Visibility.Hidden;
                password.Password = tb_pass.Text;
                password.Visibility = Visibility.Visible;
                tb_pass.Text = "";
                eyes = 0;
            }

        }
        private void img_transition(object sender, EventArgs e)
        {
            try
            {
                BitmapImage[] images =
                    {
                    new BitmapImage(new Uri(@"/Eros;component/hamburguesa.jpg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/pizza2.jpg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/hamburguesa2.jpg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/pizza.png", UriKind.Relative))
                    };
                img_login.Source = images[image_show];

            }
            catch (Exception)
            {
                MessageBox.Show("No se ha encontrado la imagen");
            };
            if (image_show == 3)
            {
                image_show = 0;
            }
            else
            {
                image_show++;
            }
        }

        private void tb_pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (password.Password.Length == 0 && tb_pass.Text.Length == 0)
            {
                btn_login.IsEnabled = false;
                pass_placeholder.Visibility = Visibility.Visible;
            }
            else if ((password.Password.Length > 3 || tb_pass.Text.Length > 3) && user.Text.Length > 3 && regex.IsMatch(user.Text))
            {

                btn_login.IsEnabled = true;
            }
            else
            {
                pass_placeholder.Visibility = Visibility.Hidden;
                btn_login.IsEnabled = false;
            }
        }

        private void user_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!regex.IsMatch(user.Text) && user.Text != "")
            {
                lbl_error.Content = "No se admiten caracteres especiales";
                user.BorderBrush = System.Windows.Media.Brushes.Red;
                btn_login.IsEnabled = false;
            }
            else if ((password.Password.Length > 3 || tb_pass.Text.Length > 3) && user.Text.Length > 3)
            {
                lbl_error.Content = "";
                user.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                btn_login.IsEnabled = true;
            }
            else
            {
                lbl_error.Content = "";
                user.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                btn_login.IsEnabled = false;
            }
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            Rol rol = new Rol();
            rol.ShowDialog();
        }

        private void user_LostFocus(object sender, RoutedEventArgs e)
        {
            if (user.Text.Length < 4)
            {
                lbl_error.Content = "El usuario debe tener +4 caracteres";
                user.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else if (user.Text.Length > 3 && !regex.IsMatch(user.Text))
            {
                user.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

        }
    }
}
