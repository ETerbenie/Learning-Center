using Captivate.Adapters;
using Captivate.Interfaces.Entities;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Captivate.Controllers
{
    [Authorize]
    public class PriorityController : Controller
    {
        PriorityAdapter priorityAdapter;
        public PriorityController()
        {
            priorityAdapter = new PriorityAdapter();
        }



        // REDIRECTS

        public ActionResult RedirectToCourseTab()
        {
            return View("~/Views/NavBar/CourseTab.cshtml");
        }


        public ActionResult RedirectToAddPriorityView()
        {
            return View("~/Views/Admin/Add/AddPriority.cshtml");
        }

        // SELECT

        public ActionResult ViewAllPriorites()
        {
            var results = priorityAdapter.SelectAllPriorities().OrderBy(x => x.Name);

            return View("~/Views/Admin/ViewAll/ViewAllPriorites.cshtml", results);
        }


        // ADD

        [HttpPost]
        public ActionResult AddPriority(string name)
        {
            try
            {
                var results = priorityAdapter.AddNewPriority(name);

                if (results != 1)
                {
                    Log.Info($"Unable to Add Priority: {name} in PriorityController -- results == {results}");
                    return View("Error");
                }

                return RedirectToAction("ViewAllPriorites");
            }
            catch 
            {
                Log.Info($"Unable to Add Priority: {name} in PriorityController");
                return View("Error");
            }
        }


        // EDIT

        public ActionResult EditPriority(Priority priority)
        {
            try
            {
                var results = priorityAdapter.EditPriority(priority);

                if (results != 1)
                {
                    Log.Info($"Unable to Edit {priority.Name} in PriorityController -- results == {results}");
                    return View("Error");
                }

                return RedirectToAction("ViewAllPriorites");
            }
            catch 
            {
                Log.Info($"Unable to Edit {priority.Name} in PriorityController");
                return View("Error");
            }
        }


        // DELETE

        public ActionResult DeletePriority(int id)
        {
            try
            {
                var results = priorityAdapter.DeletePriority(id);

                if (results != 1)
                {
                    Log.Info($"Unable to Delete {id} in PriorityController -- results == {results}");
                    return View("Error");
                }

                return RedirectToAction("ViewAllPriorites");
            }
            catch 
            {
                Log.Info($"Unable to Delete {id} in PriorityController");
                return View("Error");
            }
        }
    }
}