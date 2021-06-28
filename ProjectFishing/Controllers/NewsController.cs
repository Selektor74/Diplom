using ProjectFishing.Infrastructure;
using ProjectFishing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;

namespace ProjectFishing.Controllers
{
    public class NewsController : Controller
    {
        public static Context _db = new Context();
        // GET: News
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetNews(int? Page)
        {
            _db = new Context();
            int pageSize = 10;
            int pageNumber = (Page ?? 1);
            var model = _db.News.ToList();
            var NewsModel = new NewsViewModel();
            foreach (var item in model)
            {
                var Images = _db.Images.Where(x => x.News.NewsId == item.NewsId).ToList();
                item.Images = Images;
                foreach (var img in item.Images)
                {
                    img.News = null;
                }
            }
            NewsModel.News = model.ToPagedList(pageNumber, pageSize);
            NewsModel.TotalDishesCount = model.Count();
            return new JsonResult() { Data = NewsModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult AddNews()
        {
            return View();
        }
        public ActionResult ShowNews(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult GetNewsById(int Id)
        {
            _db = new Context();
            var model = _db.News.Where(d => d.NewsId == Id).FirstOrDefault();
            var Images = _db.Images.Where(x => x.News.NewsId == model.NewsId).ToList();
            model.Images = Images;
            foreach (var img in model.Images)
            {
                img.News = null;
            }
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddNews(News model, List<HttpPostedFileBase> item)
        {
            _db = new Context();
            if (item != null)
            {
                foreach (var image in item)
                {
                    var img = new Image();
                    string pathToSave = Server.MapPath(@"~/images/ForViews/News"); // берем путь куда будем сохранять
                    string ext = System.IO.Path.GetExtension(image.FileName); // берем расширение для файла
                    var GUID = Guid.NewGuid();
                    string uniqueName = $"{GUID}{"ImageNews"}{image.FileName}{ext}"; // создаем уникальное имя для файла
                    string fileName = System
                        .IO
                        .Path
                        .Combine(pathToSave, uniqueName); // соединяем все вместе
                    image.SaveAs(fileName); // сохраняем
                    img.Name = uniqueName;
                    _db.Images.Add(img);
                    _db.SaveChanges();
                    model.Images.Add(img);
                }
                ApplicationDbContext dbContext = new ApplicationDbContext();
                var ActiveUser = User.Identity.GetUserId();
                var UserName = dbContext.Users.Find(ActiveUser).UserName;
                model.UserName = UserName;
                model.Date = DateTime.Now.ToLongDateString();
                _db.News.Add(model);
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }
    }

}
