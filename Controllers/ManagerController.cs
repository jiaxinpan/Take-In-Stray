using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using 浪愛有家.Models;
using 浪愛有家.ViewModel;

namespace 浪愛有家.Controllers
{
    public class ManagerController : Controller
    {
        //private 浪愛有家Entities db = new 浪愛有家Entities();

        private 浪愛有家Azure_Entities db = new 浪愛有家Azure_Entities();

        //管理員註冊
        public ActionResult Manager_Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manager_Register([Bind(Include = "Account,Password")] Manager manager)
        {
            var Account = manager.Account;
            var admin = db.Manager.Find(Account);
            if (admin != null)
            {
                ViewBag.ErrMsg = "帳號重複";
                return View(manager);
            }

            if (ModelState.IsValid)
            {
                db.Manager.Add(manager);
                db.SaveChanges();
                return RedirectToAction("Manager_Login");
            }

            return View(manager);
        }

        //管理員登入
        public ActionResult Manager_Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Manager_Login(VMLogin vMLogin)
        {
            string password = BR.getHashPassword(vMLogin.Password);

            var admin = db.Manager.Where(m => m.Account == vMLogin.Account && m.Password == password).FirstOrDefault();

            if (admin == null)
            {
                ViewBag.ErrMsg = "帳號或密碼有誤";
                return View(vMLogin);
            }

            Session["admin"] = admin;
            return RedirectToAction("Manager_Index");
        }

        //管理員登出
        public ActionResult Manager_Logout()
        {
            Session["admin"] = null;
            return RedirectToAction("Manager_Login");
        }

        //管理員主頁面
        [Admin_LoginCheck]
        public ActionResult Manager_Index()
        {
            return View(db.Manager.ToList());
        }

        //管理員資料修改
        [Admin_LoginCheck]
        public ActionResult Manager_Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = db.Manager.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Account,Password")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manager_Index");
            }
            return View(manager);
        }

        //管理員資料刪除
        [Admin_LoginCheck]
        public ActionResult Manager_Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = db.Manager.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        [HttpPost, ActionName("Manager_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Manager manager = db.Manager.Find(id);
            db.Manager.Remove(manager);
            db.SaveChanges();
            return RedirectToAction("Manager_Index");
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