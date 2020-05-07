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
			ob.Nombre = Convert.ToString(FixedSizeText.Substring(0, 10));
			/*ob. = Convert.ToString(FixedSizeText.Substring(11, 20));
			ob.Campos = Convert.ToString(FixedSizeText.Substring(21, 222));
			*/
			return ob;
		}

		public Sucursal CreateNull()
		{
			return new Sucursal();
		}
	}
}