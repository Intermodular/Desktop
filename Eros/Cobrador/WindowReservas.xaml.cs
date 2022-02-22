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
using Eros.Clases;
using Eros.Controladores;
using Eros.Modelos;

namespace Eros.Cobrador
{
    /// <summary>
    /// Interaction logic for WindowReservas.xaml
    /// </summary>
    public partial class WindowReservas : Window
    {
        List<PanelReserva> listPanelesReserva;
        PanelReserva selectedPanelReserva = null;
        public WindowReservas()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            DeleteAllExpiredReservas();
            InititalizeListPanelesReserva();
            usuName.Text = GlobalVariables.username;
            if (GlobalVariables.max)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = GlobalVariables.left;
                Top = GlobalVariables.top;
                Height = GlobalVariables.height;
                Width = GlobalVariables.width;
            }
        }
        private void DeleteAllExpiredReservas()
        {
            ControladorReservas.DeleteAllExpiredFromMinute(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
        }
        private void InititalizeListPanelesReserva()
        {
            wrapPanelReservas.Children.Clear();
            listPanelesReserva = new List<PanelReserva>();
            List<Reserva> listReservas = ControladorReservas.GetAllFromApi();
            foreach (Reserva reserva in listReservas)
            {
                ContruirPanel(reserva);
            }
            CheckIsEmpty();
        }
        private void ContruirPanel(Reserva reserva)
        {
            PanelReserva panel = new PanelReserva();
            panel.reserva = reserva;

            panel.button.Background = Brushes.Transparent;
            panel.button.BorderThickness = new Thickness(0);
            panel.button.Margin = new Thickness(30);
            panel.button.Style = Application.Current.Resources["buttonPanelReserva"] as Style;
            panel.button.Click += PanelLineaReserva_Click;


            panel.border.Width = 260;
            panel.border.BorderThickness = new Thickness(4);
            panel.border.BorderBrush = Brushes.Black;
            panel.border.CornerRadius = new CornerRadius(5);

            panel.borderArriba.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF58"); ;
            panel.borderArriba.CornerRadius = new CornerRadius(4, 4, 0, 0);
            panel.borderArriba.BorderThickness = new Thickness(0, 0, 0, 2);
            panel.borderArriba.BorderBrush = Brushes.Black;

            panel.borderAbajo.CornerRadius = new CornerRadius(0, 0, 3, 3);

            panel.tbkNombre.FontSize = 24;
            panel.tbkNombre.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkNombre.TextWrapping = TextWrapping.Wrap;
            panel.tbkNombre.TextAlignment = TextAlignment.Center;

            panel.stackPanelDatos.Margin = new Thickness(0, 10, 0, 10);

            panel.tbkFecha.Margin = new Thickness(0, 2, 0, 2);
            panel.tbkFecha.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkFecha.FontSize = 20;

            panel.tbkHora.Margin = new Thickness(0, 2, 0, 2);
            panel.tbkHora.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkHora.FontSize = 20;

            panel.tbkComensales.Margin = new Thickness(0, 2, 0, 2);
            panel.tbkComensales.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkComensales.FontSize = 20;

            panel.tbkMesa.Margin = new Thickness(0, 2, 0, 2);
            panel.tbkMesa.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkMesa.FontSize = 20;

            panel.UpdateUIElementsInfo();
            panel.ContruirPanel();
            wrapPanelReservas.Children.Add(panel.button);
            listPanelesReserva.Add(panel);
        }

        private void PanelLineaReserva_Click(object sender, RoutedEventArgs e)
        {
            PanelReserva clickedPLR = GetPanelReservaFromButton(sender as Button);
            if (clickedPLR != selectedPanelReserva)
            {
                ChangeStyleToSelected(clickedPLR);
                if (selectedPanelReserva != null)
                {
                    ChangeStyleToNotSelected(selectedPanelReserva);
                }
                selectedPanelReserva = clickedPLR;
            }
            btEdit.Visibility = selectedPanelReserva == null ? Visibility.Hidden : Visibility.Visible;
            btEliminar.Visibility = selectedPanelReserva == null ? Visibility.Hidden : Visibility.Visible;
        }

        private PanelReserva GetPanelReservaFromButton(Button b)
        {
            foreach (PanelReserva panel in listPanelesReserva)
            {
                if (panel.button == b)
                {
                    return panel;
                }
            }
            return null;
        }

        private void ChangeStyleToSelected(PanelReserva panelR)
        {
            panelR.borderArriba.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0093d7");
            panelR.button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#e6ebee");
            panelR.tbkNombre.Foreground = Brushes.White;
        }

        private void ChangeStyleToNotSelected(PanelReserva panelR)
        {
            panelR.borderArriba.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF58");
            panelR.button.Background = Brushes.Transparent;
            panelR.tbkNombre.Foreground = Brushes.Black;
        }
        private void CheckIsEmpty()
        {
            tbEmptyMessage.Visibility = wrapPanelReservas.Children.Count > 0 ? Visibility.Hidden : Visibility.Visible;
        }
        //Filtros---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void btBuscar_Click(object sender, RoutedEventArgs e)
        {
            ColocarPanelesReserva();

            if (selectedPanelReserva != null)
            {
                ChangeStyleToNotSelected(selectedPanelReserva);
                selectedPanelReserva = null;
            }
            btEdit.Visibility = Visibility.Hidden;
            btEliminar.Visibility = Visibility.Hidden;
            tbEmptyMessage.Text = "No se han encontrado reservas que cumplan con estos criterios";
            CheckIsEmpty();

        }
        private void ColocarPanelesReserva()
        {
            wrapPanelReservas.Children.Clear();
            tbFiltroNombre.Text = tbFiltroNombre.Text.Trim();
            string downedCaseFiltroNombre = tbFiltroNombre.Text.ToLower();
            foreach (PanelReserva pr in listPanelesReserva)
            {
                if (NamePassesFilter(pr, downedCaseFiltroNombre) && DatePassesFilter(pr))
                {
                    wrapPanelReservas.Children.Add(pr.button);
                }
            }
        }
        private bool NamePassesFilter(PanelReserva panelR, string name)
        {
            if (panelR.reserva.nombre.ToLower().Contains(name))
            {
                return true;
            }

            return false;
        }
        private bool DatePassesFilter(PanelReserva panelR)
        {
            if (dpFiltroFecha.Text == "")
            {
                return true;
            }
            if (panelR.tbkFecha.Text == dpFiltroFecha.Text)
            {
                return true;
            }
            return false;
        }
        //Eventos generales --------------------------------------------------------------------------------------------------------------------------------------
        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPanelReserva != null)
            {
                WindowEdicionReservas wer = new WindowEdicionReservas(selectedPanelReserva.reserva);
                wer.Show();
                this.Close();
            }

        }

        private void btAnyadir_Click(object sender, RoutedEventArgs e)
        {
            WindowEdicionReservas wer = new WindowEdicionReservas();
            wer.Show();
            this.Close();
        }

        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPanelReserva != null)
            {
                MessageBoxResult result = MessageBox.Show("Está seguro que desea borrar la reserva de " + selectedPanelReserva.reserva.nombre, "Borrar Reserva", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ControladorReservas.DeleteFromApi(selectedPanelReserva.reserva._id);
                    RefreshWindow();
                }
            }
        }

        private void RefreshWindow()
        {
            DeleteAllExpiredReservas();
            InititalizeListPanelesReserva();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WindowGestionMesas wgm = new WindowGestionMesas();
            wgm.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlobalVariables.top = Top;
            GlobalVariables.left = Top;
            GlobalVariables.width = Width;
            GlobalVariables.height = Height;
            GlobalVariables.max = WindowState == WindowState.Maximized;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllExpiredReservas();
            InititalizeListPanelesReserva();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                img_cuadrado.Source = new BitmapImage(new Uri(@"/Eros;component/Img/icons/cuadrado.png", UriKind.Relative));
                GlobalVariables.max = false;
            }
            else
            {
                WindowState = WindowState.Maximized;
                img_cuadrado.Source = new BitmapImage(new Uri(@"/Eros;component/Img/icons/cuadrado2.png", UriKind.Relative));
                GlobalVariables.max = true;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
