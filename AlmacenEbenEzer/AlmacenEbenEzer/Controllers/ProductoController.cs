using System;
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
    /// Controlador para agregar productos al sistema, actualizar datos de un producto o bien, agregar múltiples productos mediante un archivo .csv
    /// </summary>
    public class ProductoController : Controller
    {
        // GET: Producto
        /// <summary>
        /// Retorna la lista de productos existentes. 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //string basePath = string.Format(@"{0}Arboles\", AppContext.BaseDirectory);

            //if (Data.Instance.blockSucursal == false)
            //{
            //    DirectoryInfo directory = Directory.CreateDirectory(basePath);

            //    var buffer = new byte[3];//contiene bytes 1 o 0, indicando si los arboles estan inicializados o no 
            //    using (var fs = new FileStream(basePath + @"init.txt", FileMode.OpenOrCreate))
            //    {
            //        fs.Read(buffer, 0, 3);
            //    }

            //    //pos0 = 0 el arbol no ha sido inicializado. pos0 = 1 el arbol ya ha sido creado y tiene datos. 
            //    if (buffer[1] == 0)
            //    {
            //        Data.Instance.productosTree = new Tree.Tree<Producto>(
            //        7,
            //        basePath + @"productos.txt",
            //        new CreateProducto());
            //        buffer[1] = 1;

            //        //cambiar el estado del archivo a creado. byte = 1.
            //        using (var fs = new FileStream(basePath + @"init.txt", FileMode.OpenOrCreate))
            //        {
            //            //fs.Seek(0, SeekOrigin.Begin);
            //            fs.Write(buffer, 0, 3);
            //        }
            //    }
            //    else
            //    {
            //        Data.Instance.productosTree = new Tree.Tree<Producto>(
            //        7,
            //        basePath + @"productos.txt",
            //        new CreateProducto(),
            //        1); // 1 indica que ya ha sido creado el arbol 
            //    }

            //    Data.Instance.blockSucursal = true;
            //}

            //List<Sucursal> response = Data.Instance.sucursalesTree.ToList();
            //response.Sort();
            //return View(response);
            return View(Data.Instance.productos);
        }

        // GET: Producto/Create
        /// <summary>
        /// Devuelve la vista para crear un producto
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: Producto/Create
        /// <summary>
        /// Crea un producto y lo almacena en el árbol B*
        /// </summary>
        /// <param name="producto">Modelo de "Producto" a insertar</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,Nombre,Precio")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                Data.Instance.productos.Add(producto);
                //Data.Instance.sucursalesTree.Add(sucursal);
                return RedirectToAction("Index");
            }

            return View(producto);
        }

        /// <summary>
        /// Agregar múltiples productos al cargar un archivo
        /// </summary>
        /// <param name="file">Archivo .CSV que contiene los productos</param>
        /// <returns></returns>
        [HttpPost, ActionName("addFile")]
        public ActionResult AddFile(HttpPostedFileBase file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.InputStream))
            {
                while (reader.Peek() >= 0)
                    Data.Instance.productos.Add(readProducto(reader.ReadLine()));
            }

            return RedirectToAction("Index");
        }

        // GET: Producto/Edit/5
        /// <summary>
        /// Devuelve los datos originales del producto que se modificará
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns></returns>       
        public ActionResult Edit(int? id)
        {
            //Producto producto = null;
            ////muestra de como funcionaría volver a cargar los datos en el form
            //for (int i = 0; i < Data.Instance.productos.Count; i++)
            //{
            //    if (Data.Instance.productos[i].ID == id)
            //    {
            //        producto = Data.Instance.productos[i];
            //    }
            //}
            //// return View(producto);
            return View();
        }

        // POST: Producto/Edit/5
        /// <summary>
        /// Actualiza los datos de un producto.
        /// </summary>
        /// <param name="producto">Modelo "Producto" con los datos actualizados</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Precio")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(movie).State = EntityState.Modified;
                //db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(producto);
        }


        static Producto readProducto(string line)
        {
            string[] items = line.Split(',');
            Producto response = new Producto();
            response.ID = int.Parse(items[0]);
            response.Nombre = items[1];
            response.Precio = decimal.Parse(items[2]);

            return response;
        }
    }
}
