using Licenta.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       

        // GET: Products
        public ActionResult Index(int id)
        {
            var products = db.Products.Include("Category").Where(prod => prod.CategoryId == id);
       
            var produse = products.ToList();

            var search = "";

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<int> productIds = db.Products.Where(pr => pr.ProductName.Contains(search) || pr.Ingredients.Contains(search)).Select(p => p.ProductId).ToList();

                List<int> commentIds = db.Comments.Where(comm => comm.Content.Contains(search)).Select(comm => comm.ProductId).ToList();
                List<int> mergedIds = productIds.Union(commentIds).ToList();

                products = (DbQuery<Product>)db.Products.Where(product => mergedIds.Contains(product.ProductId)).Include("Category");
                produse = products.ToList();
            } else if (ViewBag.SearchString != null)
            {
                search = ViewBag.SearchString.Trim();
                List<int> productIds = db.Products.Where(pr => pr.ProductName.Contains(search) || pr.Ingredients.Contains(search)).Select(p => p.ProductId).ToList();

                List<int> commentIds = db.Comments.Where(comm => comm.Content.Contains(search)).Select(comm => comm.ProductId).ToList();
                List<int> mergedIds = productIds.Union(commentIds).ToList();

                products = (DbQuery<Product>)db.Products.Where(product => mergedIds.Contains(product.ProductId)).Include("Category");
                produse = products.ToList();
            }

            ViewBag.Products = produse;

            ViewBag.SearchString = search;

            return View();
        }

       

        public ActionResult Show(int id)
        {
            Product product = db.Products.Find(id);
            SetAccessRights();
            var reviews = db.Comments.Where(a => a.ProductId == id);
            db.SaveChanges();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(product);

        }


        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            Product product = new Product();
            product.Categ = GetAllCategories();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Product product)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat!";
                    return RedirectToAction("Index/" + product.CategoryId);
                }
                else
                {
                    product.Categ = GetAllCategories();
                    return View(product);
                }
            }
            catch (Exception e)
            {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            Product product = db.Products.Find(id);
            product.Categ = GetAllCategories();
            if (User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs !";
                return RedirectToAction("Show/" + product.ProductId);
            }
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Product requestProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);

                    if (User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(product))
                        {
                            product = requestProduct;
                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat!";
                        }
                        return RedirectToAction("Index/" + product.CategoryId);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs!";
                        return RedirectToAction("Index/" + product.CategoryId);
                    }
                }
                else
                {
                    requestProduct.Categ = GetAllCategories();
                    return View(requestProduct);
                }
            }
            catch (Exception e)
            {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);

            if (User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return Redirect("/Home/Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs!";
                return Redirect("/Home/Index");
            }
        }

        private void SetAccessRights()
        {
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.UtilizatorCurent = User.Identity.GetUserId();
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            return selectList;
        }
    }
}