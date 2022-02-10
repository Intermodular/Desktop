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
    class ControladorTipos
    {
        public static Tipos GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/tipo/id/" + id);
            Tipos tipo = JsonConvert.DeserializeObject<Tipos>(respuesta);
            return tipo;
        }

        public static List<Tipos> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/tipos");
            List<Tipos> listaTipos = JsonConvert.DeserializeObject<List<Tipos>>(respuesta);
            return listaTipos;
        }

        public static string PostToApi(Tipos tipo)
        {
            string jsonTipo = JsonConvert.SerializeObject(tipo);
            StringContent content = new StringContent(jsonTipo, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/tipo", content);
            return respuesta;
        }

        public static string UpdateInApi(Tipos tipo)
        {
            string jsonTipo = JsonConvert.SerializeObject(tipo);
            StringContent content = new StringContent(jsonTipo, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/tipo", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_tipo)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/tipo/id/" + id_tipo);
            return respuesta;
        }
    }
}
