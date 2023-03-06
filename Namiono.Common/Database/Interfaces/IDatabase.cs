using Namiono.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Namiono.Database
{
    public interface IDatabase : IManager
    {
        int Count<TS>(string table, string condition, TS value);
        Dictionary<int, NameValueCollection> Query(string sql);
        bool Insert(string sql);
        string Query(string sql, string key);
    }
}
