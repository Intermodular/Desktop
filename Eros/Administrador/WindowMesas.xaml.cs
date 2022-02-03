using Eros.Administrador.UtilWindows;
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

namespace Eros.Administrador
{
    /// <summary>
    /// Lógica de interacción para WindowMesas.xaml
    /// </summary>
    public partial class WindowMesas : Window
    {
        List<Mesas> listMesas;
        List<Mesas> listFiltrada;
        Mesas selectedMesa;
        int idOfLastSelectedTable;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;
        List<TextBox> allTextBox;

        public WindowMesas()
        {
            InitializeComponent();
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Mesas>();
        }

        //Eventos
        private void dtgMesas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mesas selectedTable = (Mesas)dtgMesas.SelectedItem;
            if (selectedTable != null)
            {
                ShowMesaInfo(selectedTable);
                //idOfLastSelectedEmpleado = selectedEmpleado._id;
                selectedMesa = selectedTable;
            }
        }

        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar a esta mesa?", "Borrar Mesa"))
            {
                ControladorEmpleados.DeleteFromApi(selectedMesa._id);
                UpdateInfoFromDataBase();
            }
        }

        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Editando);
        }

        private void btAnyadirMesa_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Agregando);
        }

        private void btCancelarAgregar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Viendo);
        }

        private void btDescartarEdicion_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Viendo);
        }

        private void tbxSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbxSearchBar.Text == "")
            {
                PutListInDataGrid(listMesas);
                tbkNotFound.Visibility = Visibility.Hidden;
                return;
            }

            string filter = tbxSearchBar.Text;
            listFiltrada.Clear();
            string nom_ap;
            foreach (Mesas m in listMesas)
            {
                nom_ap = m.zona.Substring(0, 1) + m._id + " " + m.zona + " " + m.numSillas;
                if (nom_ap.ToLower().Contains(filter.ToLower()))
                {
                    listFiltrada.Add(m);
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
            Mesas newMesa = GetTableFromTextBoxes();
            string respuesta = ControladorMesas.PostToApi(newMesa);
            if (respuesta == "Error Mesa Ya Existe")
            {
                MessageBox.Show("Esta mesa ya existe pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Mesas mesaActualizada = GetTableFromTextBoxes();
                mesaActualizada._id = selectedMesa._id;
                string respuesta = ControladorMesas.UpdateInApi(mesaActualizada);
                if (respuesta == "Error Mesa Ya Existe")
                {
                    MessageBox.Show("Esta mesa ya existe pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Atras_Click(object sender, RoutedEventArgs e)
        {
            WindowZones windowZones = new WindowZones();
            windowZones.Show();
            this.Close();
        }

        private void InitializeTextBoxList()
        {
            infoTbxsList = new List<TextBox>();
            infoTbxsList.Add(tbxNumero);
            //infoTbxsList.Add(tbxZona);
            infoTbxsList.Add(tbxSillas);
            //infoTbxsList.Add(tbxEstado);
        }

        private void HideAllCheckImages()
        {
            imgCheckNumero.Visibility = Visibility.Hidden;
            imgCheckZona.Visibility = Visibility.Hidden;
            imgCheckSillas.Visibility = Visibility.Hidden;
            imgCheckEstado.Visibility = Visibility.Hidden;
        }

        private void UpdateInfoFromDataBase()
        {
            listMesas = ControladorMesas.GetAllFromApi();
            PutListInDataGrid(listMesas);
            tbxSearchBar.Text = "";
            dtgMesas.SelectedItem = listMesas[0];

        }

        private void PutListInDataGrid(List<Mesas> lista)
        {
            dtgMesas.ItemsSource = null;
            dtgMesas.ItemsSource = lista;
        }

        private void ChangeToState(state nextState)
        {
            switch (currentState)
            {
                case state.Viendo:
                    //Codigo desaparecer botones editar,borrar,ver nominas
                    gridVisualizando.Visibility = Visibility.Hidden;
                    dtgMesas.IsEnabled = false;
                    break;

                case state.Agregando:
                    //Codigo desaparecer botones agregar,cancelar y aparecer boton agregar empleado
                    gridAnyadiendo.Visibility = Visibility.Hidden;
                    btAnyadirMesa.Visibility = Visibility.Visible;
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
                    dtgMesas.IsEnabled = true;
                    dtgMesas_SelectionChanged(null, null);

                    break;

                case state.Agregando:
                    //Codigo aparecer botones agregar,cancelar y desaparecer boton agregar empleado , desbloquear ReadOnly , dejar campos vacios
                    gridAnyadiendo.Visibility = Visibility.Visible;
                    btAnyadirMesa.Visibility = Visibility.Hidden;
                    EnableTextBoxes(true);
                    EmptyTextBoxes();
                    tbxNumero.Focus();
                    break;

                case state.Editando:
                    //Codigo aparecer botones descartar cambios y guardar, desbloquear ReadOnly
                    EnableSearchTextBox(false);
                    gridEditando.Visibility = Visibility.Visible;
                    EnableTextBoxes(true);
                    tbxNumero.Focus();
                    EnableButton(btGuardarEdicion, false);
                    break;
            }

            currentState = nextState;
        }

        private void ShowMesaInfo(Mesas m)
        {
            tbxNumero.Text = m.numero.ToString();
            //cbxZona.SelectedItem
            tbxSillas.Text = m.numSillas.ToString();
            //cbxEstado.SelectedItem
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

        private Mesas GetTableFromTextBoxes()
        {
            Mesas m = new Mesas();

            m.zona = cbxZona.SelectedItem.ToString();
            m.numSillas = Int32.Parse(tbxSillas.Text);
            m.estado = cbxEstado.SelectedItem.ToString();

            return m;
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

            if (tbxNumero.Text == "")
            {
                errorString += "-El campo Nombre no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxNumero.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Nombre solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }

            if (tbxSillas.Text == "")
            {
                errorString += "-El campo Sillas no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxSillas.Text, @"^[0-9]{2}$"))
            {
                errorString += "-El número de sillas no puede ser tan alto" + Environment.NewLine;
            }

            return errorString;

        }

        
        //Validaciones
        private void tbxNumero_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxNumero.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxNumero.Text == "")
            {
                errorString = "El campo Nombre no puede estar vacío";

            }
            else if (!Regex.IsMatch(tbxNumero.Text, @"^([0-9]+)$"))
            {
                errorString = "El campo Numero solo debe contener caracteres numéricos, sin caracteres especiales";
            }

            imgCheckNumero.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckNumero.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipNumero.Text = "Correcto";
            }
            else
            {
                imgCheckNumero.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipNumero.Text = errorString;
            }
        }


        private void tbxSillas_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxSillas.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxSillas.Text == "")
            {
                errorString = "El campo Nombre no puede estar vacío";

            }
            else if (!Regex.IsMatch(tbxSillas.Text, @"^([0-9]+)$"))
            {
                errorString = "El campo Numero solo debe contener caracteres numéricos, sin caracteres especiales";
            }

            imgCheckSillas.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckSillas.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipSillas.Text = "Correcto";
            }
            else
            {
                imgCheckSillas.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipSillas.Text = errorString;
            }
        }

    }        
}
