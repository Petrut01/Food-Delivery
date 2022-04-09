using Licenta.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            Order order = new Order();
            order.OrderDate = DateTime.Now;
            var userCurent = User.Identity.GetUserId();
            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == userCurent).First();
            order.OrderDetails = db.OrderDetails.Where(ord => ord.IsInCurrentCart == true && ord.ShoppingCartId == cart.ShoppingCartId).ToList();
            float total = 0;
            foreach (var item in order.OrderDetails)
            {
                total += item.UnitPrice * item.Quantity;
            }

            order.Total = total;

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Order order)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                    TempData["message"] = "Comanda a fost plasata!";
                    return Redirect("/Products/Index");
                }
                else
                {
                    
                    return View(order);
                }
            }
            catch (Exception e)
            {
                
                return View(order);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Find(id);

            var deliveryForOrder = db.Deliveries.Where(a => a.OrderId == order.OrderId).FirstOrDefault();

            if (deliveryForOrder.IsTakenByDriver == false  )
            {
                if (deliveryForOrder.IsFinished == false)
                {
                    db.Orders.Remove(order);
                    db.SaveChanges();
                    TempData["message"] = "Comanda a fost anulata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu puteti anula o comanda deja finalizata!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "Nu puteti anula comanda, deoarece aceasta a fost deja preluata de un curier.";
                return RedirectToAction("Index");
            }
        }
    }
}