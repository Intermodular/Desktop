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
    /// Lógica de interacción para WindowEditSpecification.xaml
    /// </summary>
    public partial class WindowEditSpecification : Window
    {
        enum state { Agregando, Viendo, Editando };
        state currentState;

        public List<string> allSpecifications = new List<string>{"Vegano", "Vegetariano", "Picante", "Sin gluten", "Pescetariano"};
        public List<String> _Specifications = new List<string>();
        public bool edit = false;

        public WindowEditSpecification()
        {
            InitializeComponent();
            currentState = state.Viendo;

        }
        public WindowEditSpecification(List<String> specifications)
        {
            InitializeComponent();
            _Specifications = specifications;
            generateListView(_Specifications, lvEspecification);
            currentState = state.Viendo;
            ChangeToState();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddEspecification_Click(object sender, RoutedEventArgs e)
        {
            edit = true;
            this.Close();
        }
      

        private void generateListView(List<String> list, ListView listView)
        {
            listView.Items.Clear();
            foreach (String i in list) listView.Items.Add(i);

        }

        private void btnDeleteEspecification_Click(object sender, RoutedEventArgs e)
        {
            String delEspecification = "";
            foreach (String i in _Specifications) if (i.Equals(lvEspecification.SelectedItem)) delEspecification = i;
            _Specifications.Remove(delEspecification);

            generateListView(_Specifications, lvEspecification);
            btnDeleteEspecification.Visibility = Visibility.Hidden; 
            btnNewEspecification.IsEnabled = true;
        }

        private void btnNewEspecification_Click(object sender, RoutedEventArgs e)
        {
            cbxEspecificacion.Items.Clear();
            fillComboBox();
            currentState = state.Agregando;
            ChangeToState();
            cbxEspecificacion.SelectedItem = cbxEspecificacion.Items[0];
        }
        private void ChangeToState()
        {
            switch (currentState)
            {
                case state.Viendo:
                    lvEspecification.Visibility = Visibility.Visible;
                    spNewEspecification.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Visible;
                    btnDeleteEspecification.Visibility = Visibility.Hidden;
                    ChangeGridToNormal();
                    break;
                case state.Agregando:
                    lvEspecification.Visibility = Visibility.Hidden;
                    spNewEspecification.Visibility = Visibility.Visible;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    btnDeleteEspecification.Visibility = Visibility.Hidden;
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
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _Specifications.Add(cbxEspecificacion.SelectedItem.ToString());
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Specifications, lvEspecification);
            if (lvEspecification.Items.Count == 5)
                btnNewEspecification.IsEnabled = false;
            ChangeGridToNormal();
        }

        private void lvEspecification_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            btnDeleteEspecification.Visibility = Visibility.Visible;
            if (lvEspecification.SelectedItem == null)
                ChangeGridToNormal();
            else 
                ChangeGridToEdit();
        }

        private void fillComboBox()
        {
            foreach (String e in allSpecifications)
            {
                if (!_Specifications.Contains(e))
                {
                    cbxEspecificacion.Items.Add(e);
                }
            }
        }
    }
}
