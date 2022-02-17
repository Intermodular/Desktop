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
using System.Windows.Shell;

namespace Eros.Administrador
{
    /// <summary>
    /// Lógica de interacción para WindowMainAdministration.xaml
    /// </summary>
    public partial class WindowMainAdministration : Window
    {
        bool max = false;
        public WindowMainAdministration()
        {
            InitializeComponent();
            WindowChrome wc = new WindowChrome();
            wc.CaptionHeight = 0.1;
            WindowChrome.SetWindowChrome(this, wc);

        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualHeight > 800 && ActualWidth > 1600)
            {
                changeSize(btn_empleados, 60, 300, 25);
                changeSize(btn_zonas, 60, 300, 25);
                changeSize(btn_productos, 60, 300, 25);
                changeSize(btn_logout, 50, 50, 1);
                changeSize(btn_cobrador, 50, 50, 1);
                btn_logout.Margin = new Thickness(120, 180, 0, 0);
                btn_cobrador.Margin = new Thickness(0, 180, 120, 0);
            }
            else if (ActualHeight > 600 && ActualWidth > 1100)
            {
                changeSize(btn_empleados, 40, 225, 20);
                changeSize(btn_zonas, 40, 225, 20);
                changeSize(btn_productos, 40, 225, 20);
                changeSize(btn_logout, 40, 40, 1);
                changeSize(btn_cobrador, 40, 40, 1);
                btn_logout.Margin = new Thickness(100, 120, 0, 0);
                btn_cobrador.Margin = new Thickness(0, 120, 100, 0);
            }
            else if (ActualHeight > 400 && ActualWidth > 700)
            {
                changeSize(btn_empleados, 30, 150, 14);
                changeSize(btn_zonas, 30, 150, 14);
                changeSize(btn_productos, 30, 150, 14);
                changeSize(btn_logout, 30, 30, 1);
                changeSize(btn_cobrador, 30, 30, 1);
                btn_logout.Margin = new Thickness(70, 60, 0, 0);
                btn_cobrador.Margin = new Thickness(0, 60, 70, 0);
            }
            else
            {
                changeSize(btn_empleados, 25, 125, 11);
                changeSize(btn_zonas, 25, 125, 11);
                changeSize(btn_productos, 25, 125, 11);
                changeSize(btn_logout, 20, 20, 1);
                changeSize(btn_cobrador, 20, 20, 1);
                btn_logout.Margin = new Thickness(60, 30, 0, 0);
                btn_cobrador.Margin = new Thickness(0, 30, 60, 0);
            }
        }

        private void changeSize(Button btn, int height, int width, int fontsize)
        {
            btn.Height = height;
            btn.Width = width;
            btn.FontSize = fontsize;
        }

        private void btn_logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Estás seguro de cerrar sesión?", "Confirmación", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    MainWindow mw = new MainWindow();
                    this.Close();
                    mw.Show();
                    break;
                case MessageBoxResult.No:
                   
                    break;
            }
        }

        private void btn_zonas_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            img_gestion.Source = new BitmapImage(new Uri(@"/Eros;component/Img/zonas.jpg", UriKind.Relative));
        }

        private void btn_empleados_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            img_gestion.Source = new BitmapImage(new Uri(@"/Eros;component/Img/empleados.png", UriKind.Relative));
        }

        private void btn_productos_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            img_gestion.Source = new BitmapImage(new Uri(@"/Eros;component/Img/productos.jpg", UriKind.Relative));
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btn_window_Click(object sender, RoutedEventArgs e)
        {
            if (max == false)
            {
                WindowState = WindowState.Maximized;
                img_cuadrado.Source = new BitmapImage(new Uri(@"/Eros;component/Img/icons/cuadrado2.png", UriKind.Relative));
                max = true;
            }
            else
            {
                WindowState = WindowState.Normal;
                img_cuadrado.Source = new BitmapImage(new Uri(@"/Eros;component/Img/icons/cuadrado.png", UriKind.Relative));
                max = false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btn_productos_Click(object sender, RoutedEventArgs e)
        {
            WindowProducts wp = new WindowProducts();
            wp.Show();
            this.Close();
        }

        private void btn_empleados_Click(object sender, RoutedEventArgs e)
        {
            WindowEmpleados we = new WindowEmpleados();
            we.Show();
            this.Close();
        }

        private void btn_zonas_Click(object sender, RoutedEventArgs e)
        {
            WindowZones wz = new WindowZones();
            wz.Show();
            this.Close();
        }

        private void btn_empleados_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void btn_zonas_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void btn_productos_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
    }
}
