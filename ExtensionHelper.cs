using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Captivate.Helpers
{
    public static class ExtensionHelper
    {
        public static bool IsEmptyDataSet(this DataSet dataSet)
        {
            return (dataSet.Tables == null || dataSet.Tables.Count == 0
                    || dataSet.Tables[0].Rows == null || dataSet.Tables[0].Rows.Count == 0);
        }
    }
}