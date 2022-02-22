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
using Eros.Modelos;
using Eros.Controladores;
using Eros.Clases;
using Eros.Administrador.UtilWindows;
using System.Windows.Threading;

namespace Eros.Administrador
{
    /// <summary>
    /// Interaction logic for WindowNominas.xaml
    /// </summary>
    public partial class WindowNominas : Window
    {
        ListBoxItem lastListBoxItemSelected = null;
        List<Empleado> listEmpleados;
        List<Empleado> listEmpleadosFiltrada;
        List<PanelNomina> listPanelNomina;

        DispatcherTimer timer = new DispatcherTimer();
        double time = 0;
        Empleado empTraido = null;

        public WindowNominas()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            InitializeListEmpleados();
            dtgEmpleados.ItemsSource = listEmpleados;
            lbNominas.Items.Clear();
            InitializeListPanelNominas();
            InitializeComboBoxMes();

            if (GlobalVariables.max)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = GlobalVariables.left;
                Top = GlobalVariables.top;
                Height = GlobalVariables.height;
                Width = GlobalVariables.width;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time += timer.Interval.TotalMilliseconds;
            if (time > 200)
            {


                SelectAnEmpleado(empTraido);
                timer.Stop();
            }
        }

        public WindowNominas(Empleado selectedEm)
        {
            InitializeComponent();
            InitializeListEmpleados();
            dtgEmpleados.ItemsSource = listEmpleados;
            lbNominas.Items.Clear();
            InitializeListPanelNominas();
            InitializeComboBoxMes();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
            timer.Start();
            empTraido = selectedEm;

            if (GlobalVariables.max)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = GlobalVariables.left;
                Top = GlobalVariables.top;
                Height = GlobalVariables.height;
                Width = GlobalVariables.width;
            }
        }

        private void InitializeListEmpleados()
        {
            try
            {
                listEmpleados = ControladorEmpleados.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            
            listEmpleadosFiltrada = new List<Empleado>();
        }
        private void InitializeListPanelNominas()
        {
            List<Nominas> listNominas = new List<Nominas>();
            try
            {
                listNominas = ControladorNominas.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            
            //listNominas.Add(new Nominas() { _id = 3, idEmpleado = 1, nombreEmpleado = "Daniel",apellidoEmpleado="Long", fechaInicio = "12/02/2022", fechaFinal = "13/03/2022" });
            listPanelNomina = new List<PanelNomina>();

            foreach (Nominas nomina in listNominas)
            {
                CreateAndAddPanelNomina(nomina);
            }
        }
        private void InitializeComboBoxMes()
        {
            cBoxMeses.Items.Add("Todos");
            cBoxMeses.Items.Add("Enero");
            cBoxMeses.Items.Add("Febrero");
            cBoxMeses.Items.Add("Marzo");
            cBoxMeses.Items.Add("Abril");
            cBoxMeses.Items.Add("Mayo");
            cBoxMeses.Items.Add("Junio");
            cBoxMeses.Items.Add("Julio");
            cBoxMeses.Items.Add("Agosto");
            cBoxMeses.Items.Add("Septiembre");
            cBoxMeses.Items.Add("Octubre");
            cBoxMeses.Items.Add("Noviembre");
            cBoxMeses.Items.Add("Diciembre");
            cBoxMeses.SelectedIndex = 0;
        }

        private void SelectAnEmpleado(Empleado empleado)
        {
            foreach (Empleado em in listEmpleados)
            {
                if (em._id == empleado._id)
                {
                    dtgEmpleados.SelectedItem = em;
                    return;
                }
            }

        }

        private void CreateAndAddPanelNomina(Nominas nomina)
        {
            PanelNomina panel = new PanelNomina();
            panel.nomina = nomina;
            panel.lbItem.Margin = new Thickness(0, 10, 0, 0);
            panel.lbItem.Style = Application.Current.Resources["nominaListBox"] as Style;
            panel.border.Margin = new Thickness(10, 0, 10, 0);
            panel.border.BorderThickness = new Thickness(5);
            panel.border.BorderBrush = Brushes.Black;
            panel.border.CornerRadius = new CornerRadius(5);
            panel.stackPanel.Background = Brushes.MediumAquamarine;
            panel.nameBorder.Padding = new Thickness(0, 5, 0, 5);
            panel.datesBorder.Padding = new Thickness(0, 5, 0, 5);
            panel.datesBorder.Background = Brushes.White;
            panel.datesBorder.CornerRadius = new CornerRadius(0, 0, 2, 2);
            panel.tbkNombre.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkNombre.FontSize = 30;
            panel.tbkNombre.Text = nomina.nombreEmpleado + " " + nomina.apellidoEmpleado;
            panel.tbkDates.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkDates.FontSize = 30;
            panel.tbkDates.Text = nomina.fechaInicio + "-" + nomina.fechaFinal;
            panel.ConstruirPanel();

            listPanelNomina.Add(panel);
        }

        private void ColocarPanelesEnLista(Empleado em)
        {
            lbNominas.Items.Clear();
            int count = 0;
            foreach (PanelNomina panelN in listPanelNomina)
            {
                if (em == null || panelN.nomina.idEmpleado == em._id)
                {
                    string[] dateInicial = panelN.nomina.fechaInicio.Split('/');
                    string[] dateFinal = panelN.nomina.fechaFinal.Split('/');
                    if (cBoxMeses.SelectedIndex == 0 || Convert.ToInt32(dateInicial[1]) == cBoxMeses.SelectedIndex || Convert.ToInt32(dateFinal[1]) == cBoxMeses.SelectedIndex)
                    {
                        string trimmed = tbAnyo.Text.Trim();
                        if (trimmed == "" || trimmed == dateInicial[2] || trimmed == dateFinal[2])
                        {
                            lbNominas.Items.Insert(0, panelN.lbItem);
                            count++;
                        }

                    }
                }
            }
            tbkNotFoundNominas.Visibility = count == 0 ? Visibility.Visible : Visibility.Hidden;
        }
        private void PutListInDataGrid(List<Empleado> lista)
        {
            dtgEmpleados.ItemsSource = null;
            dtgEmpleados.ItemsSource = lista;
        }
        private PanelNomina GetPanelNominaFromLBItem(ListBoxItem lbi)
        {
            foreach (PanelNomina panel in listPanelNomina)
            {
                if (panel.lbItem == lbi)
                {
                    return panel;
                }
            }
            return null;
        }
        //Eventos
        private void tbxSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbxSearchBar.Text == "")
            {
                PutListInDataGrid(listEmpleados);
                tbkNotFound.Visibility = Visibility.Hidden;
                return;
            }

            string filter = tbxSearchBar.Text;
            listEmpleadosFiltrada.Clear();
            string nom_ap;
            foreach (Empleado em in listEmpleados)
            {
                nom_ap = em.nombre + " " + em.apellido;
                if (nom_ap.ToLower().Contains(filter.ToLower()))
                {
                    listEmpleadosFiltrada.Add(em);
                }
            }

            PutListInDataGrid(listEmpleadosFiltrada);

            if (listEmpleadosFiltrada.Count == 0)
            {
                tbkNotFound.Visibility = Visibility.Visible;
                tbkNotFound.Text = "No se han encontrado resultados con \"" + filter + "\"";
            }
            else
            {
                tbkNotFound.Visibility = Visibility.Hidden;
            }
        }

        private void dtgEmpleados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ColocarPanelesEnLista(dtgEmpleados.SelectedItem as Empleado);
            btTodosEmpleados.Visibility = dtgEmpleados.SelectedItem == null ? Visibility.Hidden : Visibility.Visible;
        }

        private void cBoxMeses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ColocarPanelesEnLista(dtgEmpleados.SelectedItem as Empleado);
        }
        private void tbAnyo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ColocarPanelesEnLista(dtgEmpleados.SelectedItem as Empleado);
        }

        private void lbNominas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbNominas.SelectedItem != null)
            {
                MostrarBotonesEdicion(true);

                if (lbNominas.SelectedItem != lastListBoxItemSelected)
                {
                    if (lastListBoxItemSelected != null)
                        ((lastListBoxItemSelected.Content as Border).Child as StackPanel).Background = Brushes.MediumAquamarine;
                    (((lbNominas.SelectedItem as ListBoxItem).Content as Border).Child as StackPanel).Background = Brushes.DarkCyan;
                    lastListBoxItemSelected = lbNominas.SelectedItem as ListBoxItem;
                }
            }
            else
            {
                MostrarBotonesEdicion(false);
                ((lastListBoxItemSelected.Content as Border).Child as StackPanel).Background = Brushes.MediumAquamarine;
                lastListBoxItemSelected = null;
            }


        }

        private void MostrarBotonesEdicion(bool mostrar)
        {
            if (mostrar)
            {
                btVer.Visibility = Visibility.Visible;
                btEditar.Visibility = Visibility.Visible;
                btEliminar.Visibility = Visibility.Visible;
                Grid.SetColumn(vbBtCrear, 0);
                Grid.SetColumnSpan(vbBtCrear, 1);
            }
            else
            {
                btVer.Visibility = Visibility.Hidden;
                btEditar.Visibility = Visibility.Hidden;
                btEliminar.Visibility = Visibility.Hidden;
                Grid.SetColumn(vbBtCrear, 1);
                Grid.SetColumnSpan(vbBtCrear, 2);
            }
        }

        private void btCrear_Click(object sender, RoutedEventArgs e)
        {
            if (dtgEmpleados.SelectedItem == null)
            {
                MessageBox.Show("<- Debes de seleccionar primero al empleado de la nómina en la lista de empleados");
                return;
            }
            Empleado empleadoSeleccionado = dtgEmpleados.SelectedItem as Empleado;
            MessageBoxResult result = MessageBox.Show("Quieres crearle una nómina a " + empleadoSeleccionado.nombre + " " + empleadoSeleccionado.apellido + " ?", "Creacion Nómina", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                WindowEditarNomina wen = new WindowEditarNomina(empleadoSeleccionado);
                wen.Show();
                this.Close();
            }

        }

        private void btTodosEmpleados_Click(object sender, RoutedEventArgs e)
        {
            dtgEmpleados.SelectedItem = null;
        }

        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            PanelNomina pn = GetPanelNominaFromLBItem(lbNominas.SelectedItem as ListBoxItem);
            if (pn != null)
            {
                WindowEditarNomina wen = new WindowEditarNomina(pn.nomina);
                wen.Show();
                this.Close();
            }
        }

        private void btEliminar_Click(object sender, RoutedEventArgs e)
        {
            PanelNomina pn = GetPanelNominaFromLBItem(lbNominas.SelectedItem as ListBoxItem);
            if (pn != null)
            {
                MessageBoxResult result = MessageBox.Show("Estás seguro de que quieres borrar esta nómina", "Borrar Nómina", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        ControladorNominas.DeleteFromApi(pn.nomina._id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }
                    listPanelNomina.Remove(pn);
                    lbNominas.Items.Remove(pn.lbItem);
                }

            }
        }

        private void btVer_Click(object sender, RoutedEventArgs e)
        {
            PanelNomina pn = GetPanelNominaFromLBItem(lbNominas.SelectedItem as ListBoxItem);
            if (pn != null)
            {
                DialogWindowInfoNomina dwin = new DialogWindowInfoNomina(pn.nomina);
                dwin.ShowDialog();
            }

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WindowEmpleados we = new WindowEmpleados();
            we.Show();
            this.Close();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            InitializeListEmpleados();
            dtgEmpleados.ItemsSource = listEmpleados;
            lbNominas.Items.Clear();
            InitializeListPanelNominas();
            InitializeComboBoxMes();
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
