using Licenta.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class OrderDetailsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OderDetails
        public ActionResult Index()
        {
            return View();
        }

        //de setat in view productid, pretul, cantitate 1 
        [HttpPost]
        public ActionResult New(OrderDetail orderDetail)
        {
            var userCurent = User.Identity.GetUserId();
            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == userCurent).First();


            orderDetail.ShoppingCartId = cart.ShoppingCartId;
            
            orderDetail.IsInCurrentCart = true;

            if (cart.OrderDetails.Where(a => a.ProductId == orderDetail.ProductId).Count() != 0 && orderDetail.IsInCurrentCart == true)
            {
                OrderDetail order_temp = cart.OrderDetails.Where(a => a.ProductId == orderDetail.ProductId).First();
                order_temp.Quantity++;
                db.SaveChanges();
            }
            else
            {
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }

            return Redirect("/Products/Index");
        }

        [HttpPut]
        public ActionResult EditPlus(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            orderDetail.Quantity++;
            db.SaveChanges();
            return Redirect("/Orders/New");
        }


        [HttpPut]
        public ActionResult EditMinus(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail.Quantity == 1)
            {
                db.OrderDetails.Remove(orderDetail);
                db.SaveChanges();
                return Redirect("/Orders/New");
            }
            else
            {
                orderDetail.Quantity--;
                db.SaveChanges();
                return Redirect("/Orders/New");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();
            return Redirect("/Orders/New");
        }
    }
}