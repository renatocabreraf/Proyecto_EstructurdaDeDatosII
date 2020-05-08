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

		public SDES cipherMethods = new SDES("");

		public Tree<Sucursal> sucursalesTree = new Tree<Sucursal>();
		public Tree<Producto> productosTree = new Tree<Producto>();
		public Tree<Sucursal_Producto> scTree = new Tree<Sucursal_Producto>();

		//variables de bloqueo de arboles
		public bool blockSucursal = false;
		public bool blockProducto = false;
		public bool blockAdmin = false;
	}
}
