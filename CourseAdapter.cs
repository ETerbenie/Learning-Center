using PTC.Database;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Captivate.Interfaces.Entities;
using Captivate.Models;
using PTC;
using System.Web.Mvc;

namespace Captivate.Adapters
{
    public class CourseAdapter
    {
        private IDatabase db;
        public CourseAdapter()
        {
            db = Database.CreateDatabase();
        }

        // SELECT

        public IEnumerable<CourseModel> SelectAllCourses()
        {
            Log.MethodStart();

            try
            {
                var sql = @"select c.ID as COURSE_ID, c.TITLE, c.DESCRIPTION, c.URL, c.CREATED_DATETIME, ct.COURSE_TYPE, ct.ID as COURSE_TYPE_ID from COURSE c
                            join CAPTIVATE_COURSE_TYPE ct on ct.id = c.course_type_id";

                var dataSet = db.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new CourseModel
                {
                    Id = r["COURSE_ID"].To<int>(),
                    Title = r["TITLE"].To<string>(),
                    Description = r["DESCRIPTION"].To<string>(),
                    URL = r["URL"].To<string>(),
                    CourseType = new CourseType { Id = r["COURSE_TYPE_ID"].To<int>(), Course_Type = r["COURSE_TYPE"].To<string>() }, 
                    Created_Datetime = r["CREATED_DATETIME"].To<DateTime>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all courses from Course table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }

        // ADD

        public int AddCourse(CourseModel newCourse)
        {
            Log.MethodStart();

            try
            {
                var sql = @"INSERT into COURSE (TITLE, DESCRIPTION, URL, COURSE_TYPE_ID, CREATED_DATETIME) 
                            values (@TITLE, @DESCRIPTION, @URL, @COURSE_TYPE_ID, @CREATED_DATETIME)";

                var parameters = new List<Parameter>
                {
                    new Parameter("@TITLE", newCourse.Title),
                    new Parameter("@DESCRIPTION", newCourse.Description),
                    new Parameter("@URL", newCourse.URL),
                    new Parameter("@COURSE_TYPE_ID", newCourse.SelectedCourseTypeId),
                    //new Parameter("@HIDDEN_COURSE", newCourse.Hidden_Course),
                    new Parameter("@CREATED_DATETIME", DateTime.Now)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);


            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Add {newCourse.Title} to Course table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }

        //EDIT

        [HttpPost]
        public int EditCourse(CourseModel course)
        {
            Log.MethodStart();

            try
            {
                var sql = @"UPDATE COURSE SET 
                            TITLE = @TITLE, DESCRIPTION = @DESCRIPTION, URL = @URL , COURSE_TYPE_ID = @COURSE_TYPE_ID
                            WHERE ID = @ID";
                //, COURSE_TYPE = @COURSE_TYPE  add this later
                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", course.Id),
                    new Parameter("@TITLE", course.Title),
                    new Parameter("@DESCRIPTION", course.Description),
                    new Parameter("@URL", course.URL),
                    new Parameter("@COURSE_TYPE_ID", course.SelectedCourseTypeId)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {course.Title} in Course Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }

        // DELETE
        public int DeleteCourse(int courseId)
        {
            Log.MethodStart();

            try
            {
                var sql = @"DELETE from COURSE where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", courseId)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {courseId} from Course Table --- {ex}");
                Log.Error(ex.ToString());
                throw ex;
            }
        }
    }
}