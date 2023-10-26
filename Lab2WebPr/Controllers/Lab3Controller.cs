using Lab2WebPr.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab2WebPr.Controllers
{
    public class Lab3Controller : Controller
    {
        // GET: Lab3
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePerson(Person newPerson)
        {
            if(ModelState.IsValid)
            {
                return RedirectToAction("ListOfPeople");
            }
            return View(newPerson);
        }
    }
}