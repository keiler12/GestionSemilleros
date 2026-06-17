using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GestionSemilleros.Models.DAO;
using GestionSemilleros.Services;

namespace GestionSemilleros.Controllers
{
    public class AsistenteController : Controller
    {
        private SemillerosContext baseDatos = new SemillerosContext();
        private GeminiService servicioGemini = new GeminiService();

        [HttpPost]
        public async Task<JsonResult> Consultar(string pregunta)
        {
            if (Session["Rol"] == null)
                return Json(new { respuesta = "Debes iniciar sesión." });

            var rol = Session["Rol"].ToString();
            var nombre = Session["Nombre"].ToString();

            var totalSemilleros = baseDatos.Semilleros.Count();
            var totalUsuarios = baseDatos.Usuarios.Count();
            var totalProyectos = baseDatos.Proyectos.Count();
            var totalReuniones = baseDatos.Reuniones.Count();
            var totalEventos = baseDatos.Eventos.Count();

            var contexto = $@"Eres un asistente del Sistema de Gestión de Semilleros de Investigación.
            El usuario que te habla es {nombre}, con rol {rol}.
            Datos actuales del sistema: {totalSemilleros} semilleros, {totalUsuarios} usuarios, {totalProyectos} proyectos, {totalReuniones} reuniones, {totalEventos} eventos.
            Responde de forma breve y clara a la siguiente pregunta del usuario: {pregunta}";

            var respuesta = await servicioGemini.ObtenerRespuesta(contexto);
            return Json(new { respuesta });
        }
    }
}