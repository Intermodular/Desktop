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
using Eros.Clases;
using Eros.Modelos;
using Eros.Controladores;

namespace Eros.Cobrador.UtilWindows
{
    /// <summary>
    /// Interaction logic for DialogMesasReservadas.xaml
    /// </summary>
    public partial class DialogMesasReservadas : Window
    {
        List<PanelMesa> listPanelesMesa;
        List<Reserva> listReservas;
        int anyo, mes, dia, hora, minuto, comensales;

        public DialogMesasReservadas(int anyo, int mes, int dia, int hora, int minuto, int comensales)
        {
            InitializeComponent();
            this.anyo = anyo;
            this.mes = mes;
            this.dia = dia;
            this.hora = hora;
            this.minuto = minuto;
            this.comensales = comensales;
            List<Mesas> listaMesas = ControladorMesas.GetAllFromApi();
            tbkTitulo.Text = String.Format("Mesas el {0:00}/{1:00}/{2:00} a las {3:00}:{4:00}", dia, mes, anyo, hora, minuto);
            InitializeListReservas();
            //Desastre de codigo (ignorar)
            listPanelesMesa = new List<PanelMesa>();
            wrapPanelTables.Children.Clear();
            listaMesas = listaMesas.OrderBy(m => m.numSillas).ToList();
            foreach (Mesas m in listaMesas)
            {
                if (m.numSillas >= comensales)
                {
                    ConstructAndAddPanelMesa(m);
                }
            }
        }

        public void InitializeListReservas()
        {
            listReservas = ControladorReservas.GetAllReservasFromMinuteWith2HourThresholdFromApi(anyo, mes, dia, hora, minuto);
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

        private void ConstructAndAddPanelMesa(Mesas mesa)
        {
            PanelMesa panel = new PanelMesa();
            panel.mesa = mesa;
            panel.button.Click += PanelMesaButton_Clicked;
            panel.button.Width = 100d;
            panel.button.Height = 100d;
            panel.button.Margin = new Thickness(20);
            panel.button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            panel.button.VerticalContentAlignment = VerticalAlignment.Stretch;
            panel.button.Style = Application.Current.Resources["clickableResizableButtonWithNoHoverResize"] as Style;

            panel.tbkSuperior.FontSize = 15d;
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
                panel.mesa.estado = "Reservada";
                panel.ttReservado.Content = reserva.nombre + Environment.NewLine + String.Format("{0:00}/{1:00}/{2:00}", reserva.dia, reserva.mes, reserva.anyo) + Environment.NewLine + String.Format("{0:00}:{1:00}", reserva.hora, reserva.minuto);
            }

            EstablishButtonMesaColor(panel);
            panel.ConstruirPanel();
            listPanelesMesa.Add(panel);
            wrapPanelTables.Children.Add(panel.button);
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
            if (panel.mesa.estado == "Reservada")
            {
                panel.button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF58");

            }
            else
            {
                panel.button.Background = new SolidColorBrush(Color.FromArgb(255, 46, 238, 93));
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

        private void PanelMesaButton_Clicked(object sender, RoutedEventArgs e)
        {
            PanelMesa panelMesaClickado = GetPanelMesaFromButton(sender as Button);
            if (panelMesaClickado.mesa.estado != "Reservada")
            {
                WindowEdicionReservas.mesa = panelMesaClickado.mesa;
                this.Close();
            }
            else
            {
                MessageBox.Show("Esta mesa ya está reservada por otra persona", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
