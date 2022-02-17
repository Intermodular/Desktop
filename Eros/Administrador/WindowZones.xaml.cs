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
using Eros.Clases;

namespace Eros.Administrador
{
    /// <summary>
    /// Lógica de interacción para WindowZones.xaml
    /// </summary>
    public partial class WindowZones : Window
    {
        List<Zonas> listZonas;
        List<Zonas> listFiltrada;
        int idOfLastSelectedZone;
        enum state { Agregando, Viendo, Editando };
        state currentState;

        List<TextBox> infoTbxsList;
        List<TextBox> enabledInfoTbxsList;

        public WindowZones()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Zonas>();
            usuName.Text = GlobalVariables.username;
            if (GlobalVariables.max)
            {
                WindowState = WindowState.Maximized;
            }
            else if (GlobalVariables.left != -999)
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
            Zonas selectedZone= (Zonas) dtgZonas.SelectedItem;
            if (selectedZone != null)
            {
                ShowZoneInfo(selectedZone);
                idOfLastSelectedZone = selectedZone._id;
            }
        }
        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar esta zona?", "Borrar Zona"))
            {
                ControladorZonas.DeleteFromApi(idOfLastSelectedZone);
                UpdateInfoFromDataBase();
            }
        }
        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Editando);
        }

        private void btAnyadirZona_Click(object sender, RoutedEventArgs e)
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
                PutListInDataGrid(listZonas);
                tbkNotFound.Visibility = Visibility.Hidden;
                return;
            }

            string filter = tbxSearchBar.Text;
            listFiltrada.Clear();
            string nom_ap;
            foreach (Zonas zo in listZonas)
            {
                nom_ap = zo.nombre;
                if (nom_ap.ToLower().Contains(filter.ToLower()))
                {
                    listFiltrada.Add(zo);
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
            Zonas newZone = GetZonaFromTextBoxes();
            string respuesta = ControladorZonas.PostToApi(newZone);
            if (respuesta == "Error zona Ya Existe")
            {
                MessageBox.Show("Esta zona existe pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres guardar los cambios?", "Editar Zona"))
            {
                int idZone = idOfLastSelectedZone;
                Zonas zonaActualizada = GetZonaFromTextBoxes();
                zonaActualizada._id = idZone;
                string respuesta = ControladorZonas.UpdateInApi(zonaActualizada);
                if (respuesta == "Error zona Ya Existe")
                {
                    MessageBox.Show("Esta zona ya existe pruebe con otra", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            infoTbxsList.Add(tbxNºMesas);
            enabledInfoTbxsList = new List<TextBox>();
            enabledInfoTbxsList.Add(tbxNombre);
            

        }
        private void UpdateInfoFromDataBase()
        {
            listZonas = ControladorZonas.GetAllFromApi();
            PutListInDataGrid(listZonas);
            tbxSearchBar.Text = "";
            dtgZonas.SelectedItem = listZonas[0];

        }

        private void PutListInDataGrid(List<Zonas> lista)
        {
            dtgZonas.ItemsSource = null;
            dtgZonas.ItemsSource = lista;
        }
        private void ChangeToState(state nextState)
        {
            switch (currentState)
            {
                case state.Viendo:
                    //Codigo desaparecer botones editar,borrar
                    gridVisualizando.Visibility = Visibility.Hidden;
                    dtgZonas.IsEnabled = false;
                    break;

                case state.Agregando:
                    //Codigo desaparecer botones agregar,cancelar y aparecer boton agregar zona
                    gridAnyadiendo.Visibility = Visibility.Hidden;
                    btAnyadirZona.Visibility = Visibility.Visible;
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
                    //Codigo aparecer botones editar,borrar , bloquear en READONLY , rellenar tbxs con item selleccionado
                    gridVisualizando.Visibility = Visibility.Visible;
                    EnableTextBoxes(false);
                    dtgZonas.IsEnabled = true;
                    dtgEmpleados_SelectionChanged(null, null);

                    break;

                case state.Agregando:
                    //Codigo aparecer botones agregar,cancelar y desaparecer boton agregar zona , desbloquear ReadOnly , dejar campos vacios
                    gridAnyadiendo.Visibility = Visibility.Visible;
                    btAnyadirZona.Visibility = Visibility.Hidden;
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

        private void ShowZoneInfo(Zonas zo)
        {
            tbxNombre.Text = zo.nombre;
            //tbxAbreviacion.Text = zo.abreviación;
            tbxNºMesas.Text = zo.numMesas.ToString();
           
        }

        private void EnableSearchTextBox(bool enable)
        {
            tbxSearchBar.IsReadOnly = !enable;
            tbxSearchBar.Background = enable ? Brushes.White : Brushes.LightGray;
        }

        private void EnableTextBoxes(bool enable)
        {
            foreach (TextBox t in enabledInfoTbxsList)
            {
                t.IsReadOnly = !enable;
            }
            
        }

        private void EmptyTextBoxes()
        {
            foreach (TextBox t in enabledInfoTbxsList)
            {
                t.Text = "";
            }
            tbxNºMesas.Text = "0";
        }

        private Zonas GetZonaFromTextBoxes()
        {
            Zonas zo = new Zonas();

            zo.nombre = tbxNombre.Text.Trim();
            //zo.abreviación = tbxAbreviacion.Text.Trim();
            zo.numMesas = int.Parse(tbxNºMesas.Text);
            

            return zo;
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
                errorString += "-El campo Nombre no puede estar vacío." + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxNombre.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Nombre solo permite caracteres alfabéticos." + Environment.NewLine;
            }
            /*
            if (tbxAbreviacion.Text == "")
            {
                errorString += "-El campo Abreciación no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxAbreviacion.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Abreviación solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }

            if (tbxNºMesas.Text == "")
            {
                errorString += "-El campo nºMesas no puede estar vacío" + Environment.NewLine;

            }*/
            
            return errorString;

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WindowMainAdministration wma = new WindowMainAdministration();
            wma.Show();
            this.Close();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfoFromDataBase();
            ChangeToState(state.Viendo);
            dtgZonas.SelectedItem = dtgZonas.Items[0];
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

        private void Btn_Mesas_Click(object sender, RoutedEventArgs e)
        {
            WindowMesas wm = new WindowMesas();
            wm.Show();
            this.Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlobalVariables.top = Top;
            GlobalVariables.left = Top;
            GlobalVariables.width = Width;
            GlobalVariables.height = Height;
            GlobalVariables.max = WindowState == WindowState.Maximized;
        }
    }
}
