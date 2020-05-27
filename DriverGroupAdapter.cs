using PTC;
using PTC.Database;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Captivate.Models;

namespace Captivate.Adapters
{
    public class DriverGroupAdapter
    {
        private IDatabase db;
        public DriverGroupAdapter()
        {
            db = Database.CreateDatabase();
        }

        // SELECT

        public IEnumerable<DriverGroupModel> SelectAllDriverGroups()
        {
            Log.MethodStart();

            try
            {
                var sql = "select ID, NAME from DRIVER_GROUP";

                var dataSet = db.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new DriverGroupModel
                {
                    Id = r["ID"].To<int>(),
                    Name = r["NAME"].To<string>()
                }).ToList();

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Select All Driver Groups from DRIVER_GROUP Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }


        // ADD

        public int AddDriverGroup(DriverGroupModel newGroupName)
        {
            Log.MethodStart();

            try
            {
                var sql = @"INSERT into DRIVER_GROUP (NAME) 
                            values (@NAME);";

                var parameters = new List<Parameter>
                {
                    new Parameter("@NAME", newGroupName.Name)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Add {newGroupName.Name} from DRIVER_GROUP --- {ex}");
                Log.Error(ex);
                throw ex;
            }
        }


        // EDIT

        public int EditDriverGroup(DriverGroupModel driverGroup)
        {
            Log.MethodStart();

            try
            {
                var sql = @"Update DRIVER_GROUP set NAME = @NAME where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", driverGroup.Id),
                    new Parameter("@NAME", driverGroup.Name)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {driverGroup.Name} in DRIVER_GROUP Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        // DELETE

        public int DeleteDriverGroup(DriverGroupModel driverGroup)
        {
            Log.MethodStart();

            try
            {
                var sql = @"DELETE from DRIVER_GROUP where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", driverGroup.Id)
                };

                return db.ExecNonQuery(sql, CommandType.Text, parameters);

            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {driverGroup.Name} from DRIVER_GROUP Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

    }
}