using Microsoft.AspNet.Identity;
using PagedList;
using ProjectFishing.Infrastructure;
using ProjectFishing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFishing.Controllers
{
    public class FleaMarketController : Controller
    {
        public static Context _db;

        [HttpPost]
        public ActionResult CloseAd(int Id)
        {
            _db = new Context();
            var post = _db.Ads.Where(x => x.PostId == Id).FirstOrDefault();
            post.Closed = true;
            _db.SaveChanges();
            return RedirectToAction("Index","Home");

        }
        // GET: FleaMarket
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowAds()
        {
            return View();
        }

        public ActionResult ShowMoreInfoAboutAd(int Id)
        {
            ViewBag.IdAd = Id;
            _db = new Context();
         
               var Ad = _db.Ads.Where(x => x.PostId == Id).FirstOrDefault();
            ViewBag.UserName = Ad.UserName;
            return View();
        }
        [HttpGet]
        public ActionResult GetUserAds(string Id, int? Page)
        {
            _db = new Context();

            int pageSize = 10;
            int pageNumber = (Page ?? 1);
            var model = _db.Ads.Where(x => x.UserId == Id&&x.Closed==false).ToList();
            var AdsList = new List<ViewModel>();
            var AdsModel = new PostViewModel();
            foreach (Post item in model)
            {
                var Images = _db.Images.Where(x => x.Post.PostId == item.PostId).ToList();
                ViewModel Ad = new ViewModel();

              
                Ad.Post = item;
                Ad.Post.Images = null;
                Ad.Images = Images;
                AdsList.Add(Ad);
            }
            AdsModel.Posts = AdsList.ToPagedList(pageNumber, pageSize);
            AdsModel.TotalDishesCount = AdsList.Count();
            return new JsonResult() { Data = AdsModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        [HttpGet]
        public ActionResult GetAdById(int Id)
        {
            _db = new Context();
            var AdModel = new ViewModel();
            var Images = _db.Images.Where(x => x.Post.PostId == Id).ToList();
            var Ad = _db.Posts.Where(x => x.PostId == Id).FirstOrDefault();
          

            _db.Ads.Where(x => x.PostId == Id).FirstOrDefault().ViewCount++;
            _db.SaveChanges();
            AdModel.Images = Images;
            AdModel.Post = Ad;
            AdModel.Post.Images = null;
          
        
           

            return new JsonResult() { Data = AdModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpGet]
        public ActionResult GetAllAds(int? Page)
        {
            _db = new Context();
            int pageSize = 10;
            int pageNumber = (Page ?? 1);
            var model = _db.Ads.Where(x => x.Closed == false).ToList();
            var AdsList = new List<ViewModel>();
            var AdsModel = new PostViewModel();
            foreach (Post item in model)
            {
                var Images = _db.Images.Where(x => x.Post.PostId == item.PostId).ToList();
                ViewModel Ad = new ViewModel();
                
               
                Ad.Post = item;
                Ad.Post.Images = null;
                Ad.Images = Images;
                AdsList.Add(Ad);
            }
            AdsModel.Posts = AdsList.ToPagedList(pageNumber, pageSize);
            AdsModel.TotalDishesCount = AdsList.Count();
            return new JsonResult() { Data = AdsModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpGet]
        public ActionResult GetCategoties()
        {
            _db = new Context();
            var Categories = _db.Categories.ToList();


           
            return new JsonResult() { Data = Categories, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpGet]
        public ActionResult GetAdsByCategory(int Id, int? Page)
        {
            
            _db = new Context();

            int pageSize = 10;
            int pageNumber = (Page ?? 1);
            var model = _db.Ads.Where(x=>x.Closed==false).ToList();
            var AdsList = new List<ViewModel>();
            var AdsModel = new PostViewModel();
            foreach (Ad item in model)
            {
                var Images = _db.Images.Where(x => x.Post.PostId == item.PostId).ToList();
                ViewModel Ad = new ViewModel();

               
                Ad.Post = item;
                Ad.Post.Images = null;
                Ad.Images = Images;
                AdsList.Add(Ad);
            }
            AdsModel.Posts = AdsList.ToPagedList(pageNumber, pageSize);
            AdsModel.TotalDishesCount = AdsList.Count();
            return new JsonResult() { Data = AdsModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [Authorize]
        public ActionResult AddAd()
        {
            _db = new Context();
            var model = new Ad();
            SelectList categories = new SelectList(_db.Categories, "CategoryAdId", "Name");
            ViewBag.Categories = categories;
            return View(model);
        }
        [Authorize]
        [HttpPost]
   
        [ValidateAntiForgeryToken]
        public ActionResult AddAd(Ad model, List<HttpPostedFileBase> image)
        {
            _db = new Context();
            if (!ModelState.IsValid || image == null && image.Count == 0) //проверяем не пустое ли значение
                return RedirectToAction("Index");
            model.Images = new List<Image>();
            foreach (var item in image)
            {
                if (item != null)
                {

                    var img = new Image();
                    string pathToSave = Server.MapPath("~/Images/ForViews/Ads"); // берем путь куда будем сохранять
                    string ext = System.IO.Path.GetExtension(item.FileName); // берем расширение для файла
                    var GUID = Guid.NewGuid();
                    string uniqueName = $"{GUID}{"ImageFish"}{item.FileName}{ext}"; // создаем уникальное имя для файла
                    string fileName = System
                        .IO
                        .Path
                        .Combine(pathToSave, uniqueName); // соединяем все вместе

                    item.SaveAs(fileName); // сохраняем
                    img.Name = uniqueName;
                    _db.Images.Add(img);
                    _db.SaveChanges();
                    model.Images.Add(img);
                    // _db.SaveChanges();
                    //  model.Images.Add(uniqueName);

                }

            }
            var ActiveUserId = User.Identity.GetUserId();
            model.UserId = ActiveUserId;
            model.UserName = User.Identity.GetUserName();
            model.Date = DateTime.Today.ToShortDateString();
            model.PostCategory = 0;
            model.ViewCount = 0;
            model.Closed = false;
            _db.Ads.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");

        }
    }
}