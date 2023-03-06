using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Namiono.Common.Database
{
	public class SqlDatabase : IDisposable, IManager
	{
		private readonly SQLiteConnection _sqlConn;

		public FileSystem FileSystem
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public SqlDatabase(FileSystem fs, string database)
		{
			if (string.IsNullOrEmpty(database))
				return;

			_sqlConn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", Path.Combine(fs.Root, database)));
			_sqlConn.Open();
		}

		public int Count<TS>(string table, string condition, TS value)
		{
			var num = 0;
			using (var sqLiteCommand = new SQLiteCommand(string.Format("SELECT Count({0}) FROM {1} WHERE {2}=\"{3}\"", condition, table, condition, value), _sqlConn))
			{
				sqLiteCommand.CommandType = CommandType.Text;
				num = Convert.ToInt32(sqLiteCommand.ExecuteScalar());
			}

			return num;
		}

		public Dictionary<int, NameValueCollection> SqlQuery(string sql)
		{
			var dictionary = new Dictionary<int, NameValueCollection>();
			using (var cmd = new SQLiteCommand(sql, _sqlConn))
			{
				Nonqry(cmd, out bool result);
				if (result)
				{
					using (var sqLiteDataReader = cmd.ExecuteReader())
					{
						var key = 0;
						while (sqLiteDataReader.Read())
						{
							if (!dictionary.ContainsKey(key))
							{
								dictionary.Add(key, sqLiteDataReader.GetValues());
								++key;
							}
						}
						sqLiteDataReader.Close();
					}
				}
				return dictionary;
			}
		}

		public bool SqlInsert(string sql)
		{
			var result = false;
			using (var cmd = new SQLiteCommand(sql))
				Nonqry(cmd, out result);
			
			return result;
		}

		public string SqlQuery(string sql, string key)
		{
			var str = string.Empty;
			using (var cmd = new SQLiteCommand(sql, _sqlConn))
			{
				Nonqry(cmd, out bool result);

				using (var sqLiteDataReader = cmd.ExecuteReader())
				{
					while (sqLiteDataReader.Read())
						str = string.Format("{0}", sqLiteDataReader[key]);
				
					sqLiteDataReader.Close();
				}
			}
			return str;
		}

		private void Nonqry(SQLiteCommand cmd, out bool result)
		{
			cmd.CommandType = CommandType.Text;
			cmd.Connection = _sqlConn;
			try
			{
				result = cmd.ExecuteNonQuery() != 0;
			}
			catch
			{
				result = false;
			}
		}

		public void Close() => Stop();

		public void Start()
		{
			if (_sqlConn.State != ConnectionState.Closed)
				return;
			_sqlConn.Open();
		}

		public void Stop()
		{
			if (_sqlConn.State == 0)
				return;

			_sqlConn.Close();
		}

		public void HeartBeat() { }

		public void Dispose() => _sqlConn.Dispose();

		public void Bootstrap()
		{
		}
	}
}
