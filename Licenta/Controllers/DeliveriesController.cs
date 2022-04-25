using Licenta.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class DeliveriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Deliveries
        [Authorize(Roles = "Driver,Admin")]
        public ActionResult AvailableOrders()
        {
            ViewBag.AvailableOrders = db.Deliveries.Where(d => d.IsTakenByDriver == false && d.IsFinished == false).ToList();
            ViewBag.AvailableOrders.Reverse();
            return View();
        }

        [Authorize(Roles = "Driver,Admin")]
        public ActionResult MyOrders()
        {
            var userCurent = User.Identity.GetUserId();
            ViewBag.MyOrders = db.Deliveries.Where(d => d.DriverId == userCurent).ToList();
            ViewBag.MyOrders.Reverse();
            return View();
        }

        [HttpPut]
        [Authorize(Roles = "Driver,Admin")]
        public ActionResult TakeOrder(int id)
        {
            Delivery delivery = db.Deliveries.Find(id);
            delivery.DriverId = User.Identity.GetUserId();
            delivery.IsTakenByDriver = true;
            db.SaveChanges();

            return RedirectToAction("AvailableOrders");

        }

        [HttpPut]
        [Authorize(Roles = "Driver,Admin")]
        public ActionResult FinishOrder(int id)
        {
            Delivery delivery = db.Deliveries.Find(id);
            delivery.IsFinished = true;
            db.SaveChanges();
            return RedirectToAction("MyOrders");
        }
    }
}