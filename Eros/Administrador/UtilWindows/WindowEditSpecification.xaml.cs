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
            btnEditEspecification.Visibility = Visibility.Hidden;
            btnDeleteEspecification.Visibility = Visibility.Hidden;
        }

        private void btnEditIngredient_Click(object sender, RoutedEventArgs e)
        {
            currentState = state.Editando;
            ChangeToState();
            tbHolderEditEspecification.Text = lvEspecification.SelectedItem.ToString();
            btnEditEspecification.Visibility = Visibility.Hidden;
            btnDeleteEspecification.Visibility = Visibility.Hidden;
            tbEditEspecification.Text = "";
        }

        private void btnNewEspecification_Click(object sender, RoutedEventArgs e)
        {
            currentState = state.Agregando;
            ChangeToState();
        }
        private void ChangeToState()
        {
            switch (currentState)
            {
                case state.Viendo:
                    lvEspecification.Visibility = Visibility.Visible;
                    spNewEspecification.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Visible;
                    spEditEspecification.Visibility = Visibility.Hidden;
                    btnEditEspecification.Visibility = Visibility.Hidden;
                    btnDeleteEspecification.Visibility = Visibility.Hidden;
                    break;
                case state.Agregando:
                    lvEspecification.Visibility = Visibility.Hidden;
                    spNewEspecification.Visibility = Visibility.Visible;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    spEditEspecification.Visibility = Visibility.Hidden;
                    btnEditEspecification.Visibility = Visibility.Hidden;
                    btnDeleteEspecification.Visibility = Visibility.Hidden;
                    break;
                case state.Editando:
                    spEditEspecification.Visibility = Visibility.Visible;
                    lvEspecification.Visibility = Visibility.Hidden;
                    spNewEspecification.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    btnEditEspecification.Visibility = Visibility.Hidden;
                    btnDeleteEspecification.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            currentState = state.Viendo;
            ChangeToState();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _Specifications.Add(tbEspecification.Text);
            tbEspecification.Text = "";
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Specifications, lvEspecification);

        }

        private void lvEspecification_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            btnEditEspecification.Visibility = Visibility.Visible;
            btnDeleteEspecification.Visibility = Visibility.Visible;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _Specifications.Remove(lvEspecification.SelectedItem.ToString());
            _Specifications.Add(tbEditEspecification.Text);
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Specifications, lvEspecification);
        }
    }
}
