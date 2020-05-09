using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlmacenEbenEzer.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			string basePath = string.Format(@"{0}Arboles\", AppContext.BaseDirectory);
			DirectoryInfo directory = Directory.CreateDirectory(basePath);

			basePath = string.Format(@"{0}Export\", AppContext.BaseDirectory); ;
			directory = Directory.CreateDirectory(basePath);

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
			if (sucursales)
				Data.Instance.compressMethods.Export(@"sucursales");

			if (productos)
				Data.Instance.compressMethods.Export(@"productos");

			if (admin)
				Data.Instance.compressMethods.Export(@"admin");

			return RedirectToAction("Index");
		}


		[HttpPost, ActionName("decompress")]
		public ActionResult DecompressTree(HttpPostedFileBase file)
		{
			string basePath = string.Format(@"{0}Export\", AppContext.BaseDirectory);
			string fullPath = basePath + file.FileName;

			byte[] txt = new byte[file.ContentLength];
			using (FileStream fs = new FileStream(fullPath, FileMode.Open))
			{
				int count;                            // actual number of bytes read
				int sum = 0;                          // total number of bytes read

				// read until Read method returns 0 (end of the stream has been reached)
				while ((count = fs.Read(txt, sum, txt.Length - sum)) > 0)
					sum += count;  // sum is a buffer offset for next reading
			}


			//si se importa una base de datos y el archivo init no ha sido inicializado
			var buffer = new byte[3]; //contiene bytes 1 o 0, indicando si los arboles estan inicializados o no 

			//leer el init
			using (var fs = new FileStream(string.Format(@"{0}Arboles\", AppContext.BaseDirectory) + @"init.txt", FileMode.OpenOrCreate))
			{
				fs.Read(buffer, 0, 3);
			}

			if (file.FileName.Contains("sucursales"))
			{
				buffer[0] = 1;
			}
			else if (file.FileName.Contains("productos"))
			{
				buffer[1] = 1;
			}
			else if (file.FileName.Contains("admin"))
			{
				buffer[2] = 1;
			}

			//cambiar el estado del archivo a creado. byte = 1.
			using (var fs = new FileStream(string.Format(@"{0}Arboles\", AppContext.BaseDirectory) + @"init.txt", FileMode.OpenOrCreate))
			{
				//fs.Seek(0, SeekOrigin.Begin);
				fs.Write(buffer, 0, 3);
			}

			Data.Instance.compressMethods.DecodeFile(txt, file.FileName.Replace(".huff", ".txt"), file.FileName);
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