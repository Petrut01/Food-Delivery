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
        private int _perPage = 9;

        private List<Product> produsePtSortare;
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include("Category").Include("User");
            if (produsePtSortare == null)
            {
                produsePtSortare = products.ToList();
            }

            if (TempData.ContainsKey("Produse"))
            {
                produsePtSortare = TempData["Produse"] as List<Product>;
            }


            var search = "";

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<int> productIds = db.Products.Where(pr => pr.ProductName.Contains(search) || pr.Ingredients.Contains(search)).Select(p => p.ProductId).ToList();

                List<int> commentIds = db.Comments.Where(comm => comm.Content.Contains(search)).Select(comm => comm.ProductId).ToList();
                List<int> mergedIds = productIds.Union(commentIds).ToList();

                products = (DbQuery<Product>)db.Products.Where(product => mergedIds.Contains(product.ProductId)).Include("Category").Include("User");
                produsePtSortare = products.ToList();
            }

           

            var totalProducts = produsePtSortare.Count();

            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }
            var paginatedProducts = produsePtSortare.Skip(offset).Take(this._perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            //ViewBag.perPage = this._perPage;
            ViewBag.total = totalProducts;
            ViewBag.lastPage = Math.Ceiling((float)totalProducts / (float)this._perPage);
            ViewBag.Products = paginatedProducts;

            ViewBag.SearchString = search;

            return View();
        }

        public ActionResult SortareProduse(int id)
        {
            switch (id)
            {
                case 1:
                    produsePtSortare = db.Products.Include("Category").Include("User").ToList();
                    break;
                case 2:
                    produsePtSortare = db.Products.Include("Category").Include("User").ToList();
                    produsePtSortare.Reverse();
                    break;
                case 3:
                    produsePtSortare = db.Products.Include("Category").Include("User").ToList();
                    break;
                default:
                    break;
            }
            TempData["Produse"] = produsePtSortare;
            return Redirect("/Products/Index");
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


        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult New()
        {
            Product product = new Product();
            product.Categ = GetAllCategories();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult New(Product product)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat!";
                    return RedirectToAction("Index");
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

        [Authorize(Roles = "Colaborator,Admin")]
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
                return RedirectToAction("Index");
            }
        }


        [HttpPut]
        [Authorize(Roles = "Colaborator,Admin")]
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
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs!";
                        return RedirectToAction("Index");
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
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);

            if (User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs!";
                return RedirectToAction("Index");
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