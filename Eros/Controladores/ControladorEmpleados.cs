using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;


namespace Eros
{
    class ControladorEmpleados
    {
        public static Empleado GetFromApi(int id)
        {
            //string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/empleado/id/" + id);
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/prueba");
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(respuesta);
            return empleado;
        }

        public static List<Empleado> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/empleados");
            //string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/pruebaLista");
            List<Empleado> listaEmpleados = JsonConvert.DeserializeObject<List<Empleado>>(respuesta);
            return listaEmpleados;
        }

        public static string PostToApi(Empleado empleado)
        {
            string jsonEmpleado = JsonConvert.SerializeObject(empleado);
            StringContent content = new StringContent(jsonEmpleado, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/empleado", content);
            //string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/prueba", content);
            return respuesta;
        }

        public static string UpdateInApi(Empleado empleado)
        {
            string jsonEmpleado = JsonConvert.SerializeObject(empleado);
            StringContent content = new StringContent(jsonEmpleado, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/empleado", content);
            //string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/prueba", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_empleado)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/empleado/id/" + id_empleado);
            //string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/prueba/id/" + empleado._id);
            return respuesta;
        }
    }
}
