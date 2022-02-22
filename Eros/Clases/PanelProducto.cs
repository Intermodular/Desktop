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
    class PanelProducto
    {
        public Productos producto { get; set; }
        public Button boton { get; set; }
        public Viewbox vBox { get; set; }
        public StackPanel stackPanel { get; set; }
        public Image imagen { get; set; }
        public TextBlock tBlock { get; set; }

        public PanelProducto()
        {
            boton = new Button();
            vBox = new Viewbox();
            stackPanel = new StackPanel();
            imagen = new Image();
            tBlock = new TextBlock();
        }

        public void ConstruirPanel()
        {
            stackPanel.Children.Add(imagen);
            stackPanel.Children.Add(tBlock);
            vBox.Child = stackPanel;
            boton.Content = vBox;
        }

        public void CargarImagen()
        {
            try
            {
                string fullFilePath = producto.imagen;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                bitmap.EndInit();

                imagen.Source = bitmap;
            }
            catch (Exception e)
            {
                imagen.Source = new BitmapImage(new Uri("../Img/icons/foodPlaceHolder.png", UriKind.Relative));
            }

        }
    }
}
