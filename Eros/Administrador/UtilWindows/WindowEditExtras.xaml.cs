using Eros.Controladores;
using Eros.Modelos;
using Eros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Eros.Administrador.UtilWindows
{
    /// <summary>
    /// Interaction logic for WindowEditExtras.xaml
    /// </summary>
    public partial class WindowEditExtras : Window
    {

        enum state { Agregando, Viendo, Editando };
        state currentState;

        public List<Extras> _Extras = new List<Extras>();
        public bool edit = false;

        public WindowEditExtras()
        {
            InitializeComponent();
            currentState = state.Viendo;

        }
        public WindowEditExtras(List<Extras> extras)
        {
            InitializeComponent();
            _Extras = extras;
            generateListView(_Extras, lvExtras);
            currentState = state.Viendo;
            ChangeToState();
        }

        private void btnAddExtra_Click(object sender, RoutedEventArgs e)
        {
            edit = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void generateListView(List<Extras> list, ListView listView)
        {
            listView.Items.Clear();
            foreach (Extras e in list)
            {
                listView.Items.Add(new { Col1 = e.nombre, Col2 = e.precio });
            }

        }



        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            foreach (Extras ex in _Extras)
            {
                if (ex.nombre.Equals(lvExtras.SelectedItem))
                {
                    _Extras.Remove(ex);
                }
            }
            generateListView(_Extras, lvExtras);
            btnEditar.Visibility = Visibility.Hidden;
            btnEliminar.Visibility = Visibility.Hidden;

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            currentState = state.Editando;
            ChangeToState();
            tbHolderEditExtra.Text = lvExtras.SelectedItem.ToString();
            btnEditar.Visibility = Visibility.Hidden;
            btnEliminar.Visibility = Visibility.Hidden;
            tbEditExtra.Text = "";
        }

        private void btnNewExtra_Click(object sender, RoutedEventArgs e)
        {
            currentState = state.Agregando;
            ChangeToState();
        }
        private void ChangeToState()
        {
            switch (currentState)
            {
                case state.Viendo:
                    lvExtras.Visibility = Visibility.Visible;
                    spNewExtra.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Visible;
                    spEditExtra.Visibility = Visibility.Hidden;
                    btnEditar.Visibility = Visibility.Hidden;
                    btnEliminar.Visibility = Visibility.Hidden;
                    ChangeGridToNormal();
                    break;
                case state.Agregando:
                    lvExtras.Visibility = Visibility.Hidden;
                    spNewExtra.Visibility = Visibility.Visible;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    spEditExtra.Visibility = Visibility.Hidden;
                    btnEditar.Visibility = Visibility.Hidden;
                    btnEliminar.Visibility = Visibility.Hidden;
                    ChangeGridToEdit();
                    break; 
                case state.Editando:
                    spEditExtra.Visibility = Visibility.Visible;
                    lvExtras.Visibility = Visibility.Hidden;
                    spNewExtra.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    btnEditar.Visibility = Visibility.Hidden;
                    btnEliminar.Visibility = Visibility.Hidden;
                    ChangeGridToEdit();
                    break;
                default:
                    break;
            }
        }

        private void ChangeGridToEdit()
        {
            GridCentral.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }

        private void ChangeGridToNormal()
        {
            GridCentral.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            currentState = state.Viendo;
            ChangeToState();
            ChangeGridToNormal();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Extras ex = new Extras();
            ex.nombre = tbExtra.Text;
            ex.precio = Single.Parse(tbPrice.Text);
            _Extras.Add(ex);
            tbExtra.Text = "";
            tbPrice.Text = "";
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Extras, lvExtras);
            ChangeGridToNormal();

        }

        private void lvExtras_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            btnEditar.Visibility = Visibility.Visible;
            btnEliminar.Visibility = Visibility.Visible;
            if (lvExtras.SelectedItem == null)
                ChangeGridToNormal();
            else
                ChangeGridToEdit();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            foreach (Extras ex in _Extras)
            {
                if (ex.nombre == lvExtras.SelectedItem.ToString())
                {
                    _Extras.Remove(ex);
                    Extras newEx = new Extras();
                    newEx.nombre = tbEditExtra.Text.ToString();
                    newEx.precio = Single.Parse(tbEditPrice.Text);
                    _Extras.Add(newEx);
                }
            }
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Extras, lvExtras);
        }
    }
}
