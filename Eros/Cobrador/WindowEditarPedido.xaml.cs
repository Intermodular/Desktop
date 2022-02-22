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
using Eros.Controladores;
using Eros.Clases;
using Eros.Modelos;
using System.Windows.Media.Animation;

namespace Eros.Cobrador
{
    /// <summary>
    /// Interaction logic for WindowEditarPedido.xaml
    /// </summary>
    /// </summary>
    public partial class WindowEditarPedido : Window
    {
        //Ventana
        enum state { Normal, Edit, Info };
        state windowState = state.Normal;


        //Variables Pedido
        Pedidos pedido;
        int numMesa;
        List<PanelLineaPedido> linePanelList;
        PanelLineaPedido linePanelBeingEdited = null;

        //Extras
        List<PanelLineaExtra> extraLinePanelList;

        //Variables Productos
        List<PanelProducto> panelList;

        //Variables Botones Tipos
        List<Tipos> typeList;
        List<Button> typeButtonList;
        double minTypeButtonWidth = 150;
        double typeButtonHeight = 50;
        double typeButtonFontsize = 14;
        int numberOfTypeButtonsPerBlock = 0;
        int currentRoundOfTypeButtons;
        double widthAux = 800, heightAux = 400;
        Button currentTypeButton;

        //Filtros
        Storyboard storyBoardFiltros;
        DoubleAnimation movemenFilterAnimation;
        int animDurationInMil = 300;
        Duration durationGo;
        Duration durationCome;
        float filterWidthProportion = 0.20f;
        List<CheckBox> cbxFiltersList;
        public WindowEditarPedido(Pedidos pedido, int numMesa)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            usuName.Text = GlobalVariables.username;
            if (GlobalVariables.max)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = GlobalVariables.left;
                Top = GlobalVariables.top;
                Height = GlobalVariables.height;
                Width = GlobalVariables.width;
            }

            CreateTypeList();
            CreateTypeButtonList();
            InitializeComponentData();
            currentRoundOfTypeButtons = 0;
            PutButtons();

            CreatePanelList();
            BuildPanelGrid();
            InitializeFilterCheckBoxes();
            PutPanelsInGrid(typeList[0].nombre);
            currentTypeButton = typeButtonList[0];
            currentTypeButton.Style = Application.Current.Resources["buttonTiposSeleccionado"] as Style;
            LoadImages();

            this.pedido = pedido;
            this.numMesa = numMesa;
            summaryListBox.Items.Clear();
            linePanelList = new List<PanelLineaPedido>();
            CreateAndAddPanelsLineaPedidoFromPedido();
            WindowTitle.Text = "Edición de Pedido (Mesa " + numMesa + ")";
            //GetAndPlaceOrCreatePedido();

            tbContador.Text = "0";

            InitializeAnimations();
            borderFiltros.Width = this.Width * filterWidthProportion;

            if (GlobalVariables.max)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                Left = GlobalVariables.left;
                Top = GlobalVariables.top;
                Height = GlobalVariables.height;
                Width = GlobalVariables.width;
            }

        }


        private void InitializeComponentData()
        {
            btLeft.Height = typeButtonList[0].Height;
            btLeft.Width = btLeft.Height;
            btRight.Height = typeButtonList[0].Height;
            btRight.Width = btRight.Height;
            borderPedido.Margin = new Thickness(btRight.Width + 5, 20, 20, 15);
            btCobrar.Margin = new Thickness(btRight.Width + 5, 0, 20, 10);
        }

        private void ChangeState(state newState)
        {
            switch (windowState)
            {
                case state.Normal:
                    gridProductos.Visibility = Visibility.Hidden;
                    btRight.Visibility = Visibility.Hidden;
                    btLeft.Visibility = Visibility.Hidden;
                    break;

                case state.Edit:
                    gridEdicionProductos.Visibility = Visibility.Hidden;
                    break;

                case state.Info:
                    gridInfoProductos.Visibility = Visibility.Hidden;
                    break;
            }

            switch (newState)
            {
                case state.Normal:
                    gridProductos.Visibility = Visibility.Visible;
                    CheckDisablingOfButtons();
                    break;

                case state.Edit:
                    gridEdicionProductos.Visibility = Visibility.Visible;
                    svEditar.ScrollToTop();
                    break;

                case state.Info:
                    gridInfoProductos.Visibility = Visibility.Visible;
                    svInfoProductos.ScrollToTop();
                    break;
            }

            windowState = newState;
        }

        private double GetProductsGridWidth()
        {
            double width = ((this.Width * (745d / (745d + 291d + 74d))) - (5 * 2)); //Estrellitas del grid y margen del grid
            if (this.WindowState != WindowState.Maximized)
            {
                width -= 10;
            }
            return width;
        }
        private void CreateTypeList()
        {
            /*typeList = new List<Tipos>();
            typeList.Add(new Tipos { _id = 1, nombre = "Bebidas",listaExtras = new List<Extras>() { new Extras {  nombre = "LimonBebidas",  precio = 0.20f }, new Extras {nombre = "NaranjaBebidas", precio = 0.30f } } });
            typeList.Add(new Tipos { _id = 2, nombre = "Hamburguesas", listaExtras = new List<Extras>() { new Extras {  nombre = "QuesoHamburguesa", precio = 0.50f }, new Extras { nombre = "BaconHamburguesa", precio = 0.70f }, new Extras { nombre = "HuevoHamburguesa", precio = 1f } } });
            typeList.Add(new Tipos { _id = 3, nombre = "Pescados", listaExtras = null });
            typeList.Add(new Tipos { _id = 4, nombre = "Pasta", listaExtras = null });
            typeList.Add(new Tipos { _id = 5, nombre = "Bocadillos", listaExtras = null });
            typeList.Add(new Tipos { _id = 6, nombre = "Combinados", listaExtras = null });
            typeList.Add(new Tipos { _id = 7, nombre = "Helados", listaExtras = null });
            typeList.Add(new Tipos { _id = 8, nombre = "Postres", listaExtras = null });
            typeList.Add(new Tipos { _id = 9, nombre = "Carnes", listaExtras = null });
            typeList.Add(new Tipos { _id = 10, nombre = "Asdsdasd", listaExtras = null });*/
            try
            {
                typeList = ControladorTipos.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }

        }
        private void CreateTypeButtonList()
        {
            typeButtonList = new List<Button>();

            foreach (Tipos tipo in typeList)
            {
                Button b = new Button();
                b.Width = minTypeButtonWidth;
                b.Height = typeButtonHeight;
                b.FontSize = typeButtonFontsize;
                b.Padding = new Thickness(5);
                b.Content = tipo.nombre;
                b.Click += btType_Click;
                b.Style = Application.Current.Resources["buttonTipos"] as Style;
                b.RenderTransformOrigin = new Point(0.5f, 0.5f);
                typeButtonList.Add(b);
            }

        }
        private void PutButtons()
        {
            double containerWidth = GetProductsGridWidth();
            numberOfTypeButtonsPerBlock = (int)(containerWidth / minTypeButtonWidth);
            if (numberOfTypeButtonsPerBlock > typeButtonList.Count)
            {
                numberOfTypeButtonsPerBlock = typeButtonList.Count;
            }
            if (numberOfTypeButtonsPerBlock == 0f)
                numberOfTypeButtonsPerBlock = 1;
            double fixedTypeButtonWidth = containerWidth / numberOfTypeButtonsPerBlock;

            for (int i = 0; i < typeButtonList.Count; i++)
            {

                typeButtonList[i].Width = fixedTypeButtonWidth;
                typeButtonList[i].FontSize = typeButtonFontsize;
                typeButtonList[i].Height = typeButtonHeight;
                btRight.Height = typeButtonHeight;
                btRight.Width = typeButtonHeight;
                btLeft.Height = typeButtonHeight;
                btLeft.Width = typeButtonHeight;
            }

            PutRoundOfTypeButtons();

        }

        private void PutRoundOfTypeButtons()
        {
            stackPanelTipos.Children.Clear();
            int firstIndex = currentRoundOfTypeButtons * numberOfTypeButtonsPerBlock;
            int lastIndex = firstIndex + numberOfTypeButtonsPerBlock - 1;
            if (lastIndex > typeButtonList.Count - 1)
            {
                /*int remainingButtons = lastIndex - (typeButtonList.Count - 1);
                
                firstIndex -= remainingButtons;
                if(firstIndex < 0)
                {
                    firstIndex = 0;
                }*/
                lastIndex = typeButtonList.Count - 1;

            }
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                stackPanelTipos.Children.Add(typeButtonList[i]);
            }

            CheckDisablingOfButtons();
        }

        private void CheckDisablingOfButtons()
        {
            if (currentRoundOfTypeButtons == 0)
            {
                btLeft.Visibility = Visibility.Hidden;
            }
            else
            {
                btLeft.Visibility = Visibility.Visible;
            }

            double numberOfRounds = Math.Ceiling(((double)typeButtonList.Count / (double)numberOfTypeButtonsPerBlock) - 1d);

            if (currentRoundOfTypeButtons == numberOfRounds)
            {
                btRight.Visibility = Visibility.Hidden;
            }
            else
            {
                btRight.Visibility = Visibility.Visible;
            }
        }

        private void btLeft_Click(object sender, RoutedEventArgs e)
        {
            currentRoundOfTypeButtons--;
            PutRoundOfTypeButtons();
        }

        private void btRight_Click(object sender, RoutedEventArgs e)
        {
            currentRoundOfTypeButtons++;
            PutRoundOfTypeButtons();
        }

        private void btType_Click(object sender, RoutedEventArgs e)
        {
            Button newBt = sender as Button;

            if (newBt != currentTypeButton)
            {
                currentTypeButton.Style = Application.Current.Resources["buttonTipos"] as Style;
                newBt.Style = Application.Current.Resources["buttonTiposSeleccionado"] as Style;
                currentTypeButton = newBt;
            }

            string type = newBt.Content as string;
            PutPanelsInGrid(type);
        }



        //Product Placing ------------------------------------------------------------------------------------------------------------------------------------------------
        private List<Productos> GetProductList()
        {
            /*List<Productos> pl = new List<Productos>();
            pl.Add(new Productos { _id = 1,precio = 2.50f, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png", stock = 30, especificaciones = new List<string>() {"Vegano","Picante"}, ingredientes = new List<string>() {"Cebolla","Pimiento","Pan","Carne","Lechuga","Huevo","TuMadre"} });
            pl.Add(new Productos { _id = 2, precio = 2.45f, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 3, precio = 2, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 4, precio = 1, tipo = "Hamburguesas", nombre = "Obama Prism", imagen = "https://cdn131.picsart.com/309150312204211.png" });
            pl.Add(new Productos { _id = 5, precio = 5, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://s3.eu-central-1.amazonaws.com/www.burgerking.com.mx/wp-content/uploads/sites/3/2021/02/24094446/1200x800_01_GuacamoleCrunchKing-1-1.png" });
            pl.Add(new Productos { _id = 6, precio = 6, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 7, precio = 2, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 8, precio = 4, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 9, precio = 7, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 10, precio = 9, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://recursos.marketingnews.es/files/884/79.png" });
            pl.Add(new Productos { _id = 11, precio = 2.5f, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://s3.eu-central-1.amazonaws.com/www.burgerking.com.mx/wp-content/uploads/sites/3/2021/02/24094446/1200x800_01_GuacamoleCrunchKing-1-1.png" });
            pl.Add(new Productos { _id = 12, precio = 2.60f, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 13, precio = 6f, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 14, precio = 2, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 15, precio = 2, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 16, precio = 2.4f, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://recursos.marketingnews.es/files/884/79.png" });
            pl.Add(new Productos { _id = 17, precio = 2.25f, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://s3.eu-central-1.amazonaws.com/www.burgerking.com.mx/wp-content/uploads/sites/3/2021/02/24094446/1200x800_01_GuacamoleCrunchKing-1-1.png" });
            pl.Add(new Productos { _id = 18, precio = 2.35f, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 19, precio = 2, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 20, precio = 2, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 21, precio = 2, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" });
            pl.Add(new Productos { _id = 22, precio = 2, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://recursos.marketingnews.es/files/884/79.png" });
            pl.Add(new Productos { _id = 23, precio = 2, tipo = "Hamburguesas", nombre = "BigMac", imagen = "https://s3.eu-central-1.amazonaws.com/www.burgerking.com.mx/wp-content/uploads/sites/3/2021/02/24094446/1200x800_01_GuacamoleCrunchKing-1-1.png" });
            pl.Add(new Productos { _id = 24, precio = 2, tipo = "Hamburguesas", nombre = "CheeseAndBacon", imagen = "https://www.chollosocial.com/media/data/2019/11/678gf34.png" });
            pl.Add(new Productos { _id = 25, precio = 3.50f, tipo = "Bebidas", nombre = "Agua 500ml", imagen = "https://mcdonalds.es/api/cms/images/mcdonalds-es/52cb6a85-a601-4993-8711-de79538c9a0c_producto-78.png?auto=compress,format" });
            pl.Add(new Productos { _id = 28, precio = 4.50f, tipo = "Bebidas", nombre = "Agua 2L", imagen = "https://cdn.metro-group.com/es/es_pim_150358001001_01.png?w=400&h=400&mode=pad" });
            pl.Add(new Productos { _id = 26, precio = 2, tipo = "Bebidas", nombre = "CocaCola 500ml", imagen = "https://www.toque.com.ar/sistema/uploads/571/articulos/591136208083.png" });
            pl.Add(new Productos { _id = 27, precio = 4, tipo = "Bebidas", nombre = "CocaCola 1L", imagen = "https://storage.googleapis.com/grandchef-apps/gc4386/images/products/6022003683260.png" });
            pl.Add(new Productos { _id = 29, precio = 2, tipo = "Bebidas", nombre = "Nestea 500ml", imagen = "https://sportavern.com/wp-content/uploads/productosImgs2Fbebida-nestea-33cl2.png" });
            pl.Add(new Productos { _id = 30, precio = 2, tipo = "Bebidas", nombre = "Fanta Naranja 500ml", imagen = "https://tiatota.com/wp-content/uploads/2020/12/Lata-de-Fanta-de-Naranja-1.png" });
            pl.Add(new Productos { _id = 31, precio = 100, tipo = "Hamburguesas", nombre = "McInvent", imagen = "asdasdasd" });*/

            List<Productos> pl = new List<Productos>();

            try
            {
                pl = ControladorProductos.GetAllFromApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }

            return pl;
        }

        private void CreatePanelList()
        {
            List<Productos> productList = GetProductList();
            panelList = new List<PanelProducto>();
            foreach (Productos p in productList)
            {
                PanelProducto panel = new PanelProducto();
                panel.producto = p;
                panel.boton.BorderThickness = new Thickness(0);
                panel.boton.Background = Brushes.Transparent;
                panel.boton.Click += ProductButtonClick;
                panel.boton.MouseRightButtonDown += ProductButtonRightClick;
                panel.boton.Style = Application.Current.Resources["clickableResizableButtonWithNoHoverResize"] as Style;
                //panel.boton.Foreground = Brushes.White;
                panel.boton.RenderTransformOrigin = new Point(0.5f, 0.5f);
                panel.stackPanel.Margin = new Thickness(40, 40, 40, 20);
                panel.imagen.Width = 250;
                panel.imagen.Height = 250;
                panel.tBlock.TextAlignment = TextAlignment.Center;
                panel.tBlock.FontSize = 25;
                panel.tBlock.Margin = new Thickness(0, 20, 0, 0);
                panel.tBlock.Text = p.nombre;
                panel.ConstruirPanel();
                panelList.Add(panel);
            }
        }

        private void BuildPanelGrid()
        {

            int maxCount = 0;
            int count = 0;
            foreach (Tipos tipo in typeList)
            {
                count = 0;
                foreach (PanelProducto panelProducto in panelList)
                {
                    if (panelProducto.producto.tipo == tipo.nombre)
                    {
                        count++;
                    }
                }
                if (count > maxCount)
                {
                    maxCount = count;
                }
            }

            int maxRows = (maxCount / 5) + 1;

            for (int i = 0; i < maxRows; i++)
            {
                gridPanelesProductos.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
        }

        private void PutPanelsInGrid(string type)
        {
            int count = 0;
            int row, column;
            gridPanelesProductos.Children.Clear();
            foreach (PanelProducto panel in panelList)
            {
                if (panel.producto.tipo == type && ProductPasesFilter(panel.producto))
                {
                    row = count / 5;
                    column = count % 5;
                    Grid.SetRow(panel.boton, row);
                    Grid.SetColumn(panel.boton, column);
                    gridPanelesProductos.Children.Add(panel.boton);
                    count++;
                }
            }
        }

        public void LoadImages()
        {
            foreach (PanelProducto p in panelList)
            {
                p.CargarImagen();
            }
        }

        private void ProductButtonClick(object sender, RoutedEventArgs e)
        {
            PanelProducto clickedPanel = GetPanelFromButton((Button)sender);
            int cantidad = tbContador.Text == "0" ? cantidad = 1 : cantidad = Convert.ToInt32(tbContador.Text);

            if (clickedPanel != null)
            {
                LineaPedido lp = new LineaPedido();
                lp.producto = clickedPanel.producto;
                lp.cantidad = cantidad;
                lp.CalcularCosteLinea();
                lp.InitializeLineasExtras(typeList);
                lp.anotaciones = "";
                CreateAndAddPanelLineaPedido(lp);
                tbContador.Text = "0";
                UpdateTotalTxt();
            }
        }

        private void ProductButtonRightClick(object sender, RoutedEventArgs e)
        {
            PanelProducto clickedPanel = GetPanelFromButton((Button)sender);
            ChangeState(state.Info);
            imgInfoProduct.Source = clickedPanel.imagen.Source;
            tbkInfoProductName.Text = clickedPanel.producto.nombre;
            tbkInfoPrecio.Text = String.Format("{0:0.00€}", clickedPanel.producto.precio);
            tbkInfoStock.Text = clickedPanel.producto.stock + "";
            tbkInfoIngredientes.Text = GetStringWithLineJumpFromStringList(clickedPanel.producto.ingredientes);
            tbkInfoEspecificaciones.Text = GetStringWithLineJumpFromStringList(clickedPanel.producto.especificaciones);

        }

        private string GetStringWithLineJumpFromStringList(List<string> list)
        {
            if (list == null)
            {
                return "";
            }
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i];
                if (i != list.Count - 1)
                {
                    res += Environment.NewLine;
                }
            }

            return res;
        }



        private PanelProducto GetPanelFromButton(Button b)
        {
            foreach (PanelProducto panel in panelList)
            {
                if (panel.boton == b)
                {
                    return panel;
                }
            }

            return null;
        }

        //Lineas de Pedido -------------------------------------------------------------------------------------------------------------------------------

        //No hacer caso
        private Vector CalculateInBetween(Vector v1, Vector v2, double proportion)
        {

            Vector v = new Vector();
            Vector d = v2 - v1;
            v = v1 + d * proportion;

            return v;
        }
        private double GetLinearInterpolation(double upperValue, double underValue, double proporcion)
        {
            return (upperValue - underValue) * proporcion + underValue;
        }

        private double GetLinearInterpolationProportion(double upperValue, double underValue, double value)
        {
            return (value - underValue) / (upperValue - underValue);
        }
        //Funcion para recalcular el fontsize de forma proporcional al tamaño de la pantalla igualando dos valores de estos
        private double GetFontSizeDependingOnScreenSize(double upperFontsize, double underFontsize, double upperScreenWidth, double underScreenWidth)
        {
            return GetLinearInterpolation(upperFontsize, underFontsize, GetLinearInterpolationProportion(upperScreenWidth, underScreenWidth, this.Width));
        }

        private double GetFontSizeOfOrderLine()
        {
            return GetFontSizeDependingOnScreenSize(25, 12, 1958, 1080);
        }

        private void ResizeAllOrderLines()
        {
            double fontsize = GetFontSizeOfOrderLine();
            foreach (PanelLineaPedido plp in linePanelList)
            {
                plp.tbPpal.FontSize = fontsize;
                plp.tbTotalDinero.FontSize = fontsize;
            }
            summaryListBox.UpdateLayout();
        }

        /*private Pedidos GetPedidoFromMesa()
        {
            //Prueba
            if(tableId == 1)
            {
                Pedidos p = new Pedidos();
                List<LineaPedido> lp = new List<LineaPedido>();
                lp.Add(new LineaPedido {anotaciones = "Sin Pepino,Sin Mayonesa",cantidad = 2,producto = new Productos{ _id = 1, precio = 2.50f, tipo = "Hamburguesas", nombre = "McAitana", imagen = "https://lapublicidad.net/wp-content/uploads/2017/11/BK_Web_bigking_XXL_500X540px.png" },lineasExtras = new List<LineaExtra> {new LineaExtra {cantidad = 1 , extra = new Extras() {_id = 20,nombre = "ChampiñonHamburguesa",nombreAMostrar="Champiñon",precio = 0.60f } }, new LineaExtra { cantidad = 0, extra = new Extras() { _id = 20, nombre = "Pepinillo", nombreAMostrar = "Pepinillo", precio = 0.70f } } } }) ;
                lp.Add(new LineaPedido { anotaciones = "", cantidad = 1, producto = new Productos { _id = 30, precio = 2, tipo = "Bebidas", nombre = "Fanta Naranja 500ml", imagen = "https://tiatota.com/wp-content/uploads/2020/12/Lata-de-Fanta-de-Naranja-1.png" }});
                p.lineasPedido = lp;
                return p;
            }
            else
            {
                return null;
            }
        }*/

        /*private void GetAndPlaceOrCreatePedido()
        {
            pedido = GetPedidoFromMesa();
            
            if (pedido == null)
            {
                pedido = new Pedidos();
                pedido.lineasPedido = new List<LineaPedido>();
            }
            else
            {
                CreateAndAddPanelsLineaPedidoFromPedido();
            }
        }*/


        private void CreateAndAddPanelsLineaPedidoFromPedido()
        {
            foreach (LineaPedido lp in pedido.lineasPedido)
            {
                lp.CalcularCosteLinea();
                CreateAndAddPanelLineaPedido(lp);
                UpdateTotalTxt();
            }
        }

        private void CreateAndAddPanelLineaPedido(LineaPedido lp)
        {
            PanelLineaPedido panelLinea = new PanelLineaPedido();
            panelLinea.lineaPedido = lp;
            panelLinea.listBoxItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            panelLinea.tbPpal.HorizontalAlignment = HorizontalAlignment.Left;
            double fontsize = GetFontSizeOfOrderLine();
            panelLinea.tbPpal.FontSize = fontsize;
            panelLinea.tbTotalDinero.HorizontalAlignment = HorizontalAlignment.Right;
            panelLinea.tbTotalDinero.FontSize = fontsize;
            panelLinea.PutInfo();
            panelLinea.ConstruirEstructuraPanel();
            summaryListBox.Items.Insert(0, panelLinea.listBoxItem);
            linePanelList.Insert(0, panelLinea);

            summaryListBox.ScrollIntoView(panelLinea.listBoxItem);
            //pedido.lineasPedido.Add(lp); L0 haremos al final mejor
        }

        private string GetEditAnnotationFromAnnotation(string annotation)
        {
            string[] annotations = annotation.Split(',');
            string editAnnotation = "";
            for (int i = 0; i < annotations.Length; i++)
            {
                editAnnotation += annotations[i] + Environment.NewLine;
            }
            return editAnnotation;
        }

        private string GetAnnotationFromEditAnnotation(string editAnnotation)
        {
            string[] lines = editAnnotation.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string annotation = "";
            string aux;
            bool primeroEncontrado = false;
            for (int i = 0; i < lines.Length; i++)
            {
                aux = lines[i].Trim();
                annotation += aux;

                if (aux != "" && !primeroEncontrado)
                    primeroEncontrado = true;

                if (i < lines.Length - 1)
                {
                    if (lines[i + 1].Trim() != "" && primeroEncontrado)
                    {
                        annotation += ",";
                    }
                }
            }
            return annotation;
        }
        private void UpdateTotalTxt()
        {
            float totalCost = 0f;
            foreach (PanelLineaPedido plp in linePanelList)
            {
                totalCost += plp.lineaPedido.costeLinea;
            }

            tbTotal.Text = String.Format("{0:0.00}€", totalCost);
        }



        private void btDeleteLine_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)summaryListBox.SelectedItem;
            PanelLineaPedido plp = GetLinePanelFromListItem(lbi);
            if (plp != null)
            {
                if (summaryListBox.SelectedIndex != 0)
                {
                    summaryListBox.SelectedIndex = summaryListBox.SelectedIndex - 1;
                }
                else
                {
                    ShowOrderLineEditionButtons(false);
                }

                summaryListBox.Items.Remove(lbi);
                linePanelList.Remove(plp);
                UpdateTotalTxt();
            }

        }

        private void btEditLine_Click(object sender, RoutedEventArgs e)
        {
            ShowOrderLineEditionButtons(false);
            ListBoxItem lbi = (ListBoxItem)summaryListBox.SelectedItem;
            linePanelBeingEdited = GetLinePanelFromListItem(lbi);
            PanelProducto pp = GetPanelProductoFromPanelLineaPedido(linePanelBeingEdited); // Para la imagen
            if (pp != null)
            {
                ChangeState(state.Edit);
                imgEditOrderLine.Source = pp.imagen.Source;
                tbkEditOrderLineName.Text = linePanelBeingEdited.lineaPedido.producto.nombre;
                tbEditOrderLineQuantity.Text = linePanelBeingEdited.lineaPedido.cantidad + "";
                tbeditOrderLineAnnotations.Text = GetEditAnnotationFromAnnotation(linePanelBeingEdited.lineaPedido.anotaciones);
                //Lo de los extras
                stackpanelExtras.Children.Clear();
                extraLinePanelList = new List<PanelLineaExtra>();
                CreateAndAddPanelesLineaExtra();
            }
        }

        private void btGuardarEdicion_Click(object sender, RoutedEventArgs e)
        {
            linePanelBeingEdited.lineaPedido.cantidad = Convert.ToInt32(tbEditOrderLineQuantity.Text);
            linePanelBeingEdited.lineaPedido.CalcularCosteLinea();
            linePanelBeingEdited.lineaPedido.anotaciones = GetAnnotationFromEditAnnotation(tbeditOrderLineAnnotations.Text);
            foreach (PanelLineaExtra ple in extraLinePanelList)
            {
                ple.lineaExtra.cantidad = Convert.ToInt32(ple.tbCantidad.Text); //las lineasextra de los paneles son las mismas que las lineasextra de la lineadepedido seleccionada
            }
            linePanelBeingEdited.PutInfo();
            UpdateTotalTxt();
            ChangeState(state.Normal);
        }

        private void btCancelarEdicion_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(state.Normal);
        }


        private PanelLineaPedido GetLinePanelFromListItem(ListBoxItem lbi)
        {
            foreach (PanelLineaPedido pLinea in linePanelList)
            {
                if (pLinea.listBoxItem == lbi)
                {
                    return pLinea;
                }
            }

            return null;
        }
        private PanelProducto GetPanelProductoFromPanelLineaPedido(PanelLineaPedido plp)
        {
            foreach (PanelProducto pp in panelList)
            {
                if (pp.producto._id == plp.lineaPedido.producto._id)
                {
                    return pp;
                }
            }

            return null;
        }

        private void summaryListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowOrderLineEditionButtons(true);
            if (windowState != state.Normal)
            {
                ChangeState(state.Normal);
            }
        }

        private void summaryListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Keyboard.FocusedElement != btDeleteLine && Keyboard.FocusedElement != btEditLine && Keyboard.FocusedElement != btDividirLinea)
            {
                ShowOrderLineEditionButtons(false);
            }

        }

        private void btMenosCantidad_Click(object sender, RoutedEventArgs e)
        {
            int number = Convert.ToInt32(tbEditOrderLineQuantity.Text);
            if (number > 1)
            {
                number--;
            }
            tbEditOrderLineQuantity.Text = number + "";
        }

        private void btMasCantidad_Click(object sender, RoutedEventArgs e)
        {
            int number = Convert.ToInt32(tbEditOrderLineQuantity.Text);
            if (number < 99)
            {
                number++;
            }
            tbEditOrderLineQuantity.Text = number + "";
        }


        private void ShowOrderLineEditionButtons(bool show)
        {
            if (show)
            {
                btEditLine.Visibility = Visibility.Visible;
                btDeleteLine.Visibility = Visibility.Visible;
                btDividirLinea.Visibility = Visibility.Visible;
            }
            else
            {
                btEditLine.Visibility = Visibility.Hidden;
                btDeleteLine.Visibility = Visibility.Hidden;
                btDividirLinea.Visibility = Visibility.Hidden;
            }
        }

        private void btResumirPedido_Click(object sender, RoutedEventArgs e)
        {
            ResumeLineaPedidoList();
        }

        private void btDividirLinea_Click(object sender, RoutedEventArgs e)
        {
            ShowOrderLineEditionButtons(false);
            ListBoxItem lbi = (ListBoxItem)summaryListBox.SelectedItem;
            PanelLineaPedido linePanel = GetLinePanelFromListItem(lbi);
            DivideLineaPedido(linePanel);
        }

        private void DivideLineaPedido(PanelLineaPedido linePanel)
        {
            if (linePanel.lineaPedido.cantidad > 1)
            {
                for (int i = 0; i < linePanel.lineaPedido.cantidad; i++)
                {
                    LineaPedido newLp = CloneLineaPedido(linePanel.lineaPedido);
                    newLp.cantidad = 1;
                    newLp.CalcularCosteLinea();
                    CreateAndAddPanelLineaPedido(newLp);
                }

                linePanelList.Remove(linePanel);
                summaryListBox.Items.Remove(linePanel.listBoxItem);
            }
        }

        private LineaPedido CloneLineaPedido(LineaPedido lp)
        {
            LineaPedido newlp = new LineaPedido();
            newlp.cantidad = lp.cantidad;
            newlp.anotaciones = lp.anotaciones;
            newlp.costeLinea = lp.costeLinea;
            newlp.producto = lp.producto;
            newlp.lineasExtras = new List<LineaExtra>();
            foreach (LineaExtra oldLineaExtra in lp.lineasExtras)
            {
                LineaExtra newLineaExtra = new LineaExtra();
                newLineaExtra.cantidad = oldLineaExtra.cantidad;
                newLineaExtra.extra = oldLineaExtra.extra;
                newlp.lineasExtras.Add(newLineaExtra);
            }

            return newlp;
        }

        private void ResumeLineaPedidoList()
        {
            for (int i = 0; i < linePanelList.Count - 1; i++)
            {
                for (int j = i + 1; j < linePanelList.Count; j++)
                {
                    if (AreLineasPedidoTheSame(linePanelList[i].lineaPedido, linePanelList[j].lineaPedido))
                    {
                        linePanelList[i].lineaPedido.cantidad += linePanelList[j].lineaPedido.cantidad;
                        linePanelList[i].lineaPedido.CalcularCosteLinea();
                        linePanelList[i].PutInfo();

                        linePanelList.RemoveAt(j);
                        summaryListBox.Items.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private bool AreLineasPedidoTheSame(LineaPedido lp1, LineaPedido lp2) //No cuenta la cantidad
        {

            if (lp1.producto._id == lp2.producto._id && lp1.anotaciones == lp2.anotaciones)
            {

                if (lp1.lineasExtras.Count != lp2.lineasExtras.Count)
                {
                    return false;
                }
                for (int i = 0; i < lp1.lineasExtras.Count; i++)
                {
                    if (lp1.lineasExtras[i].cantidad != lp2.lineasExtras[i].cantidad || lp1.lineasExtras[i].extra.nombre != lp2.lineasExtras[i].extra.nombre)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }


        }


        //EXTRAS --------------------------------------------------------------------------------------------------------------------------

        private void CreateAndAddPanelesLineaExtra()
        {
            if (linePanelBeingEdited.lineaPedido.lineasExtras != null)
            {
                foreach (LineaExtra le in linePanelBeingEdited.lineaPedido.lineasExtras)
                {
                    CreateAndAddPanelLineaExtra(le);
                }
            }

        }

        private void CreateAndAddPanelLineaExtra(LineaExtra le)
        {
            PanelLineaExtra ple = new PanelLineaExtra();

            ple.lineaExtra = le;
            ple.stackpanelSuperior.HorizontalAlignment = HorizontalAlignment.Center;
            ple.stackpanelSuperior.Margin = new Thickness(0, 0, 0, 40);
            ple.tbkNombre.Text = String.Format("Extra {0} ({1:0.00}€)", le.extra.nombre, le.extra.precio);
            ple.tbkNombre.FontSize = 20;
            ple.tbkNombre.HorizontalAlignment = HorizontalAlignment.Center;
            ple.stackpanelInferior.HorizontalAlignment = HorizontalAlignment.Center;
            ple.stackpanelInferior.Margin = new Thickness(0, 12, 0, 0);
            ple.stackpanelInferior.Orientation = Orientation.Horizontal;
            ple.btMenos.Style = Application.Current.Resources["clickableResizableButton"] as Style;
            ple.btMenos.Width = 40;
            ple.btMenos.BorderThickness = new Thickness(0);
            ple.btMenos.Margin = new Thickness(0, 0, 20, 0);
            ple.btMenos.Background = Brushes.Transparent;
            ple.btMenos.Click += btEditExtraMinus_Click;
            ple.btMas.Style = Application.Current.Resources["clickableResizableButton"] as Style;
            ple.btMas.Width = 40;
            ple.btMas.BorderThickness = new Thickness(0);
            ple.btMas.Margin = new Thickness(20, 0, 0, 0);
            ple.btMas.Background = Brushes.Transparent;
            ple.btMas.Click += btEditExtraPlus_Click;
            ple.tbCantidad.IsReadOnly = true;
            ple.tbCantidad.Width = 60;
            ple.tbCantidad.Text = le.cantidad + "";
            ple.tbCantidad.FontSize = 30;
            ple.tbCantidad.VerticalContentAlignment = VerticalAlignment.Center;
            ple.tbCantidad.HorizontalContentAlignment = HorizontalAlignment.Center;
            ple.ConstruirPanel();

            extraLinePanelList.Add(ple);
            stackpanelExtras.Children.Add(ple.stackpanelSuperior);
        }

        private void btEditExtraMinus_Click(object sender, RoutedEventArgs e)
        {
            PanelLineaExtra ple = GetPanelLineaExtraFromButton(sender as Button);

            if (ple.lineaExtra.cantidad > 0)
            {
                ple.lineaExtra.cantidad--;
                ple.tbCantidad.Text = ple.lineaExtra.cantidad + "";
            }
        }
        private void btEditExtraPlus_Click(object sender, RoutedEventArgs e)
        {
            PanelLineaExtra ple = GetPanelLineaExtraFromButton(sender as Button);
            if (ple.lineaExtra.cantidad < 10)
            {
                ple.lineaExtra.cantidad++;
                ple.tbCantidad.Text = ple.lineaExtra.cantidad + "";
            }
        }

        private PanelLineaExtra GetPanelLineaExtraFromButton(Button b)
        {
            foreach (PanelLineaExtra ple in extraLinePanelList)
            {
                if (ple.btMas == b || ple.btMenos == b)
                {
                    return ple;
                }
            }

            return null;
        }

        //Guardar Pedido ---------------------------------------------------------------------------------------------------------------------------------------------------

        private void btAtras_Click(object sender, RoutedEventArgs e)
        {
            SavePedido();
            WindowGestionMesas windowMesas = new WindowGestionMesas();
            windowMesas.Show();
            this.Close();
        }
        private void SavePedido()
        {
            pedido.lineasPedido.Clear();
            foreach (PanelLineaPedido panelLineaPedido in linePanelList)
            {
                pedido.lineasPedido.Add(panelLineaPedido.lineaPedido);
            }
            try
            {
                ControladorPedidos.UpdateInApi(pedido);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }

        }

        //Cobrar
        private void btCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (linePanelList.Count > 0)
            {
                ResumeLineaPedidoList();
                SavePedido();
                WindowCobrarPedido wcp = new WindowCobrarPedido(pedido, numMesa);
                wcp.Show();
                this.Close();
            }

        }


        //Calculadora ---------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void btNum_Click(object sender, RoutedEventArgs e)
        {
            if (tbContador.Text == "0")
                tbContador.Text = "";
            if (tbContador.Text.Length != 2)
                tbContador.Text += ((Button)sender).Content;
        }
        private void btNumCE_Click(object sender, RoutedEventArgs e)
        {
            tbContador.Text = "0";
        }

        private void btNumC_Click(object sender, RoutedEventArgs e)
        {
            tbContador.Text = tbContador.Text.Substring(0, tbContador.Text.Length - 1);
            if (tbContador.Text == "")
            {
                tbContador.Text = "0";
            }
        }

        //Filtro y Animacion

        private void InitializeAnimations()
        {
            borderFiltros.RenderTransform = new TranslateTransform() { X = -borderFiltros.Width * 2 }; // Para "evitar" (que no sea evidente) bug
            storyBoardFiltros = new Storyboard();
            durationGo = new Duration(new TimeSpan(0, 0, 0, 0, animDurationInMil));
            durationCome = new Duration(new TimeSpan(0, 0, 0, 0, animDurationInMil * 2));
            storyBoardFiltros.Duration = new Duration(new TimeSpan(0, 0, 0, 0, animDurationInMil));

            movemenFilterAnimation = new DoubleAnimation()
            {
                DecelerationRatio = 0.5d
            };

            Storyboard.SetTarget(movemenFilterAnimation, borderFiltros);
            Storyboard.SetTargetProperty(movemenFilterAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyBoardFiltros.Children.Add(movemenFilterAnimation);
            storyBoardFiltros.Completed += sbFiltros_Completed;

        }

        private void sbFiltros_Completed(object sender, EventArgs e)
        {
            if (filtroIsExpanded == false)
            {
                borderFiltros.Visibility = Visibility.Hidden;
            }
        }

        bool filtroIsExpanded = false;

        private void btExpandirFiltros_Click(object sender, RoutedEventArgs e)
        {
            borderFiltros.Visibility = Visibility.Visible;
            movemenFilterAnimation.From = -borderFiltros.Width;
            movemenFilterAnimation.To = 0;
            movemenFilterAnimation.Duration = durationGo;
            storyBoardFiltros.Begin();
            filtroIsExpanded = true;
        }

        private void btAtrasFiltros_Click(object sender, RoutedEventArgs e)
        {
            movemenFilterAnimation.From = 0;
            movemenFilterAnimation.To = -borderFiltros.Width * 2;
            movemenFilterAnimation.Duration = durationCome;
            storyBoardFiltros.Begin();
            filtroIsExpanded = false;

        }

        private void cbxFiltroClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                (sender as CheckBox).Opacity = 1f;
            }
            else
            {
                (sender as CheckBox).Opacity = 0.6f;
            }

            ColocarImagenesFiltro();
            PutPanelsInGrid(currentTypeButton.Content as string);
        }
        private void ColocarImagenesFiltro()
        {
            imgVegano.Visibility = cbxVegano.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            imgVegetariano.Visibility = cbxVegetariano.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            imgPescetariano.Visibility = cbxPescetariano.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            imgSinGluten.Visibility = cbxSinGluten.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            imgPicante.Visibility = cbxPicante.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void InitializeFilterCheckBoxes()
        {
            cbxFiltersList = new List<CheckBox>();
            cbxFiltersList.Add(cbxVegano);
            cbxFiltersList.Add(cbxVegetariano);
            cbxFiltersList.Add(cbxPescetariano);
            cbxFiltersList.Add(cbxSinGluten);
            cbxFiltersList.Add(cbxPicante);
        }

        private bool ProductPasesFilter(Productos producto)
        {
            foreach (CheckBox cbx in cbxFiltersList)
            {
                if (cbx.IsChecked == true)
                {
                    if (producto.especificaciones == null)
                    {
                        return false;
                    }

                    if (!producto.especificaciones.Contains(cbx.Content))
                    {
                        return false;
                    }
                }
            }

            return true;
        }



        //Eventos generales -----------------------------------------------------------------------------------------------------------------------------------------------------
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            currentRoundOfTypeButtons = 0;
            SetMinTypeButtonWidthAndFontsizeFromScreenWidth();
            PutButtons();
            ResizeAllOrderLines();
            borderFiltros.Width = this.ActualWidth * filterWidthProportion;

        }



        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                heightAux = this.Height;
                widthAux = this.Width;
                this.Height = SystemParameters.PrimaryScreenHeight;
                this.Width = SystemParameters.PrimaryScreenWidth;

                Window_SizeChanged(sender, null);

            }

            if (this.WindowState == WindowState.Normal)
            {
                this.Height = heightAux;
                this.Width = widthAux;

                Window_SizeChanged(sender, null);
            }

        }

        private void SetMinTypeButtonWidthAndFontsizeFromScreenWidth()
        {
            if (this.ActualWidth > 1400)
            {
                minTypeButtonWidth = 300;
                typeButtonFontsize = 24;
                typeButtonHeight = 70;

            }
            else
            {
                minTypeButtonWidth = 150;
                typeButtonFontsize = 14;
                typeButtonHeight = 41;
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            WindowGestionMesas wgm = new WindowGestionMesas();
            wgm.Show();
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                img_cuadrado.Source = new BitmapImage(new Uri(@"/Eros;component/Img/icons/cuadrado.png", UriKind.Relative));
                GlobalVariables.max = false;
            }
            else
            {
                WindowState = WindowState.Maximized;
                img_cuadrado.Source = new BitmapImage(new Uri(@"/Eros;component/Img/icons/cuadrado2.png", UriKind.Relative));
                GlobalVariables.max = true;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btBorrarPedido_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Estás seguro de que quieres borrar el pedido de la Mesa " + numMesa + " ?", "Atencion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ControladorPedidos.DeleteFromApi(pedido);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: \n" + "Pruebe que este conectado a la red e inténtalo más tarde.");
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                MessageBox.Show("El pedido de la Mesa " + numMesa + " ha sido borrado", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                WindowGestionMesas wgm = new WindowGestionMesas();
                this.Close();
                wgm.Show();
            }
        }
    }
}
