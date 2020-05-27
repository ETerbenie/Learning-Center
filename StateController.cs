using Captivate.Adapters;
using Captivate.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Captivate.Controllers
{
    [Authorize]
    public class StateController : Controller
    {
        StateAdapter stateAdapter;
        public StateController()
        {
            stateAdapter = new StateAdapter();
        }


        // REDIRECTS


        public ActionResult RedirectToCourseTab()
        {
            return View("~/Views/NavBar/CourseTab.cshtml");
        }


        public ActionResult RedirectToAddStateView()
        {
            return View("~/Views/Admin/Add/AddState.cshtml");
        }


        // SELECT

        public ActionResult ViewAllStates()
        {
            var results = stateAdapter.SelectAllStates().OrderBy(x => x.Name);

            return View("~/Views/Admin/ViewAll/ViewAllStates.cshtml", results);
        }



        // ADD

        [HttpPost]
        public ActionResult AddState(State newState)
        {
            try
            {
                var results = stateAdapter.AddNewState(newState.Name);

                if (results != 1)
                    return View("Error");

                return RedirectToAction("ViewAllStates");
            }
            catch 
            {
                return View();
            }
        }


        // EDIT

        public ActionResult EditState(State state)
        {
            try
            {
                var results = stateAdapter.EditState(state);

                if (results != 1)
                    return View("Error");

                return RedirectToAction("ViewAllStates");
            }
            catch 
            {
                return View();
            }
        }


        // DELETE

        public ActionResult DeleteState(int state)
        {
            try
            {
                var results = stateAdapter.DeleteState(state);

                if (results != 1)
                    return View("Error");

                return RedirectToAction("ViewAllStates");
            }
            catch 
            {
                return View();
            }
        }
    }
}