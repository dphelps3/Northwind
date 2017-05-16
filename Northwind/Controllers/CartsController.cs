using System.Linq;
using System.Web;
using System.Web.Mvc;
using Northwind.Models;
using Northwind.Security;
using System.Web.Security;
using System.Net;
using System.Data.Entity;
using System.Collections.Generic;

namespace Northwind.Controllers
{
    public class CartsController : Controller
    {
        private NORTHWNDEntities db = new NORTHWNDEntities();

        // GET: Carts
        /* public ActionResult Index()
        {
            var carts = db.Carts.Include(c => c.Customer).Include(c => c.Product);
            return View(carts.ToList());
        } */

        // GET: Carts/Index
        [Authorize]
        public ActionResult Index()
        {
            // find contents of the cart, based on customer's ID
            var acctId = UserAccount.GetUserID();

            var cartList = db.Carts
                .Where(c => c.CustomerID == acctId);

            // compile cart items in a list
            List<Cart> carts = cartList
                .Include(p => p.Product)
                .Include(c => c.Customer)
                .ToList();

            // return view
            return View(carts);
        }

        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        /*public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        } */

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /* [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CartID,ProductID,CustomerID,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", cart.ProductID);
            return View(cart);
        } */

        // GET: Carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", cart.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", cart.ProductID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartID,ProductID,CustomerID,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new HtmlString(cart.Customer.CompanyName);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", cart.ProductID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Cart/AddToCart 
        [HttpPost]
        public JsonResult AddToCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            // create cart item from Json object
            Cart sc = new Cart();
            sc.ProductID = cartDTO.ProductID;
            sc.CustomerID = cartDTO.CustomerID;
            sc.Quantity = cartDTO.Quantity;

            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                // if there is a duplicate product id in cart, simply update the quantity
                if (db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID).Any())
                {
                    // this product is already in the customer's cart,
                    // update the existing cart item's quantity
                    Cart cart = db.Carts.Where(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID).FirstOrDefault();
                    cart.Quantity += sc.Quantity;
                    sc = new Cart()
                    {
                        CartID = cart.CartID,
                        ProductID = cart.ProductID,
                        CustomerID = cart.CustomerID,
                        Quantity = cart.Quantity
                    };
                }
                else
                {
                    // this product is not in the customer's cart, add the product
                    db.Carts.Add(sc);
                }
                db.SaveChanges();
                //System.Threading.Thread.Sleep(1500);
            }

            return Json(sc, JsonRequestBehavior.AllowGet);
        }


        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
