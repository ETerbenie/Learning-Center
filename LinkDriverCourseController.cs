using Captivate.Adapters;
using Captivate.Helpers;
using Captivate.Interfaces.Entities;
using Captivate.Managers;
using Captivate.Models;
using PTC;
using PTC.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Captivate.Controllers
{
    [Authorize]
    public class LinkDriverCourseController : Controller
    {
        CourseAdapter courseAdapter;
        DriverAdapter driverAdapter;
        DriverGroupAdapter driverGroupAdapter;
        PriorityAdapter priorityAdapter;
        LinkDriverCourseAdapter linkDriverCourseAdapter;
        AdminAdapter adminAdapter;

        public LinkDriverCourseController()
        {
            courseAdapter = new CourseAdapter();
            driverAdapter = new DriverAdapter();
            driverGroupAdapter = new DriverGroupAdapter();
            priorityAdapter = new PriorityAdapter();
            linkDriverCourseAdapter = new LinkDriverCourseAdapter();
            adminAdapter = new AdminAdapter();
        }

        // REDIRECT

        [AllowAnonymous]
        public ActionResult GetDriverId(string driverid)
        {
            DriverLinkCourseModel sendDriverId = new DriverLinkCourseModel();
            sendDriverId.DriverId = driverid;

            return RedirectToAction("SelectAllCoursesAssignedToDriver", "LinkDriverCourse", sendDriverId);
        }

        public ActionResult RedirectToAssignTab()
        {
            return View("~/Views/NavBar/AssignTab.cshtml");
        }

        public ActionResult RedirectToAdminLogin()
        {
            return View("~/Views/Admin/Login/AdminLogin.cshtml");
        }

        public ActionResult RedirectToAssignCourseToDriver()
        {
            IEnumerable<CourseModel> courseList = courseAdapter.SelectAllCourses().OrderBy(x => x.Title);
            IEnumerable<DriverModel> driverList = driverAdapter.SelectAllDrivers().OrderBy(x => x.DriverId);
            IEnumerable<Priority> priorityList = priorityAdapter.SelectAllPriorities().OrderBy(x => x.Name);
           
            DriverLinkCourseModel linkModel = new DriverLinkCourseModel()
            {
                CourseList = new SelectList(courseList, "Id", "Title"),
                DriverList = new SelectList(driverList, "Id", "DriverId"),
                PriorityList = new SelectList(priorityList, "Id", "NAME") 
            };

            return View("~/Views/Admin/Assign/AssignCourseToDriver.cshtml", linkModel);
        }

        public ActionResult RedirectToAssignCourseToAllDrivers()
        {
            IEnumerable<CourseModel> courseList = courseAdapter.SelectAllCourses().OrderBy(x => x.Title);
            IEnumerable<Priority> priorityList = priorityAdapter.SelectAllPriorities().OrderBy(x => x.Name);

            DriverLinkCourseModel linkModel = new DriverLinkCourseModel()
            {
                CourseList = new SelectList(courseList, "Id", "Title"),
                PriorityList = new SelectList(priorityList, "Id", "NAME")
            };

            return View("~/Views/Admin/Assign/AssignCourseToAllDrivers.cshtml", linkModel);
        }

        public ActionResult RedirectToAssignCourseToDriverGroup()
        {
            IEnumerable<CourseModel> courseList = courseAdapter.SelectAllCourses().OrderBy(x => x.Title);
            IEnumerable<Priority> priorityList = priorityAdapter.SelectAllPriorities().OrderBy(x => x.Name);
            IEnumerable<DriverGroupModel> driverGroupList = driverGroupAdapter.SelectAllDriverGroups().OrderBy(x => x.Name);

            DriverLinkCourseModel assignmentModel = new DriverLinkCourseModel()
            {
                CourseList = new SelectList(courseList, "Id", "Title"),
                PriorityList = new SelectList(priorityList, "Id", "NAME"),
                GroupsList = new SelectList(driverGroupList, "Id", "NAME")
            };

            return View("~/Views/Admin/Assign/AssignCourseToDriverGroup.cshtml", assignmentModel);
        }

        //public ActionResult RedirectToEditAssignedCourse(int assignedCourse)
        //{
        //    var entryToUpdate = linkDriverCourseAdapter.SelectCourseToEdit(assignedCourse);
        //    return View("~/Views/Admin/Edit/EditAssignedCourseToDriver.cshtml", entryToUpdate);
        //}

        public ActionResult RedirectToEditAssignedCourse(int assignedCourse)
        {
            DriverLinkCourseModel entryToUpdate = linkDriverCourseAdapter.SelectCourseToEdit(assignedCourse);
            IEnumerable<Priority> priorityList = priorityAdapter.SelectAllPriorities();
            entryToUpdate.PriorityList = new SelectList(priorityList, "Id", "NAME");
            return View("~/Views/Admin/Edit/EditAssignedCourseToDriver.cshtml", entryToUpdate);
        }


        // SELECT

        public ActionResult SelectAllAssignedCourses()
        {
            try
            {
                IEnumerable<DriverLinkCourseModel> boardOverview = linkDriverCourseAdapter.SelectAllCourses().OrderBy(x => x.Driver.DriverId);

                PastDueCourseManager.ValidatePastDue(boardOverview);

                return View("~/Views/Admin/ViewAll/ViewAllAssignCourses.cshtml", boardOverview);
            }
            catch 
            {
                Log.Info($"Unable to find any courses in database");
                return View("~/Views/Error/SelectAllAssignedCoursesError.cshtml");
            }
        }

        public ActionResult SelectDriverLogin()
        {
            return View("~/Views/Driver/DriverLogin.cshtml");
        }

        public ActionResult SearchDriverIdOnTable(string driverId)
        {
            List<DriverLinkCourseModel> filteredList = new List<DriverLinkCourseModel>();

            if(!String.IsNullOrEmpty(driverId))
            {
                IEnumerable<DriverLinkCourseModel> drivers = linkDriverCourseAdapter.SelectAllCoursesForOneDriver(driverId);
                filteredList.AddRange(drivers);
            }

            return View("~/Views/Admin/ViewAll/ViewAllSearchedDriverCourses.cshtml", filteredList);
        }

        [AllowAnonymous]
        public ActionResult SelectAllCoursesAssignedToDriver(string driverid)
        {
            try
            {
                IEnumerable<DriverLinkCourseModel> driverCourses = linkDriverCourseAdapter.SelectAllCoursesForOneDriver(driverid).OrderBy(x => x.Due_Date);

                PastDueCourseManager.ValidatePastDue(driverCourses);

                return View("~/Views/Driver/DriverAssignedCourses.cshtml", driverCourses);
            }
            catch
            {
                Log.Info($"Unable to find any courses for {driverid}");
                return View("~/Views/Error/SelectAllCoursesAssignedToDriverError.cshtml");
            }
        }


        public ActionResult ValidateAdminUserLogin(string username, string password)
        {
            try
            {
                DriverModel results = adminAdapter.ValidateAdminUser(username, password);

                if (results.Username == username && results.Password == password)
                {
                    return RedirectToAction("SelectAllAssignedCourses");
                }
                else
                {
                    return View("Error");
                }
            }
            catch 
            {
                return View("~/Views/Admin/Error/ErrorAdminLogin.cshtml");
            }
        }


        // ADD
        [HttpPost]
        public ActionResult AssignCourseToDriver(DriverLinkCourseModel assignment)
        {
            try
            {
                assignment.State = "Assigned";
                int results = linkDriverCourseAdapter.AssignCourseToDriver(assignment);

                if (results != 1)
                {
                    Log.Info($"Unable to Assign {assignment.Course.Title} to {assignment.Driver.DriverId} -- results == {results}");
                    return View("~/Views/Error/AssignCourseToDriverError.cshtml");
                }

                return RedirectToAction("SelectAllAssignedCourses");
            }
            catch
            {
                Log.Info($"Unable to Assign course in LinkDriverCourseController");
                return View("~/Views/Error/AssignCourseToDriverError.cshtml");
            }
        }

        [HttpPost]
        public ActionResult AssignCourseToAllDrivers(DriverLinkCourseModel assignment)
        {
            try
            {
                int results = linkDriverCourseAdapter.AssignCourseToAllDrivers(assignment.SelectedCourseId, assignment.SelectedPriorityId,
                    assignment.Assigned_Datetime, assignment.Due_Date);

                if (results == 0)
                {
                    Log.Info($"Unable to Assign {assignment.Course.Title} to all drivers in LinkDriverCourseController -- results == {results}");
                    return View("~/Views/Error/EnterAllInfoError.cshtml");
                }

                return RedirectToAction("SelectAllAssignedCourses");
            }
            catch
            {
                Log.Info($"Unable to Assign course to all drivers in LinkDriverCourseController");
                return View("~/Views/Error/EnterAllInfoError.cshtml");
            }
        }

        public ActionResult AssignCourseToDriverGroup(DriverLinkCourseModel driverGroupAssignment)
        {
            try
            {
                int results = linkDriverCourseAdapter.AssignCourseToDriverGroup(driverGroupAssignment.SelectedCourseId, driverGroupAssignment.SelectedPriorityId,
                    driverGroupAssignment.Assigned_Datetime, driverGroupAssignment.Due_Date, driverGroupAssignment.SelectedGroupId);

                if(results == 0)
                {
                    Log.Info("Unable to assign course to driver group");
                    return RedirectToAction("SelectAllAssignedCourses");
                }

                return RedirectToAction("SelectAllAssignedCourses");
            }
            catch
            {
                Log.Info("Unable to assign course to driver group");
                return View();
            }
        }


        //[HttpPost]
        //public ActionResult AssignCourseToMultipleDrivers(List<DriverLinkCourseModel> driverList)
        //{
        //    try
        //    {
        //        foreach (var driver in driverList)
        //        {
        //            driver.State = "Assigned";
        //        }

        //        var results = linkDriverCourseAdapter.AssignCourseToMultipleDrivers(driverList);

        //        return RedirectToAction("SelectAllAssignedCourses");
        //    }
        //    catch 
        //    {
        //        return View();
        //    }
        //}


        // EDIT

        public ActionResult EditAssignedCourseToDriver(DriverLinkCourseModel editEntry)
        {
            try
            {
                int results = linkDriverCourseAdapter.EditAssignedCourseToDriver(editEntry);

                if (results != 1)
                {
                    Log.Info($"Unable to Edit {editEntry.Course.Title} to {editEntry.Driver.DriverId} in LinkDriverCourseController -- results == {results}");
                    return View("~/Views/Error/EnterAllInfoError.cshtml");
                }

                return RedirectToAction("SelectAllAssignedCourses");
            }
            catch
            {
                Log.Info($"Unable to Edit course for driver in LinkDriverCourseController");
                return View("~/Views/Error/EnterAllInfoError.cshtml");
            }
        }

        [AllowAnonymous]
        public void ChangeStateToInProgress(int id)
        {
            var changedToInProgress = linkDriverCourseAdapter.ChangeState(id, "In Progress");
        }

        // DELETE

        public ActionResult DeleteAssignedCourse(int assignmentId)
        {
            try
            {
                int results = linkDriverCourseAdapter.DeleteAssignedCourseToDriver(assignmentId);

                if (results != 1)
                {
                    Log.Info($"Unable to Delete {assignmentId} in LinkDriverCourseController -- results == {results}");
                    return View("Error");
                }

                return RedirectToAction("SelectAllAssignedCourses");
            }
            catch
            {
                Log.Info($"Unable to Delete {assignmentId} in LinkDriverCourseController");
                return View("Error");
            }
        }
    }
}