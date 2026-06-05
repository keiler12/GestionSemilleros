using GestionSemilleros.Models.DAO;
using GestionSemilleros.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionSemilleros.Controllers
{
    public class AdministradorController : Controller
    {
        private SemillerosContext baseDatos = new SemillerosContext();

        public ActionResult Index()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Inicio";
            return View();
        }

        //  SEMILLEROS 

        public ActionResult Semilleros()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Semilleros";
            var listaSemilleros = baseDatos.Semilleros.ToList();
            return View(listaSemilleros);
        }

        [HttpPost]
        public ActionResult RegistrarSemillero(Semillero semillero)
        {
            int ultimoId = baseDatos.Semilleros.Any()
                ? baseDatos.Semilleros.Max(registro => registro.IdSemillero)
                : 99;

            semillero.IdSemillero = ultimoId + 1;
            baseDatos.Semilleros.Add(semillero);
            baseDatos.SaveChanges();
            return RedirectToAction("Semilleros");
        }

        [HttpPost]
        public ActionResult ModificarSemillero(Semillero semillero)
        {
            var registro = baseDatos.Semilleros.Find(semillero.IdSemillero);
            if (registro != null)
            {
                registro.NombreSemillero = semillero.NombreSemillero;
                registro.LineaSemillero = semillero.LineaSemillero;
                registro.EnfoqueSemillero = semillero.EnfoqueSemillero;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Semilleros");
        }

        public ActionResult EliminarSemillero(decimal id)
        {
            var registro = baseDatos.Semilleros.Find(id);
            if (registro != null)
            {
                baseDatos.Semilleros.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Semilleros");
        }
    }
}