using Captivate.Adapters;
using Captivate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Managers
{
    public class AssignManager
    {
        LinkDriverCourseAdapter linkDriverCourseAdapter;
        DriverAdapter driverAdapter;
        public AssignManager()
        {
            linkDriverCourseAdapter = new LinkDriverCourseAdapter();
            driverAdapter = new DriverAdapter();
        }

        public void AssignCourseToMultipleDrivers(DriverLinkCourseModel course)
        {
            var driverList = driverAdapter.SelectAllDrivers();
            
            foreach (var driver in driverList)
            {
                linkDriverCourseAdapter.AssignCourseToDriver(course);
            }
        }

        //public IEnumerable<DriverLinkCourseModel> AddCourseToMultipleDrivers(DriverLinkCourseModel course)
        //{
        //    var assignments = linkDriverCourseAdapter.AssignCourseToDriver(course);
        //    var driverList = driverAdapter.SelectAllDrivers();

        //    foreach (var driver in driverList)
        //    {
                
        //    }
        //}
    }
}