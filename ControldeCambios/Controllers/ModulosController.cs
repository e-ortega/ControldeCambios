﻿using ControldeCambios.App_Start;
using ControldeCambios.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ControldeCambios.Controllers
{
    public class ModulosController : ToastrController
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        private void updateSprintPoints(string Proyecto, int sprint)
        {

            var sprint_actual = baseDatos.Sprints.Find(Proyecto, sprint);
            var hoy = DateTime.Today < sprint_actual.fechaInicio ? sprint_actual.fechaInicio : DateTime.Today;

            var progreso_hoy = baseDatos.Progreso_Sprint.Where(x => x.sprintProyecto == Proyecto && x.sprintNumero == sprint &&
            x.fecha.Year == hoy.Year
            && x.fecha.Month == hoy.Month
            && x.fecha.Day == hoy.Day
            ).FirstOrDefault();

            var modificado = true;

            if (progreso_hoy == default(Progreso_Sprint))
            {
                progreso_hoy = new Progreso_Sprint();
                progreso_hoy.fecha = hoy;
                progreso_hoy.sprintNumero = sprint;
                progreso_hoy.sprintProyecto = Proyecto;
                modificado = false;
            }

            var puntos = sprint_actual.Sprint_Modulos
                .Select(m => baseDatos.Modulos
                    .Find(m.proyecto, m.modulo).Requerimientos
                        .Select(x => x.estado == "Finalizado" ? 0 : (x.esfuerzo ?? 0))
                        .Sum())
                .Sum();
            progreso_hoy.puntos = puntos;
            if (modificado)
            {
                baseDatos.Entry(progreso_hoy).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                baseDatos.Progreso_Sprint.Add(progreso_hoy);
            }

            baseDatos.SaveChanges();
        }

        /// <summary>
        /// Se utiliza para revisar que el rol del usuario que intenta acceder a alguna
        /// caracteristica tenga los permisos correspondientes.
        /// </summary>
        /// <param name="permiso"> Nombre del permiso que se intenta revisar.</param>
        /// <returns>Pagina de Index</returns>
        private bool revisarPermisos(string permiso)
        {
            String userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var permisoID = baseDatos.Permisos.Where(m => m.nombre == permiso).First().codigo;
            var listaRoles = baseDatos.Rol_Permisos.Where(m => m.permiso == permisoID).ToList().Select(n => n.rol);
            bool userRol = listaRoles.Contains(rol.RoleId);

            return userRol;
        }

        // GET: Modulos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Modulos/Crear
        public ActionResult Crear(string proyecto)
        {
            if (!revisarPermisos("Modificar Proyectos"))
            {
                //Despliega mensaje en caso de no poder crear un proyecto
                this.AddToastMessage("Acceso Denegado", "No tienes permiso para crear módulos!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }
            if (String.IsNullOrEmpty(proyecto))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (baseDatos.Proyectos.Find(proyecto) != null)
            {
                var model = new ModulosModel();
                model.proyecto = proyecto;
                ViewBag.requerimientos = new MultiSelectList(baseDatos.Requerimientos.Where(m => m.proyecto == proyecto).ToList(), "id", "nombre");
                return View(model);
            } else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Funcionalidad para crear Módulos.
        /// </summary>
        /// <param name="model"> Modelo con la informacion del Módulo a crear.</param>
        /// <returns>Pagina de Index</returns>
        // POST: /Modulos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ModulosModel model)
        {
            if (ModelState.IsValid)
            {
                var modulo = new Modulo();
                modulo.proyecto = model.proyecto;
                modulo.numero = Int32.Parse(model.numero);
                if (baseDatos.Modulos.Where(m => m.numero == modulo.numero && m.proyecto == modulo.proyecto).Count() > 0)
                {
                    this.AddToastMessage("Módulo Ya Existe", "Ya existe un módulo con ese número para ese proyecto.", ToastType.Error);
                    return View(model);
                } else
                {
                    modulo.nombre = model.nombre;
                    baseDatos.Modulos.Add(modulo);
                    if (model.requerimientos != null && model.requerimientos.Count() > 0)
                    {
                        foreach (var req in model.requerimientos)
                        {
                            var requerimiento = baseDatos.Requerimientos.Find(Int32.Parse(req));
                            requerimiento.modulo = modulo.numero;
                            baseDatos.Entry(requerimiento).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    baseDatos.SaveChanges();
                    this.AddToastMessage("Módulo Creado", "El módulo " + model.nombre + " se ha creado correctamente.", ToastType.Success);
                    return RedirectToAction("Crear", "Modulos", new { proyecto = model.proyecto});
                }
            }
            return View(model);
        }

        // GET: Modulos/Detalles
        public ActionResult Detalles(string proyecto, string numero)
        {
            if (!revisarPermisos("Modificar Proyectos"))
            {
                //Despliega mensaje en caso de no poder crear un proyecto
                this.AddToastMessage("Acceso Denegado", "No tienes permiso para modificar módulos!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }
            if (String.IsNullOrEmpty(proyecto) || String.IsNullOrEmpty(numero))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modulo = new Modulo();
            modulo.proyecto = proyecto;
            modulo.numero = Int32.Parse(numero);
            var listaDeModulos = baseDatos.Modulos.Where(m => m.numero == modulo.numero && m.proyecto == modulo.proyecto);
            if (listaDeModulos.Count() > 0)
            {
                var modulo1 = listaDeModulos.First();
                var model = new ModulosModel();
                model.numero = numero;
                model.nombre = modulo1.nombre;
                model.proyecto = proyecto;
                var requerimientos = baseDatos.Requerimientos.Where(m => m.proyecto == proyecto).ToList();
                ViewBag.requerimientos = new List<ModulosModel.reqInfo>();
                foreach (var req in requerimientos)
                {
                    var req2 = new ModulosModel.reqInfo();
                    req2.id = req.id.ToString();
                    req2.nombre = req.nombre;
                    ViewBag.requerimientos.Add(req2);
                }
                var reqEnProyecto = baseDatos.Requerimientos.Where(m => m.proyecto == proyecto && m.modulo == modulo.numero).ToList();
                model.requerimientos = new List<string>();
                foreach(var req in reqEnProyecto)
                {
                    model.requerimientos.Add(req.id.ToString());
                }
                return View(model);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Funcionalidad para modificar Módulos.
        /// </summary>
        /// <param name="model"> Modelo con la informacion del Módulo a modificar.</param>
        /// <returns>Pagina de Index</returns>
        // POST: /Modulos/Detalles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(ModulosModel model)
        {
            if (ModelState.IsValid)
            {
                var modulo = new Modulo();
                modulo.proyecto = model.proyecto;
                modulo.numero = Int32.Parse(model.numero);
                modulo.nombre = model.nombre;
                baseDatos.Entry(modulo).State = System.Data.Entity.EntityState.Modified;
                var reqViejos = baseDatos.Requerimientos.Where(m => m.proyecto == modulo.proyecto && m.modulo == modulo.numero).ToList();
                if (reqViejos.Count() > 0)
                {
                    foreach (var req in reqViejos)
                    {
                        req.modulo = null;
                        baseDatos.Entry(req).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                if (model.requerimientos.Count() > 0)
                {
                    foreach (var req in model.requerimientos)
                    {
                        var requerimiento = baseDatos.Requerimientos.Find(Int32.Parse(req));
                        requerimiento.modulo = modulo.numero;
                        baseDatos.Entry(requerimiento).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                if(modulo.Sprint_Modulos.Any())
                {
                    foreach(var sprint_modulo in modulo.Sprint_Modulos.ToList())
                    {
                        updateSprintPoints(sprint_modulo.proyecto, sprint_modulo.sprint);
                    }
                } 

                baseDatos.SaveChanges();
                this.AddToastMessage("Módulo Modificado", "El módulo " + model.nombre + " se ha modificado correctamente.", ToastType.Success);
                return RedirectToAction("Detalles", "Modulos", new { proyecto = model.proyecto, numero = model.numero });
            }
            return View(model);
        }

        // POST: /Sprint/Detalles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Borrar(ModulosModel model)
        {
            if (ModelState.IsValid)
            {
               /* model.nombre;
                model.numero;
                model.proyecto;
                model.requerimientos;*/
                int model_numero = Int32.Parse(model.numero);

                var modulosViejos = baseDatos.Sprint_Modulos.Where(m => m.modulo == model_numero && m.proyecto == model.proyecto).ToList();
                if (modulosViejos.Count() > 0)
                {
                    foreach (var mod in modulosViejos)
                    {
                        baseDatos.Entry(mod).State = System.Data.Entity.EntityState.Deleted;
                    }
                }

                var requerimientosViejos = baseDatos.Requerimientos.Where(m => m.modulo == model_numero && m.proyecto == model.proyecto).ToList();
                if (requerimientosViejos.Count() > 0)
                {
                    foreach (var req in requerimientosViejos)
                    {
                        req.modulo = null;
                        baseDatos.Entry(req).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                var modulo = baseDatos.Modulos.Find(model.proyecto, model_numero);
                baseDatos.Entry(modulo).State = System.Data.Entity.EntityState.Deleted;

                baseDatos.SaveChanges();
                this.AddToastMessage("Módulo Eliminado", "El módulo " + model.numero + " se ha eliminado correctamente.", ToastType.Success);
                return RedirectToAction("Informacion", "Proyectos", new { id = model.proyecto });
               

            }
            return View(model);
        }
    }
}