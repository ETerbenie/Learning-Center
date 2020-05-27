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
    public class ReportController : Controller
    {
        ReportAdapter reportAdapter;
        LinkDriverCourseAdapter linkDriverCourseAdapter;
        public ReportController()
        {
            reportAdapter = new ReportAdapter();
            linkDriverCourseAdapter = new LinkDriverCourseAdapter();
        }


        // REDIRECTS

        public ActionResult RedirectToReportsTab()
        {
            return View("~/Views/NavBar/ReportsTab.cshtml");
        }


        public ActionResult GetDriverIdForReporting(string driverid)
        {
            ReportModel sendDriverId = new ReportModel();
            sendDriverId.DriverId = driverid;

            return RedirectToAction("SelectAllCoursesAssignedToDriverToReport", "Report", sendDriverId);
        }

        // SELECT 

        public ActionResult SearchAssignedDriverId()
        {
            return View("~/Views/Report/Assigned/SearchAssignedDriverId.cshtml");
        }

        public ActionResult SearchInProgressDriverId()
        {
            return View("~/Views/Report/InProgress/SearchInProgressDriverId.cshtml");
        }

        public ActionResult SearchPastDueDriverId()
        {
            return View("~/Views/Report/PastDue/SearchPastDueDriverId.cshtml"); 
        }

        public ActionResult SearchCompleteDriverId()
        {
            return View("~/Views/Report/Completed/SearchCompleteDriverId.cshtml");
        }

        //public ActionResult GetCoursesForDriver(string driverid, string state)
        //{
        //    var courses = report
        //}


        public ActionResult GetAssignedCoursesForDriver(string driverId)
        {
            var assignedCourses = reportAdapter.GetAllAssignedCoursesForDriver(driverId);

            return View("~/Views/Report/Assigned/AssignedCoursesReport.cshtml", assignedCourses);
        }

        public ActionResult GetInProgressCoursesForDriver(string driverId)
        {
            var inprogressCourses = reportAdapter.GetAllInProgressCoursesForDriver(driverId);

            return View("~/Views/Report/InProgress/InProgressReport.cshtml", inprogressCourses);
        }

        public ActionResult GetPastDueCoursesForDriver(string driverId)
        {
            var pastDueCourses = reportAdapter.GetAllPastDueCoursesForDriver(driverId);

            return View("~/Views/Report/PastDue/PastDueReport.cshtml", pastDueCourses);
        }

        public ActionResult GetCompletedCoursesForDriver(string driverId)
        {
            var completedCourses = reportAdapter.GetAllCompletedCoursesForDriver(driverId);

            return View("~/Views/Report/Completed/CompletedReport.cshtml", completedCourses);
        }

        // Reporting for ALL Drivers
        

        public ActionResult GetAssignedCoursesForAllDrivers()
        {
            var driverAssigned = reportAdapter.GetAssignedCoursesForAllDrivers();
            if (driverAssigned == null)
            {
                Log.Info("No Assigned Courses For All Drivers");
                return View("~/Views/Report/Error/NoAssignedCoursesForAllDriversError.cshtml");
            }
            else
            {
                return View("~/Views/Report/Assigned/AssignedAllDriversReport.cshtml", driverAssigned);
            }
        }

        public ActionResult GetInProgressCoursesForAllDrivers()
        {
            try
            {
                var driverInProgress = reportAdapter.GetInProgressCoursesForAllDrivers();
                if(driverInProgress == null)
                {
                    Log.Info("No In Progress Courses For All Drivers");
                    return View("~/Views/Report/Error/NoInProgressCoursesForAllDriversError.cshtml");
                }
                else
                {
                    return View("~/Views/Report/InProgress/InProgressAllDriversReport.cshtml", driverInProgress);
                }

            }
            catch 
            {
                Log.Info("No In Progress Courses For All Drivers");
                return View("~/Views/Report/Error/NoInProgressCoursesForAllDriversError.cshtml");
            }
        }


        public ActionResult GetPastDueCoursesForAllDrivers()
        {
            var driversPastDue = reportAdapter.GetPastDueCoursesForAllDrivers();
            if(driversPastDue == null)
            {
                Log.Info("No Past Due Courses For All Drivers");
                return View("~/Views/Report/Error/NoPastDueCoursesForAllDriversError.cshtml");
            }
            else
            {
                return View("~/Views/Report/PastDue/PastDueAllDriversReport.cshtml", driversPastDue);
            }
        }

        public ActionResult GetCompletedCoursesForAllDrivers()
        {
            var driverComplete = reportAdapter.GetCompletedCoursesForAllDrivers();
            if(driverComplete == null)
            {
                Log.Info("No Completed Courses For All Drivers");
                return View("~/Views/Report/Error/NoCompletedCoursesForAllDriversError.cshtml");
            }
            else
            {
                return View("~/Views/Report/Completed/CompleteAllDriversReport.cshtml", driverComplete);
            }
        }

        // GET COUNTS

        public ActionResult GetCountOfAssignedCourses(string driverid)
        {
            var count = reportAdapter.CountOfAllAssignedCourses(driverid);
            var total = reportAdapter.GetAllCoursesForDriver(driverid).Count();
            ReportModel model = new ReportModel
            {
                DriverId = driverid,
                Number_Of_Assigned_Courses = count,
                Number_Of_Total_Courses = total
            };
            if(model.Number_Of_Assigned_Courses <= 0)
            {
                return View("~/Views/Report/Error/NoAssignedCourses.cshtml");
            }
            else
            {
                return View("~/Views/Report/Assigned/AssignedReportView.cshtml", model);
            }
        }

        public ActionResult GetCountOfInProgressCourses(string driverid)
        {
            var count = reportAdapter.CountOfAllInProgressCourses(driverid);
            var total = reportAdapter.GetAllCoursesForDriver(driverid).Count();
            ReportModel model = new ReportModel
            {
                DriverId = driverid,
                Number_Of_InProgress_Courses = count,
                Number_Of_Total_Courses = total
            };
            if (model.Number_Of_InProgress_Courses <= 0)
            {
                return View("~/Views/Report/Error/NoInProgress.cshtml");
            }
            else
            {
                return View("~/Views/Report/InProgress/InProgressReportView.cshtml", model);
            }
        }

        public ActionResult GetCountOfPastDueCourses(string driverid)
        {
            var count = reportAdapter.CountOfAllPastDueCourses(driverid);
            var total = reportAdapter.GetAllCoursesForDriver(driverid).Count();
            ReportModel model = new ReportModel
            {
                DriverId = driverid,
                Number_Of_PastDue_Courses = count,
                Number_Of_Total_Courses = total
            };
            if (model.Number_Of_PastDue_Courses <= 0)
            {
                return View("~/Views/Report/Error/NoPastDueCourses.cshtml");
            }
            else
            {
                return View("~/Views/Report/PastDue/PastDueReportView.cshtml", model);
            }
        }

        public ActionResult GetCountOfCompletedCourses(string driverid)
        {
            var count = reportAdapter.CountOfAllCompletedCourses(driverid);
            var total = reportAdapter.GetAllCoursesForDriver(driverid).Count();
            ReportModel model = new ReportModel
            {
                DriverId = driverid,
                Number_Of_Completed_Courses = count,
                Number_Of_Total_Courses = total
            };
            if (model.Number_Of_Completed_Courses <= 0)
            {
                return View("~/Views/Report/Error/NoCompletedCourses.cshtml");
            }
            else
            {
                return View("~/Views/Report/Completed/CompletedReportView.cshtml", model);
            }
        }


    }
}