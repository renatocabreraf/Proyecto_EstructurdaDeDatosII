using Proyecto_EDII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_EDII.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AdministrarController : Controller
    {
        // GET: Administrar
        /// <summary>
        /// Devuelve los objetos Sucursal-producto existentes en el sistema
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(Data.Instance.sucursales_productos);
        }

        // GET: Administrar/Create
        /// <summary>
        /// Devuelve la vista para crear Sucursal-Producto
        /// </summary>
        /// <returns></returns>        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrar/Create
        /// <summary>
        /// Crea un objeto con el modelo de datos "Sucursal producto".
        /// </summary>
        /// <param name="relacion">Modelo a insertar en los datos del sistema</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "IDSucursal,IDProducto,Stock")] Sucursal_Producto relacion)
        {
            if (ModelState.IsValid)
            {
                Data.Instance.sucursales_productos.Add(relacion);
                //Data.Instance.sucursalesTree.Add(sucursal);
                return RedirectToAction("Index");
            }

            return View(relacion);
        }

        // GET: Administrar/Edit/5
        /// <summary>
        /// Devuelve los datos originales del objeto a modificar
        /// </summary>
        /// <param name="id">ID del objeto Sucursal-Producto a modificar</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            return View();
        }

        // POST: Administrar/Edit/5
        /// <summary>
        /// Actualiza y guarda los datos de un objeto Sucursal-Producto
        /// </summary>
        /// <param name="relacion">Modelo con datos modificados</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit([Bind(Include = "IDSucursal,IDProducto,Stock")] Sucursal_Producto relacion)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(movie).State = EntityState.Modified;
                //db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(relacion);
        }

        /// <summary>
        /// Devuelve la vista que corresponde a transferir unidades entre sucursales
        /// </summary>        
        /// <returns></returns>
        public ActionResult Transfer()
        {
            return View();
        }


        /// <summary>
        /// Transfiere unidades de una sucursal a otra
        /// </summary>
        /// <param name="id">ID Sucursal origen</param>
        /// <param name="id2">ID Sucursal destino</param>
        /// <param name="idproducto">ID del producto a trasladar</param>
        /// <param name="qty">Cantidad del producto que se va a transferir a la sucursal destino</param>
        /// <returns></returns>
        [HttpPost, ActionName("Transfer")]
        public ActionResult Transfer(int id, int id2, int idproducto, int qty)
        {
            return RedirectToAction("Index");
        }
    }
}