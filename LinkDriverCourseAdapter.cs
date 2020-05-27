using PTC.Database;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Captivate.Models;
using PTC;
using System.Text;
using Captivate.Interfaces.Entities;

namespace Captivate.Adapters
{
    public class LinkDriverCourseAdapter
    {
        IDatabase database;
        public LinkDriverCourseAdapter()
        {
            database = Database.CreateDatabase();
        }

        // SELECT

        public IEnumerable<DriverLinkCourseModel> SelectAllCourses()
        {
            Log.MethodStart();

            try
            {
                string sql = @"select link.ID, us.DRIVER_ID, link.USER_ID, link.COMPLETED_DATETIME, link.COURSE_ID, cs.TITLE, cs.URL, link.STATE, link.PRIORITY, pr.NAME, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link 
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            join PRIORITY pr on pr.ID = link.PRIORITY"; 

                DataSet dataSet = database.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                DataTable results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverLinkCourseModel
                {
                    Id = r["ID"].To<int>(),
                    Driver = new DriverModel { Id = r["USER_ID"].To<int>(), DriverId = r["DRIVER_ID"].To<string>() },
                    Course = new CourseModel { Id = r["COURSE_ID"].To<int>(), Title = r["TITLE"].To<string>(), URL = r["URL"].To<string>() },
                    State = r["STATE"].To<string>(),
                    //TestPriority = new Priority { Name = r["NAME"].To<string>() },
                    Priority = new Priority { Name = r["NAME"].To<string>() },
                    Assigned_Datetime = r["ASSIGNED_DATETIME"].To<DateTime>(),
                    Completed_DateTime = r["COMPLETED_DATETIME"].To<DateTime?>(),
                    Due_Date = r["DUE_DATE"].To<DateTime>()
                }).ToList();

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all courses from LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }



        public DriverLinkCourseModel SelectCourseToEdit(int entryToEdit)
        {
            Log.MethodStart();

            try
            {
                string sql = @"select link.ID, us.DRIVER_ID, cs.ID as CourseId, cs.TITLE, pr.ID as PriorityId, pr.NAME, link.ASSIGNED_DATETIME, link.DUE_DATE from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            join PRIORITY pr on pr.ID = link.PRIORITY
                            where link.ID = @ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@ID", entryToEdit)
                };

                DataSet dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                DataRow results = dataSet.Tables[0].Rows[0];

                DriverLinkCourseModel newEntry = new DriverLinkCourseModel
                {
                    Id = results["ID"].To<int>(),
                    DriverId = results["DRIVER_ID"].To<string>(),
                    Title = results["TITLE"].To<string>(),
                    SelectedCourseId = results["CourseId"].To<int>(),
                    Priority = new Priority { Name = results["NAME"].To<string>() },
                    SelectedPriorityId = results["PriorityId"].To<int>(),
                    Assigned_Datetime = results["ASSIGNED_DATETIME"].To<DateTime>().ToLocalTime(),
                    Due_Date = results["DUE_DATE"].To<DateTime>()
                };

                return newEntry;
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {entryToEdit} from LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex); 
                throw;
            }
        }


        public IEnumerable<DriverLinkCourseModel> SelectAllCoursesForOneDriver(string driverid)
        {
            Log.MethodStart();

            try
            {
                string sql = @"select link.ID, us.DRIVER_ID, link.USER_ID, link.COURSE_ID, cs.TITLE, link.STATE, cs.URL, pr.NAME, link.ASSIGNED_DATETIME, link.DUE_DATE, link.COMPLETED_DATETIME from LINK_DRIVER_COURSE link
                            join COURSE cs on cs.ID = link.COURSE_ID
                            join [USER] us on us.ID = link.DRIVER_ID
                            join PRIORITY pr on pr.ID = link.PRIORITY
                            where us.DRIVER_ID = @DRIVER_ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@DRIVER_ID", driverid)
                };

                DataSet dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                DataTable results = dataSet.Tables[0];
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
                    Completed_DateTime = r["COMPLETED_DATETIME"].To<DateTime>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all courses for {driverid} from LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }


        

        // ADD

        public int AssignCourseToDriver(DriverLinkCourseModel assignToDriver)
        {
            Log.MethodStart();

            try
            {
                string sql = @"Insert into LINK_DRIVER_COURSE (USER_ID, DRIVER_ID, COURSE_ID, STATE, PRIORITY, ASSIGNED_DATETIME, DUE_DATE, CREATED_DATETIME) 
                            values (@USER_ID, @DRIVER_ID, @COURSE_ID, @STATE, @PRIORITY, @ASSIGNED_DATETIME, @DUE_DATE, @CREATED_DATETIME)"; 

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@USER_ID", assignToDriver.User_Id),
                    new Parameter("@DRIVER_ID", assignToDriver.SelectedDriverId),
                    new Parameter("@COURSE_ID", assignToDriver.SelectedCourseId),
                    new Parameter("@STATE", assignToDriver.State),
                    new Parameter("@PRIORITY", assignToDriver.SelectedPriorityId),
                    new Parameter("@ASSIGNED_DATETIME", DateTime.UtcNow),
                    new Parameter("@DUE_DATE", assignToDriver.Due_Date),
                    new Parameter("@CREATED_DATETIME", DateTime.UtcNow)
                };
                //var test = database.ExecNonQuery(sql, CommandType.Text, parameters);
                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Assign {assignToDriver.Course.Title} to {assignToDriver.Driver.DriverId} in LINK_DRIVER_COURSE Table --- {ex} ");
                Log.Error(ex);
                throw;
            }
        }

        //public int AddMultipleCoursesToDriver(List<DriverLinkCourseModel> driverList, DateTime dueDate)
        //{
        //    try
        //    {
        //        var sb = new StringBuilder();
        //        sb.Append("Insert into LINK_DRIVER_COURSE (DRIVER_ID, COURSE_ID, STATE, ASSIGNED_DATETIME, DUE_DATE) values");

        //        var parameters = new List<Parameter>();
        //        for (var i = 0; i < driverList.Count(); i++)
        //        {
        //            sb.Append($@"@DRIVER_ID{i}, @COURSE_ID{i}, @STATE{i}, @ASSIGNED_DATETIME{i}, @DUE_DATE{i},");

        //            parameters.Add(new Parameter($"@DRIVER_ID", driverList[i].DriverId));
        //            parameters.Add(new Parameter($"@COURSE_ID", driverList[i].SelectedCourseId));
        //            parameters.Add(new Parameter($"@STATE", "Assigned"));
        //            parameters.Add(new Parameter($"@ASSIGNED_DATETIME", DateTime.UtcNow.ToLocalTime()));
        //            parameters.Add(new Parameter($"@DUE_DATE", dueDate.ToLocalTime()));
        //        };

        //        var sql = sb.ToString().TrimEnd(',');
        //        var result = database.ExecNonQuery(sql, CommandType.Text, parameters.ToArray());
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}

        // Need to test this out, not sure if this works
        public int AssignCourseToMultipleDrivers(List<DriverLinkCourseModel> assignedCourses)
        {
            Log.MethodStart();

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Insert into LINK_DRIVER_COURSE ( DRIVER_ID, COURSE_ID, STATE, PRIORITY, ASSIGNED_DATETIME, DUE_DATE, CREATED_DATETIME) values");

                List<Parameter> parameters = new List<Parameter>();

                for (int i = 0; i < assignedCourses.Count; i++)
                {
                    sb.Append($@"(@DRIVER_ID{i}, @COURSE_ID{i}, @STATE{i}, @PRIORITY{i}, @ASSIGNED_DATETIME{i}, @DUE_DATE,{i}, CREATED_DATETIME{i},");

                    parameters.Add(new Parameter($"@DRIVER_ID", assignedCourses[i].DriverId));
                    //parameters.Add(new Parameter($"@COURSE_ID", assignedCourses[i].Course_Id));
                    parameters.Add(new Parameter($"@COURSE_ID", assignedCourses[i].SelectedCourseId));
                    parameters.Add(new Parameter($"@STATE", assignedCourses[i].State));
                    parameters.Add(new Parameter($"@PRIORITY", assignedCourses[i].Priority));
                    parameters.Add(new Parameter($"@ASSIGNED_DATETIME", DateTime.UtcNow));
                    parameters.Add(new Parameter($"@DUE_DATE", assignedCourses[i].Due_Date));
                    parameters.Add(new Parameter("@CREATED_DATETIME", DateTime.UtcNow.ToLocalTime()));
                }

                string sql = sb.ToString().TrimEnd(',');
                int result = database.ExecNonQuery(sql, CommandType.Text, parameters.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Assign {assignedCourses.FirstOrDefault().Course.Title} to all drivers in LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public int AssignCourseToAllDrivers(int courseId, int priority, DateTime assignedDateTime, DateTime dueDateTime)
        {
            Log.MethodStart();

            try
            {
                string sql = @"Insert into LINK_DRIVER_COURSE ( USER_ID, DRIVER_ID, COURSE_ID, STATE, PRIORITY, ASSIGNED_DATETIME, DUE_DATE, CREATED_DATETIME)
                            select 0, id, @COURSE_ID, 'Assigned', @PRIORITY, @ASSIGNED_DATETIME, @DUE_DATE, @CREATED_DATETIME from [USER]";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@COURSE_ID", courseId),
                    new Parameter("@PRIORITY", priority),
                    new Parameter("@ASSIGNED_DATETIME", DateTime.Now),
                    new Parameter("@DUE_DATE", dueDateTime),
                    new Parameter("@CREATED_DATETIME", DateTime.UtcNow.ToLocalTime())
                };

                int result = database.ExecNonQuery(sql, CommandType.Text, parameters.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Assign course ID {courseId} to all drivers in LINK_DRIVER_COURSE Table ---- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public int AssignCourseToDriverGroup(int courseId, int priority, DateTime assignedDateTime, DateTime dueDateTime, int selectedDriverGroup)
        {
            try
            {
                string sql = @"Insert into LINK_DRIVER_COURSE ( USER_ID, DRIVER_ID, COURSE_ID, STATE, PRIORITY, ASSIGNED_DATETIME, DUE_DATE, CREATED_DATETIME)
                            select 0, u.ID, @COURSE_ID, 'Assigned', @PRIORITY, @ASSIGNED_DATETIME, @DUE_DATE, @CREATED_DATETIME from [USER] u 
                            join DRIVER_GROUP dg on  dg.ID = u.DRIVER_GROUP_ID
                            where dg.NAME = @DRIVER_GROUP";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@COURSE_ID", courseId),
                    new Parameter("@PRIORITY", priority),
                    new Parameter("@DRIVER_GROUP", selectedDriverGroup),
                    new Parameter("@ASSIGNED_DATETIME", DateTime.Now),
                    new Parameter("@DUE_DATE", dueDateTime),
                    new Parameter("@CREATED_DATETIME", DateTime.UtcNow.ToLocalTime()),
                };

                int result = database.ExecNonQuery(sql, CommandType.Text, parameters.ToArray());
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }


        // EDIT

        public int EditAssignedCourseToDriver(DriverLinkCourseModel updatedEntry)
        {
            Log.MethodStart();

            try
            {
                string sql = @"UPDATE LINK_DRIVER_COURSE Set PRIORITY = @PRIORITY, ASSIGNED_DATETIME = @ASSIGNED_DATETIME, DUE_DATE = @DUE_DATE
                            Where ID = @ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@ID", updatedEntry.Id),
                    new Parameter("@PRIORITY", updatedEntry.SelectedPriorityId),
                    new Parameter("@ASSIGNED_DATETIME", DateTime.UtcNow.ToLocalTime()),
                    new Parameter("@DUE_DATE", updatedEntry.Due_Date)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {updatedEntry.Course.Title} at {updatedEntry.Id} in LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public int UpdateCoursesToPastDue(DriverLinkCourseModel courseToChange)
        {
            Log.MethodStart();

            try
            {
                string sql = @"update LINK_DRIVER_COURSE set STATE = @STATE where ID = @ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@ID", courseToChange.Id),
                    new Parameter("@STATE", "Past Due")
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Update {courseToChange.Course.Title} at {courseToChange.Id} state to Past Due in LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        //public int UpdateCoursesToPastDue(List<DriverLinkCourseModel> courses)
        //{
        //    try
        //    {
        //        StringBuilder sqlBuilder = new StringBuilder();
        //        var parameters = new List<Parameter>();

        //        for (int i = 0; i < courses.Count; i++)
        //        {
        //            sqlBuilder.Append($@"update LINK_DRIVER_COURSE set STATE = 'Past Due' where ID = @ID");

        //            parameters.Add(new Parameter($"@ID{i}", courses[i].Id));
        //            //parameters.Add(new Parameter($"@STATE{i}", "Past Due"));
        //        }

        //        var sql = sqlBuilder.ToString();
        //        var result = database.ExecNonQuery(sql, CommandType.Text, parameters.ToArray());
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}

        public int UpdateCoursesToComplete(DriverLinkCourseModel course)
        {
            Log.MethodStart();

            try
            {
                string sql = @"update LINK_DRIVER_COURSE set STATE = @STATE, COMPLETED_DATETIME = @COMPLETED_DATETIME where ID = @ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@ID", course.Id),
                    new Parameter("@STATE", "Complete"),
                    new Parameter("@COMPLETED_DATETIME", DateTime.UtcNow.ToLocalTime())
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Update {course.Course.Title} for {course.Driver.DriverId} to Completed state in LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        public int ChangeState(int id, string state)
        {
            Log.MethodStart();

            try
            {
                string sql = @"UPDATE LINK_DRIVER_COURSE Set STATE = @STATE
                            Where ID = @ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@ID", id),
                    new Parameter("@STATE", state),
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to change the state for {id} in LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }


        // DELETE

        public int DeleteAssignedCourseToDriver(int deletedEntry)
        {
            Log.MethodStart();

            try
            {
                string sql = @"DELETE from LINK_DRIVER_COURSE where ID = @ID";

                List<Parameter> parameters = new List<Parameter>
                {
                    new Parameter("@ID", deletedEntry)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {deletedEntry} from LINK_DRIVER_COURSE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }
    }
}