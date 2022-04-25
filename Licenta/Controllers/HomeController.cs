using Licenta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var productIds = db.OrderDetails.Select(x => x.ProductId).ToList();

            IEnumerable<int> top3 = productIds.GroupBy(i => i).OrderByDescending(g => g.Count()).Take(3).Select(g => g.Key);
            var top3Products = db.Products.Where(p => top3.Contains(p.ProductId)).ToList();
            
            ViewBag.Top3Products = top3Products;
            return View();
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