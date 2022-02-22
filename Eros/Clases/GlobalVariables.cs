using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Clases
{
    public class GlobalVariables
    {
        //User
        static public Empleado employee { get; set; }
        static public string username { get; set; }

        //Window
        static public double width { get; set; }
        static public double height { get; set; }
        static public double top { get; set; }
        static public double left { get; set; }
        static public Boolean max { get; set; }
    }
}
