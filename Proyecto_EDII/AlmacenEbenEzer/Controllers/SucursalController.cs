using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenEbenEzer.Models;

namespace AlmacenEbenEzer.Controllers
{
    /// <summary>
    /// Controlador para agregar una sucursal en el sistema o bien, actualizar datos de una sucursal existente.
    /// </summary>
    public class SucursalController : Controller
    {
        // GET: Sucursal
        /// <summary>
        /// Retorna la lista de Sucursales existentes
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(Data.Instance.sucursales);
        }


        // GET: Sucursal/Create
        /// <summary>
        /// Devuelve la vista para crear sucursal
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sucursal/Create
        /// <summary>
        /// Crea una sucursal y la inserta en el arbol corresondiente
        /// </summary>
        /// <param name="sucursal">Modelo "Sucursal" a insertar</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,Nombre,Direccion")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                Data.Instance.sucursales.Add(sucursal);
                //cifrar información
                sucursal.ID = int.Parse(Data.Instance.cipherMethods.cipher(sucursal.ID.ToString()));
                sucursal.Nombre = Data.Instance.cipherMethods.cipher(sucursal.Nombre);
                sucursal.Direccion = Data.Instance.cipherMethods.cipher(sucursal.Direccion);

                return RedirectToAction("Index");
            }

            return View(sucursal);
        }

        // GET: Sucursal/Edit/5
        /// <summary>
        /// Devuelve los datos originales de la sucursal a modificar 
        /// </summary>
        /// <param name="id">ID de la sucursal</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            return View();
        }

        // POST: Sucursal/Edit/5
        /// <summary>
        /// Actualiza los datos de la sucursal seleccionada 
        /// </summary>
        /// <param name="sucursal">Modelo "Sucursal" con datos actualizados</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Direccion")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(movie).State = EntityState.Modified;
                //db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(sucursal);
        }
    }
}
