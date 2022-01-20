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
    /// Lógica de interacción para WindowProducts.xaml
    /// </summary>
    public partial class WindowProducts : Window
    {
        List<Productos> listProductos;
        List<Productos> listFiltrada;
        int idOfLastSelectedProduct;
        enum state { Agregando, Viendo, Editando };
        state currentState;
        List<TextBox> infoTbxsList;
        List<TextBox> allTextBox;

        public WindowProducts()
        {
            InitializeComponent();
            InitializeTextBoxList();
            currentState = state.Viendo;
            UpdateInfoFromDataBase();
            listFiltrada = new List<Productos>();
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
            if (GetYesNoMessageBoxResponse("Estás seguro de que quieres eliminar a este empleado?", "Borrar Empleado"))
            {
                ControladorProductos.DeleteFromApi(idOfLastSelectedProduct);
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
            string respuesta = ControladorProductos.PostToApi(newProduct);
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
                int idProduct = idOfLastSelectedProduct;
                Productos updateProduct= GetProductFromTextBoxes();
                updateProduct._id = idProduct;
                string respuesta = ControladorProductos.UpdateInApi(updateProduct);
                MessageBox.Show(respuesta);

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
            infoTbxsList.Add(tbxTipo);
            infoTbxsList.Add(tbxIngredientes);
            infoTbxsList.Add(tbxPrecio);
            infoTbxsList.Add(tbxEspecificaciones);
            infoTbxsList.Add(tbxImagen);
            infoTbxsList.Add(tbxStock);
            

        }
        private void UpdateInfoFromDataBase()
        {
            listProductos = ControladorProductos.GetAllFromApi();
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
                    gridVisualizando.Visibility = Visibility.Hidden;
                    dtgProductos.IsEnabled = false;
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
                    dtgProductos.IsEnabled = true;
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

        private void ShowProductInfo(Productos product)
        {
            tbxNombre.Text = product.nombre;
            tbxTipo.Text = product.tipo;
            foreach (String i in product.ingredientes) tbxIngredientes.Text += i + ",";
           
            tbxPrecio.Text = product.precio.ToString();
           
            foreach (String i in product.especificaciones) tbxEspecificaciones.Text += i + ",";
            tbxImagen.Text = product.imagen;
            tbxStock.Text = product.stock.ToString();
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

        private Productos GetProductFromTextBoxes()
        {

        
            Productos product = new Productos();
            product.nombre = tbxNombre.Text.Trim();
            product.tipo = tbxTipo.Text.Trim();
           
            product.ingredientes = tbxIngredientes.Text.Split(",").ToList();
            product.precio = float.Parse(tbxPrecio.Text);
            product.especificaciones = tbxEspecificaciones.Text.Split(",").ToList();
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
                errorString += "-El campo Nombre no puede estar vacío" + Environment.NewLine;

            }
            else if (!Regex.IsMatch(tbxNombre.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Nombre solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }

            if (tbxTipo.Text == "")
            {
                errorString += "-El campo Tipo no puede estar vacío" + Environment.NewLine;

            }

            //Validar por tipo de producto existente ¿?
           /* else if (!Regex.IsMatch(tbxApellido.Text, @"^([a-zA-Z ]+)$"))
            {
                errorString += "-El campo Apellido solo debe contener caracteres alfabéticos,sin caracteres especiales" + Environment.NewLine;
            }
           */

            if (tbxIngredientes.Text == "")
            {
                errorString += "-El campo Ingredientes no puede estar vacío" + Environment.NewLine;
            }
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
    }
}
