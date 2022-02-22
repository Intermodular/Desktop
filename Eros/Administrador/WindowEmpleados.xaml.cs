using Eros.Controladores;
using Eros.Modelos;
using Eros.Administrador;
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
using Eros.Clases;

namespace Eros
{
    /// <summary>
    /// Lógica de interacción para WindowEmpleados.xaml
    /// </summary>
    public partial class WindowEmpleados : Window
    {
        BitmapImage checkIconSource = new BitmapImage(new Uri(@"../Img/icons/check.png", UriKind.Relative));
        BitmapImage wrongIconSource = new BitmapImage(new Uri(@"../Img/icons/wrong.png", UriKind.Relative));
        List<Empleado> listEmpleados;
        List<Empleado> listFiltrada;
        Empleado selectedEmpleado;
        int idOfLastSelectedEmpleado;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;
        List<TextBox> allTextBox;


        public WindowEmpleados()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.PrimaryScreenHeight;
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Empleado>();
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

        //Eventos
        private void dtgEmpleados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Empleado selectedEmp = (Empleado)dtgEmpleados.SelectedItem;
            if (selectedEmp != null)
            {
                ShowEmpleadoInfo(selectedEmp);
                //idOfLastSelectedEmpleado = selectedEmpleado._id;
                selectedEmpleado = selectedEmp;
            }
        }
        
        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar a este empleado?", "Borrar Empleado"))
            {
                try
                {
                    ControladorEmpleados.DeleteFromApi(selectedEmpleado._id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                
                UpdateInfoFromDataBase();
            }
        }
        
        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Editando);
        }

        private void btAnyadirEmpleado_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Agregando);
        }

        private void btCancelarAgregar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Viendo);
        }

        private void btDescartarEdicion_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres descartar los cambios?", "Descartar cambios"))
            {
                ChangeToState(state.Viendo);
            }

        }

        private void tbxSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbxSearchBar.Text == "")
            {
                PutListInDataGrid(listEmpleados);
                tbkNotFound.Visibility = Visibility.Hidden;
                return;
            }

            string filter = tbxSearchBar.Text;
            listFiltrada.Clear();
            string nom_ap;
            foreach (Empleado em in listEmpleados)
            {
                nom_ap = em.nombre + " " + em.apellido;
                if (nom_ap.ToLower().Contains(filter.ToLower()))
                {
                    listFiltrada.Add(em);
                }
            }

            PutListInDataGrid(listFiltrada);

            if (listFiltrada.Count == 0)
            {
                tbkNotFound.Visibility = Visibility.Visible;
                tbkNotFound.Text = "No se han encontrado resultados con \"" + filter + "\"";
            }
            else
            {
                tbkNotFound.Visibility = Visibility.Hidden;
            }
        }

        private void btAgregar_Click(object sender, RoutedEventArgs e)
        {           
            string respuesta = "";

            bool val1 = ValidateName();
            bool val2 = ValidateSurname();
            bool val3 = ValidateDni();
            bool val4 = ValidateEmail();
            bool val5 = ValidateTelefono();
            bool val6 = ValidateUser();
            bool val7 = ValidateDir();
            bool val8 = ValidateFnac();
            if (!(val1 && val2 && val3 && val4 && val5 && val6 && val7 && val8))
            {
                MessageBox.Show("Errores encontrados...");
                return;
            }
            Empleado newEm = GetEmpleadoFromTextBoxes();
            try
            {
                respuesta = ControladorEmpleados.PostToApi(newEm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            if (respuesta == "Error Usuario Ya Existe")
            {
                MessageBox.Show("Esta cuenta de usuario ya existe pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ChangeToState(state.Viendo);
            UpdateInfoFromDataBase();
        }

        private void btGuardarEdicion_Click(object sender, RoutedEventArgs e)
        {
            bool val1 = ValidateName();
            bool val2 = ValidateSurname();
            bool val3 = ValidateDni();
            bool val4 = ValidateEmail();
            bool val5 = ValidateTelefono();
            bool val6 = ValidateUser();
            bool val7 = ValidateDir();
            bool val8 = ValidateFnac();
            if (!(val1 && val2 && val3 && val4 && val5 && val6 && val7 && val8))
            {
                MessageBox.Show("Errores encontrados...");
                return;
            }

            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres guardar los cambios?", "Editar Empleado"))
            {
                Empleado empleadoActualizado = GetEmpleadoFromTextBoxes();
                empleadoActualizado._id = selectedEmpleado._id;
                empleadoActualizado.password = selectedEmpleado.password;
                string respuesta = "";
                try
                {
                    respuesta = ControladorEmpleados.UpdateInApi(empleadoActualizado);
                } 
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                if (respuesta == "Error Usuario Ya Existe")
                {
                    MessageBox.Show("Esta cuenta de usuario ya existe pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ChangeToState(state.Viendo);
                UpdateInfoFromDataBase();
            }
        }

        private void tbxInfo_Changed(object sender, TextChangedEventArgs e)
        {
            if (currentState == state.Editando && !btGuardarEdicion.IsEnabled)
            {
                EnableButton(btGuardarEdicion, true);
            }
        }

        private void cbxRol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentState == state.Editando && !btGuardarEdicion.IsEnabled)
            {
                EnableButton(btGuardarEdicion, true);
            }
        }

        private void EnableButton(Button button, bool enable)
        {
            button.IsEnabled = enable;
            button.Opacity = enable ? 1 : 0.5;
        }

        //Funciones aux
        private void InitializeTextBoxList()
        {
            infoTbxsList = new List<TextBox>();
            infoTbxsList.Add(tbxNombre);
            infoTbxsList.Add(tbxApellido);
            infoTbxsList.Add(tbxDni);
            infoTbxsList.Add(tbxTelefono);
            infoTbxsList.Add(tbxFnac);
            infoTbxsList.Add(tbxDir);
            infoTbxsList.Add(tbxUsuario);
            infoTbxsList.Add(tbxEmail);
           // infoTbxsList.Add(tbxRol);

        }

        private void HideAllCheckImages()
        {
            imgCheckNombre.Visibility = Visibility.Hidden;
            imgCheckApellido.Visibility = Visibility.Hidden;
            imgCheckDNI.Visibility = Visibility.Hidden;
            imgCheckTelefono.Visibility = Visibility.Hidden;
            imgCheckFnac.Visibility = Visibility.Hidden;
            imgCheckDir.Visibility = Visibility.Hidden;
            imgCheckUsuario.Visibility = Visibility.Hidden;
            imgCheckEmail.Visibility = Visibility.Hidden;

        }

        private void UpdateInfoFromDataBase()
        {
            try
            {
                listEmpleados = ControladorEmpleados.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            PutListInDataGrid(listEmpleados);
            tbxSearchBar.Text = "";
            dtgEmpleados.SelectedItem = listEmpleados[0];

        }

        private void PutListInDataGrid(List<Empleado> lista)
        {
            dtgEmpleados.ItemsSource = null;
            dtgEmpleados.ItemsSource = lista;
        }

        private void ChangeToState(state nextState)
        {
            switch (currentState)
            {
                case state.Viendo:
                    //Codigo desaparecer botones editar,borrar,ver nominas
                    gridVisualizando.Visibility = Visibility.Hidden;
                    dtgEmpleados.IsEnabled = false;
                    break;

                case state.Agregando:
                    //Codigo desaparecer botones agregar,cancelar y aparecer boton agregar empleado
                    gridAnyadiendo.Visibility = Visibility.Hidden;
                    btAnyadirEmpleado.Visibility = Visibility.Visible;
                    break;

                case state.Editando:
                    //Codigo desaparecer botones descartar cambios y guardar
                    gridEditando.Visibility = Visibility.Hidden;
                    EnableSearchTextBox(true);
                    btnContrasenya.IsEnabled = false;
                    btnContrasenya.Background = Brushes.LightGray;
                    break;
            }

            switch (nextState)
            {
                case state.Viendo:
                    //Codigo aparecer botones editar,borrar,ver nominas , bloquear en READONLY , rellenar tbxs con item selleccionado
                    gridVisualizando.Visibility = Visibility.Visible;
                    HideAllCheckImages();
                    EnableTextBoxes(false);
                    dtgEmpleados.IsEnabled = true;
                    dtgEmpleados_SelectionChanged(null, null);

                    break;

                case state.Agregando:
                    //Codigo aparecer botones agregar,cancelar y desaparecer boton agregar empleado , desbloquear ReadOnly , dejar campos vacios
                    gridAnyadiendo.Visibility = Visibility.Visible;
                    btAnyadirEmpleado.Visibility = Visibility.Hidden;
                    EnableTextBoxes(true);
                    EmptyTextBoxes();
                    tbxNombre.Focus();
                    cbxRol.SelectedItem = cbxRol.Items[2];
                    break;

                case state.Editando:
                    //Codigo aparecer botones descartar cambios y guardar, desbloquear ReadOnly
                    EnableSearchTextBox(false);
                    gridEditando.Visibility = Visibility.Visible;
                    EnableTextBoxes(true);
                    tbxNombre.Focus();
                    EnableButton(btGuardarEdicion, false);
                    btnContrasenya.IsEnabled = true;
                    btnContrasenya.Background = Brushes.White;
                    break;
            }

            currentState = nextState;
        }

        private void ShowEmpleadoInfo(Empleado emp)
        {
            tbxNombre.Text = emp.nombre;
            tbxApellido.Text = emp.apellido;
            tbxDni.Text = emp.dni;
            tbxTelefono.Text = emp.telefono;
            tbxFnac.Text = emp.fnac;
            tbxDir.Text = emp.direccion;
            tbxUsuario.Text = emp.usuario;
            tbxEmail.Text = emp.email;
            foreach (ComboBoxItem i in cbxRol.Items)
            {
                if (i.Content.ToString() == emp.rol)
                {
                    cbxRol.SelectedItem = i;
                    return;
                }
            }

            cbxRol.SelectedItem = cbxRol.Items[cbxRol.Items.Count - 1]; // El ultimo siempre tiene que ser otro
        }

        private void EnableSearchTextBox(bool enable)
        {
            tbxSearchBar.IsReadOnly = !enable;
        }

        private void EnableTextBoxes(bool enable)
        {
            foreach (TextBox t in infoTbxsList)
            {
                t.IsReadOnly = !enable;
                t.Background = enable ? Brushes.White : Brushes.LightGray;
            }            
            cbxRol.IsEnabled = enable;
            cbxRol.Background = enable ? Brushes.White : Brushes.LightGray;
        }

        private void EmptyTextBoxes()
        {
            foreach (TextBox t in infoTbxsList)
            {
                t.Text = "";
            }
            cbxRol.SelectedItem = null;
        }

        private Empleado GetEmpleadoFromTextBoxes()
        {
            Empleado emp = new Empleado();

            emp.nombre = tbxNombre.Text.Trim();
            emp.apellido = tbxApellido.Text.Trim();
            emp.dni = tbxDni.Text;
            emp.telefono = tbxTelefono.Text;
            emp.email = tbxEmail.Text;
            emp.direccion = tbxDir.Text;
            emp.fnac = tbxFnac.Text;
            emp.usuario = tbxUsuario.Text;
            emp.rol = cbxRol.Text;

            return emp;
        }

        private bool GetYesNoMessageBoxResponse(string message, string title)
        {
            MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        //Validaciones
        private void tbxNombre_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxNombre.IsReadOnly)
                return;
            ValidateName();
        }

        public bool ValidateName()
        {
            imgCheckNombre.Visibility = Visibility.Visible;

            if (tbxNombre.Text == "")
            {
                imgCheckNombre.Source = wrongIconSource;
                tbkImageToolTipNombre.Text = "Campo Obligatorio";
                return false;

            }
            else if (!Regex.IsMatch(tbxNombre.Text, @"^([a-zA-Z ]+)$"))
            {
                imgCheckNombre.Source = wrongIconSource;
                tbkImageToolTipNombre.Text = "El campo Nombre solo permite caracteres alfabéticos.";
                return false;
            }

            imgCheckNombre.Source = checkIconSource;
            tbkImageToolTipNombre.Text = "Correcto";
            return true;
        }

        private void tbxApellido_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxApellido.IsReadOnly)
                return;
            ValidateSurname();
        }

        public bool ValidateSurname()
        {
            imgCheckApellido.Visibility = Visibility.Visible;

            if (tbxApellido.Text == "")
            {
                imgCheckApellido.Source = wrongIconSource;
                tbkImageToolTipApellido.Text = "Campo Obligatorio";
                return false;

            }
            else if (!Regex.IsMatch(tbxApellido.Text, @"^([a-zA-Z ]+)$"))
            {
                imgCheckApellido.Source = wrongIconSource;
                tbkImageToolTipApellido.Text = "El campo Nombre solo permite caracteres alfabéticos.";
                return false;
            }

            imgCheckApellido.Source = checkIconSource;
            tbkImageToolTipApellido.Text = "Correcto";
            return true;
        }

        private void tbxDni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxDni.IsReadOnly)
                return;
            ValidateDni();
        }

        public bool ValidateDni()
        {
            imgCheckDNI.Visibility = Visibility.Visible;

            if (tbxDni.Text == "")
            {
                imgCheckDNI.Source = wrongIconSource;
                tbkImageToolTipDNI.Text = "Campo Obligatorio";
                return false;

            }
            else if (!Regex.IsMatch(tbxDni.Text, @"^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKE]$"))
            {
                imgCheckDNI.Source = wrongIconSource;
                tbkImageToolTipDNI.Text = "El formato del DNI no es válido. Debe estar formado por 8 dígitos, seguido por un carácter alfabético.";
                return false;
            }

            imgCheckDNI.Source = checkIconSource;
            tbkImageToolTipDNI.Text = "Correcto";
            return true;
        }

        private void tbxTelefono_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxTelefono.IsReadOnly)
                return;
            ValidateTelefono();
        }

        public bool ValidateTelefono()
        {
            imgCheckTelefono.Visibility = Visibility.Visible;

            if (!Regex.IsMatch(tbxTelefono.Text, @"^(\+[0-9]{2} ?)?[0-9]{9}$"))
            {
                imgCheckTelefono.Source = wrongIconSource;
                tbkImageToolTipTelefono.Text = "El formato del Telefono no es válido. Debe estar formado por 9 dígitos (prefijo opcional).";
                return false;
            }

            imgCheckTelefono.Source = checkIconSource;
            tbkImageToolTipTelefono.Text = "Correcto";
            return true;
        }

        private void tbxFnac_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxFnac.IsReadOnly)
                return;
            ValidateFnac();
        }

        public bool ValidateFnac()
        {
            imgCheckFnac.Visibility = Visibility.Visible;

            if (tbxFnac.Text == "")
            {
                imgCheckFnac.Source = wrongIconSource;
                tbkImageToolTipFnac.Text = "Campo Obligatorio";
                return false;

            }
            else if (!Regex.IsMatch(tbxFnac.Text, @"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$"))
            {
                imgCheckFnac.Source = wrongIconSource;
                tbkImageToolTipFnac.Text = "La Fecha de Nacimiento no es valida. Debe seguir el siguiente formato: (dd-mm-aa).";
                return false;
            }

            imgCheckFnac.Source = checkIconSource;
            tbkImageToolTipFnac.Text = "Correcto";
            return true;
        }

        private void tbxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxEmail.IsReadOnly)
                return;
            ValidateEmail();
        }

        public bool ValidateEmail()
        {
            imgCheckEmail.Visibility = Visibility.Visible;

            if (tbxEmail.Text == "")
            {
                imgCheckEmail.Source = wrongIconSource;
                tbkImageToolTipEmail.Text = "Campo Obligatorio";
                return false;

            }
            else if (!Regex.IsMatch(tbxEmail.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                imgCheckEmail.Source = wrongIconSource;
                tbkImageToolTipEmail.Text = "El formato del Email no es válido.";
                return false;
            }

            imgCheckEmail.Source = checkIconSource;
            tbkImageToolTipEmail.Text = "Correcto";
            return true;
        }

        private void tbxDir_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxDir.IsReadOnly)
                return;
            ValidateDir();
        }

        public bool ValidateDir()
        {
            imgCheckDir.Visibility = Visibility.Visible;

            if (tbxDir.Text == "")
            {
                imgCheckDir.Source = wrongIconSource;
                tbkImageToolTipDir.Text = "Campo Obligatorio";
                return false;

            }

            imgCheckDir.Source = checkIconSource;
            tbkImageToolTipDir.Text = "Correcto";
            return true;
        }

        private void tbxUsuario_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxUsuario.IsReadOnly)
                return;
            imgCheckUsuario.Visibility = Visibility.Visible;

            bool userExists;

            if (tbxUsuario.Text == "")
            {
                imgCheckUsuario.Source = wrongIconSource;
                tbkImageToolTipUsuario.Text = "Campo Obligatorio";

            }
            else if (!Regex.IsMatch(tbxEmail.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                imgCheckUsuario.Source = wrongIconSource;
                tbkImageToolTipUsuario.Text = "El formato del Usuario no es válido.";
            }
            else
            {
                Task<bool> task = null;
                string userText = tbxUsuario.Text;
                if (currentState == state.Editando)
                {
                    try
                    {
                        task = Task.Run(() => ControladorEmpleados.DoesEmpleadoExistAsync(userText, selectedEmpleado._id));
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
                        task = Task.Run(() => ControladorEmpleados.DoesEmpleadoExistAsync(userText));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }
                }

                task.ContinueWith(t =>
                {
                    userExists = t.Result;
                    if (!userExists)
                    {
                        imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                        tbkImageToolTipUsuario.Text = "Correcto";
                    }
                    else
                    {
                        imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                        tbkImageToolTipUsuario.Text = "Este usuario ya existe, pruebe con otro.";
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());

                imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/waitingPoints.png", UriKind.Relative));
                tbkImageToolTipUsuario.Text = "Esperando...";

            }
        }

        public bool ValidateUser()
        {
            imgCheckUsuario.Visibility = Visibility.Visible;

            bool userExists = false;

            if (tbxUsuario.Text == "")
            {
                imgCheckUsuario.Source = wrongIconSource;
                tbkImageToolTipUsuario.Text = "Campo Obligatorio";
                return false;

            }
            else if (!Regex.IsMatch(tbxEmail.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                imgCheckUsuario.Source = wrongIconSource;
                tbkImageToolTipUsuario.Text = "El formato del Usuario no es válido.";
                return false;
            }
            else
            {
                string userText = tbxUsuario.Text;
                if (currentState == state.Editando)
                {
                    try
                    {
                        userExists = ControladorEmpleados.DoesEmpleadoExist(userText, selectedEmpleado._id);
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
                        userExists = ControladorEmpleados.DoesEmpleadoExist(userText);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }
                }    
                if (!userExists)
                {
                    imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                    tbkImageToolTipUsuario.Text = "Correcto";
                    return true;
                }
                else
                {
                    imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                    tbkImageToolTipUsuario.Text = "Este usuario ya existe, pruebe con otro.";
                    return false;
                }

            }

        }

        private void btnContrasenya_Click(object sender, RoutedEventArgs e)
        {            
            Empleado emp = GetEmpleadoFromTextBoxes();
            emp._id = selectedEmpleado._id;
            emp.password = selectedEmpleado.password;
            emp.newUser = selectedEmpleado.newUser;
            if (emp.newUser == true)
            {
                MessageBox.Show("El empleado seleccionado ya se le ha dado permiso cambiar su contraseña.");
                return;
            }
            emp.newUser = true;
            MessageBox.Show("Cambio de contraseña permitido.");
            try
            {
                ControladorEmpleados.UpdateInApi(emp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WindowMainAdministration wma = new WindowMainAdministration();
            wma.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfoFromDataBase();
            ChangeToState(state.Viendo);
            dtgEmpleados.SelectedItem = dtgEmpleados.Items[0];
            
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlobalVariables.top = Top;
            GlobalVariables.left = Top;
            GlobalVariables.width = Width;
            GlobalVariables.height = Height;
            GlobalVariables.max = WindowState == WindowState.Maximized;
        }

        private void VerNominas_Click(object sender, RoutedEventArgs e)
        {
            WindowNominas wn = new WindowNominas(selectedEmpleado);
            wn.Show();
            this.Close();
        }

        private void Nominas_Click(object sender, RoutedEventArgs e)
        {
            WindowNominas wn = new WindowNominas();
            wn.Show();
            this.Close();
        }

    }
}
