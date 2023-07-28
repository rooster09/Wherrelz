using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWherrelz.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user objUser)
        {
            if (ModelState.IsValid)
            {
                using (wheelzTaskEntities db = new wheelzTaskEntities())
                {
                    if (Request.Cookies["LoginCookies"] != null)
                    {
                        HttpCookie nameCookie = Request.Cookies["LoginCookies"];
                        if ((nameCookie.Values["userId"] != "" && nameCookie.Values["userId"] != null) && (nameCookie.Values["userPassword"] != "" && nameCookie.Values["userPassword"] != null) && (nameCookie.Values["ID"] != "" && nameCookie.Values["ID"] != null))
                        {
                            if (nameCookie.Values["userId"] == objUser.userId && nameCookie.Values["userPassword"] == objUser.userPassword)
                            {
                                Session["UserId"] = nameCookie.Values["UserId"].ToString();
                                Session["ID"] = nameCookie.Values["ID"].ToString();
                                return RedirectToAction("UserDashBoard");
                            }
                        }
                    }

                    var obj = db.users.Where(a => a.userId.Equals(objUser.userId) && a.userPassword.Equals(objUser.userPassword)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.userId.ToString();
                        Session["ID"] = obj.ID.ToString();
                        HttpCookie LoginCookies = new HttpCookie("LoginCookies");
                        LoginCookies.Values["userId"] = objUser.userId;
                        LoginCookies.Values["userPassword"] = objUser.userPassword;
                        LoginCookies.Values["ID"] = obj.ID.ToString();
                        LoginCookies.Expires = DateTime.Now.AddHours(1);
                        Response.Cookies.Add(LoginCookies);
                        Response.Flush();
                        return RedirectToAction("UserDashBoard", "Login");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult UserDashBoard()
        {
            wheelzTaskEntities1 db = new wheelzTaskEntities1();
            if (Session["UserID"] != null)
            {
                int ID =0;
                if (Session["ID"].ToString() != null && Session["ID"].ToString() != ""){
                    ID= int.Parse(Session["ID"].ToString());
                }
                else ID=1;

                IEnumerable<Entry> allEntries = new List<Entry>();
                allEntries = db.Entries.Where(b=>b.userId == ID ).OrderByDescending(b => b.createdOn);
                int totalCredit = 0, totalDebit = 0;
                
                if (allEntries != null)
                {
                    foreach (var entry in allEntries)
                    {
                        totalCredit += int.Parse(entry.creditAmount);
                        totalDebit += int.Parse(entry.debitAmount);
                        ViewBag.totaCredit = totalCredit;
                        ViewBag.totalDebit = totalDebit;
                        // More conversion on the basis of USD to INR and then adding in total can be done. 
                    }
                    
                    return View(allEntries);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult editEntry(int transactionId)
        {
            wheelzTaskEntities1 db = new wheelzTaskEntities1();
            Entry editData = db.Entries.Where(a => a.ID == transactionId).SingleOrDefault();
            return View(editData);
        }

        public ActionResult updateEntry(Entry editedObj)
        {
            using (wheelzTaskEntities1 db = new wheelzTaskEntities1())
            {
                Entry editData = db.Entries.Where(a => a.ID == editedObj.ID).SingleOrDefault();               
                editData.accountNumber = editedObj.accountNumber;
                editData.accountName = editedObj.accountName;
                editData.currency = editedObj.currency;
                editData.creditAmount = editedObj.creditAmount;
                editData.debitAmount = editedObj.debitAmount;
                editData.remarks = editedObj.remarks;
                db.SaveChanges();

            }
            return RedirectToAction("UserDashBoard", "Login", null);
        }

    }
}
