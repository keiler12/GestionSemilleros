using GestionSemilleros.Models.DAO;
using GestionSemilleros.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace GestionSemilleros.Controllers
{
    public class LiderController : Controller
    {
        private SemillerosContext baseDatos = new SemillerosContext();

        // GET: Lider
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MiSemillero()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "MiSemillero";

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);
            var semillero = baseDatos.Semilleros.Find(usuario.IdSemillero);

            return View(semillero);
        }


        // ==================== INVESTIGADORES ====================

        public ActionResult Investigadores(string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Investigadores";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);
            var idSemillero = usuario.IdSemillero;

            if (filtroCampo == "Nombre")
                ViewBag.ValoresFiltro = baseDatos.Usuarios
                    .Where(u => u.IdSemillero == idSemillero && u.RolUsuario == "Investigador")
                    .Select(u => u.NombresUsuario).Distinct().ToList();
            else if (filtroCampo == "Estado")
                ViewBag.ValoresFiltro = new List<string> { "Activo", "Inactivo" };
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Usuarios
                .Where(u => u.IdSemillero == idSemillero && u.RolUsuario == "Investigador")
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Nombre")
                    lista = lista.Where(u => u.NombresUsuario.Contains(filtroValor));
                else if (filtroCampo == "Estado")
                    lista = lista.Where(u => u.EstadoUsuario == filtroValor);
            }

            return View(lista.ToList());
        }

        [HttpPost]
        public ActionResult RegistrarInvestigador(Usuario usuario)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var idUsuarioLider = (int)Session["IdUsuario"];
            var lider = baseDatos.Usuarios.Find(idUsuarioLider);

            usuario.IdSemillero = lider.IdSemillero;
            usuario.RolUsuario = "Investigador";
            usuario.EstadoUsuario = "Activo";

            baseDatos.Usuarios.Add(usuario);
            baseDatos.SaveChanges();
            return RedirectToAction("Investigadores");
        }

        [HttpPost]
        public ActionResult ModificarInvestigador(Usuario usuario)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Usuarios.Find(usuario.IdUsuario);
            if (registro != null && registro.RolUsuario == "Investigador")
            {
                registro.NombresUsuario = usuario.NombresUsuario;
                registro.CorreoUsuario = usuario.CorreoUsuario;
                registro.ContraseñaUsuario = usuario.ContraseñaUsuario;
                registro.TelefonoUsuario = usuario.TelefonoUsuario;
                registro.EdadUsuario = usuario.EdadUsuario;
                registro.GeneroUsuario = usuario.GeneroUsuario;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Investigadores");
        }

        [HttpPost]
        public ActionResult EliminarInvestigador(int id)
        {
            var registro = baseDatos.Usuarios
                .Include("Reuniones")
                .FirstOrDefault(u => u.IdUsuario == id);

            if (registro != null && registro.RolUsuario == "Investigador")
            {
                registro.Reuniones.Clear();
                baseDatos.Usuarios.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Investigadores");
        }

        [HttpPost]
        public ActionResult CambiarEstadoInvestigador(int id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Usuarios.Find(id);
            if (registro != null && registro.RolUsuario == "Investigador")
            {
                registro.EstadoUsuario = registro.EstadoUsuario == "Activo" ? "Inactivo" : "Activo";
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Investigadores");
        }

        // ==================== PROYECTOS ====================

        public ActionResult Proyectos(int? proyectoActivo, string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Proyectos";
            ViewBag.ProyectoActivo = proyectoActivo;
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;
            ViewBag.SemanasDisponibles = 0;
            ViewBag.SemanasTotal = 0;

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

            var listaProyectos = lista.ToList();

            if (proyectoActivo.HasValue)
            {
                var proyecto = listaProyectos.FirstOrDefault(p => p.IdProyecto == proyectoActivo.Value);
                if (proyecto != null)
                {
                    var diasProyecto = (proyecto.FechaFinProyecto - proyecto.FechaInicioProyecto).TotalDays;
                    var semanasTotal = (int)(diasProyecto / 7);
                    var semanasUsadas = proyecto.Fases.Sum(f => f.DuracionFase);
                    ViewBag.SemanasDisponibles = semanasTotal - semanasUsadas;
                    ViewBag.SemanasTotal = semanasTotal;

                    var diasDisponiblesPorFase = new Dictionary<int, int>();
                    foreach (var fase in proyecto.Fases)
                    {
                        var diasTotalesFase = fase.DuracionFase * 7;
                        var diasUsados = fase.Actividades.Sum(a => string.IsNullOrEmpty(a.DuracionActividad) ? 0 : int.Parse(a.DuracionActividad));
                        diasDisponiblesPorFase[fase.IdFase] = diasTotalesFase - diasUsados;
                    }
                    ViewBag.DiasDisponiblesPorFase = diasDisponiblesPorFase;
                }
            }

            return View(listaProyectos);
        }

        [HttpPost]
        public ActionResult RegistrarProyecto(Proyecto proyecto)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);
            proyecto.IdSemillero = (int)usuario.IdSemillero;

            var hoy = DateTime.Today;
            var maxFecha = hoy.AddMonths(12);

            if (proyecto.FechaInicioProyecto < hoy)
            {
                TempData["Error"] = "La fecha de inicio no puede ser en el pasado.";
                return RedirectToAction("Proyectos");
            }

            if (proyecto.FechaFinProyecto > maxFecha)
            {
                TempData["Error"] = "La fecha de fin no puede ser mayor a 12 meses desde hoy.";
                return RedirectToAction("Proyectos");
            }

            var diferenciaDias = (proyecto.FechaFinProyecto - proyecto.FechaInicioProyecto).TotalDays;
            if (diferenciaDias < 20)
            {
                TempData["Error"] = "La diferencia entre fechas debe ser mínimo 20 días.";
                return RedirectToAction("Proyectos");
            }

            if (proyecto.FechaFinProyecto <= proyecto.FechaInicioProyecto)
            {
                TempData["Error"] = "La fecha de fin debe ser mayor que la fecha de inicio.";
                return RedirectToAction("Proyectos");
            }

            baseDatos.Proyectos.Add(proyecto);
            baseDatos.SaveChanges();
            return RedirectToAction("Proyectos");
        }

        [HttpPost]
        public ActionResult ModificarProyecto(Proyecto proyecto)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var hoy = DateTime.Today;
            var maxFecha = hoy.AddMonths(12);

            if (proyecto.FechaInicioProyecto < hoy)
            {
                TempData["Error"] = "La fecha de inicio no puede ser en el pasado.";
                return RedirectToAction("Proyectos");
            }

            if (proyecto.FechaFinProyecto > maxFecha)
            {
                TempData["Error"] = "La fecha de fin no puede ser mayor a 12 meses desde hoy.";
                return RedirectToAction("Proyectos");
            }

            var diferenciaDias = (proyecto.FechaFinProyecto - proyecto.FechaInicioProyecto).TotalDays;
            if (diferenciaDias < 20)
            {
                TempData["Error"] = "La diferencia entre fechas debe ser mínimo 20 días.";
                return RedirectToAction("Proyectos");
            }

            if (proyecto.FechaFinProyecto <= proyecto.FechaInicioProyecto)
            {
                TempData["Error"] = "La fecha de fin debe ser mayor que la fecha de inicio.";
                return RedirectToAction("Proyectos");
            }

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);

            var registro = baseDatos.Proyectos
                .FirstOrDefault(p => p.IdProyecto == proyecto.IdProyecto && p.IdSemillero == usuario.IdSemillero);

            if (registro != null)
            {
                registro.TituloProyecto = proyecto.TituloProyecto;
                registro.ObjetivoProyecto = proyecto.ObjetivoProyecto;
                registro.DescripcionProyecto = proyecto.DescripcionProyecto;
                registro.FechaInicioProyecto = proyecto.FechaInicioProyecto;
                registro.FechaFinProyecto = proyecto.FechaFinProyecto;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos");
        }

        [HttpPost]
        public ActionResult EliminarProyecto(int id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);

            var registro = baseDatos.Proyectos
                .FirstOrDefault(p => p.IdProyecto == id && p.IdSemillero == usuario.IdSemillero);

            if (registro != null)
            {
                baseDatos.Proyectos.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos");
        }

        // ==================== FASES ====================

        [HttpPost]
        public ActionResult RegistrarFase(Fase fase, int proyectoId)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            fase.IdProyecto = proyectoId;
            baseDatos.Fases.Add(fase);
            baseDatos.SaveChanges();
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        [HttpPost]
        public ActionResult ModificarFase(Fase fase, int proyectoId)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var proyecto = baseDatos.Proyectos
                .Include("Fases")
                .FirstOrDefault(p => p.IdProyecto == proyectoId);

            if (proyecto != null)
            {
                var diasProyecto = (proyecto.FechaFinProyecto - proyecto.FechaInicioProyecto).TotalDays;
                var semanasTotal = (int)(diasProyecto / 7);
                var semanasUsadas = proyecto.Fases
                    .Where(f => f.IdFase != fase.IdFase)
                    .Sum(f => f.DuracionFase);
                var semanasDisponibles = semanasTotal - semanasUsadas;

                if (fase.DuracionFase > semanasDisponibles)
                {
                    TempData["Error"] = $"Solo hay {semanasDisponibles} semanas disponibles para este proyecto.";
                    return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
                }
            }

            var registro = baseDatos.Fases.Find(fase.IdFase);
            if (registro != null)
            {
                registro.NombreFase = fase.NombreFase;
                registro.DuracionFase = fase.DuracionFase;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        [HttpPost]
        public ActionResult EliminarFase(int id, int proyectoId)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Fases.Find(id);
            if (registro != null)
            {
                baseDatos.Fases.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        // ==================== ACTIVIDADES ====================

        [HttpPost]
        public ActionResult RegistrarActividad(Actividad actividad, int proyectoId)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var fase = baseDatos.Fases
                .Include("Actividades")
                .Include("Proyecto")
                .FirstOrDefault(f => f.IdFase == actividad.IdFase);

            if (fase != null)
            {
                var diasTotalesFase = fase.DuracionFase * 7;
                var diasUsados = fase.Actividades
                    .Sum(a => string.IsNullOrEmpty(a.DuracionActividad) ? 0 : int.Parse(a.DuracionActividad));
                var diasDisponibles = diasTotalesFase - diasUsados;

                if (int.Parse(actividad.DuracionActividad) > diasDisponibles)
                {
                    TempData["Error"] = $"Solo hay {diasDisponibles} días disponibles en esta fase.";
                    return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
                }

                actividad.FechaEntregaActividad = fase.Proyecto.FechaInicioProyecto
                    .AddDays(diasUsados + int.Parse(actividad.DuracionActividad));
            }

            baseDatos.Actividades.Add(actividad);
            baseDatos.SaveChanges();
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        [HttpPost]
        public ActionResult EliminarActividad(int id, int proyectoId)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Actividades.Find(id);
            if (registro != null)
            {
                baseDatos.Actividades.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        // ==================== REUNIONES ====================

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
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            ActualizarEstadosReuniones();

            ViewBag.MenuActivo = "Reuniones";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;

            var idUsuario = (int)Session["IdUsuario"];
            var usuario = baseDatos.Usuarios.Find(idUsuario);
            var idSemillero = usuario.IdSemillero;

            ViewBag.Usuarios = baseDatos.Usuarios
                .Where(u => u.IdSemillero == idSemillero && (u.RolUsuario == "Lider" || u.RolUsuario == "Investigador"))
                .ToList();

            if (filtroCampo == "Tipo")
                ViewBag.ValoresFiltro = baseDatos.Reuniones
                    .Where(r => r.Usuarios.Any(u => u.IdSemillero == idSemillero))
                    .Select(r => r.TipoReunion).Distinct().ToList();
            else if (filtroCampo == "Estado")
                ViewBag.ValoresFiltro = new List<string> { "Pendiente", "En curso", "Realizada", "Cancelada" };
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Reuniones
                .Include("Usuarios")
                .Where(r => r.Usuarios.Any(u => u.IdSemillero == idSemillero))
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Tipo")
                    lista = lista.Where(r => r.TipoReunion == filtroValor);
                else if (filtroCampo == "Estado")
                    lista = lista.Where(r => r.EstadoReunion == filtroValor);
            }

            var listaReuniones = lista.ToList();

            ViewBag.UsuariosPorReunion = listaReuniones
                .ToDictionary(r => r.IdReunion, r => r.Usuarios.Select(u => u.IdUsuario).ToList());

            return View(listaReuniones);
        }

        [HttpPost]
        public ActionResult RegistrarReunion(Reunion reunion, int[] UsuariosIds)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var hoy = DateTime.Today;
            var ahora = DateTime.Now;
            var fechaHoraReunion = reunion.FechaReunion.Add(reunion.HoraReunion);
            var maxFecha = hoy.AddMonths(2);

            if (reunion.FechaReunion < hoy)
            {
                TempData["Error"] = "No se puede programar en una fecha pasada.";
                return RedirectToAction("Reuniones");
            }

            if (reunion.FechaReunion.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["Error"] = "No se puede programar en domingo.";
                return RedirectToAction("Reuniones");
            }

            if (reunion.FechaReunion > maxFecha)
            {
                TempData["Error"] = "No se puede programar con más de 2 meses de anticipación.";
                return RedirectToAction("Reuniones");
            }

            if (reunion.HoraReunion < TimeSpan.FromHours(7) || reunion.HoraReunion > TimeSpan.FromHours(17))
            {
                TempData["Error"] = "La hora debe estar entre 07:00 y 17:00.";
                return RedirectToAction("Reuniones");
            }


            if (reunion.FechaReunion == hoy && fechaHoraReunion <= ahora.AddHours(2))
            {
                TempData["Error"] = "Si es hoy, la reunión debe tener mínimo 2 horas de anticipación.";
                return RedirectToAction("Reuniones");
            }

            var reunionesDelDia = baseDatos.Reuniones
                .Where(r => r.FechaReunion == reunion.FechaReunion && r.EstadoReunion != "Cancelada")
                .ToList();

            var horaFin = reunion.HoraReunion.Add(TimeSpan.FromMinutes(reunion.DuracionMinutos));

            foreach (var r in reunionesDelDia)
            {
                var horaFinExistente = r.HoraReunion.Add(TimeSpan.FromMinutes(r.DuracionMinutos));
                if (reunion.HoraReunion < horaFinExistente && horaFin > r.HoraReunion)
                {
                    TempData["Error"] = "Ya existe una reunión que se cruza con ese horario.";
                    return RedirectToAction("Reuniones");
                }
            }

            if (UsuariosIds == null || UsuariosIds.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar al menos un usuario.";
                return RedirectToAction("Reuniones");
            }

            var hayLider = baseDatos.Usuarios
                .Where(u => UsuariosIds.Contains(u.IdUsuario) && u.RolUsuario == "Lider")
                .Any();

            if (!hayLider)
            {
                TempData["Error"] = "La reunión debe tener al menos un líder encargado.";
                return RedirectToAction("Reuniones");
            }

            var hayInvestigador = baseDatos.Usuarios
                .Where(u => UsuariosIds.Contains(u.IdUsuario) && u.RolUsuario == "Investigador")
                .Any();

            if (!hayInvestigador)
            {
                TempData["Error"] = "La reunión debe tener al menos un investigador.";
                return RedirectToAction("Reuniones");
            }

            reunion.EstadoReunion = "Pendiente";
            baseDatos.Reuniones.Add(reunion);
            baseDatos.SaveChanges();

            var reunionGuardada = baseDatos.Reuniones
                .Include("Usuarios")
                .FirstOrDefault(r => r.IdReunion == reunion.IdReunion);

            foreach (var idUsuario in UsuariosIds)
            {
                var usuario = baseDatos.Usuarios.Find(idUsuario);
                if (usuario != null)
                    reunionGuardada.Usuarios.Add(usuario);
            }
            baseDatos.SaveChanges();

            return RedirectToAction("Reuniones");
        }

        [HttpPost]
        public ActionResult ModificarReunion(Reunion reunion, int[] UsuariosIds)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Reuniones
                .Include("Usuarios")
                .FirstOrDefault(r => r.IdReunion == reunion.IdReunion);

            if (registro != null && registro.EstadoReunion == "Pendiente")
            {
                registro.TipoReunion = reunion.TipoReunion;
                registro.MotivoReunion = reunion.MotivoReunion;
                registro.FechaReunion = reunion.FechaReunion;
                registro.HoraReunion = reunion.HoraReunion;
                registro.DuracionMinutos = reunion.DuracionMinutos;

                registro.Usuarios.Clear();

                if (UsuariosIds != null)
                {
                    foreach (var idUsuario in UsuariosIds)
                    {
                        var usuario = baseDatos.Usuarios.Find(idUsuario);
                        if (usuario != null)
                            registro.Usuarios.Add(usuario);
                    }
                }
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Reuniones");
        }

        [HttpPost]
        public ActionResult CancelarReunion(int id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Reuniones.Find(id);
            if (registro != null && registro.EstadoReunion == "Pendiente")
            {
                registro.EstadoReunion = "Cancelada";
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Reuniones");
        }

        [HttpPost]
        public ActionResult EliminarReunion(int id)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Lider")
                return RedirectToAction("Index", "Login");

            var registro = baseDatos.Reuniones
                .Include("Usuarios")
                .FirstOrDefault(r => r.IdReunion == id);

            if (registro != null)
            {
                registro.Usuarios.Clear();
                baseDatos.Reuniones.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Reuniones");
        }

       


    }




}

