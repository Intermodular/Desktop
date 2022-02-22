using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Eros.Modelos;

namespace Eros.Clases
{
    public class PanelNomina
    {
        public Nominas nomina { get; set; }
        public ListBoxItem lbItem { get; set; }
        public Border border { get; set; }
        public StackPanel stackPanel { get; set; }
        public Border nameBorder { get; set; }
        public TextBlock tbkNombre { get; set; }
        public Border datesBorder { get; set; }
        public TextBlock tbkDates { get; set; }
        public PanelNomina()
        {
            nomina = new Nominas();
            lbItem = new ListBoxItem();
            border = new Border();
            stackPanel = new StackPanel();
            nameBorder = new Border();
            tbkNombre = new TextBlock();
            datesBorder = new Border();
            tbkDates = new TextBlock();
        }

        public void ConstruirPanel()
        {
            lbItem.Content = border;
            border.Child = stackPanel;
            stackPanel.Children.Add(nameBorder);
            stackPanel.Children.Add(datesBorder);
            nameBorder.Child = tbkNombre;
            datesBorder.Child = tbkDates;
        }
    }
}
