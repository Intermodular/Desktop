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
    class ControladorZonas
    {
        public static Zonas GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/zona/id/" + id);
            Zonas zona = JsonConvert.DeserializeObject<Zonas>(respuesta);
            return zona;
        }

        public static List<Zonas> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/zonas");
            List<Zonas> listaZonas = JsonConvert.DeserializeObject<List<Zonas>>(respuesta);
            return listaZonas;
        }

        public static string PostToApi(Zonas zona)
        {
            string jsonZona = JsonConvert.SerializeObject(zona);
            StringContent content = new StringContent(jsonZona, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/zona", content);
            return respuesta;
        }

        public static string UpdateInApi(Zonas zona)
        {
            string jsonZona = JsonConvert.SerializeObject(zona);
            StringContent content = new StringContent(jsonZona, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/zona", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_zona)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/zona/id/" + id_zona);
            return respuesta;
        }
    }
}
