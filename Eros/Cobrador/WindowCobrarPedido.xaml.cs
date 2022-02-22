using System;
using System.Collections.Generic;
using System.IO;
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
using Eros.Clases;
using Eros.Controladores;
using Eros.Modelos;

namespace Eros.Cobrador
{
    /// <summary>
    /// Interaction logic for WindowCobrarPedido.xaml
    /// </summary>
    public partial class WindowCobrarPedido : Window
    {
        Pedidos pedidoPpal;
        int numMesa;
        List<PanelLineaPedidoCobrar> listPanelLineaPedido;
        List<PanelLineaPedidoCobrar> listPanelLineaSubPedido;
        enum windowState { Inicio, CobrarEfectivo, CambioEfectivo, CobrarTarjeta, PostTarjeta, DividirCuenta }
        windowState currentWindowState = windowState.Inicio;
        enum paymentState { PedidoPpal, SubPedido }
        paymentState currentPaymentState = paymentState.PedidoPpal;
        enum windowSizeState { Small, Normal, Big }
        windowSizeState currentWinSizeState;
        float precioPedidoPpal, precioSubPedido;
        public WindowCobrarPedido(Pedidos pedido, int numMesa)
        {
            InitializeComponent();

            pedidoPpal = pedido;
            this.numMesa = numMesa;
            tbkTituloPedido.Text = "Pedido Mesa " + numMesa;
            tbkTituloSubPedido.Text = "Subpedido Mesa " + numMesa;
            listPanelLineaPedido = new List<PanelLineaPedidoCobrar>();
            listPanelLineaSubPedido = new List<PanelLineaPedidoCobrar>();
            CreatePanelesLineaPedidoAndPut();

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

        private void ChangeState(windowState newState)
        {
            switch (currentWindowState)
            {
                case windowState.Inicio:
                    gridInicio.Visibility = Visibility.Hidden;
                    break;
                case windowState.DividirCuenta:
                    ChangeToNormalGridLayout();
                    EnableAllListViewItems(false);
                    break;
                case windowState.CobrarEfectivo:
                    gridEfectivo.Visibility = Visibility.Hidden;
                    break;
                case windowState.CambioEfectivo:
                    gridCambio.Visibility = Visibility.Hidden;
                    break;
                case windowState.CobrarTarjeta:
                    gridTarjeta.Visibility = Visibility.Hidden;
                    break;

                case windowState.PostTarjeta:
                    gridPostTrajeta.Visibility = Visibility.Hidden;
                    break;
            }

            switch (newState)
            {
                case windowState.Inicio:

                    if (currentPaymentState == paymentState.PedidoPpal)
                    {
                        borderSubPedido.Visibility = Visibility.Hidden;
                        borderPedidoPpal.Visibility = Visibility.Visible;
                        Grid.SetColumn(borderSubPedido, 2);
                    }
                    else
                    {
                        borderPedidoPpal.Visibility = Visibility.Hidden;
                        Grid.SetColumn(borderSubPedido, 0);
                        borderSubPedido.Visibility = Visibility.Visible;
                    }
                    gridInicio.Visibility = Visibility.Visible;
                    break;

                case windowState.DividirCuenta:

                    btSwitchFromLists.Visibility = Visibility.Visible;
                    btSepararLinea.Visibility = Visibility.Visible;
                    btHecho.Visibility = Visibility.Visible;
                    ChangeToDivideGridLayout();
                    if (currentPaymentState == paymentState.PedidoPpal)
                    {
                        borderSubPedido.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        borderPedidoPpal.Visibility = Visibility.Visible;
                        Grid.SetColumn(borderSubPedido, 2);
                    }
                    EnableAllListViewItems(true);
                    break;
                case windowState.CobrarEfectivo:
                    gridEfectivo.Visibility = Visibility.Visible;
                    tbCalculadora.Text = "0";
                    break;

                case windowState.CambioEfectivo:
                    gridCambio.Visibility = Visibility.Visible;
                    break;
                case windowState.CobrarTarjeta:
                    gridTarjeta.Visibility = Visibility.Visible;
                    break;

                case windowState.PostTarjeta:
                    gridPostTrajeta.Visibility = Visibility.Visible;
                    break;
            }

            currentWindowState = newState;
        }

        private void ChangeToDivideGridLayout()
        {
            gridPrincipal.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
            gridPrincipal.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
            gridPrincipal.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
        }

        private void ChangeToNormalGridLayout()
        {
            gridPrincipal.ColumnDefinitions[0].Width = new GridLength(4, GridUnitType.Star);
            gridPrincipal.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
            gridPrincipal.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
        }

        private void EnableAllListViewItems(bool enable)
        {
            if (enable == false)
            {
                listViewPedidoPpal.SelectedItem = null;
                listViewSubPedido.SelectedItem = null;
            }
            foreach (PanelLineaPedidoCobrar plpc in listPanelLineaPedido)
            {
                plpc.listViewItem.IsEnabled = enable;
            }
            foreach (PanelLineaPedidoCobrar plpc in listPanelLineaSubPedido)
            {
                plpc.listViewItem.IsEnabled = enable;
            }
        }

        private void btAtras_Click(object sender, RoutedEventArgs e)
        {
            if (pedidoPpal != null)
            {
                WindowEditarPedido wep = new WindowEditarPedido(pedidoPpal, numMesa);
                wep.Show();
            }
            else
            {
                WindowGestionMesas wgm = new WindowGestionMesas();
                wgm.Show();
            }

            this.Close();
        }

        private void CreatePanelesLineaPedidoAndPut()
        {
            listViewPedidoPpal.Items.Clear();
            foreach (LineaPedido lp in pedidoPpal.lineasPedido)
            {
                PanelLineaPedidoCobrar panelLinea = BuildPanelLineaFromLinea(lp);
                listPanelLineaPedido.Add(panelLinea);
                listViewPedidoPpal.Items.Add(panelLinea.listViewItem);
            }
            UpdateTotalTxt();
        }

        private PanelLineaPedidoCobrar BuildPanelLineaFromLinea(LineaPedido lp)
        {
            PanelLineaPedidoCobrar panelLinea = new PanelLineaPedidoCobrar();

            panelLinea.lineaPedido = lp;
            panelLinea.listViewItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            panelLinea.listViewItem.Padding = new Thickness(5, 5, 5, 5);
            panelLinea.listViewItem.IsEnabled = false;
            panelLinea.tbPpal.HorizontalAlignment = HorizontalAlignment.Left;
            //double fontsize = GetFontSizeOfOrderLine();
            //panelLinea.tbPpal.FontSize = fontsize;
            panelLinea.tbPpal.FontSize = GetFontsizeFromCurrentWindowSizeState();
            panelLinea.tbTotalDinero.HorizontalAlignment = HorizontalAlignment.Right;
            //panelLinea.tbTotalDinero.FontSize = fontsize;
            panelLinea.tbTotalDinero.FontSize = panelLinea.tbPpal.FontSize;
            panelLinea.PutInfo();
            panelLinea.ConstruirEstructuraPanel();

            return panelLinea;
        }

        private void UpdateTotalTxt()
        {
            float precioTotal = 0;

            foreach (PanelLineaPedidoCobrar plpc in listPanelLineaPedido)
            {
                precioTotal += plpc.lineaPedido.costeLinea;
            }
            tbkTotal.Text = String.Format("Total: {0:0.00}€", precioTotal);
            precioPedidoPpal = precioTotal;
        }

        private void UpdateTotalTextSubPedido()
        {
            float precioTotal = 0;

            foreach (PanelLineaPedidoCobrar plpc in listPanelLineaSubPedido)
            {
                precioTotal += plpc.lineaPedido.costeLinea;
            }
            tbkTotalSubPedido.Text = String.Format("Total: {0:0.00}€", precioTotal);
            precioSubPedido = precioTotal;
        }


        //DIVISION PEDIDO

        private void TransferPanelLineaPedidoCobrarToOtherList(PanelLineaPedidoCobrar panelLinea)
        {
            if (listPanelLineaPedido.Contains(panelLinea))
            {
                listPanelLineaPedido.Remove(panelLinea);
                listViewPedidoPpal.Items.Remove(panelLinea.listViewItem);

                listPanelLineaSubPedido.Add(panelLinea);
                listViewSubPedido.Items.Add(panelLinea.listViewItem);

                listViewSubPedido.ScrollIntoView(listViewSubPedido.Items[listViewSubPedido.Items.Count - 1]);
            }
            else
            {
                listPanelLineaSubPedido.Remove(panelLinea);
                listViewSubPedido.Items.Remove(panelLinea.listViewItem);

                listPanelLineaPedido.Add(panelLinea);
                listViewPedidoPpal.Items.Add(panelLinea.listViewItem);

                listViewPedidoPpal.ScrollIntoView(listViewPedidoPpal.Items[listViewPedidoPpal.Items.Count - 1]);
            }
        }

        private void btSwitchFromLists_Click(object sender, RoutedEventArgs e)
        {
            List<ListViewItem> listaSelected = new List<ListViewItem>();
            foreach (ListViewItem lvi in listViewPedidoPpal.SelectedItems)
            {
                listaSelected.Add(lvi);
            }

            foreach (ListViewItem lvi in listViewSubPedido.SelectedItems)
            {
                listaSelected.Add(lvi);
            }

            foreach (ListViewItem lvi in listaSelected)
            {
                PanelLineaPedidoCobrar pLinea = GetLinePanelFromListItem(lvi);
                TransferPanelLineaPedidoCobrarToOtherList(pLinea);
            }

            UpdateTotalTxt();
            UpdateTotalTextSubPedido();


        }

        private PanelLineaPedidoCobrar GetLinePanelFromListItem(ListViewItem lvi)
        {
            foreach (PanelLineaPedidoCobrar pLinea in listPanelLineaPedido)
            {
                if (pLinea.listViewItem == lvi)
                {
                    return pLinea;
                }
            }

            foreach (PanelLineaPedidoCobrar pLinea in listPanelLineaSubPedido)
            {
                if (pLinea.listViewItem == lvi)
                {
                    return pLinea;
                }
            }

            return null;
        }

        private void btSepararLinea_Click(object sender, RoutedEventArgs e)
        {
            DivideLineaPedido(GetLinePanelFromListItem(listViewPedidoPpal.SelectedItem as ListViewItem));
            DivideLineaPedido(GetLinePanelFromListItem(listViewSubPedido.SelectedItem as ListViewItem));
            EnableAllListViewItems(true);
        }

        private void btHecho_Click(object sender, RoutedEventArgs e)
        {
            if (listPanelLineaPedido.Count == 0)
            {
                MessageBox.Show("No puedes dejar el pedido principal vacío");
                return;
            }

            if (listPanelLineaSubPedido.Count == 0)
            {
                currentPaymentState = paymentState.PedidoPpal;
            }
            else
            {
                currentPaymentState = paymentState.SubPedido;
            }

            ChangeState(windowState.Inicio);
            ResumeLineaPedidoList(listPanelLineaPedido, listViewPedidoPpal);
            ResumeLineaPedidoList(listPanelLineaSubPedido, listViewSubPedido);

        }

        private void btImprimir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"..\..\Recibos\recibo.txt");
        }

        private void DivideLineaPedido(PanelLineaPedidoCobrar linePanel)
        {
            if (linePanel == null)
            {
                return;
            }
            bool isInMainList = listPanelLineaPedido.Contains(linePanel);
            int indexOfLinePanel;

            if (isInMainList)
            {
                indexOfLinePanel = listViewPedidoPpal.Items.IndexOf(linePanel.listViewItem);
            }
            else
            {
                indexOfLinePanel = listViewSubPedido.Items.IndexOf(linePanel.listViewItem);
            }


            if (linePanel.lineaPedido.cantidad > 1)
            {
                for (int i = 0; i < linePanel.lineaPedido.cantidad; i++)
                {
                    LineaPedido newLp = CloneLineaPedido(linePanel.lineaPedido);
                    newLp.cantidad = 1;
                    newLp.CalcularCosteLinea();
                    PanelLineaPedidoCobrar panelLinea = BuildPanelLineaFromLinea(newLp);
                    if (isInMainList)
                    {
                        listPanelLineaPedido.Insert(indexOfLinePanel, panelLinea);
                        listViewPedidoPpal.Items.Insert(indexOfLinePanel, panelLinea.listViewItem);
                    }
                    else
                    {
                        listPanelLineaSubPedido.Insert(indexOfLinePanel, panelLinea);
                        listViewSubPedido.Items.Insert(indexOfLinePanel, panelLinea.listViewItem);
                    }
                }


                if (isInMainList)
                {
                    listPanelLineaPedido.Remove(linePanel);
                    listViewPedidoPpal.Items.Remove(linePanel.listViewItem);
                }
                else
                {
                    listPanelLineaSubPedido.Remove(linePanel);
                    listViewSubPedido.Items.Remove(linePanel.listViewItem);
                }

            }
        }

        private void ResumeLineaPedidoList(List<PanelLineaPedidoCobrar> listaPanelLinea, ListView listViewAsociada)
        {
            for (int i = 0; i < listaPanelLinea.Count - 1; i++)
            {
                for (int j = i + 1; j < listaPanelLinea.Count; j++)
                {
                    if (AreLineasPedidoTheSame(listaPanelLinea[i].lineaPedido, listaPanelLinea[j].lineaPedido))
                    {
                        listaPanelLinea[i].lineaPedido.cantidad += listaPanelLinea[j].lineaPedido.cantidad;
                        listaPanelLinea[i].lineaPedido.CalcularCosteLinea();
                        listaPanelLinea[i].PutInfo();

                        listaPanelLinea.RemoveAt(j);
                        listViewAsociada.Items.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private bool AreLineasPedidoTheSame(LineaPedido lp1, LineaPedido lp2) //No cuenta la cantidad
        {

            if (lp1.producto._id == lp2.producto._id && lp1.anotaciones == lp2.anotaciones)
            {
                if (lp1.lineasExtras.Count != lp2.lineasExtras.Count)
                {
                    return false;
                }
                for (int i = 0; i < lp1.lineasExtras.Count; i++)
                {
                    if (lp1.lineasExtras[i].cantidad != lp2.lineasExtras[i].cantidad || lp1.lineasExtras[i].extra.nombre != lp2.lineasExtras[i].extra.nombre)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }


        }

        private LineaPedido CloneLineaPedido(LineaPedido lp)
        {
            LineaPedido newlp = new LineaPedido();
            newlp.cantidad = lp.cantidad;
            newlp.anotaciones = lp.anotaciones;
            newlp.costeLinea = lp.costeLinea;
            newlp.producto = lp.producto;
            newlp.lineasExtras = new List<LineaExtra>();
            foreach (LineaExtra oldLineaExtra in lp.lineasExtras)
            {
                LineaExtra newLineaExtra = new LineaExtra();
                newLineaExtra.cantidad = oldLineaExtra.cantidad;
                newLineaExtra.extra = oldLineaExtra.extra;
                newlp.lineasExtras.Add(newLineaExtra);
            }

            return newlp;
        }

        private void btDividirCuenta_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(windowState.DividirCuenta);
        }

        //EFECTIVO

        private void btNumber_Click(object sender, RoutedEventArgs e)
        {
            if (tbCalculadora.Text == "0")
            {
                tbCalculadora.Text = "";
            }

            string[] splittedStr = tbCalculadora.Text.Split(',');

            if (splittedStr.Length > 1)
            {
                if (splittedStr[1].Length < 2)
                {
                    tbCalculadora.Text += ((Button)sender).Content;
                }
            }
            else
            {
                tbCalculadora.Text += ((Button)sender).Content;
            }

        }

        private void btNumCE_Click(object sender, RoutedEventArgs e)
        {
            tbCalculadora.Text = "0";
        }
        private void btComa_Click(object sender, RoutedEventArgs e)
        {
            if (!tbCalculadora.Text.Contains(","))
            {
                tbCalculadora.Text += ",";
            }
        }

        private void btCobrarEfectivo(object sender, RoutedEventArgs e)
        {
            float cantidadAPagar = currentPaymentState == paymentState.PedidoPpal ? precioPedidoPpal : precioSubPedido;
            float cantidadPagada = float.Parse(tbCalculadora.Text);
            if (cantidadPagada < cantidadAPagar)
            {
                MessageBox.Show(String.Format("La cantidad pagada es inferior a la cantidad a pagar ( {0:0.00}€ )", cantidadAPagar));
                return;
            }
            float cambio = cantidadPagada - cantidadAPagar;
            tbCambio.Text = String.Format("{0:0.00}€", cambio);

            CobrarPedido("Efectivo");

            ChangeState(windowState.CambioEfectivo);
        }


        private void btHechoEfectivo_Click(object sender, RoutedEventArgs e)
        {
            if (currentPaymentState == paymentState.SubPedido)
            {
                listPanelLineaSubPedido.Clear();
                listViewSubPedido.Items.Clear();
                UpdateTotalTextSubPedido();
                currentPaymentState = paymentState.PedidoPpal;
                ChangeState(windowState.Inicio);
            }
            else
            {
                WindowGestionMesas wgm = new WindowGestionMesas();
                wgm.Show();
                this.Close();
            }

        }
        private void btCancelarEfectivo_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(windowState.Inicio);
        }

        private void btCobrarEnEfectivo_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(windowState.CobrarEfectivo);
        }



        //TARJETA---------------------------------------------------------------------------------------------------------------------------------------------------
        private void btCobrarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(windowState.CobrarTarjeta);
        }

        private void btTarjetaValidada_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(windowState.PostTarjeta);

            CobrarPedido("Tarjeta");
        }

        private void btErrorTarjeta_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Error la intentar leer la tarjeta.\nPruebe otra vez", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //COBRAR EL PEDIDO ----------------------------------------------------------------------------------------------------------------------

        private void CobrarPedido(string metodoPago)
        {
            Ticket ticket = new Ticket();

            ticket.numMesa = numMesa;
            ticket.fechaYHoraDePago = DateTime.Now.ToString();
            ticket.metodoDePago = metodoPago;
            //ticket.empleado = ....
            if (currentPaymentState == paymentState.PedidoPpal)
            {
                //Crear y subir Ticket
                ticket.costeTotal = precioPedidoPpal;
                ticket.lineasPedido = pedidoPpal.lineasPedido;
                ticket.CleanLineaPedidoListFromExtras();
                try
                {
                    ControladorTickets.PostToApi(ticket);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                //Borrar Pedido de la BDDS
                try
                {
                    ControladorPedidos.DeleteFromApi(pedidoPpal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                try
                {
                    CreateTextFile(ticket.ToString());
                }
                catch (Exception ex)
                {
                    
                }
                pedidoPpal = null;
            }
            else
            {
                //Crear y subir Ticket
                ticket.costeTotal = precioSubPedido;
                ticket.lineasPedido = GetListLineaPedidoFromPanelLineaList(listPanelLineaSubPedido);
                ticket.CleanLineaPedidoListFromExtras();
                try
                {
                    ControladorTickets.PostToApi(ticket);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                //Actualizar lineas de pedido de pedido
                pedidoPpal.lineasPedido = GetListLineaPedidoFromPanelLineaList(listPanelLineaPedido);
                //Actualizar pedido en la BDDS
                
                try
                {
                    ControladorPedidos.UpdateInApi(pedidoPpal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }

                try
                {
                    CreateTextFile(ticket.ToString());
                }
                catch (Exception ex)
                {

                }
            }
        }

        private List<LineaPedido> GetListLineaPedidoFromPanelLineaList(List<PanelLineaPedidoCobrar> listaPanel)
        {
            List<LineaPedido> listaLineas = new List<LineaPedido>();
            foreach (PanelLineaPedidoCobrar panel in listaPanel)
            {
                listaLineas.Add(panel.lineaPedido);
            }
            return listaLineas;
        }

        private void btImprimirPedidoPpal_Click(object sender, RoutedEventArgs e)
        {
            Ticket ticket = new Ticket();
            Mesas m = new Mesas();
            try
            {
                m = ControladorMesas.GetFromApi(pedidoPpal.idMesa);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }

            ticket.numMesa = m.numero;
            ticket.costeTotal = precioPedidoPpal + precioSubPedido;
            ticket.lineasPedido = pedidoPpal.lineasPedido;
            //CreateTextFile(ticket.ToString());
            System.Diagnostics.Process.Start(@"..\..\Recibos\recibo.txt");  
            try
            {
                System.Diagnostics.Process.Start(@"..\..\Recibos\recibo.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Acceso denegado. \nNo podemos acceder al recibo.");
            }
        }

        private void CreateTextFile(string text)
        {
            StreamWriter sw = File.CreateText("../../Recibos/recibo.txt");
            sw.Write(text);
            sw.Close();
        }

        //Eventos Generales

        private void ChangeAllPanelLineaPedidoFontSize(float fontsize)
        {
            foreach (PanelLineaPedidoCobrar plpc in listPanelLineaPedido)
            {
                plpc.tbPpal.FontSize = fontsize;
                plpc.tbTotalDinero.FontSize = fontsize;
            }
            foreach (PanelLineaPedidoCobrar plpc in listPanelLineaSubPedido)
            {
                plpc.tbPpal.FontSize = fontsize;
                plpc.tbTotalDinero.FontSize = fontsize;
            }
        }


        private void CheckChangeWindowSizeState()
        {
            if (this.ActualWidth < 800d)
            {
                if (currentWinSizeState != windowSizeState.Small)
                {
                    ChangeAllPanelLineaPedidoFontSize(15);
                    currentWinSizeState = windowSizeState.Small;
                }

            }
            else if (this.ActualWidth < 1400d)
            {
                if (currentWinSizeState != windowSizeState.Normal)
                {
                    ChangeAllPanelLineaPedidoFontSize(25);
                    currentWinSizeState = windowSizeState.Normal;
                }
            }
            else
            {
                if (currentWinSizeState != windowSizeState.Big)
                {
                    ChangeAllPanelLineaPedidoFontSize(35);
                    currentWinSizeState = windowSizeState.Big;
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CheckChangeWindowSizeState();
            GlobalVariables.top = Top;
            GlobalVariables.left = Top;
            GlobalVariables.width = Width;
            GlobalVariables.height = Height;
            GlobalVariables.max = WindowState == WindowState.Maximized;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private float GetFontsizeFromCurrentWindowSizeState()
        {
            if (currentWinSizeState == windowSizeState.Normal)
            {
                return 25;
            }

            if (currentWinSizeState == windowSizeState.Big)
            {
                return 35;
            }

            return 15;

        }
    }
}
