using Captivate.Interfaces.Entities;
using Captivate.Models;
using PTC;
using PTC.Database;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Adapters
{
    public class StateAdapter
    {
        IDatabase database;
        public StateAdapter()
        {
            database = Database.CreateDatabase();
        }


        // SELECT 

        public IEnumerable<State> SelectAllStates()
        {
            try
            {
                var sql = @"select * from STATE";

                var dataSet = database.GetDataSet(sql);

                if (dataSet.IsEmptyDataSet())
                    return null;

                var results = dataSet.Tables[0];
                return results.AsEnumerable().Distinct().Select(r => new State
                {
                    Id = r["ID"].To<int>(),
                    Name = r["NAME"].To<string>()
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to select all States from STATE Tabe --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        // ADD

        public int AddNewState(string name)
        {
            try
            {
                var sql = @"Insert into STATE (NAME) values (@NAME)";

                var parameters = new List<Parameter>
                {
                    new Parameter("@NAME", name)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Add {name} into STATE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        // EDIT

        public int EditState(State state)
        {
            try
            {
                var sql = @"Update STATE set NAME = @NAME where ID = @ID";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", state.Id),
                    new Parameter("@NAME", state.Name)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Edit {state.Name} from STATE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }

        // DELETE

        public int DeleteState(int deletedState)
        {
            try
            {
                var sql = @"Delete from STATE where ID = @ID ";

                var parameters = new List<Parameter>
                {
                    new Parameter("@ID", deletedState)
                };

                return database.ExecNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Log.Info($"Unable to Delete {deletedState} from STATE Table --- {ex}");
                Log.Error(ex);
                throw;
            }
        }
    }
}