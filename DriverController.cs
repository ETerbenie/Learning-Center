using Captivate.Adapters;
using Captivate.Managers;
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
    public class DriverController : Controller
    {
        CourseAdapter courseAdapter;
        DriverAdapter driverAdapter;
        DriverGroupAdapter driverGroupAdapter;
        DriverValidationManager driverValidationManager;
        public DriverController()
        {
            courseAdapter = new CourseAdapter();
            driverAdapter = new DriverAdapter();
            driverGroupAdapter = new DriverGroupAdapter();
            driverValidationManager = new DriverValidationManager();
        }

        // REDIRECTS

        public ActionResult RedirectToDriverTab()
        {
            return View("~/Views/NavBar/DriverTab.cshtml");
        }

        public ActionResult RedirectToDriverAddView()
        {
            try
            {
                var driverGroupList = driverGroupAdapter.SelectAllDriverGroups();
                DriverModel driverModel = new DriverModel()
                {
                    GroupsList = new SelectList(driverGroupList, "Id", "Name")
                };

                return View("~/Views/Admin/Add/AddDriver.cshtml", driverModel);
            }
            catch (Exception)
            {
                Log.Info($"Driver cannot be added because no driver group exists in database");
                return View("~/Views/Error/RedirectToDriverAddViewError.cshtml");
            }
        }


        public ActionResult RedirectToEditDriver(DriverModel driver)
        {
            var driverGroupList = driverGroupAdapter.SelectAllDriverGroups();
            driver.GroupsList = new SelectList(driverGroupList, "Id", "Name");

            return View("~/Views/Admin/Edit/EditDriver.cshtml", driver);
        }


        // SELECT

        public ActionResult ViewAllDrivers()
        {
            try
            {
                var results = driverAdapter.SelectAllDrivers().OrderBy(x => x.DriverId);
                return View("~/Views/Admin/ViewAll/ViewAllDrivers.cshtml", results);
            }
            catch
            {
                Log.Info($"No Drivers Found in Database");
                return View("~/Views/Error/ViewAllDriverError.cshtml");
            }
            
        }

        // ADD

        //public ActionResult AddDriver(DriverModel newDriver)
        //{
        //    try
        //    {
        //        var validateInfo = driverValidationManager.CheckAllDriverInfoIsAdded(newDriver);
        //        if (validateInfo == "Error")
        //        {
        //            return View("~/Views/Error/AddDriverError.cshtml");
        //        }
        //        else
        //        {
        //            newDriver.DriverId.ToUpper();

        //            var results = driverAdapter.AddDriver(newDriver);

        //            if (results != 1)
        //                return View("Error");

        //            return RedirectToAction("ViewAllDrivers");
        //        }

        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public ActionResult AddDriver(DriverModel newDriver)
        {
            try
            {
                newDriver.DriverId.ToUpper();
                newDriver.Clearance = "Driver";

                var results = driverAdapter.AddDriver(newDriver);

                if (results != 1)
                {
                    Log.Info($"Unable to Add {newDriver.DriverId} in DriverController -- results == {results}");
                    return View("~/Views/Error/AddDriverError.cshtml");
                }

                return RedirectToAction("ViewAllDrivers");

            }
            catch
            {
                Log.Info($"Unable to Add {newDriver.DriverId} in DriverController");
                return View("~/Views/Error/AddDriverError.cshtml");
            }
        }


        // EDIT

        public ActionResult EditDriver(DriverModel driver)
        {
            try
            {
                var results = driverAdapter.EditDriver(driver);

                if (results != 1)
                {
                    Log.Info($"Unable to Edit {driver.DriverId} in DriverController -- results == {results}");
                    return View("~/Views/Error/EnterAllInfoError.cshtml");
                }

                return RedirectToAction("ViewAllDrivers");
            }
            catch
            {
                Log.Info($"Unable to Edit {driver.DriverId} in DriverController");
                return View("~/Views/Error/EnterAllInfoError.cshtml");
            }
        }


        // DELETE 

        public ActionResult DeleteDriver(int driverId)
        {
            try
            {
                var results = driverAdapter.DeleteDriver(driverId);

                if (results != 1)
                {
                    Log.Info($"Unable to Delete {driverId} in DriverController -- results == {results}");
                    return View("Error");
                }

                return RedirectToAction("ViewAllDrivers");
            }
            catch
            {
                return View("Error");
            }
        }


        // LOGIN

        public ActionResult DriverLogin(DriverModel driverLogin)
        {
            try
            {
                var verifyLogin = driverValidationManager.CheckDriverIdIsValid(driverLogin);
                if (verifyLogin == true)
                {
                    var driverCourses = courseAdapter.SelectAllCourses();
                    return View("DriverAssignedCourses", driverCourses);
                }
                else
                {
                    return View("ErrorDriverLogin");
                }
            }
            catch
            {
                return View();
            }
        }


        // SEARCH

        public ActionResult SearchDriver(string driverId)
        {
            try
            {
                var driver = driverAdapter.SelectAllDrivers().OrderBy(x => x.FirstName).OrderBy(x => x.LastName);

                return View("~/Views/Admin/Assign/AssignCourseToDriver.cshtml", driver.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

    }
}