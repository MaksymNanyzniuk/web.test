using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace Callculator.Controllers
{
    public class SettingsController : Controller
    {
        private readonly string settings = $"{Path.GetTempPath()}/settings.data";

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Date(string date)
        {
            var format = System.IO.File.Exists(settings)
                ? System.IO.File.ReadAllText(settings).Split(';')[0]
                : "dd/MMM/yyyy";

            return Json(DateTime.Parse(date).ToString(format, CultureInfo.InvariantCulture), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Number(float number)
        {
            var result = string.Format(CultureInfo.InvariantCulture, "{0:N}", number);
            var format = System.IO.File.Exists(settings)
                ? System.IO.File.ReadAllText(settings).Split(';')[1]
                : "123,456,789.00";

            switch (format)
            {
                case "123.456.789,00":
                    result = result.Replace(',', ' ').Replace('.', ',').Replace(' ', '.');
                    break;

                case "123 456 789.00":
                    result = result.Replace(',', ' ');
                    break;

                case "123 456 789,00":
                    result = result.Replace(',', ' ').Replace('.', ',');
                    break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Currency()
        {
            var result = System.IO.File.Exists(settings)
                ? System.IO.File.ReadAllText(settings).Split(';')[2]
                : "$";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(string dateFormat, string numberFormat, string currency)
        {
            System.IO.File.WriteAllText(settings, $"{dateFormat};{numberFormat};{currency.Split(' ')[0]}");
            return Json("OK");
        }
    }
}