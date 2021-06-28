using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using ProjectFishing.Infrastructure;
using ProjectFishing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace ProjectFishing.Controllers
{
  
    public class HomeController : Controller
    {
        public static Context _db = new Context();

        [HttpGet]
        public ActionResult GetChats()
        {
            var ViewModel = new ChatViewModel();
            _db = new Context();
            ApplicationDbContext dbContext = new ApplicationDbContext(); 
            var ActiveUser = User.Identity.GetUserId();
            var Chats = _db.Chats.Where(u => u.MainUser == ActiveUser || u.SecondaryUser == ActiveUser).ToList();
            ViewModel.Chat = Chats;
            string SecondaryUser = "";
            string MainUser = "";
            foreach (var chat in ViewModel.Chat)
            {
                if(chat.MainUser==ActiveUser)
                {
                    MainUser = chat.MainUser;
                     SecondaryUser = chat.SecondaryUser;
                }
                else if (chat.MainUser != ActiveUser)
                {
                    MainUser = chat.MainUser;
                     SecondaryUser = ActiveUser;
                }
                foreach (var message in chat.Messages)
                {
                    message.Chat = null;
                }
            }
            var UserMainImage = dbContext.Users.Find(MainUser).Image;
            var SecondaryUserImage = dbContext.Users.Find(SecondaryUser).Image;
            ViewModel.ImageMainUser = UserMainImage;
            ViewModel.ImageSeconaryUser = SecondaryUserImage;
            //   var SecondaryUser = dbContext.Users.Find(ActiveUser).Image;
            return new JsonResult() { Data = ViewModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }
        [Authorize]
        public ActionResult Chat(string Id)
        {
            _db = new Context();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var ActiveUser = User.Identity;
            var ActiveUserId = User.Identity.GetUserId();
            if(Id==null)
            {
               var _chats = _db.Chats.Where(u => u.MainUser == ActiveUserId || u.SecondaryUser == ActiveUserId).FirstOrDefault();
                if (_chats == null)
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(_chats);
                }

            }
            var SecondUser = dbContext.Users.Where(x => x.Id == Id).FirstOrDefault();

            var  _chat = _db.Chats.Where(u => u.MainUser == ActiveUserId && u.SecondaryUser == Id||
            u.MainUser == Id && u.SecondaryUser == ActiveUserId).FirstOrDefault();

            if(_chat!=null&&_chat.Messages[_chat.Messages.Count - 1].SenderName!=ActiveUser.Name)
            {
                _chat.Read = true;
                _db.SaveChanges();
            }
            
            if (_chat == null)
            {
                var _roomName = ActiveUser.GetUserName() + " x " +SecondUser.UserName;
                _chat = new Chat()
                {
                    MainUser = ActiveUser.GetUserId(),
                    MainUserName=ActiveUser.GetUserName(),
                    SecondaryUser = Id,
                    SecondaryUserName=SecondUser.UserName,
                    Messages = new List<Message>(),
                    RoomName = _roomName
                };
                _db.Chats.Add(_chat);
                _db.SaveChanges();
            }
            return View(_chat);
        }
      



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(CategoryAd model )
        {
            //if (!ModelState.IsValid || image == null && image.ContentLength == 0) //проверяем не пустое ли значение
            //    return RedirectToAction("Index");
            

            

            _db.Categories.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult AddCategory()
        {
            var model = new CategoryAd();
            return View(model);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowFishes()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLake(Lake model, List<HttpPostedFileBase> image)
        {
            //if (!ModelState.IsValid || image == null && image.ContentLength == 0) //проверяем не пустое ли значение
            //    return RedirectToAction("Index");
            model.Images = new List<Image>();
            foreach (var item in image)
            {
                if(item!=null)
                {
                    var img = new Image();

                    string pathToSave = Server.MapPath(@"~/Images/ForViews/Lakes"); // берем путь куда будем сохранять
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

            model.PostCategory = 2;

            _db.Lakes.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult AddLake()
        {
            var model = new Lake();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFish(Fish model, List<HttpPostedFileBase> image)
        {

            //if (!ModelState.IsValid || image == null && image.Count == 0) //проверяем не пустое ли значение
            //   return RedirectToAction("Index");
            model.Images = new List<Image>();
            
            foreach (var item in image)
            {
                if (item != null)
                {
                    var img = new Image();
                    // _db.SaveChanges();
                    string pathToSave = Server.MapPath(@"~/images/ForViews/Fishies"); // берем путь куда будем сохранять
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
                
                }
               
            }
            
            model.PostCategory = 1;
            
            _db.Fishies.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult AddFish()
        {
            var model = new Fish();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddShop(Shop model, List<HttpPostedFileBase> image)
        {
            model.Images = new List<Image>();
            foreach (var item in image)
            {
                if(item != null)
                {
                    var img = new Image();
                    string pathToSave = Server.MapPath(@"~/images/ForViews/Shops"); // берем путь куда будем сохранять
                    string ext = System.IO.Path.GetExtension(item.FileName); // берем расширение для файла
                    var GUID = Guid.NewGuid();
                    string uniqueName = $"{GUID}{"ImageShop"}{item.FileName}{ext}"; // создаем уникальное имя для файла
                    string fileName = System
                        .IO
                        .Path
                        .Combine(pathToSave, uniqueName); // соединяем все вместе

                    item.SaveAs(fileName); // сохраняем
                    img.Name = uniqueName;

                    _db.Images.Add(img);
                    _db.SaveChanges();

                    model.Images.Add(img);
                }

            }
            model.PostCategory = 3;

            _db.Shops.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddShop()
        {
            var model = new Shop();
            return View(model);
        }

        


    }
}