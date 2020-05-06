using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlmacenEbenEzer.Interfaces
{
	public interface ICreateFixedSizeText<T> where T : IFixedSizeText
	{
		T Create(string FixedSizeText);
		T CreateNull();
	}
}