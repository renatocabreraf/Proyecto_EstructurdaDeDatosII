using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlmacenEbenEzer.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost, ActionName("setKey")]
		public ActionResult SetKey(string key)
		{
			if (key != null)
			{
				Data.Instance.cipherMethods = new SDES(key);
			}
			else
			{
				Data.Instance.cipherMethods = new SDES("");
			}
			return RedirectToAction("Index");
		}

		[HttpPost, ActionName("compress")]
		public ActionResult CompressTree(bool sucursales, bool productos, bool admin)
		{
			//< input id = "Sucursales" type = "checkbox" value = "true" name = "sucursales" class="form-control" />
			if (sucursales)
			{
				//comprimir arbol de sucursales
			}
			if (productos)
			{

			}
			if (admin)
			{

			}
			return RedirectToAction("Index");
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}