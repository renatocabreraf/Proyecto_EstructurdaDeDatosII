using AlmacenEbenEzer.Interfaces;
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
    public class Producto : IComparable, IFixedSizeText
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

        public int CompareTo(object obj)
        {
            var s2 = (Producto)obj;
            return ID.CompareTo(s2.ID);
        }

        public int FixedSize { get { return 45; } }

        public string ToFixedSizeString()
        {
            return $"{ID.ToString("0000000000;-0000000000")}~" +
                $"{string.Format("{0,-25}", Nombre)}~" +
                $"{Precio.ToString("0000000000;-0000000000")}";
        }

        public int FixedSizeText
        {
            //return suma de todos los caracteres en ToFixedSizeString
            get { return FixedSize; }
        }

        public override string ToString()
        {
            return string.Format("ID: {0}\r\nNombre: {1}\r\nPrecio: {2}"
                , ID.ToString("0000000000;-0000000000")
                , string.Format("{0,-25}", Nombre)
                , Precio.ToString("0000000000;-0000000000"));
        }

    }
}