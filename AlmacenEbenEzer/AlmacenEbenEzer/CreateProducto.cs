using AlmacenEbenEzer.Interfaces;
using AlmacenEbenEzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlmacenEbenEzer
{
	public class CreateProducto : ICreateFixedSizeText<Producto>
	{
		public Producto Create(string FixedSizeText)
		{
			Producto ob = new Producto();
			ob.ID = Convert.ToInt32(FixedSizeText.Substring(0, 10));
			ob.Nombre = Convert.ToString(FixedSizeText.Substring(11, 25)).Trim();
			ob.Precio = Convert.ToDecimal(FixedSizeText.Substring(37, 10));
			return ob;
		}

		public Producto CreateNull()
		{
			Producto producto = new Producto();
			producto.ID = 0;
			producto.Nombre = "";
			producto.Precio = 0;

			return producto;
		}
	}
}