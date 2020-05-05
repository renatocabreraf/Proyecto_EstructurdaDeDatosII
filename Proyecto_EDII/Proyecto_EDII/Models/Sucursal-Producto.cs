using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_EDII.Models
{
    /// <summary>
    /// Relacion entre las clases "Sucursal" y "Producto".
    /// </summary>
    public class Sucursal_Producto
    {
        /// <summary>
        /// ID de la sucursal que contiene en stock en el producto indicado
        /// </summary>
        [Display(Name = "ID Sucursal")]
        public int IDSucursal { get; set; }

        /// <summary>
        /// ID del producto contenido en la sucursal 
        /// </summary>
        [Display(Name = "ID Producto")]
        public int IDProducto { get; set; }

        /// <summary>
        /// Cantidad en inventario
        /// </summary>
        [Display(Name = "Cantidad en inventario")]
        public int Stock { get; set; }
    }
}