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
        List<Reserva> listReservas;


        BitmapImage sourceLiberarMesa = new BitmapImage(new Uri("../Img/icons/greenCheckIcon.png", UriKind.Relative));
        BitmapImage sourceOcuparMesa = new BitmapImage(new Uri("../Img/icons/forbiddenIcon.png", UriKind.Relative));
        BitmapImage sourceCambiarPedidoDeMesa = new BitmapImage(new Uri("../Img/icons/forwardArrowIcon.png", UriKind.Relative));

        //Filter 
        float percentageHeightOfFilter = 0.15f;
        bool IsFilterExpanded = false;
        Storyboard storyboard;
        DoubleAnimation rotateAnimation;
        ThicknessAnimation thicknessAnimation;
        bool filterBoxAlreadyChanged = false;
        enum filterState { Normal, OrderedBySillas }
        filterState currentFilterState = filterState.Normal;

        //Pasar el pedido
        enum windowState { Normal, PasandoPedido };
        windowState currentWindowState = windowState.Normal;
        Storyboard sbIdleTbkPasarPedido;
        Pedidos passingPedido = null;
        Mesas passingPedidoMesa = null;
        public WindowGestionMesas()
        {
            InitializeComponent();
            InitializeStoryBoards();
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

        private void ChangeWindowState(windowState newState)
        {
            btReiniciar_Click(null, null);

            switch (currentWindowState)
            {
                case windowState.Normal:
                    break;
                case windowState.PasandoPedido:
                    gridPasarPedido.Visibility = Visibility.Hidden;
                    sbIdleTbkPasarPedido.Stop();
                    break;
            }
            switch (newState)
            {
                case windowState.Normal:
                    gridPpal.Background = Brushes.White;
                    ChangeGridToNormal();
                    break;
                case windowState.PasandoPedido:
                    ChangeGridToPassPedido();
                    gridPasarPedido.Visibility = Visibility.Visible;
                    gridPpal.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#f9f9f9");
                    sbIdleTbkPasarPedido.Begin();
                    break;
            }
            currentWindowState = newState;
        }
        public void ChangeGridToPassPedido()
        {
            gridPpal.RowDefinitions[3].Height = new GridLength(60, GridUnitType.Star);
        }
        public void ChangeGridToNormal()
        {
            gridPpal.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Star);
        }

        //FILTRO UI ------------------------------------------------------------------------------------------------------------------------------------------

        private void InitializeStoryBoards()
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

            //StoryBoard2
            sbIdleTbkPasarPedido = new Storyboard();
            storyboard.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));
            tbkPasandoPedido.RenderTransform = new TranslateTransform();
            DoubleAnimation idleAnimation = new DoubleAnimation()
            {
                From = 9,
                To = -5,
                Duration = sbIdleTbkPasarPedido.Duration,
                DecelerationRatio = 0.5d,
                AccelerationRatio = 0.5d,
                AutoReverse = true

            };
            idleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTarget(idleAnimation, tbkPasandoPedido);
            Storyboard.SetTargetProperty(idleAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            sbIdleTbkPasarPedido.Children.Add(idleAnimation);

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
            //Prueba
            /*List<Mesas> list = new List<Mesas>();
            list.Add(new Mesas() { _id = 1, numero = 1, zona = "Interior", numSillas = 6,estado="Libre" });
            list.Add(new Mesas() { _id = 17, numero = 2, zona = "Interior", numSillas = 5, estado = "Libre" });
            list.Add(new Mesas() { _id = 2, numero = 14, zona = "Interior", numSillas = 8, estado = "Libre" });
            list.Add(new Mesas() { _id = 4, numero = 3, zona = "Interior", numSillas = 4, estado = "Ocupada" });
            list.Add(new Mesas() { _id = 6, numero = 4, zona = "Interior", numSillas = 4, estado = "Libre" });
            list.Add(new Mesas() { _id = 8, numero = 5, zona = "Interior", numSillas = 4, estado = "Libre" });
            list.Add(new Mesas() { _id = 9, numero = 6, zona = "Interior", numSillas = 6, estado = "Ocupada" });
            list.Add(new Mesas() { _id = 10, numero = 7, zona = "Interior", numSillas = 4, estado = "Libre" });
            list.Add(new Mesas() { _id = 11, numero = 8, zona = "Interior", numSillas = 6, estado = "Libre" });
            list.Add(new Mesas() { _id = 12, numero = 9, zona = "Terraza", numSillas = 4, estado = "Ocupada" });
            list.Add(new Mesas() { _id = 13, numero = 10, zona = "Terraza", numSillas = 6, estado = "Libre" });
            list.Add(new Mesas() { _id = 14, numero = 11, zona = "Terraza", numSillas = 10, estado = "Libre" });
            list.Add(new Mesas() { _id = 15, numero = 12, zona = "Terraza", numSillas = 4, estado = "Ocupada" });
            list.Add(new Mesas() { _id = 16, numero = 13, zona = "Terraza", numSillas = 4, estado = "Libre" });*/
            List<Mesas> list = new List<Mesas>();
            try
            {
                list = ControladorMesas.GetAllFromApi();
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }

            return list;
        }

        private void InitializeAndPlaceListPanelesMesa()
        {
            List<Mesas> listaMesas = GetMesas();
            InitializeListReservas();

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

        private void InitializeListReservas()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            try
            {
                listReservas = ControladorReservas.GetAllFromApi(year, month, day, hour, minute);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
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
            panel.button.Style = Application.Current.Resources["clickableResizableButtonWithNoHoverResize"] as Style;
            panel.tbkSuperior.FontSize = 20d;
            panel.tbkSuperior.VerticalAlignment = VerticalAlignment.Top;
            panel.tbkSuperior.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkSuperior.Text = mesa.zona + " (" + mesa.numSillas + ")";
            panel.tbkNumeroMesa.FontSize = 40d;
            panel.tbkNumeroMesa.VerticalAlignment = VerticalAlignment.Center;
            panel.tbkNumeroMesa.HorizontalAlignment = HorizontalAlignment.Center;
            panel.tbkNumeroMesa.Text = mesa.numero + "";

            Reserva reserva = GetReservaFromMesa(mesa);
            if (reserva != null)
            {
                mesa.estado = "Reservada";
                panel.ttReservado.Content = reserva.nombre + Environment.NewLine + String.Format("{0:00}/{1:00}/{2:00}", reserva.dia, reserva.mes, reserva.anyo) + String.Format("{0:00}:{1:00}", reserva.hora, reserva.minuto);
                panel.tbkReservado.FontSize = 20d;
                panel.tbkReservado.VerticalAlignment = VerticalAlignment.Bottom;
                panel.tbkReservado.HorizontalAlignment = HorizontalAlignment.Center;
                panel.tbkReservado.TextWrapping = TextWrapping.Wrap;
                panel.tbkReservado.Text = reserva.nombre;

            }
            else
            {
                if (mesa.estado == "Libre")
                {
                    panel.menuItemOcupar.Header = "Dejar Ocupada";
                    panel.menuItemOcupar.Height = 40d;
                    panel.menuItemOcupar.FontSize = 20d;
                    panel.menuItemOcupar.Click += CtxMenuOcupar_Click;
                    panel.imgMIO.Source = sourceOcuparMesa;
                    panel.imgMIO.Width = 20d;
                    panel.imgMIO.Height = 20d;
                    panel.imgMIO.Margin = new Thickness(-3, -3, -3, -3);
                    RenderOptions.SetBitmapScalingMode(panel.imgMIO, BitmapScalingMode.HighQuality);
                }
                else
                {
                    if (mesa.estado == "Ocupada")
                    {
                        panel.menuItemLibre.Header = "Dejar Libre";
                        panel.menuItemLibre.Height = 40d;
                        panel.menuItemLibre.FontSize = 20d;
                        panel.menuItemLibre.Click += CtxMenuDejarLibre_Click;
                        panel.imgMIL.Source = sourceLiberarMesa;
                        panel.imgMIL.Width = 20d;
                        panel.imgMIL.Height = 20d;
                        panel.imgMIL.Margin = new Thickness(-3, -3, -3, -3);
                        RenderOptions.SetBitmapScalingMode(panel.imgMIL, BitmapScalingMode.HighQuality);

                        panel.menuItemPasarPedido.Header = "Pasar pedido a otra mesa";
                        panel.menuItemPasarPedido.Height = 40d;
                        panel.menuItemPasarPedido.FontSize = 20d;
                        panel.menuItemPasarPedido.Click += CtxMenuPasarPedido_Click;
                        panel.imgMIPP.Source = sourceCambiarPedidoDeMesa;
                        panel.imgMIPP.Width = 20d;
                        panel.imgMIPP.Height = 20d;
                        panel.imgMIPP.Margin = new Thickness(-3, -3, -3, -3);
                        RenderOptions.SetBitmapScalingMode(panel.imgMIPP, BitmapScalingMode.HighQuality);
                    }
                }
            }

            EstablishButtonMesaColor(panel);
            panel.ConstruirPanel();

            listPanelesMesa.Add(panel);
        }
        private Reserva GetReservaFromMesa(Mesas mesa)
        {
            foreach (Reserva reserva in listReservas)
            {
                if (reserva.idMesa == mesa._id)
                {
                    return reserva;
                }
            }
            return null;
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
            else if (panel.mesa.estado == "Reservada")
            {
                panel.button.Background = new SolidColorBrush(Color.FromArgb(255, 242, 240, 80));
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
            string respuesta = "";
            try
            {
                respuesta = ControladorPedidos.PostToApi(pedido); //Meter funcion api para comprobar al postear que no hay ningun pedido para esta mesa, por si alguien lo ha creado mientras le dabas al si del messagebox
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            if (respuesta == "Error idMesa repetido")
            {
                return null;
            }
            string asignedId = respuesta.Split('=')[1];
            pedido._id = Convert.ToInt32(asignedId);
            return pedido;
        }


        private void PanelMesaButton_Clicked(object sender, RoutedEventArgs e)
        {
            PanelMesa panelMesaClickado = GetPanelMesaFromButton(sender as Button);
            Pedidos pedidoMesa = new Pedidos();
            try
            {
                pedidoMesa = ControladorPedidos.GetFromApiByIdMesa(panelMesaClickado.mesa._id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            if (currentWindowState == windowState.Normal)
            {
                if (pedidoMesa == null)
                {
                    if (panelMesaClickado.mesa.estado == "Reservada")
                    {
                        MessageBoxResult result = MessageBox.Show("Quieres ocupar la mesa reservada?", "Ocupar Reserva", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            panelMesaClickado.mesa.estado = "Ocupada";
                            try
                            {
                                ControladorMesas.UpdateInApi(panelMesaClickado.mesa);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                                MainWindow mw = new MainWindow();
                                mw.Show();
                                this.Close();
                            }
                            Reserva reserva = GetReservaFromMesa(panelMesaClickado.mesa);
                            //ControladorReservas
                            btReiniciar_Click(sender, null);
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Quieres crear un pedido nuevo para esta mesa?", "Creacion pedido", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Pedidos pedido = CreateGetAndPostNewPedido(panelMesaClickado.mesa._id);
                            if (pedido == null)
                            {
                                MessageBox.Show("A esta mesa se le acaba de asignar otro pedido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                panelMesaClickado.mesa.estado = "Ocupada";
                                try
                                {
                                    ControladorMesas.UpdateInApi(panelMesaClickado.mesa);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                                    MainWindow mw = new MainWindow();
                                    mw.Show();
                                    this.Close();
                                }
                                MessageBox.Show("Pedido Creado");
                                OpenWindowEditarPedido(pedido, panelMesaClickado.mesa.numero);
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Tiene un pedido");
                    OpenWindowEditarPedido(pedidoMesa, panelMesaClickado.mesa.numero);
                }
            }
            else if (currentWindowState == windowState.PasandoPedido)
            {
                if (pedidoMesa == null)
                {
                    string response = "";
                    try
                    {
                        response = ControladorPedidos.DeleteFromApi(passingPedido);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }
                    if (response == "Not Found")
                    {
                        MessageBox.Show("El pedido que intentas cambiar ya no existe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        passingPedido.idMesa = panelMesaClickado.mesa._id;
                        try
                        {
                            ControladorPedidos.PostToApi(passingPedido);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            this.Close();
                        }

                        MessageBox.Show("Pedido traspasado de la Mesa " + passingPedidoMesa.numero + " a la Mesa " + panelMesaClickado.mesa.numero, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Esta mesa ya tiene un pedido asignado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                ChangeWindowState(windowState.Normal);
            }
        }

        private void OpenWindowEditarPedido(Pedidos pedido, int numMesa)
        {
            WindowEditarPedido windowEditarPedido = new WindowEditarPedido(pedido, numMesa);
            windowEditarPedido.Show();
            this.Close();
        }

        private PanelMesa GetPanelMesaFromContextMenuItem(MenuItem menuI)
        {
            foreach (PanelMesa pm in listPanelesMesa)
            {
                if (pm.menuItemLibre == menuI || pm.menuItemOcupar == menuI || pm.menuItemPasarPedido == menuI)
                {
                    return pm;
                }

            }
            return null;
        }
        private void CtxMenuDejarLibre_Click(object sender, RoutedEventArgs e)
        {
            PanelMesa pm = GetPanelMesaFromContextMenuItem(sender as MenuItem);
            Pedidos pedidoMesa = new Pedidos();
            try
            {
                pedidoMesa = ControladorPedidos.GetFromApiByIdMesa(pm.mesa._id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            if (pedidoMesa == null)
            {
                pm.mesa.estado = "Libre";
                try
                {
                    ControladorMesas.UpdateInApi(pm.mesa);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                btReiniciar_Click(sender, null);
            }
            else
            {
                MessageBox.Show("Esta mesa tiene un pedido pendiente, no se puede dejar Libre", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CtxMenuOcupar_Click(object sender, RoutedEventArgs e)
        {
            PanelMesa pm = GetPanelMesaFromContextMenuItem(sender as MenuItem);
            pm.mesa.estado = "Ocupada";
            try
            {
                ControladorMesas.UpdateInApi(pm.mesa);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            btReiniciar_Click(sender, null);
        }

        private void CtxMenuPasarPedido_Click(object sender, RoutedEventArgs e)
        {
            PanelMesa pm = GetPanelMesaFromContextMenuItem(sender as MenuItem);
            Pedidos pedidoMesa = new Pedidos();
            try
            {
                pedidoMesa = ControladorPedidos.GetFromApiByIdMesa(pm.mesa._id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            if (pedidoMesa == null)
            {
                MessageBox.Show("Esta mesa no contiene ningún pedido que se pueda trasladar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                btReiniciar_Click(sender, null);
                return;
            }

            passingPedido = pedidoMesa;
            passingPedidoMesa = pm.mesa;
            ChangeWindowState(windowState.PasandoPedido);

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
            try
            {
                if (tbComensales.Text != "")
                    Convert.ToInt32(tbComensales.Text);

            }
            catch (Exception ex)
            {
                tbComensales.Text = tbComensales.Text.Substring(0, tbComensales.Text.Length - 1);
                return;
            }
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

        private void btCancelarTraspasoDePedido_Click(object sender, RoutedEventArgs e)
        {
            ChangeWindowState(windowState.Normal);
        }
    }
}
