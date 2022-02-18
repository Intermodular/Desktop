using Eros.Clases;
using Eros.Controladores;
using Eros.Modelos;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Eros.Cobrador
{
    /// <summary>
    /// Lógica de interacción para WindowGestionMesas.xaml
    /// </summary>
    public partial class WindowGestionMesas : Window
    {
        //Mesas
        List<PanelMesa> listPanelesMesa;
        List<PanelMesa> listPanelesMesaSortedBySillas;
        List<Zonas> listZonas;
        //Filter 
        float percentageHeightOfFilter = 0.15f;
        bool IsFilterExpanded = false;
        Storyboard storyboard;
        DoubleAnimation rotateAnimation;
        ThicknessAnimation thicknessAnimation;
        bool filterBoxAlreadyChanged = false;
        enum filterState { Normal, OrderedBySillas }
        filterState currentFilterState = filterState.Normal;

        public WindowGestionMesas()
        {
            InitializeComponent();
            InitializeStoryBoard();
            InitializeUIControlProperties();

            InitializeAndPlaceListPanelesMesa();
            InitializeAndPlaceZonas();
            InitializeComboBoxEstado();
        }


        private void InitializeUIControlProperties()
        {
            borderFiltros.Height = percentageHeightOfFilter * this.Height;
            borderFiltros.Margin = new Thickness(0, -borderFiltros.Height, 0, 0);
        }

        //FILTRO UI ------------------------------------------------------------------------------------------------------------------------------------------

        private void InitializeStoryBoard()
        {
            storyboard = new Storyboard();
            storyboard.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            imgFilterArrow.RenderTransform = new RotateTransform();
            rotateAnimation = new DoubleAnimation()
            {
                From = 0,
                To = -180,
                Duration = storyboard.Duration,
                DecelerationRatio = 0.5d
            };
            Storyboard.SetTarget(rotateAnimation, imgFilterArrow);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            storyboard.Children.Add(rotateAnimation);

            thicknessAnimation = new ThicknessAnimation()
            {
                From = borderFiltros.Margin,
                To = new Thickness(0, 0, 0, 0),
                Duration = storyboard.Duration,
                DecelerationRatio = 0.5d
            };

            Storyboard.SetTarget(thicknessAnimation, borderFiltros);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath(Border.MarginProperty));
            storyboard.Children.Add(thicknessAnimation);

            storyboard.Completed += StoryBoardCompleted;

        }

        private void StoryBoardCompleted(object sender, EventArgs e)
        {
            if (IsFilterExpanded == false)
            {
                borderFiltros.Visibility = Visibility.Hidden;
            }
        }
        private void btExpandirFiltros_Click(object sender, RoutedEventArgs e)
        {
            ToggleFilterExpansion();
        }

        private void ToggleFilterExpansion()
        {
            if (IsFilterExpanded)
            {
                CollapseFilter();
                IsFilterExpanded = false;
            }
            else
            {
                ExpandFilter();
                IsFilterExpanded = true;
            }
        }

        private void ExpandFilter()
        {
            borderFiltros.Visibility = Visibility.Visible;
            thicknessAnimation.From = borderFiltros.Margin;
            thicknessAnimation.To = new Thickness(0, 0, 0, 0);
            rotateAnimation.From = 0d;
            rotateAnimation.To = -180d;
            storyboard.Begin();
        }

        private void CollapseFilter()
        {
            rotateAnimation.From = -180d;
            rotateAnimation.To = 0d;
            thicknessAnimation.From = new Thickness(0, 0, 0, 0);
            thicknessAnimation.To = new Thickness(0, -borderFiltros.Height, 0, 0);
            storyboard.Begin();

        }

        //ZONAS
        private List<Zonas> GetZonas()
        {
            List<Zonas> list = new List<Zonas>();
            list.Add(new Zonas { _id = 1, nombre = "Interior" });
            list.Add(new Zonas { _id = 2, nombre = "Terraza" });
            return list;
        }

        private void InitializeAndPlaceZonas()
        {
            listZonas = GetZonas();
            cbxZona.Items.Add("Todas");
            foreach (Zonas z in listZonas)
            {
                cbxZona.Items.Add(z.nombre);
            }
            cbxZona.SelectedIndex = 0;
        }

        private void InitializeComboBoxEstado()
        {
            cbxEstado.Items.Add("Todas");
            cbxEstado.Items.Add("Libre");
            cbxEstado.Items.Add("Ocupada");
            cbxEstado.SelectedIndex = 0;
        }

        //MESAS---------------------------------------------------------------------------------------------------------------------

        private List<Mesas> GetMesas()
        {
           
            List<Mesas> list = ControladorMesas.GetAllFromApi();

            return list;
        }

        private void InitializeAndPlaceListPanelesMesa()
        {
            List<Mesas> listaMesas = GetMesas();
            listPanelesMesa = new List<PanelMesa>();
            wrapPanelTables.Children.Clear();
            foreach (Mesas mesa in listaMesas)
            {
                ConstructPanelMesa(mesa);
            }
            listPanelesMesa = listPanelesMesa.OrderBy(p => p.mesa.numero).ToList();
            listPanelesMesaSortedBySillas = listPanelesMesa.OrderBy(p => p.mesa.numSillas).ToList();
            FillWrapPanelWithPanelesMesas(listPanelesMesa);
        }

        private void ConstructPanelMesa(Mesas mesa)
        {
            PanelMesa panel = new PanelMesa();
            panel.mesa = mesa;
            panel.button.Click += PanelMesaButton_Clicked;
            panel.button.Width = 150d;
            panel.button.Height = 150d;
            panel.button.Margin = new Thickness(10);
            panel.button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            panel.button.VerticalContentAlignment = VerticalAlignment.Stretch;
            panel.tbkSuperior.FontSize = 20d;
            panel.tbkSuperior.VerticalAlignment = VerticalAlignment.Top;
            panel.tbkSuperior.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkSuperior.Text = mesa.zona + " (" + mesa.numSillas + ")";
            panel.tbkNumeroMesa.FontSize = 40d;
            panel.tbkNumeroMesa.VerticalAlignment = VerticalAlignment.Center;
            panel.tbkNumeroMesa.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkNumeroMesa.Text = mesa.numero + "";
            panel.menuItemLibre.Header = "Dejar Libre";
            panel.menuItemLibre.Click += CtxMenuDejarLibre_Click;
            panel.menuItemOcupar.Header = "Dejar Ocupada";
            panel.menuItemOcupar.Click += CtxMenuOcupar_Click;
            EstablishButtonMesaColor(panel);
            panel.ConstruirPanel();

            listPanelesMesa.Add(panel);
        }
        private void EstablishButtonMesaColor(PanelMesa panel)
        {
            if (panel.mesa.estado == "Libre")
            {
                panel.button.Background = new SolidColorBrush(Color.FromArgb(255, 88, 182, 88));
            }
            else if (panel.mesa.estado == "Ocupada")
            {
                panel.button.Background = new SolidColorBrush(Color.FromArgb(255, 199, 77, 77));
            }
        }


        private PanelMesa GetPanelMesaFromButton(Button b)
        {
            foreach (PanelMesa panelM in listPanelesMesa)
            {
                if (panelM.button == b)
                {
                    return panelM;
                }
            }
            return null;
        }
        private Pedidos CreateGetAndPostNewPedido(int idMesa)
        {
            Pedidos pedido = new Pedidos();
            pedido.lineasPedido = new List<LineaPedido>();
            pedido.idMesa = idMesa;
            string respuesta = ControladorPedidos.PostToApi(pedido); //Meter funcion api para comprobar al postear que no hay ningun pedido para esta mesa, por si alguien lo ha creado mientras le dabas al si del messagebox
            string asignedId = respuesta.Split('=')[1];
            pedido._id = Convert.ToInt32(asignedId);
            return pedido;
        }


        private void PanelMesaButton_Clicked(object sender, RoutedEventArgs e)
        {
            PanelMesa panelMesaClickado = GetPanelMesaFromButton(sender as Button);
            Pedidos pedidoMesa = ControladorPedidos.GetFromApiByIdMesa(panelMesaClickado.mesa._id);
            if (pedidoMesa == null)
            {
                MessageBoxResult result = MessageBox.Show("Quieres crear un pedido nuevo para esta mesa?", "Creacion pedido", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Pedidos pedido = CreateGetAndPostNewPedido(panelMesaClickado.mesa._id);
                    panelMesaClickado.mesa.estado = "Ocupada";
                    ControladorMesas.UpdateInApi(panelMesaClickado.mesa);
                    MessageBox.Show("Pedido Creado");
                    OpenWindowEditarPedido(pedido);
                }
            }
            else
            {
                MessageBox.Show("Tiene un pedido");
                OpenWindowEditarPedido(pedidoMesa);
            }
        }

        private void OpenWindowEditarPedido(Pedidos pedido)
        {
            WindowEditarPedido windowEditarPedido = new WindowEditarPedido(pedido);
            windowEditarPedido.Show();
            this.Close();
        }

        private PanelMesa GetPanelMesaFromContextMenuItem(MenuItem menuI)
        {
            foreach (PanelMesa pm in listPanelesMesa)
            {
                if (pm.menuItemLibre == menuI || pm.menuItemOcupar == menuI)
                {
                    return pm;
                }

            }
            return null;
        }
        private void CtxMenuDejarLibre_Click(object sender, RoutedEventArgs e)
        {
            PanelMesa pm = GetPanelMesaFromContextMenuItem(sender as MenuItem);
            Pedidos pedidoMesa = ControladorPedidos.GetFromApiByIdMesa(pm.mesa._id);
            if (pedidoMesa == null)
            {
                pm.mesa.estado = "Libre";
                ControladorMesas.UpdateInApi(pm.mesa);
                btReiniciar_Click(sender, null);
            }
            else
            {
                MessageBox.Show("Esta mesa tiene un pedido pendiente, no se puede dejar Libre");
            }
        }

        private void CtxMenuOcupar_Click(object sender, RoutedEventArgs e)
        {
            PanelMesa pm = GetPanelMesaFromContextMenuItem(sender as MenuItem);
            pm.mesa.estado = "Ocupada";
            ControladorMesas.UpdateInApi(pm.mesa);
            btReiniciar_Click(sender, null);
        }

        //FILTRO
        private void ChangeFilterState(filterState newState)
        {
            if (newState == currentFilterState)
            {
                return;
            }
            currentFilterState = newState;
            wrapPanelTables.Children.Clear();
            switch (newState)
            {
                case filterState.Normal:
                    FillWrapPanelWithPanelesMesas(listPanelesMesa);
                    break;

                case filterState.OrderedBySillas:
                    FillWrapPanelWithPanelesMesas(listPanelesMesaSortedBySillas);
                    break;
            }
        }

        private void FillWrapPanelWithPanelesMesas(List<PanelMesa> listPaneles)
        {
            foreach (PanelMesa panelMesa in listPaneles)
            {
                wrapPanelTables.Children.Add(panelMesa.button);
            }
        }
        private void tbMesa_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (filterBoxAlreadyChanged == true)
            {
                return;
            }
            filterBoxAlreadyChanged = true;
            cbxZona.SelectedIndex = 0;
            cbxEstado.SelectedIndex = 0;
            tbComensales.Text = "";
            UpdatePanelesMesasLayout();
            filterBoxAlreadyChanged = false;
        }

        private void cbxZona_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterBoxAlreadyChanged == true)
            {
                return;
            }
            filterBoxAlreadyChanged = true;
            tbMesa.Text = "";
            UpdatePanelesMesasLayout();
            filterBoxAlreadyChanged = false;
        }
        private void cbxEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterBoxAlreadyChanged == true)
            {
                return;
            }
            filterBoxAlreadyChanged = true;
            tbMesa.Text = "";
            UpdatePanelesMesasLayout();
            filterBoxAlreadyChanged = false;
        }

        private void tbComensales_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (filterBoxAlreadyChanged == true)
            {
                return;
            }
            filterBoxAlreadyChanged = true;
            tbMesa.Text = "";
            UpdatePanelesMesasLayout();
            filterBoxAlreadyChanged = false;
        }
        private void UpdatePanelesMesasLayout()
        {

            if (tbComensales.Text.Trim() == "")
            {
                ChangeFilterState(filterState.Normal);
            }
            else
            {
                ChangeFilterState(filterState.OrderedBySillas);
            }

            foreach (PanelMesa panelMesa in listPanelesMesa)
            {
                if (PassesFilterMesa(panelMesa) && PassesFilterZona(panelMesa) && PassesFilterEstado(panelMesa) && PassesFilterComensales(panelMesa))
                {
                    panelMesa.button.Visibility = Visibility.Visible;
                }
                else
                {
                    panelMesa.button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool PassesFilterMesa(PanelMesa panelMesa)
        {
            string trimmedText = tbMesa.Text.Trim();
            if (trimmedText == "")
            {
                return true;
            }
            else if (Convert.ToString(panelMesa.mesa.numero) == trimmedText)
            {
                return true;
            }

            return false;
        }

        private bool PassesFilterZona(PanelMesa panelMesa)
        {
            if (cbxZona.SelectedItem as string == "Todas")
            {
                return true;
            }
            else if (panelMesa.mesa.zona == cbxZona.SelectedItem as string)
            {
                return true;
            }

            return false;
        }

        private bool PassesFilterEstado(PanelMesa panelMesa)
        {
            if (cbxEstado.SelectedItem as string == "Todas")
            {
                return true;
            }
            else if (panelMesa.mesa.estado == cbxEstado.SelectedItem as string)
            {
                return true;
            }

            return false;
        }

        private bool PassesFilterComensales(PanelMesa panelMesa)
        {
            if (tbComensales.Text.Trim() == "")
            {
                return true;
            }
            else
            {                
                int comensales = Convert.ToInt32(tbComensales.Text.Trim());
                if (panelMesa.mesa.numSillas >= comensales)
                {
                    return true;
                }
            }

            return false;
        }

        //EVENTOS GENERALES -----------------------------------------------------------------------------------------------------------------------------------------------------
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            borderFiltros.Height = percentageHeightOfFilter * this.ActualHeight;
            if (!IsFilterExpanded)
            {
                borderFiltros.Margin = new Thickness(0, -borderFiltros.Height, 0, 0);
            }
        }

        private void btReiniciar_Click(object sender, RoutedEventArgs e)
        {
            InitializeAndPlaceListPanelesMesa();
            UpdatePanelesMesasLayout();
        }

    }
}
