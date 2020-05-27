using Captivate.Adapters;
using Captivate.Models;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Captivate.Controllers
{
    [Authorize]
    public class DriverGroupController : Controller
    {
        DriverGroupAdapter driverGroupAdapter;
        public DriverGroupController()
        {
            driverGroupAdapter = new DriverGroupAdapter();
        }

        // REDIRECTS

        public ActionResult RedirectToDriverTab()
        {
            return View("~/Views/NavBar/DriverTab.cshtml");
        }


        public ActionResult RedirectToAddDriverGroupView()
        {
            return View("~/Views/Admin/Add/AddDriverGroup.cshtml");
        }

        public ActionResult RedirectToEditDriverGroup(DriverGroupModel driverGroup)
        {
            return View("~/Views/Admin/Edit/EditDriverGroup.cshtml", driverGroup);
        }


        // SELECT

        public ActionResult ViewAllDriverGroups()
        {
            try
            {
                var results = driverGroupAdapter.SelectAllDriverGroups().OrderBy(x => x.Name);
                return View("~/Views/Admin/ViewAll/ViewAllDriverGroups.cshtml", results);
            }
            catch 
            {
                Log.Info("No driver groups to display");
                return View("~/Views/Error/ViewAllDriverGroupsError.cshtml");
            }
            
        }



        // ADD

        public ActionResult AddDriverGroup(DriverGroupModel driverGroup)
        {
            try
            {
                var results = driverGroupAdapter.AddDriverGroup(driverGroup);

                if (results != 1)
                    return View("~/Views/Error/AddDriverGroupError.cshtml");

                return RedirectToAction("ViewAllDriverGroups");

            }
            catch
            {
                return View("~/Views/Error/AddDriverGroupError.cshtml");
            }
        }


        // EDIT

        public ActionResult EditDriverGroup(DriverGroupModel driverGroup)
        {
            try
            {
                var results = driverGroupAdapter.EditDriverGroup(driverGroup);

                if (results != 1)
                    return View("~/Views/Error/EnterAllInfoError.cshtml");

                return RedirectToAction("ViewAllDriverGroups");
            }
            catch
            {
                return View("~/Views/Error/EnterAllInfoError.cshtml");
            }
        }


        // DELETE

        public ActionResult DeleteDriverGroup(DriverGroupModel deleteGroup)
        {
            try
            {
                var results = driverGroupAdapter.DeleteDriverGroup(deleteGroup);

                if (results != 1)
                {
                    Log.Info($"Unable to Delete {deleteGroup.Name} -- results == {results}");
                    return View("~/Views/Error/DeleteCourseTypeError.cshtml");
                }

                return RedirectToAction("ViewAllDriverGroups");
            }
            catch
            {
                Log.Info($"Unable to Delete {deleteGroup.Name} in DriverGroupController --- Id == {deleteGroup.Id}");
                return View("~/Views/Error/DeleteDriverGroupError.cshtml");
            }
        }
    }
}