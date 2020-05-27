using Captivate.Adapters;
using Captivate.Models;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Managers
{
    public class DriverValidationManager
    {
        DriverAdapter driverAdapter;
        public DriverValidationManager()
        {
            driverAdapter = new DriverAdapter();
        }

        public bool CheckDriverIdIsValid(DriverModel driver)
        {
            var driverList = driverAdapter.SelectAllDrivers();
            var targetDriver = driverList.Where(x => x.DriverId == driver.DriverId);

            if (targetDriver.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string CheckAllDriverInfoIsAdded(DriverModel newDriver)
        {
            if (newDriver.DriverId == null)
            {
                return "Error";
            }
            else if (newDriver.FirstName == null)
            {
                return "Error";
            }
            else if (newDriver.LastName == null)
            {
                return "Error";
            }
            else if(newDriver.Email == null)
            {
                return "Error";
            }
            else if(newDriver.Driver_Group_Id == 0)
            {
                return "Error";
            }
            else
            {
                return "No Error";
            }
        }
    }
}