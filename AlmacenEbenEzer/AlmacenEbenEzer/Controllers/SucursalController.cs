﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            string basePath = string.Format(@"{0}Arboles\", AppContext.BaseDirectory);

            if (Data.Instance.blockSucursal == false)
            {
                DirectoryInfo directory = Directory.CreateDirectory(basePath);

                var buffer = new byte[3];//contiene bytes 1 o 0, indicando si los arboles estan inicializados o no 
                using (var fs = new FileStream(basePath + @"init.txt", FileMode.OpenOrCreate))
                {
                    fs.Read(buffer, 0, 3);
                }

                //pos0 = 0 el arbol no ha sido inicializado. pos0 = 1 el arbol ya ha sido creado y tiene datos. 
                if (buffer[0] == 0)
                {
                    Data.Instance.sucursalesTree = new Tree.Tree<Sucursal>(
                    5,
                    basePath + @"sucursales.txt",
                    new CreateSucursal());
                    buffer[0] = 1;

                    //cambiar el estado del archivo a creado. byte = 1.
                    using (var fs = new FileStream(basePath + @"init.txt", FileMode.OpenOrCreate))
                    {
                        //fs.Seek(0, SeekOrigin.Begin);
                        fs.Write(buffer, 0, 3);
                    }
                }
                else
                {
                    Data.Instance.sucursalesTree = new Tree.Tree<Sucursal>(
                    5,
                    basePath + @"sucursales.txt",
                    new CreateSucursal(),
                    1); // 1 indica que ya ha sido creado el arbol 
                }

                Data.Instance.blockSucursal = true;
            }

            List<Sucursal> response = new List<Sucursal>();
            List<Sucursal> temp = Data.Instance.sucursalesTree.ToList();

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].ID != 0)
                {
                    //temp[i].Nombre = Data.Instance.cipherMethods.decipher(temp[i].Nombre);
                    //temp[i].Direccion = Data.Instance.cipherMethods.decipher(temp[i].Direccion);
                    response.Add(temp[i]);
                }
            }
            response.Sort();
            return View(response);
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
                //cifrar información                
                //ssucursal.Nombre = Data.Instance.cipherMethods.cipher(sucursal.Nombre);
                //sucursal.Direccion = Data.Instance.cipherMethods.cipher(sucursal.Direccion);

                Data.Instance.sucursalesTree.Add(sucursal);

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
            Sucursal sucursal = new Sucursal();
            List<Sucursal> elements = Data.Instance.sucursalesTree.ToList();

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].ID == id)
                {
                    sucursal = elements[i];
                }
            }

            return View(sucursal);
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
                Sucursal auxiliar = new Sucursal();

                List<Sucursal> elements = Data.Instance.sucursalesTree.ToList();
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].ID == sucursal.ID)
                    {
                        auxiliar = elements[i];
                    }
                }

                Data.Instance.sucursalesTree.UpDate(auxiliar, sucursal);
                return RedirectToAction("Index");
            }
            return View(sucursal);
        }
    }
}
