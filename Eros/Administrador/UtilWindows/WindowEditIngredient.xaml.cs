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
    /// Lógica de interacción para WindowAddIngredient.xaml
    /// </summary>
    public partial class WindowEditIngredient : Window
    {
        enum state { Agregando, Viendo, Editando };
        state currentState;

        public List<String> _Ingredients = new List<string>();
        public bool edit = false;
        public WindowEditIngredient()
        {
            InitializeComponent();
            currentState = state.Viendo;
            
        }
        public WindowEditIngredient(List<String> ingredientes)
        {
            InitializeComponent();
            _Ingredients = ingredientes;
            generateListView(_Ingredients, lvIngredient);
            currentState = state.Viendo;
            ChangeToState();
        }

        private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            edit = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void generateListView(List<String> list,ListView listView)
        {
            listView.Items.Clear();
           foreach (String i in list) listView.Items.Add(i);
           
        }

    

        private void btnDeleteIngredient_Click(object sender, RoutedEventArgs e)
        {
            String delIngredient = "";
            foreach (String i in _Ingredients) if (i.Equals(lvIngredient.SelectedItem)) delIngredient = i;
            _Ingredients.Remove(delIngredient);
            
            generateListView(_Ingredients, lvIngredient);
            btnEditIngredient.Visibility = Visibility.Hidden;
            btnDeleteIngredient.Visibility = Visibility.Hidden;
            
        }

        private void btnEditIngredient_Click(object sender, RoutedEventArgs e)
        {
            currentState = state.Editando;
            ChangeToState();
            tbHolderEditIngredient.Text = lvIngredient.SelectedItem.ToString();
            btnEditIngredient.Visibility = Visibility.Hidden;
            btnDeleteIngredient.Visibility = Visibility.Hidden;
            tbEditIngredient.Text = "";
        }

        private void btnNewIngrediente_Click(object sender, RoutedEventArgs e)
        {
            currentState = state.Agregando;
            ChangeToState();
        }
        private void ChangeToState()
        {
            switch (currentState)
            {
                case state.Viendo:
                    lvIngredient.Visibility = Visibility.Visible;
                    spNewIngredient.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Visible;
                    spEditIngredient.Visibility = Visibility.Hidden;
                    btnEditIngredient.Visibility = Visibility.Hidden;
                    btnDeleteIngredient.Visibility = Visibility.Hidden;
                    ChangeGridToNormal();
                    break;
                case state.Agregando:
                    lvIngredient.Visibility = Visibility.Hidden;
                    spNewIngredient.Visibility = Visibility.Visible;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    spEditIngredient.Visibility = Visibility.Hidden;
                    btnEditIngredient.Visibility = Visibility.Hidden;
                    btnDeleteIngredient.Visibility = Visibility.Hidden;
                    ChangeGridToEdit();
                    break;
                case state.Editando:
                    spEditIngredient.Visibility = Visibility.Visible;
                    lvIngredient.Visibility = Visibility.Hidden;
                    spNewIngredient.Visibility = Visibility.Hidden;
                    spDefaultButtons.Visibility = Visibility.Hidden;
                    btnEditIngredient.Visibility = Visibility.Hidden;
                    btnDeleteIngredient.Visibility = Visibility.Hidden;
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
            _Ingredients.Add(tbIngredient.Text);
            tbIngredient.Text = "";
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Ingredients, lvIngredient);

        }

        private void lvIngredient_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            btnEditIngredient.Visibility = Visibility.Visible;
            btnDeleteIngredient.Visibility = Visibility.Visible;
            if (lvIngredient.SelectedItem == null)
                ChangeGridToNormal();
            else
                ChangeGridToEdit();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _Ingredients.Remove(lvIngredient.SelectedItem.ToString());
            _Ingredients.Add(tbEditIngredient.Text);
            currentState = state.Viendo;
            ChangeToState();
            generateListView(_Ingredients, lvIngredient);
        }
    }
}
