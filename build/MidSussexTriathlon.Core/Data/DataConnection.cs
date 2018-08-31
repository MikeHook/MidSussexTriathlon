using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidSussexTriathlon.Core.Data
{
    public interface IDataConnection
    {
        IDbConnection SqlConnection { get; }
    }

    public class DataConnection : IDataConnection
    {
        public IDbConnection SqlConnection
        {
            get { return new SqlConnection(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString); }
        }
    }


}
