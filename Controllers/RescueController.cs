using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using 浪愛有家.Models;

namespace 浪愛有家.Controllers
{
    public class RescueController : Controller
    {
        private 浪愛有家Entities db = new 浪愛有家Entities();


        //////////////////////////To會員：User開頭//////////////////////////


        //會員管理救援文章頁面
        [User_LoginCheck]
        public ActionResult User_Rescue_Index()
        {           
            string user = ((Member)Session["user"]).Account;
            var rescue = db.Rescue.Where(a => a.Account == user).OrderByDescending(r => r.PublicationDate);

            return View(rescue.ToList());
        }

        //會員刊登救援文(部分檢視)
        [User_LoginCheck]
        public ActionResult _User_Rescue_Create()
        {
            Rescue rescue = new Rescue();
            rescue.Account = ((Member)Session["user"]).Account;

            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName");
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName");
            ViewBag.Account = new SelectList(db.Member, "Account", "Account");
            return PartialView(rescue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _User_Rescue_Create(Rescue rescue, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;

                if (image != null && image.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }

                    rescue.ImageType = image.ContentType;
                }

                rescue.PhotoFile = imageData;
              
                string newRescueID = db.Database.SqlQuery<string>("select dbo.getRescueID()").FirstOrDefault();

                rescue.RescueID = newRescueID;

                rescue.PublicationDate = DateTime.Today;

                db.Rescue.Add(rescue);
                db.SaveChanges();
                return RedirectToAction("User_Rescue_Index");
            }

            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", rescue.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", rescue.CountryID);
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", rescue.Account);
            return View(rescue);
        }

        // 會員修改救援文內容
        [User_LoginCheck]
        public ActionResult User_Rescue_Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rescue rescue = db.Rescue.Find(id);
            if (rescue == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", rescue.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", rescue.CountryID);
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", rescue.Account);
            return View(rescue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult User_Rescue_Edit(Rescue rescue, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;

                if (image != null && image.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }

                    rescue.ImageType = image.ContentType;
                }

                rescue.PhotoFile = imageData ?? rescue.PhotoFile;

                db.Entry(rescue).State = EntityState.Detached;

                db.Rescue.Attach(rescue);

                db.Entry(rescue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("User_Rescue_Index");
            }
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", rescue.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", rescue.CountryID);
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", rescue.Account);
            return View(rescue);
        }

        //會員刪除送養文(不顯示頁面)
        [User_LoginCheck]
        public ActionResult User_Rescue_Delete(string id)
        {
            Rescue rescue = db.Rescue.Find(id);
            db.Rescue.Remove(rescue);
            db.SaveChanges();
            return RedirectToAction("User_Rescue_Index");
        }


        //////////////////////////To共用：Rescue開頭//////////////////////////


        //走失文頁面
        public ActionResult Rescue_Index(string cityName, string animalKind)
        {
            var rescue = db.Rescue.Include(r => r.City).Include(r => r.Country).Include(r => r.Member);

            if (!string.IsNullOrEmpty(cityName))
            {
                rescue = rescue.Where(r => r.City.CityName == cityName);
            }

            if (!string.IsNullOrEmpty(animalKind))
            {
                rescue = rescue.Where(r => r.AnimalKind == animalKind);
            }

            rescue = rescue.OrderByDescending(r => r.PublicationDate);

            ViewBag.CityList = new SelectList(db.City, "CityName", "CityName");
            ViewBag.AnimalKindList = new SelectList(db.Rescue.Select(r => r.AnimalKind).Distinct().OrderBy(r => r));

            return View(rescue.ToList());
        }

        //查看救援文詳細資訊
        public ActionResult _Rescue_Detail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rescue rescue = db.Rescue.Find(id);
            if (rescue == null)
            {
                return HttpNotFound();
            }
            return PartialView(rescue);
        }


        //////////////////////////To管理員：Admin開頭//////////////////////////


        //管理員管理救援文頁面
        [Admin_LoginCheck]
        public ActionResult Admin_Rescue_Index()
        {
            var rescue = db.Rescue.Include(r => r.City).Include(r => r.Country).Include(r => r.Member).OrderByDescending(r => r.PublicationDate);
            return View(rescue.ToList());
        }

        //管理員刪除走失文(不顯示頁面)
        [Admin_LoginCheck]
        public ActionResult Admin_Rescue_Delete(string id)
        {
            Rescue rescue = db.Rescue.Find(id);
            db.Rescue.Remove(rescue);
            db.SaveChanges();
            return RedirectToAction("Admin_Rescue_Index");
        }


        //////////////////////////To功能///////////////////////


        //地址連動功能，選擇City後讀取鄉鎮市區資料
        public JsonResult GetCountryList(int CityID)
        {
            var CountryList = db.Country.Where(c => c.CityID == CityID).ToList();
            
            var CountryData = CountryList.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryID.ToString()
            });
            
            return Json(CountryData, JsonRequestBehavior.AllowGet);
        }

        //讀取二進位制圖片
        public FileContentResult ShowImage(string id)
        {
            var photo = db.Rescue.Find(id);
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
