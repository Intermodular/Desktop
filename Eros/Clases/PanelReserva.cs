using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Eros.Modelos;

namespace Eros.Clases
{
    public class PanelReserva
    {
        public Reserva reserva { get; set; }
        public Button button { get; set; }
        public Border border { get; set; }
        public StackPanel stackPanel { get; set; }
        public Border borderArriba { get; set; }
        public Border borderAbajo { get; set; }
        public StackPanel stackPanelDatos { get; set; }
        public TextBlock tbkNombre { get; set; }
        public TextBlock tbkFecha { get; set; }
        public TextBlock tbkHora { get; set; }
        public TextBlock tbkComensales { get; set; }
        public TextBlock tbkMesa { get; set; }

        public PanelReserva()
        {
            button = new Button();
            border = new Border();
            stackPanel = new StackPanel();
            borderArriba = new Border();
            borderAbajo = new Border();
            stackPanelDatos = new StackPanel();
            tbkNombre = new TextBlock();
            tbkFecha = new TextBlock();
            tbkHora = new TextBlock();
            tbkComensales = new TextBlock();
            tbkMesa = new TextBlock();
        }

        public void ContruirPanel()
        {
            button.Content = border;
            border.Child = stackPanel;
            stackPanel.Children.Add(borderArriba);
            stackPanel.Children.Add(borderAbajo);
            borderArriba.Child = tbkNombre;
            borderAbajo.Child = stackPanelDatos;
            stackPanelDatos.Children.Add(tbkFecha);
            stackPanelDatos.Children.Add(tbkHora);
            stackPanelDatos.Children.Add(tbkComensales);
            stackPanelDatos.Children.Add(tbkMesa);
        }

        public void UpdateUIElementsInfo()
        {
            tbkNombre.Text = reserva.nombre;
            tbkFecha.Text = String.Format("{0:00}/{1:00}/{2}", reserva.dia, reserva.mes, reserva.anyo);
            tbkHora.Text = String.Format("{0:00}:{1:00}", reserva.hora, reserva.minuto);
            tbkComensales.Text = reserva.numComensales > 1 ? reserva.numComensales + " personas" : reserva.numComensales + " persona";
            tbkMesa.Text = "Mesa " + reserva.numMesa;
        }
    }
}
