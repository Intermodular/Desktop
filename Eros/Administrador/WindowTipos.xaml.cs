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
    /// Interaction logic for WindowTipos.xaml
    /// </summary>
    public partial class WindowTipos : Window
    {
        public WindowTipos()
        {
            InitializeComponent();
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Tipos>();
        }

        List<Tipos> listTipos;
        List<Tipos> listFiltrada;
        int idOfLastSelectedTipo;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;


        //Eventos
        private void dtgTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tipos selectedTipo = (Tipos)dtgTipos.SelectedItem;
            if (selectedTipo != null)
            {
                ShowTipoInfo(selectedTipo);
                idOfLastSelectedTipo = selectedTipo._id;
            }
        }
        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar este tipo?", "Borrar Tipo"))
            {
                ControladorTipo.DeleteFromApi(idOfLastSelectedTipo);
                UpdateInfoFromDataBase();
            }
        }
        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Editando);
        }

        private void btAnyadirTipo_Click(object sender, RoutedEventArgs e)
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
                PutListInDataGrid(listTipos);
                tbkNotFound.Visibility = Visibility.Hidden;
                return;
            }

            string filter = tbxSearchBar.Text;
            listFiltrada.Clear();
            string nom_ap;
            foreach (Tipos t in listTipos)
            {
                nom_ap = t.nombre;
                if (nom_ap.ToLower().Contains(filter.ToLower()))
                {
                    listFiltrada.Add(t);
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
            Tipos newTipo = GetTipoFromTextBoxes();
            string respuesta = ControladorTipo.PostToApi(newTipo);
            if (respuesta == "Error Tipo Ya Existe")
            {
                MessageBox.Show("Este tipo ya existe pruebe con otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres guardar los cambios?", "Editar Tipo"))
            {
                int idTipo = idOfLastSelectedTipo;
                Tipos updateTipo = GetTipoFromTextBoxes();
                updateTipo._id = idTipo;
                string respuesta = ControladorTipo.UpdateInApi(updateTipo);
                MessageBox.Show(respuesta);

                if (respuesta == "Error Tipo Ya Existe")
                {
                    MessageBox.Show("Este tipo ya existe pruebe con otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            infoTbxsList.Add(tbxImagen);


        }
        private void UpdateInfoFromDataBase()
        {
            listTipos = ControladorTipo.GetAllFromApi();
            //
            PutListInDataGrid(listTipos);
            tbxSearchBar.Text = "";
            dtgTipos.SelectedItem = listTipos[0];

        }

        private void PutListInDataGrid(List<Tipos> lista)
        {
            List<dtgTipo> newList = new List<dtgTipo>();
            List<string> extras = new List<string>();

            foreach (Tipos t in lista)
            {
                foreach (Extras e in t.listaExtras)
                {
                    extras.Add(e.nombre);
                }
                dtgTipo dtg = new dtgTipo(t.nombre, extras);
                newList.Add(dtg);
                extras.Clear();
            }

            dtgTipos.ItemsSource = null;
            dtgTipos.ItemsSource = lista;
        }

        private void ChangeToState(state nextState)
        {
            switch (currentState)
            {
                case state.Viendo:
                    //Codigo desaparecer botones editar,borrar,ver nominas

                    gridVisualizando.Visibility = Visibility.Hidden;
                    dtgTipos.IsEnabled = false;
                    btnAddExtras.Visibility = Visibility.Visible;
                    break;

                case state.Agregando:
                    //Codigo desaparecer botones agregar,cancelar y aparecer boton agregar empleado


                    gridAnyadiendo.Visibility = Visibility.Hidden;
                    btAnyadirTipo.Visibility = Visibility.Visible;
                    hideIcons();
                    break;

                case state.Editando:
                    //Codigo desaparecer botones descartar cambios y guardar

                    gridEditando.Visibility = Visibility.Hidden;
                    EnableSearchTextBox(true);
                    hideIcons();
                    break;
            }

            switch (nextState)
            {
                case state.Viendo:
                    //Codigo aparecer botones editar,borrar,ver nominas , bloquear en READONLY , rellenar tbxs con item selleccionado
                    gridVisualizando.Visibility = Visibility.Visible;
                    EnableTextBoxes(false);
                    dtgTipos.IsEnabled = true;
                    btnAddExtras.Visibility = Visibility.Hidden;
                    dtgTipos_SelectionChanged(null, null);

                    break;

                case state.Agregando:
                    //Codigo aparecer botones agregar,cancelar y desaparecer boton agregar producto , desbloquear ReadOnly , dejar campos vacios
                    gridAnyadiendo.Visibility = Visibility.Visible;
                    btAnyadirTipo.Visibility = Visibility.Hidden;
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
        //Aquí
        private void ShowTipoInfo(Tipos tipo)
        {
            List<String> listExtras = new List<string>();

            tbxNombre.Text = tipo.nombre;
            tbxImagen.Text = tipo.img;

            if (tipo.listaExtras != null)
            {
                foreach (Extras e in tipo.listaExtras)
                {
                        listExtras.Add(e.nombre);
                }
            }

            fillComboBox(cbExtras, listExtras);
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
                t.Text = "";
            cbExtras.Items.Clear();
        }

        private Tipos GetTipoFromTextBoxes()
        {


            Tipos tipo = new Tipos();
            tipo.nombre = tbxNombre.Text.Trim();

            List<String> extras = new List<string>();
            foreach (String e in cbExtras.Items) extras.Add(e);
            //tipo.extras = extras;

            tipo.img = tbxImagen.Text;

            return tipo;
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

            if (tbxImagen.Text == "")
            {
                errorString += "-El campo Imagen no puede estar vacío." + Environment.NewLine;

            }


            //Validar por tipo de producto existente ¿?
            /* else if (!Regex.IsMatch(tbxApellido.Text, @"^([a-zA-Z ]+)$"))
             {
                 errorString += "-El campo Apellido solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
             }
            */
            /*
             if (tbxIngredientes.Text == "")
             {
                 errorString += "-El campo Ingredientes no puede estar vacío" + Environment.NewLine;
             }*/
            /*
            else if (!Regex.IsMatch(tbxIngredientes.Text.Trim(), @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Tipo solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }
            */
            /*

             if (tbxPrecio.Text != "")
             {

                 errorString += "-El campo Precio No puede estar vacio" + Environment.NewLine;
             }
             else if (!Regex.IsMatch(tbxPrecio.Text, @"^[0-9]."));
             {
                 errorString += "-El campo Precio debe ser numérico" + Environment.NewLine;

             }
             */
            return errorString;

        }

        private void tbxNombre_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxNombre.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxNombre.Text == "")
            {
                errorString = "El campo Nombre no puede estar vacío.";

            }
            else if (!Regex.IsMatch(tbxNombre.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString = "El campo Nombre solo permite caracteres alfabéticos.";
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

        private void tbxImagen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxImagen.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxImagen.Text == "")
            {
                errorString = "El campo Imagen no puede estar vacío.";

            }

            imgCheckImagen.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckImagen.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipImagen.Text = "Correcto";
            }
            else
            {
                imgCheckImagen.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipImagen.Text = errorString;
            }
        }

        private void hideIcons()
        {
            imgCheckNombre.Visibility = Visibility.Hidden;
            imgCheckImagen.Visibility = Visibility.Hidden;
        }

        public void fillComboBox(ComboBox cb, List<String> values)
        {
            cb.Items.Clear();
            foreach (String i in values) cb.Items.Add(i);
            if (values.Count > 0) cb.Text = values[0].ToString();
        }

        public void btnAddExtras_Click(object sender, RoutedEventArgs e)
        {

            List<Tipos> tipos = ControladorTipo.GetAllFromApi();
            List<Extras> extras = new List<Extras>();
            foreach (Tipos t in tipos)
            {
                if (t.nombre == tbxNombre.Text)
                {
                    foreach (Extras ex in t.listaExtras) 
                    {
                        extras.Add(ex);
                    }
                }
            }

            WindowEditExtras wee = new WindowEditExtras(extras);
            wee.ShowDialog();
            if (wee.edit)
            {
                List<String> stringExtras = new List<string>();
                foreach (Extras ex in wee._Extras)
                {
                    stringExtras.Add(ex.nombre);
                }
                fillComboBox(cbExtras, stringExtras);
                EnableButton(btGuardarEdicion, true);
            }
            
        }

    }
}
