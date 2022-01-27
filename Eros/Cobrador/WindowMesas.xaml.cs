using Eros.Controladores;
using Eros.Modelos;
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
    /// Interaction logic for WindowMesas.xaml
    /// </summary>
    public partial class WindowMesas : Window
    {
        List<Mesas> listMesas = new List<Mesas>();
        public WindowMesas()
        {
            InitializeComponent();
            SetupTables();
        }

        public void SetupTables()
        {
           
            for (int i = 0; i < 30; i++)
            {
                Button newBtn = new Button();

                newBtn.Content = "M" + (i + 1).ToString();
                newBtn.Name = "Table" + i.ToString();
                newBtn.Width = 35;
                newBtn.Height = 35;
                newBtn.Margin = new Thickness(5, 5, 5, 5);

                if (i < 15)
                {
                    WrapPanel1.Children.Add(newBtn);
                }
                else
                {
                    WrapPanel2.Children.Add(newBtn);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}
