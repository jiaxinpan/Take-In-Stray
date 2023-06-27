using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using 浪愛有家.Models;

namespace 浪愛有家.Controllers
{
    public class AdoptionController : Controller
    {
        private 浪愛有家Entities db = new 浪愛有家Entities();


        //////////////////////////To會員：User開頭//////////////////////////
        

        //會員管理送養文章頁面
        [User_LoginCheck]
        public ActionResult User_Adoption_Index()
        {
            //取得目前已登入的會員帳號
            string user = ((Member)Session["user"]).Account;
            //從資料庫中取得以登入會員發布的文章
            var adoption = db.Adoption.Where(a => a.Account == user);

            return View(adoption.ToList());
        }

        //會員刊登送養文(部分檢視)
        [User_LoginCheck]
        public ActionResult _User_Adoption_Create()
        {
            //發文時直接帶入目前已登入的會員帳號
            Adoption adoption = new Adoption();
            adoption.Account = ((Member)Session["user"]).Account;

            ViewBag.Account = new SelectList(db.Member, "Account", "Account");
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName");
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName");

            return PartialView(adoption);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _User_Adoption_Create(Adoption adoption, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                // 將圖片轉換為二進位制格式
                byte[] imageData = null;

                if (image != null && image.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }
                    // 設置圖片類型
                    adoption.ImageType = image.ContentType;
                }

                // 將圖片數據存儲到資料庫中
                adoption.PhotoFile = imageData;

                //呼叫函數產生新的AdoptionID，即newAdoptionID
                string newAdoptionID = db.Database.SqlQuery<string>("select dbo.getAdoptionID()").FirstOrDefault();
                //將newAdoptionID設定給adoption物件
                adoption.AdoptionID = newAdoptionID;

                //自動取得今天日期
                adoption.PublicationDate = DateTime.Today;

                db.Adoption.Add(adoption);
                db.SaveChanges();
                return RedirectToAction("User_Adoption_Index");
            }

            ViewBag.Account = new SelectList(db.Member, "Account", "Account", adoption.Account);
            ViewBag.CityID = new SelectList(db.City, "CityName", "CityName", adoption.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryName", "CountryName", adoption.CountryID);

            return View(adoption);
        }

        //會員修改送養文內容
        [User_LoginCheck]
        public ActionResult User_Adoption_Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adoption adoption = db.Adoption.Find(id);
            if (adoption == null)
            {
                return HttpNotFound();
            }
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", adoption.Account);
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", adoption.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", adoption.CountryID);
            return View(adoption);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult User_Adoption_Edit(Adoption adoption, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                // 將圖片轉換為二進位制格式
                byte[] imageData = null;

                if (image != null && image.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }
                    // 設置圖片類型
                    adoption.ImageType = image.ContentType;
                }

                // 將圖片數據存儲到資料庫中
                adoption.PhotoFile = imageData ?? adoption.PhotoFile;

                // 將實體從上下文中分離出來
                db.Entry(adoption).State = EntityState.Detached;

                // 附加實體到上下文中
                db.Adoption.Attach(adoption);

                // 設置實體狀態為修改過的狀態
                db.Entry(adoption).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("User_Adoption_Index");
            }
            ViewBag.Account = new SelectList(db.Member, "Account", "Account", adoption.Account);
            ViewBag.CityID = new SelectList(db.City, "CityID", "CityName", adoption.CityID);
            ViewBag.CountryID = new SelectList(db.Country, "CountryID", "CountryName", adoption.CountryID);
            return View(adoption);
        }

        //會員刪除送養文(不顯示頁面)
        [User_LoginCheck]
        public ActionResult User_Adoption_Delete(string id)
        {
            Adoption adoption = db.Adoption.Find(id);
            db.Adoption.Remove(adoption);
            db.SaveChanges();
            return RedirectToAction("User_Adoption_Index");
        }


        //////////////////////////To共用：Adoption開頭//////////////////////////


        //送養文頁面
        public ActionResult Adoption_Index(string cityName, string animalKind)
        {
            var adoption = db.Adoption.Include(a => a.Member).Include(a => a.City).Include(a => a.Country);

            //縣市篩選
            if (!string.IsNullOrEmpty(cityName))
            {
                adoption = adoption.Where(a => a.City.CityName == cityName);
            }

            //動物種類篩選
            if (!string.IsNullOrEmpty(animalKind))
            {
                adoption = adoption.Where(a => a.AnimalKind == animalKind);
            }

            ViewBag.CityList = new SelectList(db.City, "CityName", "CityName");
            ViewBag.AnimalKindList = new SelectList(db.Adoption.Select(a => a.AnimalKind).Distinct().OrderBy(a => a));

            return View(adoption.ToList());
        }

        //查看送養文詳細資訊
        public ActionResult _Adoption_Detail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adoption adoption = db.Adoption.Find(id);
            if (adoption == null)
            {
                return HttpNotFound();
            }
            return PartialView(adoption);
        }


        //////////////////////////To管理員：Admin開頭//////////////////////////


        //管理員管理送養文頁面
        [Admin_LoginCheck]
        public ActionResult Admin_Adoption_Index()
        {
            var adoption = db.Adoption.Include(a => a.Member).Include(a => a.City).Include(a => a.Country);
            return View(adoption.ToList());
        }

        //管理員刪除送養文(不顯示頁面)
        [Admin_LoginCheck]
        public ActionResult Admin_Adoption_Delete(string id)
        {
            Adoption adoption = db.Adoption.Find(id);
            db.Adoption.Remove(adoption);
            db.SaveChanges();
            return RedirectToAction("Admin_Adoption_Index");
        }


        //////////////////////////To功能///////////////////////


        //地址連動功能，選擇City後讀取鄉鎮市區資料
        public JsonResult GetCountryList(int CityID)
        {
            //GetCountryList方法接收一個cityID參數
            var CountryList = db.Country.Where(c => c.CityID == CityID).ToList();
            //根據此參數查詢鄉鎮市區數據
            var CountryData = CountryList.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryID.ToString()
            });
            //並返回一個JSON數據對象
            return Json(CountryData, JsonRequestBehavior.AllowGet);
        }

        //讀取二進位制圖片
        public FileContentResult ShowImage(string id)
        {
            var photo = db.Adoption.Find(id);
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
