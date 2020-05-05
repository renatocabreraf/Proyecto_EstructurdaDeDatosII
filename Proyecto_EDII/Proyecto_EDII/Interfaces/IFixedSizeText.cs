using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_EDII.Interfaces
{
	public interface IFixedSizeText
	{
		int FixedSize { get; }
		string ToFixedSizeString();
	}
}