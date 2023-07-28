using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWherrelz.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginPage()
        {
            return View();
        }


        public ActionResult Welcome()
        {
            ViewBag.Message = "Your Welcome Page";
            return View();//newForm
        }

        public ActionResult newForm()
        {
            ViewBag.Message = "New Entry";
            return View();//newForm
        }

        [HttpPost]
        public ActionResult saveForm(Entry entriesObj)
        {
            wheelzTaskEntities1 db = new wheelzTaskEntities1();
            try
            {
                if (ModelState.IsValid)
                {
                    entriesObj.createdOn = DateTime.Now;
                    entriesObj.userId = int.Parse(Session["ID"].ToString());
                    db.Entries.Add(entriesObj);
                    db.SaveChanges();
                }
              
                return RedirectToAction("UserDashBoard", "Login");
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                db.Dispose();
            }
        }


        public ActionResult userList()
        {
            wheelzTaskEntities db = new wheelzTaskEntities();
            IEnumerable<user> allUsers = new List<user>();
            allUsers = db.users.OrderByDescending(b => b.createdOn);
            return View(allUsers);
        }

        public ActionResult editUser(int transactionId)
        {
            wheelzTaskEntities db = new wheelzTaskEntities();
            user editData = db.users.Where(d => d.ID == transactionId).SingleOrDefault();
            return View(editData);
        }

        public ActionResult updateUser(user editedObj)
        {
            using (wheelzTaskEntities db = new wheelzTaskEntities())
            {
                user editUser = db.users.Where(a => a.ID == editedObj.ID).SingleOrDefault();
               // editData.createdOn = DateTime.Now;
                editedObj.ID = editedObj.ID;    
                editUser.userId = editedObj.userId;
                editUser.firstName = editedObj.firstName;
                editUser.lastName = editedObj.lastName;
                editUser.addressDetails = editedObj.addressDetails;
                editUser.mobileNumber = editedObj.mobileNumber;
                editUser.isActive = editedObj.isActive;
                db.SaveChanges();

            }
            return RedirectToAction("userList", "Home", null);           
        }

        public ActionResult deleteUser(int userID)
        {
            using (wheelzTaskEntities db = new wheelzTaskEntities())
            {
                user deletedUser = db.users.Where(a => a.ID == userID).SingleOrDefault();
                deletedUser.isActive = false;
                db.SaveChanges();
            }
            return RedirectToAction("userList", "Home", null);   
        }

      
        public ActionResult addUser()
        {           
            return View();  
        }

        [HttpPost]
        public ActionResult addNewUser(user userData)
        {
            if (ModelState.IsValid)
            {
                using (wheelzTaskEntities db = new wheelzTaskEntities())
                {
                    userData.createdOn = DateTime.Now;
                    db.users.Add(userData);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("userList", "Home", null);
       
        }

        public ActionResult showHistory()
        {
            int userid = int.Parse(Session["ID"].ToString());
            wheelzTaskEntities3 db = new wheelzTaskEntities3();
            IEnumerable<Audit> allData = new List<Audit>();
            allData = db.Audits.Where(d => d.userId == userid ).OrderByDescending(b => b.transactionDate);
            return View(allData);
           
        }

        public ActionResult logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("LoginPage","Login");
        }
    }
}
