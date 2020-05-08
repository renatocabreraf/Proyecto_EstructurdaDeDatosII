using AlmacenEbenEzer.Interfaces;
using AlmacenEbenEzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlmacenEbenEzer
{
	public class CreateObject : ICreateFixedSizeText<Sucursal_Producto>
	{
		public Sucursal_Producto Create(string FixedSizeText)
		{
			Sucursal_Producto ob = new Sucursal_Producto();
			ob.IDSucursal = Convert.ToInt32(FixedSizeText.Substring(0, 10));
			ob.IDProducto = Convert.ToInt32(FixedSizeText.Substring(11, 10));
			ob.Stock = Convert.ToInt32(FixedSizeText.Substring(22, 10));
			return ob;
		}

		public Sucursal_Producto CreateNull()
		{
			Sucursal_Producto sucursal = new Sucursal_Producto();
			sucursal.IDSucursal = 0;
			sucursal.IDProducto = 0;
			sucursal.Stock = 0;

			return sucursal;
		}
	}
}