using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 浪愛有家.Models;

namespace 浪愛有家.Controllers
{
    public class HomeController : Controller
    {
        //private 浪愛有家Entities db = new 浪愛有家Entities();
        private 浪愛有家Azure_Entities db = new 浪愛有家Azure_Entities();

        //網站主頁
        public ActionResult Index()
        {
            return View();
        }

        //會員註冊(部分檢視)
        public ActionResult _Register()
        {
            return PartialView();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Register(Member member, string image)
        {
            var Account = member.Account;
            var user = db.Member.Find(Account);
            if (user != null)
            {
                ViewBag.ErrMsg = "帳號重複";
                return View(member);
            }

            if (ModelState.IsValid)
            {
                //將選擇的頭像存入資料庫
                var photoPath = Server.MapPath("~/photos/Avatar/" + image);
                var photoBytes = System.IO.File.ReadAllBytes(photoPath);
                member.PhotoFile = photoBytes;
                member.ImageType = "image/png";

                db.Member.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(member);
        }

        //會員登入(部分檢視)
        public ActionResult _Login()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _Login(Member member)
        {
            var user = db.Member.Where(m => m.Account == member.Account && m.Password == member.Password).FirstOrDefault();

            if (user == null)
            {
                ViewBag.ErrMsg = "帳號或密碼有誤";
                return View();
            }

            Session["user"] = user;

            return RedirectToAction("Index");
        }

        //會員登入(完整頁面)
        public ActionResult Login()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Login(Member member)
        {
            var user = db.Member.Where(m => m.Account == member.Account && m.Password == member.Password).FirstOrDefault();

            if (user == null)
            {
                ViewBag.ErrMsg = "帳號或密碼有誤";
                return View();
            }

            Session["user"] = user;

            return RedirectToAction("Index");
        }

        //會員登出
        public ActionResult Logout()
        {
            Session["user"] = null;

            return RedirectToAction("Index");
        }
    }
}