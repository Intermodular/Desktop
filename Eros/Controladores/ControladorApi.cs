using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eros
{
    class ControladorApi
    {
        public static string GetHttp(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var endpoint = new Uri(url);
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                return json;
            }
        }

        public static string PostHttp(string url, StringContent content)
        {
            using (HttpClient client = new HttpClient())
            {
                var endpoint = new Uri(url);
                var result = client.PostAsync(endpoint, content).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        public static string PutHttp(string url, StringContent content)
        {
            using (HttpClient client = new HttpClient())
            {
                var endpoint = new Uri(url);
                var result = client.PutAsync(endpoint, content).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        public static string DeleteHttp(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var endpoint = new Uri(url);
                var result = client.DeleteAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }
    }
}
