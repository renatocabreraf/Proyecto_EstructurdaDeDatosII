using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AlmacenEbenEzer.Models;
using AlmacenEbenEzer.Tree;

namespace AlmacenEbenEzer
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


		public Tree<Sucursal> sucursalesTree = new Tree<Sucursal>();
		public Tree<Producto> productosTree = new Tree<Producto>();

		//variables de bloqueo de arboles
		public bool blockSucursal = false;
		public bool blockProducto = false;

		//listas para pruebas temporales 
		public List<Sucursal_Producto> sucursales_productos = new List<Sucursal_Producto>();
	}
}
