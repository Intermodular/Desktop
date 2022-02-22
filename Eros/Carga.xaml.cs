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
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
        public Carga()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(img_transition);
            dispatcherTimer2.Tick += new EventHandler(pb_transition);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 20);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();
            dispatcherTimer2.Start();
        }
        static Random rnd = new Random();
        int image_show = rnd.Next(0,5);
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
        }

        private void pb_transition(object sender, EventArgs e)
        {
            pBar.Value += 1;
            if (pBar.Value == pBar.Maximum)
            {
                dispatcherTimer.Stop();
                dispatcherTimer2.Stop();
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
        }

    }
}
