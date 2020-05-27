using PTC;
using PTC.Database;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Captivate.Models;
using Captivate.Interfaces.Entities;

namespace Captivate.Adapters
{
    public class ReportAdapter
    {
        IDatabase database;
        public string Assigned = AppSettings.GetStringValue("Assigned"); // need to still add this to the webconfig
        public string InProgress = AppSettings.GetStringValue("In Progress"); // need to still add this to the webconfig
        public string Completed = AppSettings.GetStringValue("Completed"); // need to still add this to the webconfig
        public string PastDue = AppSettings.GetStringValue("PastDue"); // need to still add this to the webconfig

        public ReportAdapter()
        {
            database = Database.CreateDatabase();
        }

        // One Driver Reporting 
        public IEnumerable<DriverLinkCourseModel> GetAllCoursesForDriver(string driverid)
        {
            Log.MethodStart();

            try
            {
                var sql = @"select us.ID, cs.ID, us.DRIVER_ID, cs.TITLE, cs.URL, link.STATE, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid)
                };

                var dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { Id = r["ID"].To<int>(), DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Id = r["ID"].To<int>(), Title = r["TITLE"].To<string>(), URL = r["URL"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>().ToLocalTime(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get all courses for {driverid} from LINK_DRIVER_COURSE Table for Reporting --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetAllAssignedCoursesForDriver(string driverId)
        {
            Log.MethodStart();

            try
            {
                var sql = @"select us.ID, cs.ID, us.DRIVER_ID, cs.TITLE, cs.URL, link.STATE, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and 
                            link.STATE = @STATE";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverId),
                    new Parameter("@STATE", Assigned)
                };

                var dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { Id = r["ID"].To<int>(), DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Id = r["ID"].To<int>(), Title = r["TITLE"].To<string>(), URL = r["URL"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>().ToLocalTime(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get Assigned courses for {driverId} from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetAllInProgressCoursesForDriver(string driverId)
        {
            try
            {
                var sql = @"select us.ID, cs.ID, us.DRIVER_ID, cs.TITLE, cs.URL, link.STATE, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and 
                            link.STATE = @STATE";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverId),
                    new Parameter("@STATE", InProgress)
                };

                var dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { Id = r["ID"].To<int>(), DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Id = r["ID"].To<int>(), Title = r["TITLE"].To<string>(), URL = r["URL"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>().ToLocalTime(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get In Progress courses for {driverId} from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetAllPastDueCoursesForDriver(string driverid)
        {
            try
            {
                var sql = @"Select link.ID, us.DRIVER_ID, link.USER_ID, link.COURSE_ID, cs.TITLE, link.STATE, cs.URL, pr.NAME, link.ASSIGNED_DATETIME, link.DUE_DATE, link.COMPLETED_DATETIME from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join[USER] us on us.ID = link.DRIVER_ID
                            join PRIORITY pr on pr.ID = link.PRIORITY
                            where us.DRIVER_ID = @DRIVER_ID and link.STATE = @STATE";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid),
                    new Parameter("@STATE", PastDue)
                };

                var dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];

                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    Id = r["ID"].To<int>(),
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { Id = r["USER_ID"].To<int>(), DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Id = r["COURSE_ID"].To<int>(), Title = r["TITLE"].To<string>(), URL = r["URL"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Priority = new Priority { Name = r["NAME"].To<string>() },
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>().ToLocalTime(),
                    Due_Date = r["DUE_DATE"].To<DateTime>(),
                    Completed_DateTime = r["COMPLETED_DATETIME"].To<DateTime>().ToLocalTime()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get Past Due courses for {driverid} from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetAllCompletedCoursesForDriver(string driverId)
        {
            try
            {
                var sql = @"select us.ID, cs.ID, us.DRIVER_ID, cs.TITLE, cs.URL, link.STATE, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and 
                            link.STATE = @STATE";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverId),
                    new Parameter("@STATE", "Completed")
                };

                var dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { Id = r["ID"].To<int>(), DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Id = r["ID"].To<int>(), Title = r["TITLE"].To<string>(), URL = r["URL"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>().ToLocalTime(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get Completed courses for {driverId} from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }


        // All Driver Reporting
        public IEnumerable<DriverLinkCourseModel> GetAssignedCoursesForAllDrivers()
        {
            try
            {
                var sql = $@"select link.STATE, us.DRIVER_ID, cs.TITLE, link.ASSIGNED_DATETIME, link.DUE_DATE  from LINK_DRIVER_COURSE link
							join COURSE cs on cs.ID = link.COURSE_ID
							join [USER] us on us.ID = link.DRIVER_ID
							where link.STATE = 'Assigned'";

                var dataSet = database.GetDataSet(sql, CommandType.Text);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Title = r["TITLE"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get Assigned courses for all drivers from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetInProgressCoursesForAllDrivers()
        {
            try
            {
                var sql = $@"select link.STATE, us.DRIVER_ID, cs.TITLE, link.ASSIGNED_DATETIME, link.DUE_DATE  from LINK_DRIVER_COURSE link
							join COURSE cs on cs.ID = link.COURSE_ID
							join [USER] us on us.ID = link.DRIVER_ID
							where link.STATE = 'In Progress'";

                var dataSet = database.GetDataSet(sql, CommandType.Text);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Title = r["TITLE"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get In Progress courses for all drivers from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetPastDueCoursesForAllDrivers()
        {
            try
            {
                var sql = $@"select link.STATE, us.DRIVER_ID, cs.TITLE, link.ASSIGNED_DATETIME, link.DUE_DATE  from LINK_DRIVER_COURSE link
							join COURSE cs on cs.ID = link.COURSE_ID
							join [USER] us on us.ID = link.DRIVER_ID
							where link.STATE = 'Past Due'";

                var dataSet = database.GetDataSet(sql, CommandType.Text);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Title = r["TITLE"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get Past Due courses for all drivers from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<DriverLinkCourseModel> GetCompletedCoursesForAllDrivers()
        {
            try
            {
                var sql = $@"select link.STATE, us.DRIVER_ID, cs.TITLE, link.ASSIGNED_DATETIME, link.DUE_DATE  from LINK_DRIVER_COURSE link
							join COURSE cs on cs.ID = link.COURSE_ID
							join [USER] us on us.ID = link.DRIVER_ID
							where link.STATE = 'Completed'";

                var dataSet = database.GetDataSet(sql, CommandType.Text);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    DriverId = r["DRIVER_ID"].To<string>(),
                    Driver = new DriverModel { DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Title = r["TITLE"].To<string>() },
                    State = r["STATE"].To<string>(),
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to get Completed courses for all drivers from LINK_DRIVER_COURSE Table for Reporting ---  {ex}");
                Log.Error(ex);
                throw;
            }
        }


        // GET COUNTS FOR COURSES
        public int CountOfAllAssignedCourses(string driverid)
        {
            try
            {
                var sql = @"select count(cs.TITLE), us.DRIVER_ID  from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join[USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and
                            link.STATE = @STATE
                            group by us.DRIVER_ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid),
                    new Parameter("@STATE", Assigned)
                };

                var dataSet = database.ExecScalar(sql, CommandType.Text, parameters.ToArray());

                if(dataSet == null)
                {
                    return 0;
                }
                else
                {
                    return (int)dataSet;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public int CountOfAllInProgressCourses(string driverid)
        {
            try
            {
                var sql = @"select count(cs.TITLE), us.DRIVER_ID  from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join[USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and
                            link.STATE = @STATE
                            group by us.DRIVER_ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid),
                    new Parameter("@STATE", InProgress)
                };

                var dataSet = database.ExecScalar(sql, CommandType.Text, parameters.ToArray());

                if (dataSet == null)
                {
                    return 0;
                }
                else
                {
                    return (int)dataSet;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public int CountOfAllPastDueCourses(string driverid)
        {
            try
            {
                var sql = @"select count(cs.TITLE), us.DRIVER_ID  from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join[USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and
                            link.STATE = @STATE
                            group by us.DRIVER_ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid),
                    new Parameter("@STATE", PastDue)
                };

                var dataSet = database.ExecScalar(sql, CommandType.Text, parameters.ToArray());

                if (dataSet == null)
                {
                    return 0;
                }
                else
                {
                    return (int)dataSet;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public int CountOfAllCompletedCourses(string driverid)
        {
            try
            {
                var sql = @"select count(cs.TITLE), us.DRIVER_ID  from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join[USER] us on us.ID = link.DRIVER_ID
                            where us.DRIVER_ID = @DRIVER_ID and
                            link.STATE = @STATE
                            group by us.DRIVER_ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid),
                    new Parameter("@STATE", "Completed")
                };

                var dataSet = database.ExecScalar(sql, CommandType.Text, parameters.ToArray());

                if (dataSet == null)
                {
                    return 0;
                }
                else
                {
                    return (int)dataSet;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        
    }
}