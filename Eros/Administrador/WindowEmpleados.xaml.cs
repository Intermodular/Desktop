using Eros.Controladores;
using Eros.Modelos;
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
    /// Lógica de interacción para WindowEmpleados.xaml
    /// </summary>
    public partial class WindowEmpleados : Window
    {
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
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Empleado>();
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
                ControladorEmpleados.DeleteFromApi(selectedEmpleado._id);
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
            string errorMessage;
            if ((errorMessage = GetValidationErrorString()) != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }
            Empleado newEm = GetEmpleadoFromTextBoxes();
            string respuesta = ControladorEmpleados.PostToApi(newEm);
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
            string errorMessage;
            if ((errorMessage = GetValidationErrorString()) != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres guardar los cambios?", "Editar Empleado"))
            {
                Empleado empleadoActualizado = GetEmpleadoFromTextBoxes();
                empleadoActualizado._id = selectedEmpleado._id;
                empleadoActualizado.password = selectedEmpleado.password;
                string respuesta = ControladorEmpleados.UpdateInApi(empleadoActualizado);
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
            imgCheckUsuario.Visibility = Visibility.Hidden;
            imgCheckEmail.Visibility = Visibility.Hidden;
            imgCheckRol.Visibility = Visibility.Hidden;

        }

        private void UpdateInfoFromDataBase()
        {
            listEmpleados = ControladorEmpleados.GetAllFromApi();
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
                    break;

                case state.Editando:
                    //Codigo aparecer botones descartar cambios y guardar, desbloquear ReadOnly
                    EnableSearchTextBox(false);
                    gridEditando.Visibility = Visibility.Visible;
                    EnableTextBoxes(true);
                    tbxNombre.Focus();
                    EnableButton(btGuardarEdicion, false);
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
            tbxSearchBar.Background = enable ? Brushes.White : Brushes.LightGray;
        }

        private void EnableTextBoxes(bool enable)
        {
            foreach (TextBox t in infoTbxsList)
            {
                t.IsReadOnly = !enable;
                t.Background = enable ? Brushes.White : Brushes.LightGray;
                cbxRol.IsEnabled = enable;
                cbxRol.Background = enable ? Brushes.White : Brushes.LightGray;
            }
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
            emp.fnac = tbxFnac.Text;
            emp.usuario = tbxUsuario.Text;
            emp.password = tbxEmail.Text;
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

        private string GetValidationErrorString()
        {
            string errorString = "";

            if (tbxNombre.Text == "")
            {
                errorString += "-El campo Nombre no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxNombre.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Nombre solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }

            if (tbxApellido.Text == "")
            {
                errorString += "-El campo Apellido no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxApellido.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Apellido solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }

            if (tbxDni.Text == "")
            {
                errorString += "-El campo DNI no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxDni.Text, @"^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKE]$"))
            {
                errorString += "-El DNI no es valido" + Environment.NewLine;
            }

            if (tbxTelefono.Text != "")
            {
                if (!Regex.IsMatch(tbxTelefono.Text, @"^(\+[0-9]{2} ?)?[0-9]{9}$"))
                {
                    errorString += "-El Telefono no es valido" + Environment.NewLine;
                }
            }

            if (tbxFnac.Text != "")
            {
                if (!Regex.IsMatch(tbxFnac.Text, @"^[0-9]{1,2}[-/][0-9]{1,2}[-/][0-9]{1,4}$"))
                {
                    errorString += "-La fecha de nacimiento no es válida (dd-mm-aa)" + Environment.NewLine;
                }
            }

            if (tbxUsuario.Text == "")
            {
                errorString += "-El campo Usuario no puede estar vacío" + Environment.NewLine;
            }
            else if (!Regex.IsMatch(tbxUsuario.Text, @"^[a-zA-Z0-9]+$"))
            {
                errorString += "-El campo usuario solo puede tener letras y numeros sin espacios" + Environment.NewLine;
            }

            return errorString;

        }

        //Validaciones
        private void tbxNombre_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxNombre.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxNombre.Text == "")
            {
                errorString = "El campo Nombre no puede estar vacío";

            }
            else if (!Regex.IsMatch(tbxNombre.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString = "El campo Nombre solo debe contener caracteres alfabéticos,sin caracteres especiales";
            }

            imgCheckNombre.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckNombre.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipNombre.Text = "Correcto";
            }
            else
            {
                imgCheckNombre.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipNombre.Text = errorString;
            }
        }

        private void tbxApellido_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxApellido.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxApellido.Text == "")
            {
                errorString = "El campo Apellido no puede estar vacío";

            }
            else if (!Regex.IsMatch(tbxApellido.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString = "El campo Apellido solo debe contener caracteres alfabéticos,sin caracteres especiales";
            }

            imgCheckApellido.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckApellido.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipApellido.Text = "Correcto";
            }
            else
            {
                imgCheckApellido.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipApellido.Text = errorString;
            }
        }

        private void tbxDni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxDni.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxDni.Text == "")
            {
                errorString = "El campo DNI no puede estar vacío";

            }
            else if (!Regex.IsMatch(tbxDni.Text, @"^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKE]$"))
            {
                errorString += "El DNI no es valido";
            }

            imgCheckDNI.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckDNI.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipDNI.Text = "Correcto";
            }
            else
            {
                imgCheckDNI.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipDNI.Text = errorString;
            }
        }

        private void tbxTelefono_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxTelefono.IsReadOnly)
            {
                return;
            }
            //Le ponemos la visibilidad primero porque  en caso de que esta vacio la pondremos en hidden
            imgCheckTelefono.Visibility = Visibility.Visible;
            string errorString = "";
            if (tbxTelefono.Text != "")
            {
                if (!Regex.IsMatch(tbxTelefono.Text, @"^(\+[0-9]{2} ?)?[0-9]{9}$"))
                {
                    errorString = "El formato del Telefono no es valido";
                }
            }
            else
            {
                imgCheckTelefono.Visibility = Visibility.Hidden;
                return;
            }



            if (errorString == "")
            {
                imgCheckTelefono.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipTelefono.Text = "Correcto";
            }
            else
            {
                imgCheckTelefono.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipTelefono.Text = errorString;
            }
        }

        private void tbxFnac_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxFnac.IsReadOnly)
            {
                return;
            }
            imgCheckFnac.Visibility = Visibility.Visible;
            string errorString = "";
            if (tbxFnac.Text != "")
            {
                if (!Regex.IsMatch(tbxFnac.Text, @"^[0-9]{1,2}[-/][0-9]{1,2}[-/][0-9]{2,4}$"))
                {
                    errorString = "La fecha de nacimiento no es valida (dd-mm-aa)";
                }
            }
            else
            {
                imgCheckFnac.Visibility = Visibility.Hidden;
                return;
            }

            if (errorString == "")
            {
                imgCheckFnac.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipFnac.Text = "Correcto";
            }
            else
            {
                imgCheckFnac.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipFnac.Text = errorString;
            }
        }

        private void tbxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxEmail.IsReadOnly)
            {
                return;
            }
            imgCheckEmail.Visibility = Visibility.Visible;
            string errorString = "";
            if (tbxEmail.Text != "")
            {
                if (!Regex.IsMatch(tbxEmail.Text, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                {
                    errorString = "El email no es valido";
                }
            }
            else
            {
                imgCheckEmail.Visibility = Visibility.Hidden;
                return;
            }

            if (errorString == "")
            {
                imgCheckEmail.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipEmail.Text = "Correcto";
            }
            else
            {
                imgCheckEmail.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipEmail.Text = errorString;
            }
        }

        private void cbxRol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!cbxRol.IsEnabled)
            {
                return;
            }
            string errorString = "";
            if (cbxRol.Text == "")
            {
                errorString = "El campo Rol no puede estar vacío";
            }

            imgCheckRol.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckRol.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipRol.Text = "Correcto";
            }
            else
            {
                imgCheckRol.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipRol.Text = errorString;
            }
        }

        private void tbxUsuario_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxUsuario.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            bool userExists;

            imgCheckUsuario.Visibility = Visibility.Visible;

            if (tbxUsuario.Text == "")
            {
                errorString = "El campo Usuario no puede estar vacío";
                imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipUsuario.Text = errorString;

            }
            else if (!Regex.IsMatch(tbxUsuario.Text, @"^[a-zA-Z0-9]+$"))
            {
                errorString = "El campo usuario solo puede tener letras y numeros sin espacios";
                imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipUsuario.Text = errorString;

            }
            else
            {
                Task<bool> task;
                string userText = tbxUsuario.Text;
                if (currentState == state.Editando)
                {
                    task = Task.Run(() => ControladorEmpleados.DoesEmpleadoExistAsync(userText, selectedEmpleado._id));
                }
                else
                {
                    task = Task.Run(() => ControladorEmpleados.DoesEmpleadoExistAsync(userText));
                }

                task.ContinueWith(t =>
                {
                    userExists = t.Result;
                    if (userExists)
                    {
                        errorString = "Este usuario ya existe, pruebe con otro";
                    }

                    if (errorString == "")
                    {
                        imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                        tbkImageToolTipUsuario.Text = "Correcto";
                    }
                    else
                    {
                        imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                        tbkImageToolTipUsuario.Text = errorString;
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());

                imgCheckUsuario.Source = new BitmapImage(new Uri(@"/Img/icons/waitingPoints.png", UriKind.Relative));
                tbkImageToolTipUsuario.Text = "Esperando...";
              
            }
        }
    }
}
