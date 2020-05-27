using PTC.Database;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Captivate.Models;
using PTC;
using Captivate.Interfaces.Entities;

namespace Captivate.Adapters
{
    public class DriverAdapter
    {
        private IDatabase db;
        public DriverAdapter()
        {
            db = Database.CreateDatabase();
        }

        // SELECT

        public IEnumerable<DriverModel> SelectAllDrivers()
        {
            Log.MethodStart();

            try
            {
                var sql = @"select u.ID, u.DRIVER_ID, u.FIRST_NAME, u.LAST_NAME, u.EMAIL, u.CREATED_DATETIME, dg.ID as DRIVER_GROUP_ID, dg.NAME as DRIVER_GROUP_NAME from [USER] u
                            join DRIVER_GROUP dg on dg.ID = u.DRIVER_GROUP_ID";

                var dataSet = db.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverModel
                {
                    Id = r["ID"].To<int>(),
                    DriverId = r["DRIVER_ID"].To<string>(),
                    FirstName = r["FIRST_NAME"].To<string>(),
                    LastName = r["LAST_NAME"].To<string>(),
                    Email = r["EMAIL"].To<string>(),
                    Created_Datetime = r["CREATED_DATETIME"].To<DateTime>(),
                    DriverGroup = new DriverGroupModel { Id = r["DRIVER_GROUP_ID"].To<int>(), Name = r["DRIVER_GROUP_NAME"].To<string>() }
                }).ToList().OrderBy(x => x.LastName).OrderBy(x => x.FirstName);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all drivers from [USER] Table --- {ex} ");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverModel> SelectAdminLogins()
        {
            Log.MethodStart();

            try
            {
                var sql = @"select ID, USERNAME, PASSWORD from [USER]";

                var dataSet = db.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverModel
                {
                    Id = r["ID"].To<int>(),
                    Username = r["USERNAME"].To<string>(),
                    Password = r["PASSWORD"].To<string>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all drivers from [USER] Table --- {ex} ");
                Log.Error(ex);
                throw;
            }
        }

        //public IEnumerable<DriverLinkCourseModel> SelectAllCoursesForOneDriver(string driverId)
        //{
        //    try
        //    {
        //        var sql = $@"Select link.COURSE_ID, link.STATE, pr.NAME, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link
        //                    join [USER] us on us.ID = link.DRIVER_ID
        //                    join PRIORITY pr on pr.ID = link.PRIORITY
        //                    where us.DRIVER_ID = {driverId}";

        //        var dataSet = db.GetDataSet(sql);

        //        if (dataSet.IsEmptyDataSet())
        //            return null;

        //        var results = dataSet.Tables[0];
        //        return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
        //        {
        //            Id = r["ID"].To<int>(),
        //            DriverId = r["DRIVER_ID"].To<string>(), // potentially have to change this back to DriverModel?
        //            Course_Id = r["COURSE_ID"].To<int>(),
        //            State = r["STATE"].To<string>(),
        //            Priority = new Priority { Name = r["NAME"].To<string>() },
        //            Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
        //            Due_Date = r["DUE_DATE"].To<DateTime>()
        //        }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}

        // Test Version
        //public IEnumerable<DriverLinkCourseModel> SelectAllCoursesForOneDriverTest(DriverModel driverId)
        //{
        //    try
        //    {
        //        var sql = $@"Select COURSE_ID, STATE, PRIORITY, ASSIGNED_DATETIME, DUE_DATE from LINK_DRIVER_COURSE 
        //                    where DRIVER_ID = @DRIVER_ID";

        //        var dataSet = db.GetDataSet(sql);

        //        if (dataSet.IsEmptyDataSet())
        //            return null;

        //        var results = dataSet.Tables[0];
        //        return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
        //        {
        //            Id = r["ID"].To<int>(),
        //            DriverId = r["DRIVER_ID"].To<string>(), // potentially have to change this back to DriverModel?
        //            Course = new CourseModel { Id = r["COURSE_ID"].To<int>(), Title = r["TITLE"].To<string>() },
        //            State = r["STATE"].To<string>(),
        //            Priority = r["PRIORITY"].To<int>(),
        //            Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
        //            Due_Date = r["DUE_DATE"].To<DateTime>()
        //        }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}

        // ADD

        public int AddDriver(DriverModel driver)
        {
            Log.MethodStart();

            try
            {
                var sql = $@"INSERT into [USER] (DRIVER_ID, USERNAME, PASSWORD, FIRST_NAME, LAST_NAME, EMAIL, DRIVER_GROUP_ID, CLEARANCE_ID) 
                            values (@DRIVER_ID, @USERNAME, @PASSWORD, @FIRST_NAME, @LAST_NAME, @EMAIL, @DRIVER_GROUP_ID, 0)";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driver.DriverId),
                    new Parameter("@USERNAME", driver.Username),
                    new Parameter("@PASSWORD", driver.Password),
                    new Parameter("@FIRST_NAME", driver.FirstName),
                    new Parameter("@LAST_NAME", driver.LastName),
                    new Parameter("@EMAIL", driver.Email),
                    new Parameter("@DRIVER_GROUP_ID", driver.SelectedGroupId),
                    //new Parameter("@CLEARANCE_ID", driver.ClearanceId)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Add {driver.DriverId} to [USER] Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }

        public int AddAdmin(DriverModel admin)
        {
            Log.MethodStart();

            try
            {
                var sql = $@"INSERT into [USER] (DRIVER_ID, USERNAME, PASSWORD, FIRST_NAME, LAST_NAME, EMAIL, DRIVER_GROUP_ID, CLEARANCE) 
                            values (@DRIVER_ID, @USERNAME, @PASSWORD, @FIRST_NAME, @LAST_NAME, @EMAIL, @DRIVER_GROUP_ID, @CLEARANCE)";

                var parameters = new List<Parameter>
                {
                    new Parameter("@USERNAME", admin.Username),
                    new Parameter("@PASSWORD", admin.Password),
                    new Parameter("@FIRST_NAME", admin.FirstName),
                    new Parameter("@LAST_NAME", admin.LastName),
                    new Parameter("@EMAIL", admin.Email),
                    new Parameter("@CLEARANCE", "Admin"),
                    //new Parameter("@CLEARANCE_ID", driver.ClearanceId)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }



        // EDIT 

        public int EditDriver(DriverModel driver)
        {
            Log.MethodStart();

            try
            {
                var sql = @"UPDATE [USER] SET DRIVER_ID = @DRIVER_ID, USERNAME = @USERNAME, PASSWORD = @PASSWORD, FIRST_NAME = @FIRST_NAME, LAST_NAME = @LAST_NAME, EMAIL = @EMAIL, DRIVER_GROUP_ID = @DRIVER_GROUP_ID 
                            WHERE ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", driver.Id),
                    new Parameter("@DRIVER_ID", driver.DriverId),
                    new Parameter("@USERNAME", driver.Username),
                    new Parameter("@PASSWORD", driver.Password),
                    new Parameter("@FIRST_NAME", driver.FirstName),
                    new Parameter("@LAST_NAME", driver.LastName),
                    new Parameter("@EMAIL", driver.Email),
                    new Parameter("@DRIVER_GROUP_ID", driver.SelectedGroupId)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {driver.DriverId} in [USER] Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }


        // DELETE

        public int DeleteDriver(int driverId)
        {
            Log.MethodStart();

            try
            {
                var sql = @"DELETE from [USER] where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", driverId)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {driverId} from [USER] Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }
    }
}