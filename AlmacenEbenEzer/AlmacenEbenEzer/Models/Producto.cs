using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AlmacenEbenEzer.Models
{
    /// <summary>
    /// Productos que se ingresan al sistema del almacen.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// ID del producto agregado
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Precio del producto agregado al sistema
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }
    }
}