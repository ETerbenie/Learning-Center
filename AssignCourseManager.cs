using Captivate.Adapters;
using Captivate.Models;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Managers
{
    public class AssignCourseManager
    {
        SelectAdapter selectAdapter;
        AddAdapter addAdapter;
        string Available = AppSettings.GetStringValue("Available");
        string Assigned = AppSettings.GetStringValue("Assigned");
        string InProgress = AppSettings.GetStringValue("InProgress");
        string Complete = AppSettings.GetStringValue("Complete");

        public AssignCourseManager()
        {
            selectAdapter = new SelectAdapter();
            addAdapter = new AddAdapter();
        }

        public DriverLinkCourseModel AssignCourseToDriver(CourseModel selectedCourse, DriverModel driver, CourseConfigModel config)
        {
            try
            {
                var newEntry = new DriverLinkCourseModel()
                {
                    User_Id = driver.Id,
                    DriverId = driver.DriverId,
                    Driver_Group_Id = driver.Driver_Group_Id,
                    Course_Id = selectedCourse.Id,
                    Course_Type = selectedCourse.Course_Type,
                    Title = selectedCourse.Title,
                    State = Assigned,
                    Priority = config.Priority,
                    Required = config.Required,
                    Reactivate_Training = config.Reactivate_Training,
                    Hidden_Course = selectedCourse.Hidden_Course,
                    Assigned_Datetime = DateTime.Now
                };

                return newEntry;
            }
            catch (Exception ex)
            {
                Log.Trace(ex.ToString());
                throw;
            }
        }
    }
}