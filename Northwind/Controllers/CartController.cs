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
    public class CartController : Controller
    {

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

        // GET: Cart/DisplayCart
        [Authorize]
        public ActionResult DisplayCart()
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
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
        }
    }
}