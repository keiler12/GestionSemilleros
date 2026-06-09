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
            var usuario = baseDatos.Usuarios
           .FirstOrDefault(registro => registro.CorreoUsuario == correo
                    && registro.ContraseñaUsuario == contrasena);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            if (usuario.EstadoUsuario == "Inactivo")
            {
                ViewBag.Error = "Tu cuenta está inactiva. Comunícate con el administrador.";
                return View();
            }

            Session["IdUsuario"] = usuario.IdUsuario;
            Session["Nombre"] = usuario.NombresUsuario;
            Session["Rol"] = usuario.RolUsuario;

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