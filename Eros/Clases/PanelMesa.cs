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
        public MenuItem menuItemPasarPedido { get; set; }
        public Image imgMIL { get; set; }
        public Image imgMIO { get; set; }
        public Image imgMIPP { get; set; }
        public ToolTip ttReservado { get; set; }
        public TextBlock tbkReservado { get; set; }


        public PanelMesa()
        {
            button = new Button();
            grid = new Grid();
            tbkSuperior = new TextBlock();
            tbkNumeroMesa = new TextBlock();
            contextMenu = new ContextMenu();
            menuItemLibre = new MenuItem();
            menuItemOcupar = new MenuItem();
            menuItemPasarPedido = new MenuItem();
            imgMIL = new Image();
            imgMIO = new Image();
            imgMIPP = new Image();
            ttReservado = new ToolTip();
            tbkReservado = new TextBlock();


        }

        public void ConstruirPanel()
        {
            grid.Children.Add(tbkSuperior);
            grid.Children.Add(tbkNumeroMesa);
            button.Content = grid;
            button.ContextMenu = contextMenu;
            if (mesa.estado == "Libre")
            {
                menuItemOcupar.Icon = imgMIO;
                contextMenu.Items.Add(menuItemOcupar);
            }
            else if (mesa.estado == "Ocupada")
            {
                menuItemLibre.Icon = imgMIL;
                contextMenu.Items.Add(menuItemLibre);

                menuItemPasarPedido.Icon = imgMIPP;
                contextMenu.Items.Add(menuItemPasarPedido);
            }
            else if (mesa.estado == "Reservada")
            {
                button.ToolTip = ttReservado;
                grid.Children.Add(tbkReservado);
            }





        }
    }
}
