using GestionSemilleros.Models.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionSemilleros.Controllers
{
    public class LoginController : Controller
    {
        private SemillerosContext baseDatos = new SemillerosContext();// Instancia del contexto de la base de datos

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string contrasena)// Acción que maneja el inicio de sesión del usuario
        {
            var usuario = baseDatos.Usuarios// Consulta a la tabla Usuarios para verificar las credenciales del usuario
                .FirstOrDefault(registro => registro.CorreoUsuario == correo// Verifica si el correo y la contraseña coinciden con algún registro en la base de datos
                             && registro.ContraseñaUsuario == contrasena);

            if (usuario == null)// Si no se encuentra un usuario con las credenciales proporcionadas, se muestra un mensaje de error, el null indica que no se encontró ningún registro que coincida con las credenciales proporcionadas
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }
            // Si se encuentra un usuario con las credenciales proporcionadas, se almacenan algunos datos del usuario en la sesión para su uso posterior
            Session["IdUsuario"] = usuario.IdUsuario;
            Session["Nombre"] = usuario.NombresUsuario;
            Session["Rol"] = usuario.RolUsuario;

            // Redirige al usuario a la página correspondiente según su rol
            if (usuario.RolUsuario == "Administrador")
                return RedirectToAction("Index", "Administrador");
            else if (usuario.RolUsuario == "Lider")
                return RedirectToAction("Index", "Lider");
            else
                return RedirectToAction("Index", "Investigador");
        }

        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}