using Eros.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Eros.Clases
{
    class PanelLineaPedido
    {
        public LineaPedido lineaPedido { get; set; }
        public ListBoxItem listBoxItem { get; set; }
        public Grid grid { get; set; }
        public TextBlock tbPpal { get; set; }
        public TextBlock tbTotalDinero { get; set; }

        public PanelLineaPedido()
        {
            lineaPedido = new LineaPedido();
            listBoxItem = new ListBoxItem();
            grid = new Grid();
            tbPpal = new TextBlock();
            tbTotalDinero = new TextBlock();
        }

        public void ConstruirEstructuraPanel()
        {
            grid.Children.Add(tbPpal);
            grid.Children.Add(tbTotalDinero);
            listBoxItem.Content = grid;
        }

        public void PutInfo()
        {
            tbPpal.Text = lineaPedido.producto.nombre + " x " + lineaPedido.cantidad + GetVisualRespresentationOfAnnotation() + GetVisualRepresentationOfExtras();
            tbTotalDinero.Text = String.Format("{0:0.00}€", lineaPedido.costeLinea);
        }

        private string GetVisualRespresentationOfAnnotation()
        {
            if (lineaPedido.anotaciones == "")
            {
                return "";
            }
            string[] annotations = lineaPedido.anotaciones.Split(',');
            string visualAnnotation = "";
            for (int i = 0; i < annotations.Length; i++)
            {
                visualAnnotation += Environment.NewLine + "  *" + annotations[i];
            }
            return visualAnnotation;
        }

        private string GetVisualRepresentationOfExtras()
        {
            if (lineaPedido.lineasExtras == null)
            {
                return "";
            }

            string res = "";
            foreach (LineaExtra le in lineaPedido.lineasExtras)
            {
                if (le.cantidad > 0)
                {
                    res += Environment.NewLine + "  *EXTRA " + le.extra.nombre;
                    if (le.cantidad > 1)
                    {
                        res += " x" + le.cantidad;
                    }
                }
            }

            return res;
        }
    }
}
