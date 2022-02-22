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
using Eros.Modelos;

namespace Eros.Administrador.UtilWindows
{
    /// <summary>
    /// Interaction logic for DialogWindowInfoNomina.xaml
    /// </summary>
    public partial class DialogWindowInfoNomina : Window
    {
        public DialogWindowInfoNomina(Nominas nomina)
        {
            InitializeComponent();
            tbkFechas.Text = "Del " + nomina.fechaInicio + " al " + nomina.fechaFinal;
            tbkNombreYAp.Text = nomina.nombreEmpleado + " " + nomina.apellidoEmpleado;
            tbkDni.Text = nomina.dniEmpleado;
            tbkDireccion.Text = nomina.direccionEmpleado;
            tbkHoras.Text = "Horas Corrientes: " + nomina.horasCorrientes;
            tbkHorasEx.Text = "Horas Extra: " + nomina.horasExtras;
            tbkPrecioHora.Text = String.Format("€/H.C: {0:0.00}€", nomina.precioHoraCorriente);
            tbkPrecioHoraEx.Text = String.Format("€/H.E: {0:0.00}€", nomina.precioHoraExtra);
            tbkRemuneracion.Text = String.Format("{0:0.00}€", nomina.remuneracionTotal);

        }
    }
}
