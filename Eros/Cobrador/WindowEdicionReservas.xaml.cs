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
using System.Text.RegularExpressions;
using Eros.Clases;
using Eros.Modelos;
using Eros.Controladores;
using Eros.Cobrador.UtilWindows;

namespace Eros.Cobrador
{
    /// <summary>
    /// Interaction logic for WindowEdicionReservas.xaml
    /// </summary>
    public partial class WindowEdicionReservas : Window
    {
        Reserva reserva;
        BitmapImage checkIconSource;
        BitmapImage wrongIconSource;
        public static Mesas mesa = null;
        bool isPosting;
        public WindowEdicionReservas()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            isPosting = true;
            checkIconSource = new BitmapImage(new Uri(@"../Img/icons/check.png", UriKind.Relative));
            wrongIconSource = new BitmapImage(new Uri(@"../Img/icons/wrong.png", UriKind.Relative));
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
        public WindowEdicionReservas(Reserva reserva)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            isPosting = false;
            btGuardar.Content = "Editar";
            checkIconSource = new BitmapImage(new Uri(@"../Img/icons/check.png", UriKind.Relative));
            wrongIconSource = new BitmapImage(new Uri(@"../Img/icons/wrong.png", UriKind.Relative));
            this.reserva = reserva;
            ColocarDatosDeReserva();
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

        private void ColocarDatosDeReserva()
        {
            tbNombre.Text = reserva.nombre;
            tbComensales.Text = reserva.numComensales + "";
            dpFecha.Text = String.Format("{0:00}/{1:00}/{2:0000}", reserva.dia, reserva.mes, reserva.anyo);
            tbHora.Text = String.Format("{0:00}", reserva.hora);
            tbMin.Text = String.Format("{0:00}", reserva.minuto);
            tbNumMesa.Text = reserva.numMesa + "";
        }

        private void tbNombre_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateNombre();
        }

        private void tbComensales_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateComensales();
        }

        private void dpFecha_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFecha();
        }

        private void tbHora_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateHora();
        }

        private void tbMin_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateMinuto();
        }

        private void tbNumMesa_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateNumMesa();
        }

        //Botones --------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void btBuscarMesa_Click(object sender, RoutedEventArgs e)
        {
            bool val1 = ValidateFecha();
            bool val2 = ValidateHora();
            bool val3 = ValidateMinuto();
            bool val4 = ValidateComensales();
            if (val1 && val2 && val3 && val4)
            {
                int anyo, mes, dia, hora, minuto, comensales;
                anyo = dpFecha.SelectedDate.Value.Year;
                mes = dpFecha.SelectedDate.Value.Month;
                dia = dpFecha.SelectedDate.Value.Day;
                hora = Convert.ToInt32(tbHora.Text);
                minuto = Convert.ToInt32(tbMin.Text);
                comensales = Convert.ToInt32(tbComensales.Text);

                DialogMesasReservadas dmr = new DialogMesasReservadas(anyo, mes, dia, hora, minuto, comensales);
                dmr.ShowDialog();
                if (mesa != null)
                {
                    tbNumMesa.Text = mesa.numero + "";
                }
            }
            else
            {
                MessageBox.Show("Tienes que rellenar correctamente los campos de fecha, hora, minutos y comensales para ver las mesas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            WindowReservas wr = new WindowReservas();
            this.Close();
            wr.Show();
        }

        private void btGuardar_Click(object sender, RoutedEventArgs e)
        {
            bool val1 = ValidateNombre();
            bool val2 = ValidateComensales();
            bool val3 = ValidateFecha();
            bool val4 = ValidateHora();
            bool val5 = ValidateMinuto();
            bool val6 = ValidateNumMesa();
            if (val1 && val2 && val3 && val4 && val5 && val6)
            {
                if (mesa != null && Convert.ToInt32(tbComensales.Text) > mesa.numSillas)
                {
                    MessageBox.Show("No hay bastantes sillas en esta mesa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (isPosting)
                {
                    reserva = new Reserva();
                    reserva.idMesa = mesa._id;
                    reserva.numMesa = mesa.numero;
                    reserva.nombre = tbNombre.Text;
                    reserva.numComensales = Convert.ToInt32(tbComensales.Text);
                    reserva.anyo = dpFecha.SelectedDate.Value.Year;
                    reserva.mes = dpFecha.SelectedDate.Value.Month;
                    reserva.dia = dpFecha.SelectedDate.Value.Day;
                    reserva.hora = Convert.ToInt32(tbHora.Text);
                    reserva.minuto = Convert.ToInt32(tbMin.Text);

                    string respuesta = ControladorReservas.PostToApi(reserva);
                    if (respuesta == "Error reserva ocupada")
                    {
                        MessageBox.Show("Se ha encontrado otra reserva en esta mesa a esta hora, porfavor pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else if (respuesta != "OK")
                    {
                        MessageBox.Show("Ha ocurrido un error al subir la reserva", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        WindowReservas wr = new WindowReservas();
                        this.Close();
                        wr.Show();
                    }
                }
                else
                {
                    if (mesa != null)
                    {
                        reserva.idMesa = mesa._id;
                        reserva.numMesa = mesa.numero;
                    }
                    reserva.nombre = tbNombre.Text;
                    reserva.numComensales = Convert.ToInt32(tbComensales.Text);
                    reserva.anyo = dpFecha.SelectedDate.Value.Year;
                    reserva.mes = dpFecha.SelectedDate.Value.Month;
                    reserva.dia = dpFecha.SelectedDate.Value.Day;
                    reserva.hora = Convert.ToInt32(tbHora.Text);
                    reserva.minuto = Convert.ToInt32(tbMin.Text);
                    string respuesta = ControladorReservas.UpdateInApi(reserva);
                    if (respuesta == "Error reserva ocupada")
                    {
                        MessageBox.Show("Se ha encontrado otra reserva en esta mesa a esta hora, porfavor pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else if (respuesta != "OK")
                    {
                        MessageBox.Show("Ha ocurrido un error al subir la reserva", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        WindowReservas wr = new WindowReservas();
                        this.Close();
                        wr.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("Se han encontrado errores. Por favor revíselos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        //Validaciones ------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public bool ValidateNombre()
        {
            imgFiltroNombre.Visibility = Visibility.Visible;
            tbNombre.Text = tbNombre.Text.Trim();

            if (tbNombre.Text == "")
            {
                imgFiltroNombre.Source = wrongIconSource;
                toolTipNombre.Text = "No puedes dejar este campo vacío";
                return false;
            }
            if (Regex.IsMatch(tbNombre.Text, @"^([a-zA-Z]{1}[ a-zA-Z]{0,20})$"))
            {
                imgFiltroNombre.Source = checkIconSource;
                toolTipNombre.Text = "Correcto";
                return true;
            }


            imgFiltroNombre.Source = wrongIconSource;
            toolTipNombre.Text = "Solo puede contener caracteres alfabéticos no especiales (max 20 caracteres)";
            return false;

        }

        public bool ValidateComensales()
        {
            imgFiltroComensales.Visibility = Visibility.Visible;
            tbComensales.Text = tbComensales.Text.Trim();

            if (tbComensales.Text == "")
            {
                imgFiltroComensales.Source = wrongIconSource;
                toolTipComensales.Text = "No puedes dejar este campo vacío";
                return false;
            }
            if (Regex.IsMatch(tbComensales.Text, @"^([0-9]{1,2})$"))
            {
                imgFiltroComensales.Source = checkIconSource;
                toolTipComensales.Text = "Correcto";
                return true;
            }

            imgFiltroComensales.Source = wrongIconSource;
            toolTipComensales.Text = "Debe de ser un numero entero de 1 o 2 cifras";
            return false;
        }

        public bool ValidateFecha()
        {
            imgFiltroFecha.Visibility = Visibility.Visible;

            if (dpFecha.Text == "")
            {
                imgFiltroFecha.Source = wrongIconSource;
                toolTipFecha.Text = "No puedes dejar este campo vacío";
                return false;
            }
            if (dpFecha.SelectedDate < DateTime.Today)
            {
                imgFiltroFecha.Source = wrongIconSource;
                toolTipFecha.Text = "La reserva no puede ser para un dia anterior";
                return false;
            }

            imgFiltroFecha.Source = checkIconSource;
            toolTipFecha.Text = "Correcta";
            return true;
        }

        public bool ValidateHora()
        {
            imgFiltroHora.Visibility = Visibility.Visible;
            tbHora.Text = tbHora.Text.Trim();

            if (tbHora.Text == "")
            {
                imgFiltroHora.Source = wrongIconSource;
                toolTipHora.Text = "No puedes dejar este campo vacío";
                return false;
            }
            try
            {
                int hora = Convert.ToInt32(tbHora.Text);
                if (hora < 0 || hora > 24)
                {
                    imgFiltroHora.Source = wrongIconSource;
                    toolTipHora.Text = "La hora debe ser mayor que 0 y menor que 24";
                    return false;
                }
                else
                {
                    imgFiltroHora.Source = checkIconSource;
                    toolTipHora.Text = "Correcto";
                    return true;
                }

            }
            catch (Exception e)
            {
                imgFiltroHora.Source = wrongIconSource;
                toolTipHora.Text = "La hora debe ser un numero entero";
                return false;
            }
        }

        public bool ValidateMinuto()
        {
            imgFiltroMin.Visibility = Visibility.Visible;
            tbMin.Text = tbMin.Text.Trim();

            if (tbMin.Text == "")
            {
                imgFiltroMin.Source = checkIconSource;
                toolTipMinutos.Text = "Correcto";
                tbMin.Text = "00";
                return true;
            }
            try
            {
                int minuto = Convert.ToInt32(tbMin.Text);
                if (minuto < 0 || minuto >= 60)
                {
                    imgFiltroMin.Source = wrongIconSource;
                    toolTipMinutos.Text = "El minuto debe ser mayor que 0 y menor que 60";
                    return false;
                }
                else
                {
                    imgFiltroMin.Source = checkIconSource;
                    toolTipMinutos.Text = "Correcto";
                    return true;
                }

            }
            catch (Exception e)
            {
                imgFiltroMin.Source = wrongIconSource;
                toolTipMinutos.Text = "El minuto debe ser un numero entero";
                return false;
            }
        }

        public bool ValidateNumMesa()
        {
            imgFiltroNumMesa.Visibility = Visibility.Visible;

            if (tbNumMesa.Text == "")
            {
                imgFiltroNumMesa.Source = wrongIconSource;
                toolTipNumMesa.Text = "Debes de asignarle un numero de mesa";
                return false;
            }
            imgFiltroNumMesa.Source = checkIconSource;
            toolTipNumMesa.Text = "Correcto";
            return true;
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WindowReservas wr = new WindowReservas();
            wr.Show();
            this.Close();
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
