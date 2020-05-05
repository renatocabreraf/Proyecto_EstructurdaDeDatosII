using Proyecto_EDII.Interfaces;
using Proyecto_EDII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_EDII
{
	public class CreateSucursal : ICreateFixedSizeText<Sucursal>
	{
		public Sucursal Create(string FixedSizeText)
		{
			Sucursal ob = new Sucursal();
			ob.Nombre = Convert.ToString(FixedSizeText.Substring(0, 10));
			return ob;
		}

		public Sucursal CreateNull()
		{
			return new Sucursal();
		}
	}
}