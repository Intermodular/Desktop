using Eros.Administrador.UtilWindows;
using Eros.Administrador;
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

namespace Eros
{
    /// <summary>
    /// Lógica de interacción para WindowProducts.xaml
    /// </summary>
    public partial class WindowProducts : Window
    {
        List<Productos> listProductos;
        List<Tipos> listTipos;
        List<Productos> listFiltrada;
        int idOfLastSelectedProduct;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;

        public WindowProducts()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Productos>();
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
            Productos selectedProduct = (Productos)dtgProductos.SelectedItem;
            if (selectedProduct != null)
            {
                ShowProductInfo(selectedProduct);
                idOfLastSelectedProduct = selectedProduct._id;
            }
        }
        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar este producto?", "Borrar Producto"))
            {
                try
                {
                    ControladorProductos.DeleteFromApi(idOfLastSelectedProduct);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Posible error de conexión: \n" + ex);
                }
                UpdateInfoFromDataBase();
            }
        }
        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            ChangeToState(state.Editando);
        }

        private void btAnyadirProducto_Click(object sender, RoutedEventArgs e)
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
                PutListInDataGrid(listProductos);
                tbkNotFound.Visibility = Visibility.Hidden;
                return;
            }

            string filter = tbxSearchBar.Text;
            listFiltrada.Clear();
            string nom_ap;
            foreach (Productos em in listProductos)
            {
                nom_ap = em.nombre;
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
            Productos newProduct = GetProductFromTextBoxes();
            try
            {
                string respuesta = ControladorProductos.PostToApi(newProduct);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Posible error de conexión: \n" + ex);
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
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres guardar los cambios?", "Editar Producto"))
            {
                int idProduct = idOfLastSelectedProduct;
                Productos updateProduct= GetProductFromTextBoxes();
                updateProduct._id = idProduct;
                string respuesta = "";
                try
                {
                    respuesta = ControladorProductos.UpdateInApi(updateProduct);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Posible error de conexión: \n" + ex);
                }
                MessageBox.Show(respuesta);

                if (respuesta == "Error Producto Ya Existe")
                {
                    MessageBox.Show("Este producto ya existe pruebe con otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //infoTbxsList.Add(tbxTipo);
            //infoTbxsList.Add(tbxIngredientes);
            infoTbxsList.Add(tbxPrecio);
            //infoTbxsList.Add(tbxEspecificaciones);
            infoTbxsList.Add(tbxImagen);
            infoTbxsList.Add(tbxStock);
            

        }
        private void UpdateInfoFromDataBase()
        {
            try
            {
                listProductos = ControladorProductos.GetAllFromApi();
                listTipos = ControladorTipos.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Posible error de conexión: \n" + ex);
            }
            cbTipo.Items.Clear();
            foreach (Tipos t in listTipos)
            {
                cbTipo.Items.Add(t.nombre);
            }
            PutListInDataGrid(listProductos);
            tbxSearchBar.Text = "";
            dtgProductos.SelectedItem = listProductos[0];

        }

        private void PutListInDataGrid(List<Productos> lista)
        {
            dtgProductos.ItemsSource = null;
            dtgProductos.ItemsSource = lista;
        }

        private void ChangeToState(state nextState)
        {
            switch (currentState)
            {
                case state.Viendo:
                    //Codigo desaparecer botones editar,borrar,ver nominas
                    btnAddSpecifications.Visibility = Visibility.Visible;
                    btnAddIngredientes.Visibility = Visibility.Visible;

                    gridVisualizando.Visibility = Visibility.Hidden;
                    dtgProductos.IsEnabled = false;
                    break;

                case state.Agregando:
                    //Codigo desaparecer botones agregar,cancelar y aparecer boton agregar empleado
                   

                    gridAnyadiendo.Visibility = Visibility.Hidden;
                    btAnyadirEmpleado.Visibility = Visibility.Visible;
                    hideIcons();
                    break;

                case state.Editando:
                    //Codigo desaparecer botones descartar cambios y guardar
                    
                    btnAddIngredientes.Visibility = Visibility.Hidden;
                    btnAddSpecifications.Visibility = Visibility.Hidden;
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
                    dtgProductos.IsEnabled = true;
                    dtgEmpleados_SelectionChanged(null, null);

                    break;

                case state.Agregando:
                    //Codigo aparecer botones agregar,cancelar y desaparecer boton agregar producto , desbloquear ReadOnly , dejar campos vacios
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
        //Aquí
        private void ShowProductInfo(Productos product)
        {
            tbxNombre.Text = product.nombre;
            cbTipo.Text = product.tipo;
            tbxPrecio.Text = product.precio.ToString();
            tbxImagen.Text = product.imagen;
            tbxStock.Text = product.stock.ToString();

         
            fillComboBox(cbIngredientes, product.ingredientes);
            fillComboBox(cbEspecificaciones, product.especificaciones);
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
            cbTipo.IsEnabled = enable;
        }

        private void EmptyTextBoxes()
        {
            foreach (TextBox t in infoTbxsList)
                t.Text = "";
            cbEspecificaciones.Items.Clear();
            cbIngredientes.Items.Clear();
        }

        private Productos GetProductFromTextBoxes()
        {

        
            Productos product = new Productos();
            product.nombre = tbxNombre.Text.Trim();
            product.tipo = cbTipo.Text.Trim();

            List<String> ingredientes = new List<string>();
            foreach (String i in cbIngredientes.Items) ingredientes.Add(i);
            product.ingredientes = ingredientes;

            product.precio = float.Parse(tbxPrecio.Text);

            List<String> especificaciones = new List<string>();
            foreach (String i in cbEspecificaciones.Items) especificaciones.Add(i);
            product.especificaciones = especificaciones;

            product.imagen = tbxImagen.Text;
            product.stock = int.Parse(tbxStock.Text);

            

            return product;
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

            if (tbxPrecio.Text == "")
            {
                errorString += "-El campo Precio no puede estar vacío." + Environment.NewLine;

            }
            else if (double.TryParse(tbxPrecio.Text, out _))
            {
                errorString += "-El campo Precio tiene que ser de tipo numérico." + Environment.NewLine;
            }

            if (tbxImagen.Text == "")
            {
                errorString += "-El campo Imagen no puede estar vacío." + Environment.NewLine;

            }

            if (tbxStock.Text == "")
            {
                errorString += "-El campo Stock no puede estar vacío." + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxStock.Text, @"^[0-9]+$"))
            {
                errorString += "-El campo Stock tiene que ser de tipo numérico." + Environment.NewLine;
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

        private void tbxPrecio_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxPrecio.IsReadOnly)
            {
                return;
            }
            string errorString = "";
            if (tbxPrecio.Text == "")
            {
                errorString = "El campo Precio no puede estar vacío.";

            }

            imgCheckPrecio.Visibility = Visibility.Visible;

            if (errorString == "")
            {
                imgCheckPrecio.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipPrecio.Text = "Correcto";
            }
            else
            {
                imgCheckPrecio.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipPrecio.Text = errorString;
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
                imgCheckPrecio.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipPrecio.Text = errorString;
            }
        }

        private void tbxStock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxStock.IsReadOnly)
            {
                return;
            }
            imgCheckStock.Visibility = Visibility.Visible;
            string errorString = "";
            if (tbxStock.Text != "")
            {
                if (!Regex.IsMatch(tbxStock.Text, @"^[0-9]+$"))
                {
                    errorString = "El campo Stock tiene que ser de tipo numérico.";
                }
            }
            else
            {
                imgCheckStock.Visibility = Visibility.Hidden;
                return;
            }

            if (errorString == "")
            {
                imgCheckStock.Source = new BitmapImage(new Uri(@"/Img/icons/check.png", UriKind.Relative));
                tbkImageToolTipStock.Text = "Correcto";
            }
            else
            {
                imgCheckStock.Source = new BitmapImage(new Uri(@"/Img/icons/wrong.png", UriKind.Relative));
                tbkImageToolTipStock.Text = errorString;
            }
        }

        private void hideIcons()
        {
            imgCheckNombre.Visibility = Visibility.Hidden;
            imgCheckPrecio.Visibility = Visibility.Hidden;
            imgCheckImagen.Visibility = Visibility.Hidden;
            imgCheckStock.Visibility = Visibility.Hidden;
        }

        public void fillComboBox(ComboBox cb, List<String> values)
        {
            cb.Items.Clear();
            foreach (String i in values) cb.Items.Add(i);
            if(values.Count > 0) cb.Text = values[0];
        }

        private void btnAddIngredientes_Click(object sender, RoutedEventArgs e)
        {
            List<String> ingredientes = new List<string>();
            foreach (String i in cbIngredientes.Items) ingredientes.Add(i);
            

            WindowEditIngredient wei = new WindowEditIngredient(ingredientes);
            wei.ShowDialog();
            if (wei.edit)
            {
                fillComboBox(cbIngredientes, wei._Ingredients);
                EnableButton(btGuardarEdicion, true);
            }
        }

        private void btnAddSpecifications_Click(object sender, RoutedEventArgs e)
        {
            List<String> especificaciones = new List<string>();
            foreach (String i in cbEspecificaciones.Items) especificaciones.Add(i);

            WindowEditSpecification wes = new WindowEditSpecification(especificaciones);
            wes.ShowDialog();
            if (wes.edit)
            {
                fillComboBox(cbEspecificaciones, wes._Specifications);
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
            WindowMainAdministration wma = new WindowMainAdministration();
            wma.Show();
            this.Close();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfoFromDataBase();
            ChangeToState(state.Viendo);
            dtgProductos.SelectedItem = dtgProductos.Items[0];
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowTipos wt = new WindowTipos();
            wt.Show();
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
