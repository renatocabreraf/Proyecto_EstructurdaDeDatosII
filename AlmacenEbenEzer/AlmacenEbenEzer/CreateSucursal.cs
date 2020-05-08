using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlmacenEbenEzer.Models;
using AlmacenEbenEzer.Interfaces;

namespace AlmacenEbenEzer
{
	public class CreateSucursal : ICreateFixedSizeText<Sucursal>
	{
		public Sucursal Create(string FixedSizeText)
		{
			Sucursal ob = new Sucursal();
			ob.ID = Convert.ToInt32(FixedSizeText.Substring(0, 10));
			ob.Nombre = Convert.ToString(FixedSizeText.Substring(11, 25)).Trim();
			ob.Direccion = Convert.ToString(FixedSizeText.Substring(37, 25)).Trim();
			return ob;
		}

		public Sucursal CreateNull()
		{
			Sucursal sucursal = new Sucursal();
			sucursal.ID = 0;
			sucursal.Nombre = "";
			sucursal.Direccion = "";

			return sucursal;
		}
	}
}