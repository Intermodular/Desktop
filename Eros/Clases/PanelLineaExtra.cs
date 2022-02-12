using Eros.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Eros.Clases
{
    class PanelLineaExtra
    {
        public LineaExtra lineaExtra;
        public StackPanel stackpanelSuperior;
        public TextBlock tbkNombre;
        public StackPanel stackpanelInferior;
        public Button btMenos;
        public Image imgBtMenos;
        public TextBox tbCantidad;
        public Button btMas;
        public Image imgBtMas;

        public PanelLineaExtra()
        {
            stackpanelSuperior = new StackPanel();
            tbkNombre = new TextBlock();
            stackpanelInferior = new StackPanel();
            btMenos = new Button();
            tbCantidad = new TextBox();
            btMas = new Button();
            imgBtMenos = new Image();
            imgBtMas = new Image();
            imgBtMenos.Source = new BitmapImage(new Uri("../Img/icons/minusIcon.png", UriKind.Relative));
            imgBtMas.Source = new BitmapImage(new Uri("../Img/icons/plusIcon.png", UriKind.Relative));
        }

        public void ConstruirPanel()
        {
            btMas.Content = imgBtMas;
            btMenos.Content = imgBtMenos;
            stackpanelInferior.Children.Add(btMenos);
            stackpanelInferior.Children.Add(tbCantidad);
            stackpanelInferior.Children.Add(btMas);
            stackpanelSuperior.Children.Add(tbkNombre);
            stackpanelSuperior.Children.Add(stackpanelInferior);
        }
    }
}
