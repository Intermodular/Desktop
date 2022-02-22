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
using Eros.Clases;

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
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Tipos>();
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

        BitmapImage checkIconSource = new BitmapImage(new Uri(@"../Img/icons/check.png", UriKind.Relative));
        BitmapImage wrongIconSource = new BitmapImage(new Uri(@"../Img/icons/wrong.png", UriKind.Relative));
        List<Tipos> listTipos;
        List<Tipos> listFiltrada;
        int idOfLastSelectedTipo;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;
        List<Extras> addedExtras = new List<Extras>();


        //Eventos
        private void dtgTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tipoImage.Source = null;
            Tipos selectedTipo = (Tipos)dtgTipos.SelectedItem;
            if (selectedTipo != null)
            {
                ShowTipoInfo(selectedTipo);
                idOfLastSelectedTipo = selectedTipo._id;
                if (tbxImagen.Text != "")
                {
                    try
                    {
                        tipoImage.Source = new BitmapImage(new Uri(tbxImagen.Text));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("URL de la imagen no válido.");
                    }
                }
            }
        }
        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar este tipo?", "Borrar Tipo"))
            {
                try
                {
                    ControladorTipos.DeleteFromApi(idOfLastSelectedTipo);
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
            bool val1 = ValidateNombre();
            bool val2 = ValidateImagen();
            if (!(val1 && val2))
            {
                MessageBox.Show("Errores encontrados...");
                return;
            }
            string respuesta = "";
            Tipos newTipo = GetTipoFromTextBoxes();
            try
            {
                respuesta = ControladorTipos.PostToApi(newTipo);
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            if (respuesta == "Error Tipo Ya Existe")
            {
                MessageBox.Show("Este tipo ya existe pruebe con otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            addedExtras.Clear();
            ChangeToState(state.Viendo);
            UpdateInfoFromDataBase();
        }
        private void btGuardarEdicion_Click(object sender, RoutedEventArgs e)
        {
            bool val1 = ValidateNombre();
            bool val2 = ValidateImagen();
            if (!(val1 && val2))
            {
                MessageBox.Show("Errores encontrados...");
                return;
            }
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres guardar los cambios?", "Editar Tipo"))
            {
                int idTipo = idOfLastSelectedTipo;
                Tipos updateTipo = GetTipoFromTextBoxes();
                updateTipo._id = idTipo;
                string respuesta = "";
                try
                {
                    respuesta = ControladorTipos.UpdateInApi(updateTipo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }

                if (respuesta == "Error Tipo Ya Existe")
                {
                    MessageBox.Show("Este tipo ya existe pruebe con otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                addedExtras.Clear();
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
            try
            {
                listTipos = ControladorTipos.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }            
            PutListInDataGrid(listTipos);
            tbxSearchBar.Text = "";
            dtgTipos.SelectedItem = listTipos[0];

        }

        private void PutListInDataGrid(List<Tipos> lista)
        {
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
                    try
                    {
                        listExtras.Add(e.nombre);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                        
                }
            }

            fillComboBox(cbExtras, listExtras);
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
            tipo.listaExtras = addedExtras;
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

        private void tbxNombre_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxNombre.IsReadOnly)
                return;
            ValidateNombre();
        }

        public bool ValidateNombre()
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

        private void tbxImagen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxImagen.IsReadOnly)
                return;
            ValidateImagen();
        }

        public bool ValidateImagen()
        {
            imgCheckImagen.Visibility = Visibility.Visible;

            if (tbxImagen.Text == "")
            {
                imgCheckImagen.Source = checkIconSource;
                tbkImageToolTipImagen.Text = "Correcto";
                return true;

            }
            
            try
            {
                tipoImage.Source = new BitmapImage(new Uri(tbxImagen.Text));
                imgCheckImagen.Source = checkIconSource;
                tbkImageToolTipImagen.Text = "Correcto";
                previewImage.Visibility = Visibility.Visible;
                return true;
            }
            catch (Exception ex)
            {
                imgCheckImagen.Source = wrongIconSource;
                tbkImageToolTipImagen.Text = "URL inválido";
                previewImage.Visibility = Visibility.Hidden;
                return false;
            }

            
        }

        private void hideIcons()
        {
            imgCheckNombre.Visibility = Visibility.Hidden;
            imgCheckImagen.Visibility = Visibility.Hidden;
            previewImage.Visibility = Visibility.Hidden;
        }

        public void fillComboBox(ComboBox cb, List<String> values)
        {
            cb.Items.Clear();
            foreach (String i in values) cb.Items.Add(i);
            if (values.Count > 0) cb.Text = values[0].ToString();
        }

        public void btnAddExtras_Click(object sender, RoutedEventArgs e)
        {
            List<Tipos> tipos = new List<Tipos>();
            try
            {
                tipos = ControladorTipos.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
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
                    addedExtras.Add(ex);
                }
                fillComboBox(cbExtras, stringExtras);
                EnableButton(btGuardarEdicion, true);
            }
            
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
            WindowProducts wp = new WindowProducts();
            wp.Show();
            this.Close();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfoFromDataBase();
            ChangeToState(state.Viendo);
            dtgTipos.SelectedItem = dtgTipos.Items[0];
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
