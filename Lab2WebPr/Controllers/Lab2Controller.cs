using Lab2WebPr.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab2WebPr.Models.ViewModels;

namespace Lab2WebPr.Controllers
{
    public class Lab2Controller : Controller
    {
        // GET: Lab2
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListOfPeople()
        {
            List<Person> people = new List<Person>();
            using (var db = new Entities())
            {
                people = db.Person.OrderByDescending(x => x.Age).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            }
            return View(people);
        }

        public ActionResult PersonDetail(Guid personId)
        {

            Person model = new Person();
            using (var db = new Entities())
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

        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePerson(PersonVM newPerson)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
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
                    };
                    context.Person.Add(pers);
                    context.SaveChanges();
                }
                return RedirectToAction("ListOfPeople");
            }
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            return View(newPerson);

        }
    }
}