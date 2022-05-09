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

        

        //de setat in view productid, pretul, cantitate 1 
        [HttpPost]
        [Authorize(Roles = "User,Driver,Admin")]
        public ActionResult New(OrderDetail orderDetail)
        {
            var userCurent = User.Identity.GetUserId();
            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == userCurent).First();


            orderDetail.ShoppingCartId = cart.ShoppingCartId;
            
            orderDetail.IsInCurrentCart = true;

            if (cart.OrderDetails.Where(a => a.ProductId == orderDetail.ProductId && a.IsInCurrentCart == true).Count() != 0)
            {
                OrderDetail order_temp = cart.OrderDetails.Where(a => a.ProductId == orderDetail.ProductId && a.IsInCurrentCart == true).First();
                order_temp.Quantity++;
                db.SaveChanges();
            }
            else
            {
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }

            var productAddedInCart = db.Products.Where(p => p.ProductId == orderDetail.ProductId).First();

            return Redirect("/Products/Index/" + productAddedInCart.CategoryId);
        }

        [HttpPut]
        [Authorize(Roles = "User,Driver,Admin")]
        public ActionResult EditPlus(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            orderDetail.Quantity++;
            db.SaveChanges();
            return Redirect("/Orders/New");
        }


        [HttpPut]
        [Authorize(Roles = "User,Driver,Admin")]
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
        [Authorize(Roles = "User,Driver,Admin")]
        public ActionResult Delete(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();
            return Redirect("/Orders/New");
        }
    }
}