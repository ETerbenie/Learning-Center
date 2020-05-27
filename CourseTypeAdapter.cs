using Captivate.Interfaces.Entities;
using PTC;
using PTC.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Captivate.Adapters
{
    public class CourseTypeAdapter
    {
        IDatabase db;
        public CourseTypeAdapter()
        {
            db = Database.CreateDatabase();
        }

        //testing
        // SELECT

        public IEnumerable<CourseType> SelectAllCourseTypes()
        {
            Log.MethodStart();

            try
            {
                var sql = @"select ID, COURSE_TYPE from CAPTIVATE_COURSE_TYPE";

                var dataSet = db.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new CourseType
                {
                    Id = r["ID"].To<int>(),
                    Course_Type = r["COURSE_TYPE"].To<string>(),
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all Course Types from CAPTIVATE_COURSE_TYPE Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }

        public CourseType GetCourseTypeById(int id)
        {
            Log.MethodStart();

            try
            {
                var sql = @"select * from CAPTIVATE_COURSE_TYPE where id = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", id)
                };

                var dataSet = db.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0].Rows[0];
                return new CourseType
                {
                    Id = results["ID"].To<int>(),
                    Course_Type = results["COURSE_TYPE"].To<string>()
                };
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select {id} from CAPTIVATE_COURSE_TYPE Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }


        // ADD

        public int AddCourseType(CourseType newCourseType)
        {
            Log.MethodStart();

            try
            {
                var sql = $@"Insert into CAPTIVATE_COURSE_TYPE (COURSE_TYPE) values (@COURSE_TYPE);";

                var parameters = new List<Parameter>
                {
                    new Parameter("@COURSE_TYPE", newCourseType.Course_Type)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Add {newCourseType.Course_Type} to CAPTIVATE_COURSE_TYPE Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }


        // EDIT

        public int EditCourseType(CourseType courseType)
        {
            Log.MethodStart();

            try
            {
                var sql = @"UPDATE CAPTIVATE_COURSE_TYPE SET COURSE_TYPE = @COURSE_TYPE
                            WHERE ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", courseType.Id),
                    new Parameter("@COURSE_TYPE", courseType.Course_Type)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {courseType.Course_Type} in CAPTIVATE_COURSE_TYPE Table --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }

        // DELETE

        public int DeleteCourseType(CourseType deletedCourseType)
        {
            Log.MethodStart();

            try
            {
                var sql = @"DELETE from CAPTIVATE_COURSE_TYPE where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", deletedCourseType.Id)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {deletedCourseType.Course_Type} in CAPTIVATE_COURSE_TYPE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }
    }
}