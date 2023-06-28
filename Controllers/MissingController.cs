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
    public class MissingController : Controller
    {
        private 浪愛有家Entities db = new 浪愛有家Entities();


        //////////////////////////To會員：User開頭//////////////////////////


        //會員管理走失文章頁面
        [User_LoginCheck]
        public ActionResult User_Missing_Index()
        {
            string user = ((Member)Session["user"]).Account;
            var missing = db.Missing.Where(a => a.Account == user).OrderByDescending(m => m.PublicationDate);

            return View(missing.ToList());
        }

        //會員刊登走失文(部分檢視)
        [User_LoginCheck]
        public ActionResult _User_Missing_Create()
        {
            Missing missing = new Missing();
            missing.Account = ((Member)Session["user"]).Account;

            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName");
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName");
            ViewBag.Account = new SelectList(db.Member, "Account", "Account");
            return PartialView(missing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _User_Missing_Create(Missing missing, HttpPostedFileBase image)
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
                    
                    missing.ImageType = image.ContentType;
                }
              
                missing.PhotoFile = imageData;
              
                string newMissingID = db.Database.SqlQuery<string>("select dbo.getMissingID()").FirstOrDefault();
                
                missing.MissingID = newMissingID;
                
                missing.PublicationDate = DateTime.Today;

                db.Missing.Add(missing);
                db.SaveChanges();
                return RedirectToAction("User_Missing_Index");
            }

            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", missing.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", missing.CountryID);
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", missing.Account);
            return View(missing);
        }

        // 會員修改走失文內容
        [User_LoginCheck]
        public ActionResult User_Missing_Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Missing missing = db.Missing.Find(id);
            if (missing == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", missing.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", missing.CountryID);
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", missing.Account);
            return View(missing);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult User_Missing_Edit(Missing missing, HttpPostedFileBase image)
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

                    missing.ImageType = image.ContentType;
                }

                missing.PhotoFile = imageData ?? missing.PhotoFile;

                db.Entry(missing).State = EntityState.Detached;

                db.Missing.Attach(missing);

                db.Entry(missing).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("User_Missing_Index");
            }
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", missing.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", missing.CountryID);
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", missing.Account);
            return View(missing);
        }

        //會員刪除送養文(不顯示頁面)
        [User_LoginCheck]
        public ActionResult User_Missing_Delete(string id)
        {
            Missing missing = db.Missing.Find(id);
            db.Missing.Remove(missing);
            db.SaveChanges();
            return RedirectToAction("User_Missing_Index");
        }


        //////////////////////////To共用：Missing開頭//////////////////////////


        //走失文頁面
        public ActionResult Missing_Index(string cityName, string animalKind)
        {
            var missing = db.Missing.Include(m => m.City).Include(m => m.Country).Include(m => m.Member);

            if (!string.IsNullOrEmpty(cityName))
            {
                missing = missing.Where(m => m.City.CityName == cityName);
            }

            if (!string.IsNullOrEmpty(animalKind))
            {
                missing = missing.Where(m => m.AnimalKind == animalKind);
            }

            missing = missing.OrderByDescending(m => m.PublicationDate);

            ViewBag.CityList = new SelectList(db.City, "CityName", "CityName");
            ViewBag.AnimalKindList = new SelectList(db.Adoption.Select(m => m.AnimalKind).Distinct().OrderBy(m => m));

            return View(missing.ToList());
        }

        //查看走失文詳細資訊
        public ActionResult _Missing_Detail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Missing missing = db.Missing.Find(id);
            if (missing == null)
            {
                return HttpNotFound();
            }
            return PartialView(missing);
        }


        //////////////////////////To管理員：Admin開頭//////////////////////////
        

        //管理員管理走失文頁面
        [Admin_LoginCheck]
        public ActionResult Admin_Missing_Index()
        {
            var missing = db.Missing.Include(m => m.City).Include(m => m.Country).Include(m => m.Member).OrderByDescending(m => m.PublicationDate);
            return View(missing.ToList());
        }

        //管理員刪除走失文(不顯示頁面)
        [Admin_LoginCheck]
        public ActionResult Admin_Missing_Delete(string id)
        {
            Missing missing = db.Missing.Find(id);
            db.Missing.Remove(missing);
            db.SaveChanges();
            return RedirectToAction("Admin_Missing_Index");
        }


        //////////////////////////To功能///////////////////////


        //地址連動功能，讀取鄉鎮市區資料
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
            var photo = db.Missing.Find(id);
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
