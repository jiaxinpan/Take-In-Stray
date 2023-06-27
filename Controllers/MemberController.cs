using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using 浪愛有家.Models;
using 浪愛有家.ViewModel;
using PagedList;
using Microsoft.Ajax.Utilities;
using Member = 浪愛有家.Models.Member;

namespace 浪愛有家.Controllers
{
    public class MemberController : Controller
    {
        private 浪愛有家Entities db = new 浪愛有家Entities();
        int pageSize = 5;


        //////////////////////////To會員：User開頭//////////////////////////


        //會員個人資料頁面
        [User_LoginCheck]
        public ActionResult User_Member_Index()
        {           
            string user = ((Member)Session["user"]).Account;
            var member = db.Member.Where(a => a.Account == user);

            return View(member.ToList());
        }

        //會員修改個人資料
        [User_LoginCheck]
        public ActionResult User_Member_Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Member.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult User_Member_Edit(Member member, string image)
        {
            if (ModelState.IsValid)
            {
                var photoPath = Server.MapPath("~/photos/Avatar/" + image);
                var photoBytes = System.IO.File.ReadAllBytes(photoPath);
                member.PhotoFile = photoBytes;
                member.ImageType = "image/png";

                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("User_Member_Index");
            }
            return View(member);
        }


        //////////////////////////To會員：User開頭//////////////////////////


        //管理員管理會員頁面
        [Admin_LoginCheck]
        public ActionResult Admin_Member_Index(int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var member = db.Member.ToList();
            var result = member.ToPagedList(currentPage, pageSize);
            return View(result);
        }

        //管理員刪除會員(不顯示頁面)
        [Admin_LoginCheck]
        public ActionResult Admin_Member_Delete(string id)
        {
            Member member = db.Member.Find(id);
            db.Member.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Admin_Member_Index");
        }


        //////////////////////////To功能///////////////////////


        //讀取二進位制圖片
        public FileContentResult ShowImage(string id)
        {
            var photo = db.Member.Find(id);
            if (photo != null)
                return File(photo.PhotoFile, photo.ImageType);

            return null;
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
