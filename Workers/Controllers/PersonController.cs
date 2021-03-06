﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Workers.Controllers
{
   [Authorize]
  
    public class PersonController : Controller
    {

        public IEFGenericRepository<Team> TeamRepository { get; set; }

        public IEFGenericRepository<Person> Personrepository { get; set; }

        public PersonController(IEFGenericRepository<Person> personrepository, IEFGenericRepository<Team> teamRepository)
        {
            Personrepository = personrepository;
            TeamRepository = teamRepository;

        }
    
        // GET: /<controller>/
        [HttpGet]
        [Route("/PersonController/CreatenewPerson")]
        [Authorize(Roles = "admin")]
        public IActionResult AddnewPerson() {
            ViewBag.Teams = TeamRepository.Get().ToList();
            ViewData["Message"] = "Add NewPerson  page.";
            return View();
        }
        [HttpPost]
        [Route("")]
        public IActionResult CreatenewPerson(Person person)
        {
           
            Person newPerson = new Person();
            newPerson.Id = Guid.NewGuid();
            newPerson.Year = DateTime.Now.Year;
            newPerson.Name = person.Name;
            newPerson.LastName = person.LastName;
           // newPerson.TeamId = TeamId;
            newPerson.Days = person.Days;
            Team team = new Team();
            bool teamChecknull = true;
            if (person.TeamId == null)
                teamChecknull = false;
            else
            {
                team = TeamRepository.FindById((Guid)person.TeamId);
            }
            if (!teamChecknull)
                newPerson.Team = null;
            else
            {
                newPerson.Team = team;
            }
           
            if(newPerson.Days>18||newPerson.Days<0)
                Redirect("~/Home/Workers/Home/Workers");
            Personrepository.Create(newPerson);
            return Redirect("~/Home/Workers/Home/Workers");
        }
        [HttpGet]
        [Route("ChangePerson/{personId}")]
        [Authorize(Roles = "admin,employee")]
        public IActionResult ChangePerson(Guid personId)
        {
            ViewBag.person = Personrepository.FindById(personId);

            Person person = Personrepository.IncludeGet(p => p.Team).FirstOrDefault(p => p.Id == personId);
            ViewBag.Teams = TeamRepository.Get().ToList();
            return View(person);
        }
        [HttpPost]
        [Route("ChangePerson/{personId}")]
        [Authorize(Roles = "admin,employee")]
        public IActionResult ChangePersonPost(Person person,Guid personId)
        
        {
            Team team = new Team();
            bool teamChecknull = true;
            if(person.TeamId==null)
                 teamChecknull=false;
            else { 
                 team = TeamRepository.FindById((Guid)person.TeamId);
            }
            Person Updateperson = Personrepository.IncludeGet(p=>p.Team).FirstOrDefault(p=>p.Id==personId);
            Updateperson.Name = person.Name;
            Updateperson.LastName = person.LastName;
            Updateperson.Days = person.Days;
            if (!teamChecknull)
                Updateperson.Team = null;
            else
            {
                Updateperson.Team = team;
            }
            if(Updateperson.Days<0||Updateperson.Days>20)
                return Redirect("~/Home/Workers/Home/Workers");
            Personrepository.Update(Updateperson);
            return Redirect("~/Home/Workers/Home/Workers");
        }
        
              [Route("DeletePerson/{personId}")]
        [Authorize(Roles = "admin")]
        public IActionResult DeletePerson(Guid personId)
        {
            Person person = Personrepository.FindById(personId);
            Personrepository.Remove(person);
            return Redirect("~/Home/Workers/Home/Workers");
        }
    }
}
