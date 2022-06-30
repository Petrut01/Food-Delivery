using Licenta.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {

            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            
            ViewBag.UsersList = users;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            string currentRole = user.Roles.FirstOrDefault().RoleId;

            var userRoleName = (from role in db.Roles
                                where role.Id == currentRole
                                select role.Name).First();

            ViewBag.roleName = userRoleName;

            return View(user);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


                if (TryUpdateModel(user))
                {
                   
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }

                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                newData.Id = id;
                return View(newData);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AllOrders()
        {
            var deliveries = db.Deliveries.ToList();
            deliveries.Reverse();
            ViewBag.Deliveries = deliveries;
            return View();
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);

            ShoppingCart cart = db.ShoppingCarts.Where(a => a.UserId == id).First();

            var orderDetails = db.OrderDetails.Where(ord => ord.ShoppingCartId == cart.ShoppingCartId);

            var orders = db.Orders.Where(order => orderDetails.Intersect(order.OrderDetails).Count() > 0);
            if (orderDetails.Count() > 0)
            {
                foreach (var orderDetail in orderDetails)
                {
                    db.OrderDetails.Remove(orderDetail);
                }
            }
            
            if(orders.Count() > 0)
            {
                foreach (var order in orders)
                {
                    db.Orders.Remove(order);
                }
            }
            

            var comments = db.Comments.Where(comm => comm.UserId == id);

            if(comments.Count() > 0)
            {
                foreach (var comm in comments)
                {
                    db.Comments.Remove(comm);
                }
            }
            List<int> ordersIds = orders.Select(o => o.OrderId).ToList();

            var deliveries = db.Deliveries.Where(d => ordersIds.Contains(d.OrderId));

            if (deliveries.Count() > 0)
            {
                foreach (var del in deliveries)
                {
                    db.Deliveries.Remove(del);
                }
            }

            db.ShoppingCarts.Remove(cart);
            

            db.SaveChanges();
            UserManager.Delete(user);
            return RedirectToAction("Index");
        }

        public JsonResult IsEmailAvailable(string Email)
        {
            return Json(!db.Users.Any(u => u.Email == Email), JsonRequestBehavior.AllowGet);
        }
    }

}
