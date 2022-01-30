using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;

namespace Eros
{
    class ControladorEmpleados
    {
        public static Empleado GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/empleado/id/" + id);
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(respuesta);
            return empleado;
        }

        public static List<Empleado> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/empleados");
            List<Empleado> listaEmpleados = JsonConvert.DeserializeObject<List<Empleado>>(respuesta);
            return listaEmpleados;
        }

        public static string PostToApi(Empleado empleado)
        {
            string jsonEmpleado = JsonConvert.SerializeObject(empleado);
            StringContent content = new StringContent(jsonEmpleado, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/empleado", content);
            return respuesta;
        }

        public static string UpdateInApi(Empleado empleado)
        {
            string jsonEmpleado = JsonConvert.SerializeObject(empleado);
            StringContent content = new StringContent(jsonEmpleado, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/empleado", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_empleado)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/empleado/id/" + id_empleado);
            return respuesta;
        }



        //Funciones de validación

        public async static Task<bool> DoesEmpleadoExistAsync(string user)
        {
            string respuesta = await ControladorApi.GetHttpAsync("http://localhost:8080/api/empleado/usuario/" + user);
            Thread.Sleep(2000);
            if (respuesta == "Not Found")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async static Task<bool> DoesEmpleadoExistAsync(string user, int exceptionId)
        {
            string respuesta = await ControladorApi.GetHttpAsync("http://localhost:8080/api/empleado/usuario/" + user);
            Thread.Sleep(2000);
            if (respuesta == "Not Found")
            {
                return false;
            }
            else
            {
                Empleado e = JsonConvert.DeserializeObject<Empleado>(respuesta);
                if (e._id == exceptionId)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool DoesEmpleadoExist(string user)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/empleado/usuario/" + user);
            if (respuesta == "Not Found")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool DoesEmpleadoExist(string user, int exceptionId)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/empleado/usuario/" + user);
            if (respuesta == "Not Found")
            {
                return false;
            }
            else
            {
                Empleado e = JsonConvert.DeserializeObject<Empleado>(respuesta);
                if (e._id == exceptionId)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }
}
