using Eros.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Eros.Clases
{
    class PanelMesa
    {
        public Mesas mesa { get; set; }
        public Button button { get; set; }
        public Grid grid { get; set; }
        public TextBlock tbkSuperior { get; set; }
        public TextBlock tbkNumeroMesa { get; set; }
        public ContextMenu contextMenu { get; set; }
        public MenuItem menuItemLibre { get; set; }
        public MenuItem menuItemOcupar { get; set; }

        public PanelMesa()
        {
            button = new Button();
            grid = new Grid();
            tbkSuperior = new TextBlock();
            tbkNumeroMesa = new TextBlock();
            contextMenu = new ContextMenu();
            menuItemLibre = new MenuItem();
            menuItemOcupar = new MenuItem();
        }

        public void ConstruirPanel()
        {
            grid.Children.Add(tbkSuperior);
            grid.Children.Add(tbkNumeroMesa);
            button.Content = grid;
            button.ContextMenu = contextMenu;
            if (mesa.estado == "Libre")
            {
                contextMenu.Items.Add(menuItemOcupar);
            }
            else
            {
                contextMenu.Items.Add(menuItemLibre);
            }


        }
    }
}
