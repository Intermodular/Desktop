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
        int idOfLastSelectedEmpleado;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;

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
            Empleado selectedEmpleado = (Empleado)dtgEmpleados.SelectedItem;
            if (selectedEmpleado != null)
            {
                ShowEmpleadoInfo(selectedEmpleado);
                idOfLastSelectedEmpleado = selectedEmpleado._id;
            }
        }
        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar a este empleado?", "Borrar Empleado"))
            {
                ControladorEmpleados.DeleteFromApi(idOfLastSelectedEmpleado);
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
                int idEmpleado = idOfLastSelectedEmpleado;
                Empleado empleadoActualizado = GetEmpleadoFromTextBoxes();
                empleadoActualizado._id = idEmpleado;
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
            infoTbxsList.Add(tbxContrasenya);
            infoTbxsList.Add(tbxRol);

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
            tbxContrasenya.Text = emp.password;
            tbxRol.Text = emp.rol;
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
            }
        }

        private void EmptyTextBoxes()
        {
            foreach (TextBox t in infoTbxsList)
            {
                t.Text = "";
            }
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
            emp.password = tbxContrasenya.Text;
            emp.rol = tbxRol.Text;

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
                    errorString += "-La fecha de nacimiento no es valida (dd-mm-aa)" + Environment.NewLine;
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


    }
}
