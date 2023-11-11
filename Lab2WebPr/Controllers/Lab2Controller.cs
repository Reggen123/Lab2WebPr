using Lab2WebPr.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab2WebPr.Models.ViewModels;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace Lab2WebPr.Controllers
{
    public class Lab2Controller : Controller
    {
        // GET: Lab2
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ListOfPeople()
        {
            List<Person> people = new List<Person>();
            using (var db = new WebProgEntities())
            {
                people = db.Person.OrderByDescending(x => x.Age).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            }
            return View(people);
        }

        [HttpGet]
        [Authorize]
        public ActionResult PersonDetail(Guid personId)
        {

            Person model = new Person();
            using (var db = new WebProgEntities())
            {
                model = db.Person.Find(personId);
            }
            return View(model);
        }

        List<Tuple<string, string>> GetGenderList()
        {
            List<Tuple<String, string>> genders = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Ж", "Женский"),
                new Tuple<string, string>("М", "Мужской")
            };
            return genders;
        }

        List<Tuple<Guid, string>> GetUserList()
        {
            List<Tuple<Guid, String>> users = new List<Tuple<Guid, string>>();
            using (var context = new WebProgEntities())
            {
                foreach(var p in context.User.ToList())
                {
                    users.Add(new Tuple<Guid, string>(p.ID, p.Login));
                }
            }
            return users;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreatePerson()
        {
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            ViewBag.Users = new SelectList(GetUserList(), "Item1", "Item2");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult CreatePerson(PersonVM newPerson)
        {

            if (ModelState.IsValid)
            {
                using (var context = new WebProgEntities())
                {
                    Person pers = new Person
                    {
                        IdPerson = Guid.NewGuid(),
                        LastName = newPerson.LastName,
                        FirstName = newPerson.FirstName,
                        Patronymic = newPerson.Patronymic,
                        Gender = newPerson.Gender,
                        Age = newPerson.Age,
                        HasJob = newPerson.HasJob,
                        UserID = newPerson.UserID
                    };
                    context.Person.Add(pers);
                    context.SaveChanges();
                }
                return RedirectToAction("ListOfPeople");
            }
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            ViewBag.Users = new SelectList(GetUserList(), "Item1", "Item2");
            return View(newPerson);

        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditPerson(Guid personId)
        {
            PersonVM model;
            using(var context = new WebProgEntities())
            {
                Person person = context.Person.Find(personId);
                model = new PersonVM()
                {
                    Id = person.IdPerson,
                    LastName = person.LastName,
                    FirstName = person.FirstName,
                    Patronymic = person.Patronymic,
                    Gender = person.Gender,
                    Age = person.Age,
                    HasJob = person.HasJob,
                    UserID = person.UserID
                };
            }

            ViewBag.Users = new SelectList(GetUserList(), "Item1", "Item2");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditPerson(PersonVM model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new WebProgEntities())
                {
                    Person editpers = new Person
                    {
                        IdPerson = model.Id,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        Patronymic = model.Patronymic,
                        Gender = model.Gender,
                        Age = model.Age,
                        HasJob = model.HasJob,
                        UserID = model.UserID
                    };
                    context.Person.Attach(editpers);
                    context.Entry(editpers).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("ListOfPeople");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePerson(Guid personId)
        {
            Person persdel;
            using (var context = new WebProgEntities())
            {
                persdel = context.Person.Find(personId);
            }
            return View(persdel);
        }

        [HttpPost, ActionName("DeletePerson")]
        public ActionResult DeleteConfirm(Guid personId)
        {
            using (var context = new WebProgEntities())
            {
                Person persdel = new Person
                { IdPerson = personId };
                context.Entry(persdel).State = System.Data.Entity.EntityState.Deleted;

                context.SaveChanges();
            }
            return RedirectToAction("ListOfPeople");
        }

        public ActionResult QuestionAnswered(Guid personId)
        {
            string message = "";
            using(var context = new WebProgEntities())
            {
                int questionAns = context.Person.Find(personId).Answer.Count;
                message = $"Вопросов отвечено: {questionAns}.";
            }
            return PartialView("QuestionAnswered", message);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if(ModelState.IsValid) 
            {
                using(WebProgEntities context =  new WebProgEntities()) 
                {
                    User user = null;
                    user = context.User.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if(user != null)
                    {
                        string passwordHash = ReturnHashCode(webUser.Password + user.Salt.ToString().ToUpper());
                        if(passwordHash == user.PasswordHash)
                        {
                            string userRole = "";
                            switch(user.UserRole)
                            {
                                case 1:
                                    userRole = "Admin";
                                    break;
                                case 2:
                                    userRole = "User";
                                    break;
                            }

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                                                        1,
                                                                        user.Login,
                                                                        DateTime.Now,
                                                                        DateTime.Now.AddDays(1),
                                                                        false,
                                                                        userRole
                                                                        );
                            string encrTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName,encrTicket));
                            return RedirectToAction("ListOfPeople", "Lab2");
                        }
                    }
                }
            }
            ViewBag.Error = "Такого пользователя не существует";
            return View(webUser);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("ListOfPeople", "Lab2");
        }
        string ReturnHashCode(string LoginAndSalt)
        {
            string hash = "";
            using (SHA1 sHA1 = SHA1.Create())
            {
                byte[] data = sHA1.ComputeHash(Encoding.UTF8.GetBytes(LoginAndSalt));
                StringBuilder sBuild = new StringBuilder();
                for(int i = 0; i < data.Length; i++)
                {
                    sBuild.Append(data[i].ToString("x2"));
                }
                hash = sBuild.ToString().ToUpper();
            }
            return hash;
        }
    }
}