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
            string basePath = string.Format(@"{0}Arboles\", AppContext.BaseDirectory);

            if (Data.Instance.blockAdmin == false)
            {
                DirectoryInfo directory = Directory.CreateDirectory(basePath);

                var buffer = new byte[3];//contiene bytes 1 o 0, indicando si los arboles estan inicializados o no 
                using (var fs = new FileStream(basePath + @"init.txt", FileMode.OpenOrCreate))
                {
                    fs.Read(buffer, 0, 3);
                }

                //pos0 = 0 el arbol no ha sido inicializado. pos0 = 1 el arbol ya ha sido creado y tiene datos. 
                if (buffer[2] == 0)
                {
                    Data.Instance.scTree = new Tree.Tree<Sucursal_Producto>(
                    5,
                    basePath + @"admin.txt",
                    new CreateObject());
                    buffer[2] = 1;

                    //cambiar el estado del archivo a creado. byte = 1.
                    using (var fs = new FileStream(basePath + @"init.txt", FileMode.OpenOrCreate))
                    {
                        //fs.Seek(0, SeekOrigin.Begin);
                        fs.Write(buffer, 0, 3);
                    }
                }
                else
                {
                    Data.Instance.scTree = new Tree.Tree<Sucursal_Producto>(
                    5,
                    basePath + @"admin.txt",
                    new CreateObject(),
                    1); // 1 indica que ya ha sido creado el arbol 
                }

                Data.Instance.blockAdmin = true;
            }

            List<Sucursal_Producto> response = new List<Sucursal_Producto>();
            List<Sucursal_Producto> temp = Data.Instance.scTree.ToList();

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].IDSucursal != 0)
                {
                    response.Add(temp[i]);
                }
            }

            response.Sort();
            return View(response);
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
                Data.Instance.scTree.Add(relacion);
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
            Sucursal_Producto producto = new Sucursal_Producto();
            List<Sucursal_Producto> elements = Data.Instance.scTree.ToList();

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].IDSucursal == id)
                {
                    producto = elements[i];
                }
            }

            return View(producto);
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
                Sucursal_Producto auxiliar = new Sucursal_Producto();

                List<Sucursal_Producto> elements = Data.Instance.scTree.ToList();
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].IDSucursal == relacion.IDSucursal)
                    {
                        auxiliar = elements[i];
                    }
                }

                Data.Instance.scTree.UpDate(auxiliar, relacion);
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
            if (ModelState.IsValid)
            {
                Sucursal_Producto origen = new Sucursal_Producto();
                Sucursal_Producto destino = new Sucursal_Producto();

                List<Sucursal_Producto> elements = Data.Instance.scTree.ToList();
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].IDSucursal == id)
                    {
                        origen = elements[i];
                    }

                    if (elements[i].IDSucursal == id2)
                    {
                        destino = elements[i];
                    }
                }

                //comprobar si la sucursal destino tiene el producto 
                if (destino.IDProducto == idproducto) //transferir
                {
                    Sucursal_Producto origenModified = new Sucursal_Producto
                    {
                        IDSucursal = origen.IDSucursal,
                        IDProducto = origen.IDProducto,
                        Stock = origen.Stock - qty
                    };


                    Sucursal_Producto destinoModified = new Sucursal_Producto
                    {
                        IDSucursal = destino.IDSucursal,
                        IDProducto = destino.IDProducto,
                        Stock = destino.Stock + qty
                    };

                    Data.Instance.scTree.UpDate(origen, origenModified);
                    Data.Instance.scTree.UpDate(destino, destinoModified);
                }
                else //crear nueva relacion
                {
                    Sucursal_Producto origenModified = new Sucursal_Producto
                    {
                        IDSucursal = origen.IDSucursal,
                        IDProducto = origen.IDProducto,
                        Stock = origen.Stock - qty
                    };

                    Sucursal_Producto nueva = new Sucursal_Producto
                    {
                        IDSucursal = id2,
                        IDProducto = idproducto,
                        Stock = qty
                    };

                    Data.Instance.scTree.UpDate(origen, origenModified);
                    Data.Instance.scTree.Add(nueva);

                }

                //Data.Instance.scTree.UpDate(auxiliar, producto);
                return RedirectToAction("Index");
            }
            return View();
        }
    }

}
