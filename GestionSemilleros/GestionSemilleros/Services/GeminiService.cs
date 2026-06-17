using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace GestionSemilleros.Services
{
    public class GeminiService
    {
        public async Task<string> ObtenerRespuesta(string pregunta)
        {
            var apiKey = WebConfigurationManager.AppSettings["GroqApiKey"];
            var url = "https://api.groq.com/openai/v1/chat/completions";

            var cuerpoSolicitud = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = pregunta }
                }
            };

            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                var contenido = new StringContent(JsonConvert.SerializeObject(cuerpoSolicitud), Encoding.UTF8, "application/json");
                var respuesta = await cliente.PostAsync(url, contenido);
                var resultadoTexto = await respuesta.Content.ReadAsStringAsync();

                if (!respuesta.IsSuccessStatusCode)
                    return "ERROR: " + resultadoTexto;

                var resultado = JsonConvert.DeserializeObject<RespuestaGroq>(resultadoTexto);
                return resultado?.choices?[0]?.message?.content ?? "No se pudo obtener respuesta.";
            }
        }
    }

    public class RespuestaGroq
    {
        public Opcion[] choices { get; set; }
    }

    public class Opcion
    {
        public Mensaje message { get; set; }
    }

    public class Mensaje
    {
        public string content { get; set; }
    }
}