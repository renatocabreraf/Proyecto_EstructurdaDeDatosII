using Proyecto_EDII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_EDII
{
	public class Data
	{
		private static Data instance = null;
		public static Data Instance
		{
			get
			{
				if (instance == null) instance = new Data();
				return instance;
			}
		}

		public SDES cipherMethods = new SDES();
		//listas para pruebas temporales 
		public List<Sucursal> sucursales = new List<Sucursal>();
		public List<Producto> productos = new List<Producto>();
		public List<Sucursal_Producto> sucursales_productos = new List<Sucursal_Producto>();
	}
}
