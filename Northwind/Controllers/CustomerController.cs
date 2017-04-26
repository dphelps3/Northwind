using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Northwind.Models;
using Northwind.Security;
using System.Web.Security;

namespace Northwind.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer/Account
        [Authorize]
        public ActionResult Account()
        {
            ViewBag.CustomerID = UserAccount.GetUserID();
            return View();
        }
        // GET: Customer/Register
        public ActionResult Register()
        {
            return View();
        }
        // GET: Customer/SignIn
        public ActionResult SignIn()
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                // create drop-down list box for company name
                ViewBag.CustomerID = new SelectList(db.Customers.OrderBy(c => c.CompanyName), "CustomerID", "CompanyName").ToList();
            }
            return View();
        }

        // POST: Customer/SignIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include="CustomerId,Password")] CustomerSignIn customerSignIn, string ReturnUrl)
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                // find customer by CustomerId
                Customer customer = db.Customers.Find(customerSignIn.CustomerId);
                // hash & salt the posted password
                string str = UserAccount.HashSHA1(customerSignIn.Password + customer.UserGuid);
                // Compared posted Password to customer password
                if (str == customer.Password)
                {
                    // Passwords match
                    // authenticate user (this stores the CustomerID in an encrypted cookie)
                    // normally, you would require HTTPS
                    FormsAuthentication.SetAuthCookie(customer.CustomerID.ToString(), false);
                    
                    // if there is a return url, redirect to the url
                    if (ReturnUrl != null)
                    {
                        return Redirect(ReturnUrl);
                    }

                    // Redirect to Home page
                    //return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else
                {
                    // Passwords do not match
                }
                ViewBag.CustomerID = new SelectList(db.Customers.OrderBy(c => c.CompanyName), "CustomerID", "CompanyName").ToList();
                return View();
            }
        }
        // POST: Customer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Email,Password,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            // Add new customer to database
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                // Generate guid for this customer
                customer.UserGuid = System.Guid.NewGuid();
                // Hash & Salt the customer Password using SHA-1 algorithm
                customer.Password = UserAccount.HashSHA1(customer.Password + customer.UserGuid);
                // Save customer to database
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            //return View();
        }
    }
}