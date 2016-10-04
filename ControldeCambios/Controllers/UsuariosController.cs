﻿using System.Linq;
using System.Web.Mvc;
using ControldeCambios.Models;
using System.Net;
using System;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace ControldeCambios.Controllers
{
    public class UsuariosController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            UsuariosModelo modelo = new UsuariosModelo();
            modelo.roles = context.Roles.ToList();
            modelo.usuarios = baseDatos.Usuarios.ToList();
            modelo.identityUsuarios = context.Users.ToList();
            return View(modelo);
        }

        // GET: Detalles
        public ActionResult Detalles(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuariosModelo modelo = new UsuariosModelo();
            modelo.usuario = baseDatos.Usuarios.Find(id);
            if (modelo.usuario == null)
            {
                return HttpNotFound();
            }
            modelo.identityUsuario = context.Users.Find(modelo.usuario.id);
            if (modelo.identityUsuario == null)
            {
                return HttpNotFound();
            }
            modelo.telefonos = baseDatos.Usuarios_Telefonos.Where(a => a.usuario == modelo.usuario.cedula).ToList();
            if (modelo.telefonos != null && modelo.telefonos.Count > 0)
            {
                modelo.tel1 = modelo.telefonos.ElementAt(0);
            }
            if (modelo.telefonos.Count > 1)
            {
                modelo.tel2 = modelo.telefonos.ElementAt(1);
            }
            if(modelo.telefonos.Count > 2)
            {
                modelo.tel3 = modelo.telefonos.ElementAt(2);
            }
            

            modelo.rol = context.Roles.Find(modelo.identityUsuario.Roles.First().RoleId);
            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name", modelo.rol);
            return View(modelo);
        }

        //POST: Delete
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Borrar(UsuariosModelo model)
        {
            var user = await baseDatos.Usuarios.FindAsync(model.usuario.cedula);
            baseDatos.Entry(user).State = System.Data.Entity.EntityState.Deleted;
            baseDatos.SaveChanges();

            var aspUser = await UserManager.FindByIdAsync(model.identityUsuario.Id);
            await UserManager.DeleteAsync(aspUser);

            return RedirectToAction("Index", "Usuarios");
        }

        // POST: Detalles
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(UsuariosModelo model)
        {
            var telefonos_viejos = baseDatos.Usuarios_Telefonos.Where(m => m.usuario == model.usuario.cedula);

            foreach (Usuarios_Telefonos telefono in telefonos_viejos)
            {
                baseDatos.Entry(telefono).State = System.Data.Entity.EntityState.Deleted;
            }
            baseDatos.SaveChanges();

            var telefonoEntry = new Usuarios_Telefonos();
            telefonoEntry.telefono = model.tel1.telefono;
            telefonoEntry.usuario = model.usuario.cedula;

            baseDatos.Usuarios_Telefonos.Add(telefonoEntry);

            if (model.tel2.telefono != null)
            {
                var telefonoEntry2 = new Usuarios_Telefonos();
                telefonoEntry2.telefono = model.tel2.telefono;
                telefonoEntry2.usuario = model.usuario.cedula;
                baseDatos.Usuarios_Telefonos.Add(telefonoEntry2);
            }

            if (model.tel3.telefono != null)
            {
                var telefonoEntry3 = new Usuarios_Telefonos();
                telefonoEntry3.telefono = model.tel3.telefono;
                telefonoEntry3.usuario = model.usuario.cedula;
                baseDatos.Usuarios_Telefonos.Add(telefonoEntry3);
            }

            baseDatos.SaveChanges();

            var usuario = baseDatos.Usuarios.Find(model.usuario.cedula);
            usuario.nombre = model.usuario.nombre;

            baseDatos.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
            baseDatos.SaveChanges();

            var aspUser = UserManager.FindById(model.identityUsuario.Id);
            aspUser.UserName = model.identityUsuario.Email;
            aspUser.Email = model.identityUsuario.Email;

            UserManager.Update(aspUser);

            var rolViejo = aspUser.Roles.SingleOrDefault().RoleId;
            var nombreRolViejo = context.Roles.SingleOrDefault(m => m.Id == rolViejo).Name;
            UserManager.RemoveFromRole(model.identityUsuario.Id, nombreRolViejo);
            UserManager.AddToRole(model.identityUsuario.Id, model.rol.Name);

            return RedirectToAction("Index", "Usuarios");
        }



        // GET: /Usuarios/Crear
        [AllowAnonymous]
        public ActionResult Crear()
        {

            ViewBag.Name = new SelectList(context.Roles
                                .ToList(), "Name", "Name");
            return View();
        }

        // POST: /Usuarios/Crear
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(CrearUsuarioModel model)
        {
            Entities db = new Entities();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    var userEntry = new Usuario();
                    userEntry.cedula = model.Cedula;
                    userEntry.nombre = model.Nombre;
                    userEntry.id = context.Users.Where(u => u.Email == model.Email).FirstOrDefault().Id;

                    db.Usuarios.Add(userEntry);
                    db.SaveChanges();

                    var telefonoEntry = new Usuarios_Telefonos();
                    telefonoEntry.telefono = model.Telefono;
                    telefonoEntry.usuario = model.Cedula;

                    db.Usuarios_Telefonos.Add(telefonoEntry);

                    if (model.Telefono2 != null)
                    {
                        var telefonoEntry2 = new Usuarios_Telefonos();
                        telefonoEntry2.telefono = model.Telefono2;
                        telefonoEntry2.usuario = model.Cedula;
                        db.Usuarios_Telefonos.Add(telefonoEntry2);
                    }

                    if (model.Telefono3 != null)
                    {
                        var telefonoEntry3 = new Usuarios_Telefonos();
                        telefonoEntry3.telefono = model.Telefono3;
                        telefonoEntry3.usuario = model.Cedula;
                        db.Usuarios_Telefonos.Add(telefonoEntry3);
                    }

                    db.SaveChanges();




                    // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

                    await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);

                    return RedirectToAction("Index", "Usuarios");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }


            ViewBag.Name = new SelectList(context.Roles
                    .ToList(), "Name", "Name");
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }


    }
}