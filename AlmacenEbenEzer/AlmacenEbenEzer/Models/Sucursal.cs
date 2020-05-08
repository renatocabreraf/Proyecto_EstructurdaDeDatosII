using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlmacenEbenEzer.Interfaces;

namespace AlmacenEbenEzer.Models
{
	/// <summary>
	/// 
	/// </summary>
	public class Sucursal : IComparable, IFixedSizeText
	{
		/// <summary>
		/// ID de la sucursal perteneciente al almacen
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Nombre de la sucursal
		/// </summary>
		public string Nombre { get; set; }

		/// <summary>
		/// Dirección de ubicación de la sucursal
		/// </summary>
		public string Direccion { get; set; }

		/// <summary>
		/// Método para comparar sucursales
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			var s2 = (Sucursal)obj;
			return ID.CompareTo(s2.ID);
		}

		/// <summary>
		/// Definir el tamaño de escritura en el archivo del arbol B*
		/// </summary>
		public int FixedSize { get { return 62; } }

		/// <summary>
		/// Cambia las propiedades de como se mostrarán los atributos del modelo
		/// </summary>
		/// <returns></returns>
		public string ToFixedSizeString()
		{
			return $"{ID.ToString("0000000000;-0000000000")}~" +
				$"{string.Format("{0,-25}", Nombre)}~" +
				$"{string.Format("{0,-25}", Direccion)}";
		}

		/// <summary>
		/// 
		/// </summary>
		public int FixedSizeText
		{
			//return suma de todos los caracteres en ToFixedSizeString
			get { return FixedSize; }
		}

		/// <summary>
		/// Representación de la sucursal en el archivo de texto del árbol B* correspondiente
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("ID: {0}\r\nNombre: {1}\r\nDireccion: {2}"
				, ID.ToString("0000000000;-0000000000")
				, string.Format("{0,-25}", Nombre)
				, string.Format("{0,-25}", Direccion));
		}
	}
}