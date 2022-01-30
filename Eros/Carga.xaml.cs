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

namespace Eros
{
    /// <summary>
    /// Lógica de interacción para Carga.xaml
    /// </summary>
    public partial class Carga : Window
    {
        public Carga()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(img_transition);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }
        int image_show = 0;
        private void img_transition(object sender, EventArgs e)
        {
            try
            {
                BitmapImage[] images =
                    {
                    new BitmapImage(new Uri(@"/Eros;component/Img/carga/hamburguesa2.jpeg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/Img/carga/pizza.jpeg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/Img/carga/hamburguesa3.jpeg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/Img/carga/patatas.jpeg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/Img/carga/pizza2.jpeg", UriKind.Relative)),
                    new BitmapImage(new Uri(@"/Eros;component/Img/carga/hamburguesa.jpeg", UriKind.Relative))
                    };
                img_carga.Source = images[image_show];

            }
            catch (Exception)
            {
                MessageBox.Show("No se ha encontrado la imagen");
            };
            if (image_show == 5)
            {
                image_show = 0;
            }
            else
            {
                image_show++;
            }
        }

    }
}
