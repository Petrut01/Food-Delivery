using Licenta.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + comm.ProductId);
                }
                else
                {
                    Product p = db.Products.Find(comm.ProductId);
                    SetAccessRights();
                    TempData["message"] = "Campul nu poate fi necompletat!";
                    return Redirect("/Products/Show/" + comm.ProductId);

                }
            }

            catch (Exception e)
            {
                Product p = db.Products.Find(comm.ProductId);
                SetAccessRights();
                return View(p);
            }

        }

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Products/Show/" + comm.ProductId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un comentariu care nu va apartine";
                return RedirectToAction("Index", "Products");
            }
        }


        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.Review = comm;
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comm = db.Comments.Find(id);

                if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(comm))
                    {
                        comm.Content = requestComment.Content;
                     
                        db.SaveChanges();
                    }
                    return Redirect("/Products/Show/" + comm.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Products");
                }

            }
            catch (Exception e)
            {
                return View(requestComment);
            }

        }
        private void SetAccessRights()
        {
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.UtilizatorCurent = User.Identity.GetUserId();
        }
    }
}