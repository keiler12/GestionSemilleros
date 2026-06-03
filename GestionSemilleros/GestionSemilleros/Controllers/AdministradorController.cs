using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionSemilleros.Controllers
{
    public class AdministradorController : Controller
    {
        public ActionResult Index()
        {
            // Verifica si el usuario tiene el rol de "Administrador" en la sesión, si no es así, redirige al inicio de sesión
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            return RedirectToAction("Usuarios");
        }

        public ActionResult Usuarios()
        {
            // Verifica si el usuario tiene el rol de "Administrador" en la sesión, si no es así, redirige al inicio de sesión
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Usuarios";
            return View();
        }

        public ActionResult Semilleros()
        {

            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Semilleros";
            return View();
        }

        public ActionResult Proyectos()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Proyectos";
            return View();
        }

        public ActionResult Eventos()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Eventos";
            return View();
        }

        public ActionResult Reportes()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Reportes";
            return View();
        }

        public ActionResult Patrocinadores()
        {
            
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Patrocinadores";
            return View();
        }
    }
}