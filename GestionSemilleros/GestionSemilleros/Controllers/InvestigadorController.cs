using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using GestionSemilleros.Models.DAO;
using GestionSemilleros.Models.Entidades;

namespace GestionSemilleros.Controllers
{
    public class InvestigadorController : Controller
    {
        private SemillerosContext baseDatos = new SemillerosContext();

        public ActionResult Index()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Investigador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Inicio";
            return View();
        }

        public ActionResult MiSemillero()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Investigador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "MiSemillero";

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);
            var semillero = baseDatos.Semilleros.Find(usuario.IdSemillero);

            return View(semillero);
        }

        public ActionResult Proyectos(int? proyectoActivo, string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Investigador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Proyectos";
            ViewBag.ProyectoActivo = proyectoActivo;
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);
            var idSemillero = usuario.IdSemillero;

            if (filtroCampo == "Titulo")
                ViewBag.ValoresFiltro = baseDatos.Proyectos
                    .Where(p => p.IdSemillero == idSemillero)
                    .Select(p => p.TituloProyecto).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Proyectos
                .Include("Fases.Actividades")
                .Where(p => p.IdSemillero == idSemillero)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Titulo")
                    lista = lista.Where(p => p.TituloProyecto.Contains(filtroValor));
            }

            return View(lista.ToList());
        }

        private void ActualizarEstadosReuniones()
        {
            var ahora = DateTime.Now;
            var reuniones = baseDatos.Reuniones
                .Where(r => r.EstadoReunion != "Cancelada")
                .ToList();

            foreach (var reunion in reuniones)
            {
                var horaInicio = reunion.FechaReunion.Add(reunion.HoraReunion);
                var horaFin = horaInicio.AddMinutes(reunion.DuracionMinutos);

                if (ahora < horaInicio)
                    reunion.EstadoReunion = "Pendiente";
                else if (ahora >= horaInicio && ahora < horaFin)
                    reunion.EstadoReunion = "En curso";
                else if (ahora >= horaFin)
                    reunion.EstadoReunion = "Realizada";
            }

            baseDatos.SaveChanges();
        }

        public ActionResult Reuniones(string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Investigador")
                return RedirectToAction("Index", "Login");

            ActualizarEstadosReuniones();

            ViewBag.MenuActivo = "Reuniones";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;

            var idUsuario = (int)Session["IdUsuario"];

            if (filtroCampo == "Tipo")
                ViewBag.ValoresFiltro = baseDatos.Reuniones
                    .Where(r => r.Usuarios.Any(u => u.IdUsuario == idUsuario))
                    .Select(r => r.TipoReunion).Distinct().ToList();
            else if (filtroCampo == "Estado")
                ViewBag.ValoresFiltro = new List<string> { "Pendiente", "En curso", "Realizada", "Cancelada" };
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Reuniones
                .Include("Usuarios")
                .Where(r => r.Usuarios.Any(u => u.IdUsuario == idUsuario))
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Tipo")
                    lista = lista.Where(r => r.TipoReunion == filtroValor);
                else if (filtroCampo == "Estado")
                    lista = lista.Where(r => r.EstadoReunion == filtroValor);
            }

            return View(lista.ToList());
        }


    }
}