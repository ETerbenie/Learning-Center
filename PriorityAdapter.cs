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
    public class PriorityAdapter
    {
        IDatabase database;
        public PriorityAdapter()
        {
            database = Database.CreateDatabase();
        }

        // SELECT

        public IEnumerable<Priority> SelectAllPriorities()
        {
            Log.MethodStart();

            try
            {
                var sql = @"select ID, NAME from PRIORITY";

                var dataSet = database.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];

                return results.AsEnumerable().Distinct().Select(r => new Priority
                {
                    Id = r["ID"].To<int>(),
                    Name = r["NAME"].To<string>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all Priorities from PRIORITY Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        // ADD

        public int AddNewPriority(string name)
        {
            Log.MethodStart();

            try
            {
                var sql = @"Insert into PRIORITY (NAME) values (@NAME)";

                var parameters = new List<Parameter>
                {
                   new Parameter("@NAME", name)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Add {name} to PRIORITY Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        // EDIT

        public int EditPriority(Priority priority)
        {
            try
            {
                var sql = @"Update PRIORITY set NAME = @NAME where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", priority.Id),
                    new Parameter("@NAME", priority.Name)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {priority.Name} in PRIORITY Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }


        // DELETE 

        public int DeletePriority(int id)
        {
            try
            {
                var sql = @"Delete from PRIORITY where ID = @ID ";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", id)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {id} from PRIORITY Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }
    }
}