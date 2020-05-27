using Captivate.Adapters;
using Captivate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Managers
{
    public class CourseValidationManager
    {
        LinkDriverCourseAdapter linkDriverCourseAdapter;
        public CourseValidationManager()
        {
            linkDriverCourseAdapter = new LinkDriverCourseAdapter();
        }
        public DriverLinkCourseModel ChangeStateToComplete(DriverLinkCourseModel course)
        {
            linkDriverCourseAdapter.UpdateCoursesToComplete(course);
            return course;
        }
    }
}