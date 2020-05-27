using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTC;
using PTC.Database;
using Captivate.Models;

namespace Captivate.Adapters
{
    public class AdminAdapter
    {
        IDatabase database;
        public AdminAdapter()
        {
            database = Database.CreateDatabase();
        }

        public DriverModel ValidateAdminUser(string username, string password)
        {
            Log.MethodStart();

            try
            {
                var sql = @"select ID, USERNAME, PASSWORD from [USER]
                            where USERNAME = @USERNAME and PASSWORD = @PASSWORD";

                var parameters = new List<Parameter>
                {
                    new Parameter("@USERNAME", username),
                    new Parameter("@PASSWORD", password)
                };

                var dataSet = database.GetDataSet(sql, CommandType.Text, parameters.ToArray());

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0].Rows[0];

                var adminEntry = new DriverModel
                {
                    Id = results["ID"].To<int>(),
                    Username = results["USERNAME"].To<string>(),
                    Password = results["PASSWORD"].To<string>()
                };

                return adminEntry;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        //public DriverModel AddAdmin()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        throw;
        //    }
        //}
    }
}