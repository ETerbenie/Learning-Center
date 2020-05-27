using Captivate.Adapters;
using Captivate.Models;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Managers
{
    public static class PastDueCourseManager
    {

        public static void ValidatePastDue(IEnumerable<DriverLinkCourseModel> driverAssignedCourses)
        {
            Log.MethodStart();
            Log.Info($"Found {driverAssignedCourses.Count()} courses");

            //LinkDriverCourseAdapter linkDriverCourseAdapter = new LinkDriverCourseAdapter();
            //var coursesToUpdate = driverAssignedCourses.Where(x => x.Due_Date < DateTime.Now).ToList();

            //linkDriverCourseAdapter.UpdateCoursesToPastDue(coursesToUpdate);
            LinkDriverCourseAdapter linkDriverCourseAdapter = new LinkDriverCourseAdapter();

            foreach (var course in driverAssignedCourses)
            {
                if (DateTime.UtcNow > course.Due_Date && course.State != "Completed" && course.State != "In Progress")
                {
                    linkDriverCourseAdapter.UpdateCoursesToPastDue(course);
                    Log.Info($"Updating {course.Course.Title} assigned to {course.Driver.DriverId} to Past Due State");
                }
                else
                {
                    continue;
                }
            }
        }
    }
}