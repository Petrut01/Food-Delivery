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

       [Authorize(Roles = "User,Admin")]
        public ActionResult Index()
        {
            var userCurent = User.Identity.GetUserId();
            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == userCurent).First();

            var orderDetails = from orderdetail in db.OrderDetails
                           where orderdetail.ShoppingCartId == cart.ShoppingCartId
                           select orderdetail;

            IEnumerable<Order> myOrders = db.Orders.Where(order => orderDetails.Intersect(order.OrderDetails).Count() > 0).ToList();
            myOrders.Reverse();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            if (TempData.ContainsKey("error"))
            {
                ViewBag.error = TempData["error"].ToString();
            }

            

            ViewBag.Orders = myOrders;
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            Order order = new Order();
            
            var userCurent = User.Identity.GetUserId();
            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == userCurent).First();
            order.OrderDetails = db.OrderDetails.Where(ord => ord.IsInCurrentCart == true && ord.ShoppingCartId == cart.ShoppingCartId).ToList();
            
            ViewBag.OrderDetails = order.OrderDetails;
            float total = 0;
            foreach (var item in order.OrderDetails)
            {
                total += item.UnitPrice * item.Quantity;
            }
            ViewBag.Total = total;

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Order order)
        {

            order.OrderDate = DateTime.Now;
            var userCurent = User.Identity.GetUserId();
            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == userCurent).First();
            order.OrderDetails = db.OrderDetails.Where(ord => ord.IsInCurrentCart == true && ord.ShoppingCartId == cart.ShoppingCartId).ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    
                    if (order.OrderDetails.Count > 0)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            orderDetail.IsInCurrentCart = false;
                        }
                        db.Orders.Add(order);
                        Delivery delivery = new Delivery();
                        delivery.OrderId = order.OrderId;
                        delivery.IsFinished = false;
                       
                        db.Deliveries.Add(delivery);
                        db.SaveChanges();
                        TempData["message"] = "Comanda a fost plasata!";
                        return RedirectToAction("Index");
                    } else
                    {
                        TempData["error"] = "Adaugati produse in cos inainte de a plasa comanda.";
                        return RedirectToAction("Index");
                    }
                    
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

        [Authorize(Roles = "User,Admin")]
        public ActionResult Show(int id)
        {
            Order order = db.Orders.Find(id);
            ViewBag.OrderDetails = order.OrderDetails;
            Delivery delivery = db.Deliveries.Where(d => d.OrderId == order.OrderId).First();

            if (delivery.IsFinished == true)
            {
                ViewBag.Status = "finalizata";
            }else
            {
                ViewBag.Status = "in desfasurare";
            }
            
            return View(order);

        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Find(id);

            var deliveryForOrder = db.Deliveries.Where(a => a.OrderId == order.OrderId).FirstOrDefault();

            if (deliveryForOrder.DriverId != null)
            {
                if (deliveryForOrder.IsFinished == false)
                {

                    TempData["error"] = "Nu puteti anula comanda, deoarece aceasta a fost deja preluata de un curier.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Nu puteti anula o comanda deja finalizata!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                foreach (var detail in order.OrderDetails.ToList())
                {
                    db.OrderDetails.Remove(detail);
                }
                db.Orders.Remove(order);
                db.SaveChanges();
                TempData["message"] = "Comanda a fost anulata!";
                
                return RedirectToAction("Index");
            }
        }
    }
}