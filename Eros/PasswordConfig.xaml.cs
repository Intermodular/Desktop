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
using System.Windows.Shapes;

namespace Eros
{
    /// <summary>
    /// Interaction logic for PasswordConfig.xaml
    /// </summary>
    public partial class PasswordConfig : Window
    {
        List<Empleado> empleados = new List<Empleado>();
        public Boolean _Valid = false;
        Empleado emp = new Empleado();

        public PasswordConfig()
        {
            InitializeComponent();
        }

        public PasswordConfig(Empleado user)
        {
            InitializeComponent();
            emp = user;
        }

        private void Atras_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (ttPassword.Text == "Correcto." && ttRepeatPassword.Text == "Correcto.")
            {
                _Valid = true;
                empleados = ControladorEmpleados.GetAllFromApi();

                foreach (Empleado empleado in empleados)
                {
                    if (emp.usuario == empleado.usuario)
                    {
                        emp = empleado;
                        emp.password = Password.Password;
                        emp.newUser = false;

                        ControladorEmpleados.UpdateInApi(emp);
                    }
                }
            }
            
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            imgCheckPassword.Visibility = Visibility.Visible;

            if (Password.Password == "")
            {
                errorString = "El campo Contraseña no puede estar vacío.";
                imgCheckPassword.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                ttPassword.Text = errorString;

            }
            else if (!Regex.IsMatch(Password.Password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$"))
            {
                errorString = "El campo Contraseña debe tener un mínimo de 8 caracteres, y debe incluir por lo menos una letra, un número y uno de los siguientes caracteres especiales: [@$!%*#?&].";
                imgCheckPassword.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                ttPassword.Text = errorString;

            }
            else
            {
                imgCheckPassword.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                ttPassword.Text = "Correcto.";
            }
        }

        private void RepeatPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            imgCheckRepeatPassword.Visibility = Visibility.Visible;

            if (RepeatPassword.Password == "")
            {
                errorString = "El campo de repetición de Contraseña no puede estar vacío.";
                imgCheckRepeatPassword.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                ttRepeatPassword.Text = errorString;

            }
            else if (!Regex.IsMatch(RepeatPassword.Password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$"))
            {
                errorString = "El campo de repetición de Contraseña debe tener un mínimo de 8 caracteres, y debe incluir por lo menos una letra, un número y uno de los siguientes caracteres especiales: [@$!%*#?&].";
                imgCheckRepeatPassword.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                ttRepeatPassword.Text = errorString;
            }
            else if (RepeatPassword.Password != Password.Password)
            {
                errorString = "El campo de repetición de Contraseña debe ser idéntico al campo de Contraseña.";
                imgCheckRepeatPassword.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                ttRepeatPassword.Text = errorString;
            }
            else
            {
                imgCheckRepeatPassword.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                ttRepeatPassword.Text = "Correcto.";
            }
        }
    }
}
