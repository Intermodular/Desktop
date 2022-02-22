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
using Eros.Modelos;
using Eros.Controladores;
using Eros.Clases;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Eros.Administrador
{
    /// <summary>
    /// Interaction logic for WindowEditarNomina.xaml
    /// </summary>
    public partial class WindowEditarNomina : Window
    {
        bool creandoNueva;
        Nominas nomina;
        BitmapImage checkIconSource;
        BitmapImage wrongIconSource;
        public WindowEditarNomina(Empleado empleado)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            checkIconSource = new BitmapImage(new Uri(@"../Img/icons/check.png", UriKind.Relative));
            wrongIconSource = new BitmapImage(new Uri(@"../Img/icons/wrong.png", UriKind.Relative));
            creandoNueva = true;
            nomina = new Nominas();
            nomina.idEmpleado = empleado._id;
            nomina.nombreEmpleado = empleado.nombre;
            nomina.apellidoEmpleado = empleado.apellido;
            nomina.dniEmpleado = empleado.dni;
            nomina.direccionEmpleado = empleado.direccion;
            PutEmpleadoData();

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

        public WindowEditarNomina(Nominas nomina)
        {
            InitializeComponent();
            checkIconSource = new BitmapImage(new Uri(@"../Img/icons/check.png", UriKind.Relative));
            wrongIconSource = new BitmapImage(new Uri(@"../Img/icons/wrong.png", UriKind.Relative));
            creandoNueva = false;
            this.nomina = nomina;
            btGuardar.Content = "Editar";
            tbkTitle.Text = "Editar Nómina";
            PutEmpleadoData();
            PutNominaData();

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

        private void PutEmpleadoData()
        {
            tbNombre.Text = nomina.nombreEmpleado;
            tbApellido.Text = nomina.apellidoEmpleado;
            tbDNI.Text = nomina.dniEmpleado;
            tbDomicilio.Text = nomina.direccionEmpleado;
        }

        private void PutNominaData()
        {
            dpFInicial.Text = nomina.fechaInicio;
            dpFFinal.Text = nomina.fechaFinal;
            tbHoras.Text = nomina.horasCorrientes + "";
            tbEurosHora.Text = nomina.precioHoraCorriente + "";
            tbHorasEx.Text = nomina.horasExtras + "";
            tbEurosHoraEx.Text = nomina.precioHoraExtra + "";
            tbRemuneracion.Text = String.Format("{0:0.00}€", nomina.remuneracionTotal);
        }

        private float CalculateRemuneracionTotalBasedOnText()
        {
            return nomina.horasCorrientes * nomina.precioHoraCorriente + nomina.horasExtras * nomina.precioHoraExtra;
        }

        private float CalcularRemuneracion()
        {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ",";

            int horasC, horasEx = 0;
            float pHorasC, pHorasE = 0;
            try
            {
                horasC = Convert.ToInt32(tbHoras.Text);
                if (horasC < 0)
                    return -1;

                pHorasC = float.Parse(tbEurosHora.Text, ci);
                if (pHorasC < 0)
                    return -1;


                if (tbHorasEx.Text != "")
                {
                    horasEx = Convert.ToInt32(tbHorasEx.Text);
                    if (horasEx < 0)
                        return -1;
                }

                if (tbEurosHoraEx.Text != "")
                {
                    pHorasE = float.Parse(tbEurosHoraEx.Text, ci);
                    if (pHorasE < 0)
                        return -1;
                }

                return horasC * pHorasC + horasEx * pHorasE;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        private void UpdateRemText()
        {
            float total = CalcularRemuneracion();
            if (total == -1)
            {
                tbRemuneracion.Text = "Error";
            }
            else
            {
                tbRemuneracion.Text = String.Format("{0:0.00}€", total);
            }

        }
        //Eventos TextBoxes
        private void tbHoras_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarHoras();
            UpdateRemText();
        }

        private void tbEurosHora_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarPrecioHoras();
            UpdateRemText();
        }

        private void tbHorasEx_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarHorasExtra();
            UpdateRemText();
        }

        private void tbEurosHoraEx_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarPrecioHorasExtra();
            UpdateRemText();
        }

        private void dpFInicial_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarFechaInicial();
            UpdateRemText();
        }

        private void dpFFinal_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidarFechaFinal();
            UpdateRemText();
        }
        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            WindowNominas wn = new WindowNominas();
            wn.Show();
            this.Close();
        }
        private void btGuardar_Click(object sender, RoutedEventArgs e)
        {
            bool val1 = ValidarHoras(); // Hay que hacerlo asi ya que si concatenas && no se ejecutan todos
            bool val2 = ValidarPrecioHoras();
            bool val3 = ValidarHorasExtra();
            bool val4 = ValidarPrecioHorasExtra();
            bool val5 = ValidarFechaInicial();
            bool val6 = ValidarFechaFinal();
            if (val1 && val2 && val3 && val4 && val5 && val6)
            {
                var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.NumberDecimalSeparator = ",";

                nomina.fechaInicio = dpFInicial.Text;
                nomina.fechaFinal = dpFFinal.Text;
                nomina.horasCorrientes = Convert.ToInt32(tbHoras.Text);
                nomina.precioHoraCorriente = float.Parse(tbEurosHora.Text, ci);
                nomina.horasExtras = tbHorasEx.Text == "" ? 0 : Convert.ToInt32(tbHorasEx.Text);
                nomina.precioHoraExtra = tbEurosHoraEx.Text == "" ? 0 : float.Parse(tbEurosHoraEx.Text, ci);
                nomina.remuneracionTotal = nomina.horasCorrientes * nomina.precioHoraCorriente + nomina.horasExtras * nomina.precioHoraExtra;
                if (creandoNueva)
                {
                    try
                    {
                        ControladorNominas.PostToApi(nomina);
                        MessageBox.Show("Nómina Creada");
                    } 
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }
                }
                else
                {
                    try
                    {
                        ControladorNominas.UpdateInApi(nomina);
                        MessageBox.Show("Nómina Editada");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }
                }

                WindowNominas wn = new WindowNominas();
                wn.Show();
                this.Close();
                return;
            }

            MessageBox.Show("Errores encontrados");
        }

        //Validaciones

        private bool ValidarHoras()
        {
            imgCheckHoras.Visibility = Visibility.Visible;
            tbHoras.Text = tbHoras.Text.Trim();

            if (tbHoras.Text == "")
            {
                imgCheckHoras.Source = wrongIconSource;
                tbkImageToolTipHoras.Text = "Campo Obligatorio";
                return false;
            }
            if (!Regex.IsMatch(tbHoras.Text, @"^([0-9]+)$"))
            {
                imgCheckHoras.Source = wrongIconSource;
                tbkImageToolTipHoras.Text = "Introduzca un numero entero positivo";
                return false;
            }

            imgCheckHoras.Source = checkIconSource;
            tbkImageToolTipHoras.Text = "Correcto";
            return true;
        }

        private bool ValidarPrecioHoras()
        {
            imgCheckPHoras.Visibility = Visibility.Visible;
            tbEurosHora.Text = tbEurosHora.Text.Trim();
            if (tbEurosHora.Text == "")
            {
                imgCheckPHoras.Source = wrongIconSource;
                tbkImageToolTipPHoras.Text = "Campo Obligatorio";

                return false;
            }
            if (!Regex.IsMatch(tbEurosHora.Text, @"^[0-9]+(,[0-9]{1,2})?$"))
            {
                imgCheckPHoras.Source = wrongIconSource;
                tbkImageToolTipPHoras.Text = "Introduzca un numero decimal positivo (2 decimales max)";

                return false;
            }
            imgCheckPHoras.Source = checkIconSource;
            tbkImageToolTipPHoras.Text = "Correcto";

            return true;

        }

        private bool ValidarHorasExtra()
        {
            imgCheckHorasEx.Visibility = Visibility.Visible;
            tbHorasEx.Text = tbHorasEx.Text.Trim();

            if (!Regex.IsMatch(tbHorasEx.Text, @"^[0-9]*$"))
            {
                imgCheckHorasEx.Source = wrongIconSource;
                tbkImageToolTipHorasEx.Text = "Introduzca un numero entero positivo";
                ValidarPrecioHorasExtra();
                return false;
            }

            imgCheckHorasEx.Source = checkIconSource;
            tbkImageToolTipHorasEx.Text = "Correcto";
            ValidarPrecioHorasExtra();
            return true;
        }

        private bool ValidarPrecioHorasExtra()
        {
            imgCheckPHorasEx.Visibility = Visibility.Visible;
            tbEurosHoraEx.Text = tbEurosHoraEx.Text.Trim();
            if (tbHorasEx.Text.Trim() != "" && tbEurosHoraEx.Text == "")
            {
                imgCheckPHorasEx.Source = wrongIconSource;
                tbkImageToolTipPHorasEx.Text = "Campo Obligatorio si H.E. no esta vacío";
                return false;
            }
            if (!Regex.IsMatch(tbEurosHoraEx.Text, @"^([0-9]+(,[0-9]{1,2})?)?$"))
            {
                imgCheckPHorasEx.Source = wrongIconSource;
                tbkImageToolTipPHorasEx.Text = "Introduzca un numero decimal (2)";
                return false;
            }
            imgCheckPHorasEx.Source = checkIconSource;
            tbkImageToolTipPHorasEx.Text = "Correcto";
            return true;
        }

        private bool ValidarFechaInicial()
        {
            imgCheckFInicial.Visibility = Visibility.Visible;
            string fecha = dpFInicial.Text;
            if (fecha == "")
            {
                imgCheckFInicial.Source = wrongIconSource;
                tbkImageToolTipFInic.Text = "Campo Obligatorio";
                return false;
            }
            imgCheckFInicial.Source = checkIconSource;
            tbkImageToolTipFInic.Text = "Correcto";
            return true;
        }
        private bool ValidarFechaFinal()
        {
            imgCheckFFinal.Visibility = Visibility.Visible;
            string fecha = dpFFinal.Text;
            if (fecha == "")
            {
                imgCheckFFinal.Source = wrongIconSource;
                tbkImageToolTipFFinal.Text = "Campo Obligatorio";
                return false;
            }
            if (dpFInicial.SelectedDate != null && dpFFinal.SelectedDate <= dpFInicial.SelectedDate)
            {
                imgCheckFFinal.Source = wrongIconSource;
                tbkImageToolTipFFinal.Text = "La fecha final tiene que ser mayor que la inicial";
                return false;
            }
            imgCheckFFinal.Source = checkIconSource;
            tbkImageToolTipFFinal.Text = "Correcto";
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
            WindowNominas wn = new WindowNominas();
            wn.Show();
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
