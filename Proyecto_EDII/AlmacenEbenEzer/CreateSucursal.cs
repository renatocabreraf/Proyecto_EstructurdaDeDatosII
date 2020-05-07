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
			Sucursal ob = new Sucursal
			{
				ID = Convert.ToInt32(FixedSizeText.Substring(0, 10)),
				Nombre = Convert.ToString(FixedSizeText.Substring(11, 25)),
				Direccion = Convert.ToString(FixedSizeText.Substring(26, 50))
			};
		
			return ob;
		}

		public Sucursal CreateNull()
		{
			return new Sucursal();
		}
	}
}