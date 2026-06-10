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

        public ActionResult Usuarios()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")// Verifica si el usuario no ha iniciado sesión o no tiene el rol de "Administrador"
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Usuarios";// Establece la variable ViewBag.MenuActivo para resaltar el menú de "Usuarios" en la vista
            ViewBag.Semilleros = baseDatos.Semilleros.ToList();// Obtiene la lista de semilleros de la base de datos y la asigna a ViewBag.Semilleros para su uso en la vista
            var listaUsuarios = baseDatos.Usuarios.Include("Semillero").ToList();// Obtiene la lista de usuarios de la base de datos, incluyendo la información del semillero asociado a cada usuario, y la asigna a la variable listaUsuarios
            return View(listaUsuarios);// Devuelve la vista "Usuarios" con la lista de usuarios obtenida de la base de datos
        }

        [HttpPost]
        public ActionResult RegistrarUsuario(Usuario usuario)// Recibe un objeto Usuario como parámetro, que contiene la información del nuevo usuario a registrar
        {
            usuario.EstadoUsuario = "Activo";// Establece el estado del nuevo usuario como "Activo" por defecto
            baseDatos.Usuarios.Add(usuario);// Agrega el nuevo usuario a la base de datos
            baseDatos.SaveChanges();// Guarda los cambios en la base de datos
            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public ActionResult ModificarUsuario(Usuario usuario)
        {
            var registro = baseDatos.Usuarios.Find(usuario.IdUsuario);// Busca el usuario existente en la base de datos por su ID
            if (registro != null)// Si se encuentra el usuario, actualiza sus propiedades con los nuevos valores proporcionados
            {
                // Actualiza las propiedades del usuario con los nuevos valores proporcionados
                registro.NombresUsuario = usuario.NombresUsuario;
                registro.CorreoUsuario = usuario.CorreoUsuario;
                registro.ContraseñaUsuario = usuario.ContraseñaUsuario;
                registro.TelefonoUsuario = usuario.TelefonoUsuario;
                registro.RolUsuario = usuario.RolUsuario;
                registro.EdadUsuario = usuario.EdadUsuario;
                registro.GeneroUsuario = usuario.GeneroUsuario;
                registro.IdSemillero = usuario.IdSemillero;
                baseDatos.SaveChanges();// Guarda los cambios en la base de datos
            }
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

        public ActionResult Proyectos(int? proyectoActivo)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Proyectos";
            ViewBag.Semilleros = baseDatos.Semilleros.ToList();
            ViewBag.ProyectoActivo = proyectoActivo;
            ViewBag.SemanasDisponibles = 0;
            ViewBag.SemanasTotal = 0;

            var listaProyectos = baseDatos.Proyectos
                .Include("Fases.Actividades")
                .ToList();

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
                }
            }
            // Calcular días disponibles por fase
            var diasDisponiblesPorFase = new Dictionary<int, int>();
            if (proyectoActivo.HasValue)
            {
                var proyecto = listaProyectos.FirstOrDefault(p => p.IdProyecto == proyectoActivo.Value);
                if (proyecto != null)
                {
                    foreach (var fase in proyecto.Fases)
                    {
                        var diasTotalesFase = fase.DuracionFase * 7;
                        var diasUsados = fase.Actividades.Sum(a => string.IsNullOrEmpty(a.DuracionActividad) ? 0 : int.Parse(a.DuracionActividad));
                        diasDisponiblesPorFase[fase.IdFase] = diasTotalesFase - diasUsados;
                    }
                }
            }
            ViewBag.DiasDisponiblesPorFase = diasDisponiblesPorFase;

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

        public ActionResult Eventos(int? eventoActivo)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Eventos";
            ViewBag.EventoActivo = eventoActivo;
            ViewBag.Proyectos = baseDatos.Proyectos.ToList();
            ViewBag.Patrocinadores = baseDatos.Patrocinadores.ToList();
            var listaEventos = baseDatos.Eventos
                .Include("Proyectos")
                .Include("Patrocinadores")
                .ToList();
            return View(listaEventos);
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

        public ActionResult Patrocinadores()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Patrocinadores";
            var listaPatrocinadores = baseDatos.Patrocinadores.ToList();
            return View(listaPatrocinadores);
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

        public ActionResult Reuniones()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
                return RedirectToAction("Index", "Login");

            ViewBag.MenuActivo = "Reuniones";
            ViewBag.Usuarios = baseDatos.Usuarios
             .Where(u => u.RolUsuario == "Lider" || u.RolUsuario == "Investigador")
             .ToList();
            var listaReuniones = baseDatos.Reuniones
                .Include("Usuarios")
                .ToList();
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

            bool horaOcupada = baseDatos.Reuniones.Any(r =>
                r.FechaReunion == reunion.FechaReunion &&
                r.HoraReunion == reunion.HoraReunion &&
                r.EstadoReunion != "Cancelada");

            if (horaOcupada)
            {
                TempData["Error"] = "Ya existe una reunión programada a esa hora ese día.";
                return RedirectToAction("Reuniones");
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

            // Verificar que haya al menos un investigador
            var hayInvestigador = baseDatos.Usuarios
                .Where(u => UsuariosIds.Contains(u.IdUsuario) && u.RolUsuario == "Investigador")
                .Any();

            if (!hayInvestigador)
            {
                TempData["Error"] = "La reunión debe tener al menos un investigador además del líder.";
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
        public ActionResult ModificarReunion(Reunion reunion)
        {
            var registro = baseDatos.Reuniones.Find(reunion.IdReunion);
            if (registro != null)
            {
                registro.TipoReunion = reunion.TipoReunion;
                registro.HoraReunion = reunion.HoraReunion;
                registro.MotivoReunion = reunion.MotivoReunion;
                registro.FechaReunion = reunion.FechaReunion;
               
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
    }

}