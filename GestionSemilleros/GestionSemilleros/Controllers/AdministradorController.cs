using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GestionSemilleros.Models.DAO;
using GestionSemilleros.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public ActionResult Semilleros(string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Semilleros";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;

            // Cargar valores según campo seleccionado
            if (filtroCampo == "Nombre")
                ViewBag.ValoresFiltro = baseDatos.Semilleros.Select(s => s.NombreSemillero).Distinct().ToList();
            else if (filtroCampo == "Linea")
                ViewBag.ValoresFiltro = baseDatos.Semilleros.Select(s => s.LineaSemillero).Distinct().ToList();
            else if (filtroCampo == "Enfoque")
                ViewBag.ValoresFiltro = baseDatos.Semilleros.Select(s => s.EnfoqueSemillero).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Semilleros.AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Nombre")
                    lista = lista.Where(s => s.NombreSemillero.Contains(filtroValor));
                else if (filtroCampo == "Linea")
                    lista = lista.Where(s => s.LineaSemillero.Contains(filtroValor));
                else if (filtroCampo == "Enfoque")
                    lista = lista.Where(s => s.EnfoqueSemillero.Contains(filtroValor));
            }

            return View(lista.ToList());
        }

        [HttpPost]
        public ActionResult RegistrarSemillero(Semillero semillero)
        {
            
            baseDatos.Semilleros.Add(semillero);// Agrega el nuevo semillero a la base de datos
            baseDatos.SaveChanges();// Guarda los cambios en la base de datos
            return RedirectToAction("Semilleros");// Redirige a la acción "Semilleros" para mostrar la lista actualizada
        }

        [HttpPost]
        public ActionResult ModificarSemillero(Semillero semillero)
        {
            var registro = baseDatos.Semilleros.Find(semillero.IdSemillero);// Busca el semillero existente en la base de datos por su ID
            if (registro != null)// Si se encuentra el semillero, actualiza sus propiedades con los nuevos valores proporcionados
            {
                registro.NombreSemillero = semillero.NombreSemillero;// Actualiza el nombre del semillero
                registro.LineaSemillero = semillero.LineaSemillero;// Actualiza la línea de investigación del semillero
                registro.EnfoqueSemillero = semillero.EnfoqueSemillero;// Actualiza el enfoque del semillero
                baseDatos.SaveChanges();// Guarda los cambios en la base de datos
            }
            return RedirectToAction("Semilleros");
        }

        public ActionResult EliminarSemillero(decimal id)
        {
            var registro = baseDatos.Semilleros.Find(id);// Busca el semillero existente en la base de datos por su ID
            if (registro != null)// Si se encuentra el semillero, lo elimina de la base de datos
            {
                baseDatos.Semilleros.Remove(registro);// Elimina el semillero de la base de datos
                baseDatos.SaveChanges();// Guarda los cambios en la base de datos
            }
            return RedirectToAction("Semilleros");
        }


        //USUARIOS 

        public ActionResult Usuarios(string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Usuarios";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;
            ViewBag.Semilleros = baseDatos.Semilleros.ToList();

            if (filtroCampo == "Nombre")
                ViewBag.ValoresFiltro = baseDatos.Usuarios.Select(u => u.NombresUsuario).Distinct().ToList();
            else if (filtroCampo == "Rol")
                ViewBag.ValoresFiltro = new List<string> { "Administrador", "Lider", "Investigador" };
            else if (filtroCampo == "Estado")
                ViewBag.ValoresFiltro = new List<string> { "Activo", "Inactivo" };
            else if (filtroCampo == "Semillero")
                ViewBag.ValoresFiltro = baseDatos.Semilleros.Select(s => s.NombreSemillero).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Usuarios.Include("Semillero").AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Nombre")
                    lista = lista.Where(u => u.NombresUsuario.Contains(filtroValor));
                else if (filtroCampo == "Rol")
                    lista = lista.Where(u => u.RolUsuario == filtroValor);
                else if (filtroCampo == "Estado")
                    lista = lista.Where(u => u.EstadoUsuario == filtroValor);
                else if (filtroCampo == "Semillero")
                    lista = lista.Where(u => u.Semillero.NombreSemillero == filtroValor);
            }

            return View(lista.ToList());
        }

        [HttpPost]
        public ActionResult RegistrarUsuario(Usuario usuario)
        {
            // Validar que el teléfono solo contenga números
            if (usuario.TelefonoUsuario.ToString().Any(c => !char.IsDigit(c)))
            {
                TempData["Error"] = "El teléfono solo puede contener números.";
                return RedirectToAction("Usuarios");
            }

            baseDatos.Usuarios.Add(usuario);
            baseDatos.SaveChanges();
            return RedirectToAction("Usuarios");
        }


        [HttpPost]
        public ActionResult CambiarEstado(int id)// Recibe el ID del usuario cuyo estado se desea cambiar
        {
            var registro = baseDatos.Usuarios.Find(id);// Busca el usuario existente en la base de datos por su ID
            if (registro != null)// Si se encuentra el usuario, cambia su estado de "Activo" a "Inactivo" o viceversa
            {
                registro.EstadoUsuario = registro.EstadoUsuario == "Activo" ? "Inactivo" : "Activo";// Cambia el estado del usuario utilizando un operador ternario para alternar entre "Activo" e "Inactivo"
                baseDatos.SaveChanges();// Guarda los cambios en la base de datos
            }
            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public ActionResult ModificarUsuario(Usuario usuario)
        {
            var registro = baseDatos.Usuarios.Find(usuario.IdUsuario);
            if (registro != null)
            {
                registro.NombresUsuario = usuario.NombresUsuario;
                registro.CorreoUsuario = usuario.CorreoUsuario;
                registro.ContraseñaUsuario = usuario.ContraseñaUsuario;
                registro.TelefonoUsuario = usuario.TelefonoUsuario;
                registro.RolUsuario = usuario.RolUsuario;
                registro.EdadUsuario = usuario.EdadUsuario;
                registro.GeneroUsuario = usuario.GeneroUsuario;
                registro.IdSemillero = usuario.IdSemillero;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Usuarios");
        }


        [HttpPost]
        public ActionResult EliminarUsuario(int id)// Recibe el ID del usuario que se desea eliminar de la base de datos
        {
            var registro = baseDatos.Usuarios.Find(id);// Busca el usuario existente en la base de datos por su ID
            if (registro != null)
            {
                baseDatos.Usuarios.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Usuarios");
        }


        // ==================== PROYECTOS ====================
        public ActionResult Proyectos(int? proyectoActivo, string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Proyectos";
            ViewBag.Semilleros = baseDatos.Semilleros.ToList();
            ViewBag.ProyectoActivo = proyectoActivo;
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;
            ViewBag.SemanasDisponibles = 0;
            ViewBag.SemanasTotal = 0;

            if (filtroCampo == "Titulo")
                ViewBag.ValoresFiltro = baseDatos.Proyectos.Select(p => p.TituloProyecto).Distinct().ToList();
            else if (filtroCampo == "Semillero")
                ViewBag.ValoresFiltro = baseDatos.Semilleros.Select(s => s.NombreSemillero).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Proyectos.Include("Fases.Actividades").AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Titulo")
                    lista = lista.Where(p => p.TituloProyecto.Contains(filtroValor));
                else if (filtroCampo == "Semillero")
                    lista = lista.Where(p => p.Semillero.NombreSemillero == filtroValor);
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
            var hoy = DateTime.Today;
            var maxFecha = hoy.AddMonths(12);

            if (proyecto.FechaInicioProyecto < hoy)
            {
                TempData["Error"] = "La fecha de inicio no puede ser en el pasado.";
                return RedirectToAction("Proyectos");
            }

            if (proyecto.FechaFinProyecto > maxFecha)
            {
                TempData["Error"] = "La fecha de fin no puede ser mayor a 2 meses desde hoy.";
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

            var registro = baseDatos.Proyectos.Find(proyecto.IdProyecto);
            if (registro != null)
            {
                registro.TituloProyecto = proyecto.TituloProyecto;
                registro.ObjetivoProyecto = proyecto.ObjetivoProyecto;
                registro.DescripcionProyecto = proyecto.DescripcionProyecto;
                registro.FechaInicioProyecto = proyecto.FechaInicioProyecto;
                registro.FechaFinProyecto = proyecto.FechaFinProyecto;
                registro.IdSemillero = proyecto.IdSemillero;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos");
        }

        [HttpPost]
        public ActionResult EliminarProyecto(int id)
        {
            var registro = baseDatos.Proyectos.Find(id);
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
            
            fase.IdProyecto = proyectoId;
            baseDatos.Fases.Add(fase);
            baseDatos.SaveChanges();
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        [HttpPost]
        public ActionResult ModificarFase(Fase fase, int proyectoId)
        {
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
            var registro = baseDatos.Actividades.Find(id);
            if (registro != null)
            {
                baseDatos.Actividades.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Proyectos", new { proyectoActivo = proyectoId });
        }

        // ==================== EVENTOS ====================

        public ActionResult Eventos(int? eventoActivo, string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Eventos";
            ViewBag.EventoActivo = eventoActivo;
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;
            ViewBag.Proyectos = baseDatos.Proyectos.ToList();
            ViewBag.Patrocinadores = baseDatos.Patrocinadores.ToList();

            if (filtroCampo == "Nombre")
                ViewBag.ValoresFiltro = baseDatos.Eventos.Select(e => e.NombreEvento).Distinct().ToList();
            else if (filtroCampo == "Tipo")
                ViewBag.ValoresFiltro = baseDatos.Eventos.Select(e => e.TipoEvento).Distinct().ToList();
            else if (filtroCampo == "Organizador")
                ViewBag.ValoresFiltro = baseDatos.Eventos.Select(e => e.OrganizadorEvento).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Eventos
                .Include("Proyectos")
                .Include("Patrocinadores")
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Nombre")
                    lista = lista.Where(e => e.NombreEvento.Contains(filtroValor));
                else if (filtroCampo == "Tipo")
                    lista = lista.Where(e => e.TipoEvento == filtroValor);
                else if (filtroCampo == "Organizador")
                    lista = lista.Where(e => e.OrganizadorEvento.Contains(filtroValor));
            }

            return View(lista.ToList());
        }

        [HttpPost]
        public ActionResult RegistrarEvento(Evento evento)
        {
           
            baseDatos.Eventos.Add(evento);
            baseDatos.SaveChanges();
            return RedirectToAction("Eventos");
        }

        [HttpPost]
        public ActionResult ModificarEvento(Evento evento)
        {
            var registro = baseDatos.Eventos.Find(evento.IdEvento);
            if (registro != null)
            {
                registro.NombreEvento = evento.NombreEvento;
                registro.TipoEvento = evento.TipoEvento;
                registro.LugarEvento = evento.LugarEvento;
                registro.FechaEvento = evento.FechaEvento;
                registro.OrganizadorEvento = evento.OrganizadorEvento;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Eventos");
        }

        [HttpPost]
        public ActionResult EliminarEvento(int id)
        {
            var registro = baseDatos.Eventos
                .Include("Proyectos")
                .Include("Patrocinadores")
                .FirstOrDefault(e => e.IdEvento == id);
            if (registro != null)
            {
                registro.Proyectos.Clear();
                registro.Patrocinadores.Clear();
                baseDatos.Eventos.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Eventos");
        }

        [HttpPost]
        public ActionResult AsociarProyecto(int eventoId, int? proyectoId)
        {
            if (proyectoId == null)
                return RedirectToAction("Eventos", new { eventoActivo = eventoId });

            var evento = baseDatos.Eventos
                .Include("Proyectos")
                .FirstOrDefault(e => e.IdEvento == eventoId);
            var proyecto = baseDatos.Proyectos.Find(proyectoId);
            if (evento != null && proyecto != null && !evento.Proyectos.Any(p => p.IdProyecto == proyectoId))
            {
                evento.Proyectos.Add(proyecto);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Eventos", new { eventoActivo = eventoId });
        }

        [HttpPost]
        public ActionResult DesasociarProyecto(int eventoId, int proyectoId)
        {
            var evento = baseDatos.Eventos
                .Include("Proyectos")
                .FirstOrDefault(e => e.IdEvento == eventoId);
            var proyecto = evento?.Proyectos.FirstOrDefault(p => p.IdProyecto == proyectoId);
            if (proyecto != null)
            {
                evento.Proyectos.Remove(proyecto);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Eventos", new { eventoActivo = eventoId});
        }

        [HttpPost]
        public ActionResult AsociarPatrocinador(int eventoId, int? patrocinadorId)
        {
            if (patrocinadorId == null)
                return RedirectToAction("Eventos", new { eventoActivo = eventoId });

            var evento = baseDatos.Eventos
                .Include("Patrocinadores")
                .FirstOrDefault(e => e.IdEvento == eventoId);
            var patrocinador = baseDatos.Patrocinadores.Find(patrocinadorId);
            if (evento != null && patrocinador != null && !evento.Patrocinadores.Any(p => p.IdPatrocinador == patrocinadorId))
            {
                evento.Patrocinadores.Add(patrocinador);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Eventos", new { eventoActivo = eventoId });
        }

        [HttpPost]
        public ActionResult DesasociarPatrocinador(int eventoId, int patrocinadorId)
        {
            var evento = baseDatos.Eventos
                .Include("Patrocinadores")
                .FirstOrDefault(e => e.IdEvento == eventoId);
            var patrocinador = evento?.Patrocinadores.FirstOrDefault(p => p.IdPatrocinador == patrocinadorId);
            if (patrocinador != null)
            {
                evento.Patrocinadores.Remove(patrocinador);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Eventos", new { eventoActivo = eventoId });
        }

        // ==================== PATROCINADORES ====================
        public ActionResult Patrocinadores(string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Patrocinadores";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;

            if (filtroCampo == "Nombre")
                ViewBag.ValoresFiltro = baseDatos.Patrocinadores.Select(p => p.NombrePatrocinador).Distinct().ToList();
            else if (filtroCampo == "Tipo")
                ViewBag.ValoresFiltro = baseDatos.Patrocinadores.Select(p => p.TipoPatrocinador).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Patrocinadores.AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Nombre")
                    lista = lista.Where(p => p.NombrePatrocinador.Contains(filtroValor));
                else if (filtroCampo == "Tipo")
                    lista = lista.Where(p => p.TipoPatrocinador == filtroValor);
            }

            return View(lista.ToList());
        }

        [HttpPost]
        public ActionResult RegistrarPatrocinador(Patrocinador patrocinador)
        {
          
            baseDatos.Patrocinadores.Add(patrocinador);
            baseDatos.SaveChanges();
            return RedirectToAction("Patrocinadores");
        }

        [HttpPost]
        public ActionResult ModificarPatrocinador(Patrocinador patrocinador)
        {
            var registro = baseDatos.Patrocinadores.Find(patrocinador.IdPatrocinador);
            if (registro != null)
            {
                registro.NombrePatrocinador = patrocinador.NombrePatrocinador;
                registro.TipoPatrocinador = patrocinador.TipoPatrocinador;
                registro.TelefonoPatrocinador = patrocinador.TelefonoPatrocinador;
                registro.CorreoPatrocinador = patrocinador.CorreoPatrocinador;
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Patrocinadores");
        }

        [HttpPost]
        public ActionResult EliminarPatrocinador(int id)
        {
            var registro = baseDatos.Patrocinadores
                .Include("Eventos")
                .FirstOrDefault(p => p.IdPatrocinador == id);
            if (registro != null)
            {
                registro.Eventos.Clear();
                baseDatos.Patrocinadores.Remove(registro);
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Patrocinadores");
        }

        // ==================== REUNIONES ====================
        public ActionResult Reuniones(string filtroCampo, string filtroValor)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ActualizarEstadosReuniones();

            ViewBag.MenuActivo = "Reuniones";
            ViewBag.FiltroCampo = filtroCampo;
            ViewBag.FiltroValor = filtroValor;
            ViewBag.Usuarios = baseDatos.Usuarios
                .Where(u => u.RolUsuario == "Lider" || u.RolUsuario == "Investigador")
                .ToList();

            if (filtroCampo == "Tipo")
                ViewBag.ValoresFiltro = baseDatos.Reuniones.Select(r => r.TipoReunion).Distinct().ToList();
            else if (filtroCampo == "Estado")
                ViewBag.ValoresFiltro = new List<string> { "Pendiente", "En curso", "Realizada", "Cancelada" };
            else if (filtroCampo == "Fecha")
                ViewBag.ValoresFiltro = baseDatos.Reuniones.Select(r => r.FechaReunion.ToString()).Distinct().ToList();
            else
                ViewBag.ValoresFiltro = new List<string>();

            var lista = baseDatos.Reuniones.Include("Usuarios").AsQueryable();

            if (!string.IsNullOrEmpty(filtroCampo) && !string.IsNullOrEmpty(filtroValor))
            {
                if (filtroCampo == "Tipo")
                    lista = lista.Where(r => r.TipoReunion == filtroValor);
                else if (filtroCampo == "Estado")
                    lista = lista.Where(r => r.EstadoReunion == filtroValor);
                else if (filtroCampo == "Fecha")
                    lista = lista.Where(r => r.FechaReunion.ToString() == filtroValor);
            }

            var listaReuniones = lista.ToList();
            ViewBag.UsuariosPorReunion = listaReuniones
                .ToDictionary(r => r.IdReunion, r => r.Usuarios.Select(u => u.IdUsuario).ToList());

            return View(listaReuniones);
        }

        [HttpPost]
        public ActionResult RegistrarReunion(Reunion reunion, int[] UsuariosIds)
        {
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

            if (reunion.HoraReunion == TimeSpan.FromHours(12))
            {
                TempData["Error"] = "No se puede programar a las 12:00 PM.";
                return RedirectToAction("Reuniones");
            }

            if (reunion.FechaReunion == hoy && fechaHoraReunion <= ahora.AddHours(2))
            {
                TempData["Error"] = "Si es hoy, la reunión debe tener mínimo 2 horas de anticipación.";
                return RedirectToAction("Reuniones");
            }

            // Verificar hora ocupada considerando duración
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

        [HttpPost]
        public ActionResult ModificarReunion(Reunion reunion, int[] UsuariosIds)
        {
            var registro = baseDatos.Reuniones
                .Include("Usuarios")
                .FirstOrDefault(r => r.IdReunion == reunion.IdReunion);

            if (registro != null && registro.EstadoReunion == "Pendiente")
            {
                registro.TipoReunion = reunion.TipoReunion;
                registro.MotivoReunion = reunion.MotivoReunion;
                registro.FechaReunion = reunion.FechaReunion;
                registro.HoraReunion = reunion.HoraReunion;

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
        public ActionResult EliminarReunion(int id)
        {
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

        [HttpPost]
        public ActionResult CancelarReunion(int id)
        {
            var registro = baseDatos.Reuniones.Find(id);
            if (registro != null && registro.EstadoReunion == "Pendiente")
            {
                registro.EstadoReunion = "Cancelada";
                baseDatos.SaveChanges();
            }
            return RedirectToAction("Reuniones");
        }

        public ActionResult ReporteSemilleros()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            var rutaReporte = Server.MapPath("~/Reportes/ReporteSemilleros.rpt");
            reporte.Load(rutaReporte);

            var dataSet = new ReporteSemillerosDataSet();
            var tablaDatos = dataSet.Tables[0];

            foreach (var semillero in baseDatos.Semilleros.ToList())
            {
                var fila = tablaDatos.NewRow();
                fila["IdSemillero "] = semillero.IdSemillero;
                fila["NombreSemillero"] = semillero.NombreSemillero;
                fila["LineaSemillero "] = semillero.LineaSemillero;
                fila["EnfoqueSemillero "] = semillero.EnfoqueSemillero;
                tablaDatos.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);

            Response.Buffer = false;
            var streamReporte = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(streamReporte, "application/pdf");
        }

        public ActionResult ReporteUsuarios()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReporteUsuarios.rpt"));

            var dataSet = new ReporteUsuariosDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var usuario in baseDatos.Usuarios.Include("Semillero").ToList())
            {
                var fila = tabla.NewRow();
                fila["IdUsuario"] = usuario.IdUsuario;
                fila["NombresUsuario"] = usuario.NombresUsuario;
                fila["CorreoUsuario"] = usuario.CorreoUsuario;
                fila["ContraseñaUsuario"] = usuario.ContraseñaUsuario;
                fila["TelefonoUsuario"] = usuario.TelefonoUsuario;
                fila["EdadUsuario"] = usuario.EdadUsuario;
                fila["GeneroUsuario"] = usuario.GeneroUsuario;
                fila["RolUsuario"] = usuario.RolUsuario;
                fila["EstadoUsuario"] = usuario.EstadoUsuario;
                fila["Semillero"] = usuario.Semillero != null ? usuario.Semillero.NombreSemillero : "Sin semillero";
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult ReporteProyectos()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReporteProyectos.rpt"));

            var dataSet = new ReportesproyectosDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var proyecto in baseDatos.Proyectos.Include("Semillero").ToList())
            {
                var fila = tabla.NewRow();
                fila["IdProyecto"] = proyecto.IdProyecto;
                fila["TituloProyecto"] = proyecto.TituloProyecto;
                fila["ObjetivoProyecto"] = proyecto.ObjetivoProyecto;
                fila["DescripcionProyecto"] = proyecto.DescripcionProyecto;
                fila["FechaInicio"] = proyecto.FechaInicioProyecto.ToString("yyyy-MM-dd");
                fila["FechaFin"] = proyecto.FechaFinProyecto.ToString("yyyy-MM-dd");
                fila["Semillero"] = proyecto.Semillero != null ? proyecto.Semillero.NombreSemillero : "Sin semillero";
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult ReporteFases()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReporteFases.rpt"));

            var dataSet = new ReporteFasesDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var fase in baseDatos.Fases.Include("Proyecto").ToList())
            {
                var fila = tabla.NewRow();
                fila["IdFase"] = fase.IdFase;
                fila["NombreFase"] = fase.NombreFase;
                fila["DuracionFase"] = fase.DuracionFase;
                fila["Proyecto"] = fase.Proyecto != null ? fase.Proyecto.TituloProyecto : "Sin proyecto";
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult ReporteActividades()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReporteActividades.rpt"));

            var dataSet = new ReporteActividadesDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var actividad in baseDatos.Actividades.Include("Fase.Proyecto").ToList())
            {
                var fila = tabla.NewRow();
                fila["IdActividad"] = actividad.IdActividad;
                fila["NombreActividad"] = actividad.NombreActividad;
                fila["DuracionActividad"] = actividad.DuracionActividad;
                fila["FechaEntrega"] = actividad.FechaEntregaActividad.ToString("yyyy-MM-dd");
                fila["Fase"] = actividad.Fase != null ? actividad.Fase.NombreFase : "Sin fase";
                fila["Proyecto"] = actividad.Fase != null && actividad.Fase.Proyecto != null ? actividad.Fase.Proyecto.TituloProyecto : "Sin proyecto";
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult ReporteEventos()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReportesEventos.rpt"));

            var dataSet = new ReportesEventosDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var eventos in baseDatos.Eventos.ToList())
            {
                var fila = tabla.NewRow();
                fila["IdEvento"] = eventos.IdEvento;
                fila["LugarEvento"] = eventos.LugarEvento;
                fila["NombreEvent"] = eventos.NombreEvento;
                fila["TipoEvento"] = eventos.TipoEvento;
                fila["FechaEvento"] = eventos.FechaEvento.ToString("yyyy-MM-dd");
                fila["OrganizadorEvento"] = eventos.OrganizadorEvento;
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult ReporteReuniones()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ActualizarEstadosReuniones();

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReporteReuniones.rpt"));

            var dataSet = new ReporteEventosDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var reunion in baseDatos.Reuniones.ToList())
            {
                var horaFin = reunion.HoraReunion.Add(TimeSpan.FromMinutes(reunion.DuracionMinutos));

                var fila = tabla.NewRow();
                fila["IdReunion"] = reunion.IdReunion;
                fila["TipoReunion"] = reunion.TipoReunion;
                fila["MotivoReunion"] = reunion.MotivoReunion;
                fila["FechaReunion"] = reunion.FechaReunion.ToString("yyyy-MM-dd");
                fila["HoraInicio"] = reunion.HoraReunion.ToString(@"hh\:mm");
                fila["HoraFin"] = horaFin.ToString(@"hh\:mm");
                fila["EstadoReunion"] = reunion.EstadoReunion;
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public ActionResult ReportePatrocinadores()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            var reporte = new ReportDocument();
            reporte.Load(Server.MapPath("~/Reportes/ReportePatrocinador.rpt"));

            var dataSet = new ReportePatrocinadoresDataSet();
            var tabla = dataSet.Tables[0];

            foreach (var patro in baseDatos.Patrocinadores.ToList())
            {
                var fila = tabla.NewRow();
                fila["IdPatrocinador"] = patro.IdPatrocinador;
                fila["NombrePatrocinador"] = patro.NombrePatrocinador;
                fila["TipoPatrocinador"] = patro.TipoPatrocinador;
                fila["TelefonoPatrocinador"] = patro.TelefonoPatrocinador;
                
                tabla.Rows.Add(fila);
            }

            reporte.SetDataSource(dataSet);
            Response.Buffer = false;
            var stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

    }

}