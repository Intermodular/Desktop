using Eros.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Controladores
{
    class ControladorNominas
    {
        public static Nominas GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/nomina/id/" + id);
            Nominas nomina = JsonConvert.DeserializeObject<Nominas>(respuesta);
            return nomina;
        }

        public static List<Nominas> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/nominas");
            List<Nominas> listaNominas = JsonConvert.DeserializeObject<List<Nominas>>(respuesta);
            return listaNominas;
        }

        public static string PostToApi(Nominas nomina)
        {
            string jsonNominas = JsonConvert.SerializeObject(nomina);
            StringContent content = new StringContent(jsonNominas, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/nomina", content);
            return respuesta;
        }

        public static string UpdateInApi(Nominas nomina)
        {
            string jsonNomina = JsonConvert.SerializeObject(nomina);
            StringContent content = new StringContent(jsonNomina, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/nomina", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_nomina)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/nomina/id/" + id_nomina);
            return respuesta;
        }
    }
}
