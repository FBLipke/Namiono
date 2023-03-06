// Decompiled with JetBrains decompiler
// Type: Namiono.Common.Provider.Provider
// Assembly: Namiono.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CE4FCADF-C52D-4962-B4B8-C6D36FAB8FAE
// Assembly location: C:\Users\LipkeGu\Desktop\namiono___\Namiono.Common.dll

using Namiono.Common.Database;
using Namiono.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Namiono.Common.Provider
{
	public class Provider
	{
		public static event ModuleLoadedEventHandler ModuleLoaded;

		public static bool HasMethod(object obj, string name) => obj.GetType().GetMethod(name) != null;
		public static bool HasInterFace(object obj, string name) => obj.GetType().GetInterface(name) != (MethodInfo)null;

		public static void LoadModule(string name)
		{
			var path = Path.Combine(Environment.CurrentDirectory, name + ".dll");

			if (!File.Exists(path))
				return;

			var instance = (IProvider)Activator.CreateInstance(Assembly.LoadFile(path).GetType(name));

			ModuleLoaded.DynamicInvoke(null, new ModuleLoadedEventArgs(name, instance));
		}

		public static bool HasProperty(object obj, string name) => obj.GetType().GetProperty(name) != null;

		public static bool HasField(object obj, string name) => obj.GetType().GetField(name) != null;

		public static bool HasEvent(object obj, string name) => obj.GetType().GetEvent(name) != null;

        public static TY GetPropertyValue<TY>(object member, string propertyName)
			=> AsType<TY>(member.GetType().GetProperties()
                .Single(pi => pi.Name == propertyName).GetValue(member, null));

        public static void SetPropertyValue<TS>(
		  object obj,
		  string name,
		  TS value,
		  bool append = false,
		  string delimeter = ";")
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			var propertyInfo = ((IEnumerable<PropertyInfo>)obj.GetType().GetProperties()).Single(pi => pi.Name == name) ?? throw new ArgumentNullException();

			if (typeof(TS) == typeof(string))
			{
				if (!append)
				{
					var runtimeMethod = propertyInfo.PropertyType.GetRuntimeMethod("Parse", new Type[1]
					{
			typeof (string)
					});
					if (runtimeMethod != null)
					{
						var str = string.Format("{0}", value);
						if (propertyInfo.PropertyType == typeof(bool))
							str = str != "0" ? "true" : "false";

						var obj1 = runtimeMethod.Invoke(propertyInfo.PropertyType, new object[1]
						{
			   str
						});
						propertyInfo.SetValue(obj, obj1);
					}
					else
						propertyInfo.SetValue(obj, value);
				}
				else
				{
					var str = GetPropertyValue<TS>(obj, name).ToString() + delimeter + value;
					propertyInfo.SetValue(obj, str);
				}
			}
			else
			{
				var runtimeMethod = propertyInfo.PropertyType.GetRuntimeMethod("Parse", new Type[1]
				{
		  typeof (string)
				});
				if (runtimeMethod != null)
				{
					var str = string.Format("{0}", value);
					if (propertyInfo.PropertyType == typeof(bool))
						str = str != "0" ? "true" : "false";
					var obj2 = runtimeMethod.Invoke(propertyInfo.PropertyType, new object[1]
					{
			 str
					});
					propertyInfo.SetValue(obj, obj2);
				}
				else
					propertyInfo.SetValue(obj, value);
			}
		}

        public static TS InvokeMethod<TS>(object obj, string name, object[] parameters = null)
			=> AsType<TS>(obj.GetType().GetMethod(name).Invoke(obj, parameters));

		public static void InvokeMethod(object obj, string name, object[] parameters = null)
			=> obj.GetType().GetMethod(name).Invoke(obj, parameters);

		public static Dictionary<Guid, IMember> LoadFromDataBase(
		  IDatabase db,
		  string name)
		{
			var dictionary1 = new Dictionary<Guid, IMember>();
			var propertyInfos = typeof(IMember).GetProperties().Where(p => p.GetGetMethod().IsPublic)
				.Where(p => p.PropertyType.FullName != null && p.PropertyType.FullName.StartsWith("System")).Where(p => !p.PropertyType.FullName.Contains("Collections"));
			var dictionary2 = db.Query(string.Format("SELECT {0} FROM {1}", "*", name));
			for (var key = 0; key < dictionary2.Count; ++key)
			{
				var member = new Member();
				foreach (var propertyInfo in propertyInfos)
				{
					SetPropertyValue(member, propertyInfo.Name, dictionary2[key][propertyInfo.Name]);
					if (propertyInfo.Name == "ExtraData" && string.IsNullOrEmpty(dictionary2[key]["ExtraData"]))
						member.Members = JsonConvert.DeserializeObject<Dictionary<Guid, IMember>>(dictionary2[key]["ExtraData"]);
				}
				if (!dictionary1.ContainsKey(member.Id))
					dictionary1.Add(member.Id, member);
			}
			NamionoCommon.Log("I", name, string.Format("Loaded {0} entries from Database...", dictionary2.Count));
			return dictionary1;
		}

        public static IEnumerable<IProvider> CanDo(string ability)
			=> NamionoCommon.Providers?.Values.Where(p => p.GetType()
				.GetInterface("I" + ability.Captitalize(), true) != null && p.Active);

        public static void SubscribeEvent(object obj, object origin, string name, string method)
		{
			if (obj == null)
				return;

			var eventInfo = obj.GetType().GetEvent(name);
			var eventHandlerType = eventInfo.EventHandlerType;
			var firstArgument = obj;
			var handler = Delegate.CreateDelegate(eventHandlerType, firstArgument,
				origin.GetType().GetMethod(method) ?? throw new InvalidOperationException());
			eventInfo.AddEventHandler(obj, handler);
		}

		public static void Insert(
		  Dictionary<Guid, IMember> members,
		  IDatabase db,
		 string name,
		  FileSystem fs)
		{
			var path = fs.Combine(fs.Root, "database_insert.sql");
			foreach (var member in members.Values)
			{
				if (db.Count(name, "Id", member.Id) == 0)
				{
					member.ExtraData = JsonConvert.SerializeObject(member.Members);
					using (var streamWriter = new StreamWriter(path))
					{
						var propertyInfos = member.GetType().GetProperties()
							.Where(p => p.GetGetMethod().IsPublic).Where(p => p.PropertyType.FullName != null && p.PropertyType.FullName.StartsWith("System"))
							.Where(p => !p.PropertyType.FullName.Contains("Collections"));

						streamWriter.NewLine = Environment.NewLine;
						streamWriter.AutoFlush = true;
						var num = 1;
						var output = new StringBuilder("VALUES (");
						var str1 = string.Format("INSERT INTO " + name + " (");
						foreach (var propertyInfo in propertyInfos)
						{
							var strArray = propertyInfo.PropertyType.ToString().Split('.');
							var str2 = strArray[strArray.Length - 1].ToLower();
							var stringBuilder2 = new StringBuilder();
							if (str2 == "guid")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<Guid>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "ipaddress" || str2 == "string")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<string>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "double")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<double>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "boolean")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<bool>(member, propertyInfo.Name) ? 49 : 48);
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "uint64")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<ulong>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "uint32")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<uint>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "uint16")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<ushort>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "int16")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<short>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else if (str2 == "int32")
							{
								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<int>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							else
							{
								if (!(str2 == "int64"))
									throw new Exception(string.Format("I dont know to use for {0}", propertyInfo.Name));

								stringBuilder2.AppendFormat("'{0}',", GetPropertyValue<long>(member, propertyInfo.Name));
								str1 = str1 + propertyInfo.Name + ",";
							}
							output.Append(stringBuilder2);
							++num;
						}
						var str3 = str1.Substring(0, str1.LastIndexOf(",", StringComparison.Ordinal)) + ")";
						var str4 = output.ToString();
						if (str4.EndsWith(","))
							str4 = str4.Substring(0, str4.LastIndexOf(",", StringComparison.Ordinal)) + ")";
						streamWriter.WriteLine(str3);
						streamWriter.WriteLine(str4);
						streamWriter.Close();
					}
				}
			}
			var end = string.Empty;

			using (var streamReader = new StreamReader(path, true))
			{
				end = streamReader.ReadToEnd();
				streamReader.Close();
			}

			if (db.Insert(end))
				File.Delete(path);
			else
				NamionoCommon.Log("E", name, "SQL Error : " + end);
		}

		public static void Commit(
		  Dictionary<Guid, IMember> members,
		  string name,
		  IDatabase db,
		  FileSystem fs)
		{
			var path = fs.Combine(fs.Root, "database_update.sql");
			if (db == null)
				NamionoCommon.Log("E", string.Join(".", new string[2]
				{
		  "Namiono",
		  name
				}), "This Provider does not maintain a database!");
			else if (!members.Any())
			{
				NamionoCommon.Log("W", name, "Commit for Modul '" + name + "' skipped (no entries)!");
			}
			else
			{
				NamionoCommon.Log("I", name, "Commit for Modul '" + name + "' started...");
				foreach (var member in members.Values)
				{
					member.ExtraData = JsonConvert.SerializeObject(member.Members);
					var id = member.Id;
					if (db.Count(name, "Id", id) == 0)
					{
						Insert(members, db, name, fs);
					}
					else
					{
						var propertyInfos = member.GetType().GetProperties().Where(p => p.GetGetMethod().IsPublic).Where(p => p.PropertyType.FullName != null &&
						p.PropertyType.FullName.StartsWith("System")).Where(p => !p.PropertyType.FullName.Contains("Collections"));

						using (var streamWriter = new StreamWriter(path, fs.Exists(path)))
						{
							streamWriter.NewLine = Environment.NewLine;
							streamWriter.AutoFlush = true;

							foreach (var propertyInfo in propertyInfos)
							{
								var strArray = propertyInfo.PropertyType.ToString().Split('.');
								var str = strArray[strArray.Length - 1].ToLower();

								if (str == "guid")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<Guid>(member, propertyInfo.Name), id);
								else if (str == "ipaddress" || str == "string")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<string>(member, propertyInfo.Name).Trim(), id);
								else if (str == "double")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<double>(member, propertyInfo.Name), id);
								else if (str == "boolean")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<bool>(member, propertyInfo.Name) ? 49 : 48, id);
								else if (str == "uint64")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<ulong>(member, propertyInfo.Name), id);
								else if (str == "uint32")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<uint>(member, propertyInfo.Name), id);
								else if (str == "uint16")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<ushort>(member, propertyInfo.Name), id);
								else if (str == "int16")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<short>(member, propertyInfo.Name), id);
								else if (str == "int32")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<int>(member, propertyInfo.Name), id);
								else if (str == "int64")
									streamWriter.WriteLine("UPDATE {0} SET {1} = \"{2}\" WHERE Id = \"{3}\";", name, propertyInfo.Name, GetPropertyValue<long>(member, propertyInfo.Name), id);
							}
						}
					}
				}

				if (!fs.Exists(path))
					return;
				var end = string.Empty;

				using (var streamReader = new StreamReader(path, true))
				{
					end = streamReader.ReadToEnd();
					streamReader.Close();
				}

				if (db.Insert(end))
				{
					File.Delete(path);
					NamionoCommon.Log("I", name, "Commit for Modul '" + name + "' completed...");
				}
				else
					NamionoCommon.Log("E", name, "Commit for Modul '" + name + "' completed with errors!");
			}
		}

		public static void Install(
		  string name,
		  Dictionary<Guid, IMember> members,
		  IDatabase db,
		  FileSystem fs,
		  ICrypto crypter)
		{
			var filename = fs.Combine(fs.Root, "install.xml");
			if (!fs.Exists(filename.ToLowerInvariant()))
				NamionoCommon.Log("E", NamionoCommon.Providers[name].FriendlyName, "Installation failed: Script (" + filename + ")not found!");
			else if (fs.Exists("installed.tag"))
			{
				NamionoCommon.Log("I", NamionoCommon.Providers[name].FriendlyName, "Installation already completed...");
			}
			else
			{
				NamionoCommon.Log("I", NamionoCommon.Providers[name].FriendlyName, "Installer started...");

				var xmlDocument = new XmlDocument();
				xmlDocument.Load(filename);

				var childNodes = xmlDocument.DocumentElement?.SelectSingleNode("Module")?.SelectSingleNode(nameof(Install))?.SelectSingleNode("Entries")?.ChildNodes;
				if (childNodes != null)
				{
					for (var i = childNodes.Count - 1; i >= 0; --i)
					{
						if (childNodes[i].Name != "Entries")
							continue;

						var attributes = childNodes[i].Attributes;
						
						if (attributes != null)
						{
							var email = "me@you.de";
							var memberName = string.Empty;

							if (attributes.GetNamedItem("Email") != null)
								email = attributes["Email"].Value;

							if (attributes.GetNamedItem("Name") != null)
								memberName = attributes.GetNamedItem("Name").Value;
							var totalSeconds = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
							var s = attributes["Level"].Value;
							var str2 = attributes["Provider"].Value;
							var str3 = attributes["Description"].Value;
							var password = attributes["Password"].Value;
							var key = Guid.NewGuid();

							var member = new Member()
							{
								Members = new Dictionary<Guid, IMember>(),
								Id = key,
								Name = memberName,
								EMail = email,
								Password = crypter.GetHash(password, email),
								Created = totalSeconds,
								Updated = totalSeconds,
								Level = ulong.Parse(s),
								Provider = str2,
								Description = str3
							};

							members.Add(key, member);
						}
					}
				}
				
				var path = Path.Combine(fs.Root, "database_create.sql");
				using (var streamWriter = new StreamWriter(path))
				{
					streamWriter.WriteLine("CREATE TABLE IF NOT EXISTS '{0}' (", name);
					streamWriter.NewLine = Environment.NewLine;
					streamWriter.AutoFlush = true;
					streamWriter.WriteLine("\t'_id'\tINTEGER PRIMARY KEY AUTOINCREMENT,");
					
					var num = 1;
					
					var source1 = ((IEnumerable<PropertyInfo>)typeof(IMember).GetProperties()).Where((p => p.GetGetMethod().IsPublic)).Where
						(p => p.PropertyType.FullName != null && p.PropertyType.FullName.StartsWith("System"))
							.Where(p => !p.PropertyType.FullName.Contains("Collections"));
					
					if (!(source1 is PropertyInfo[] propertyInfoArray))
						propertyInfoArray = source1.ToArray();
					
					var source2 = propertyInfoArray;

					foreach (var propertyInfo in source2)
					{
						var strArray = propertyInfo.PropertyType.ToString().Split('.');
						var str4 = strArray[strArray.Length - 1];
						var str5 = "TEXT";
						var str6 = string.Empty;

						switch (str4.ToLower())
						{
							case "boolean":
							case "double":
							case "int16":
							case "int32":
							case "int64":
							case "uint16":
							case "uint32":
							case "uint64":
								str6 = "INTEGER";
								break;
							case "guid":
							case "ipaddress":
							case "string":
								str6 = "TEXT";
								break;
							default:
								throw new Exception("Dont know what to write for: " + str5);
						}
						if (num != source2.Count())
						{
							streamWriter.WriteLine("\t'" + propertyInfo.Name + "'\t" + str6 + ",");
							++num;
						}
						else
						{
							streamWriter.WriteLine("\t'" + propertyInfo.Name + "'\t" + str6);
							break;
						}
					}
					
					streamWriter.WriteLine(")");
					streamWriter.Close();
				}
				var end = string.Empty;
				using (var streamReader = new StreamReader(path, true))
				{
					end = streamReader.ReadToEnd();
					streamReader.Close();
				}

				if (db.Insert(end))
					File.Delete(path);
				
				if (members.Any())
					Insert(members, db, name, fs);
				
				using (var text = File.CreateText(Path.Combine(fs.Root, "installed.tag")))
				{
					text.WriteLine("Installed...");
					text.Close();
					NamionoCommon.Log("I", NamionoCommon.Providers[name].FriendlyName, "Installation completed...");
				}
			}
		}

		public static T AsType<T>(object obj)
        {
			return (T)Convert.ChangeType(obj, typeof(T));
		}

		public delegate void ModuleLoadedEventHandler(object sender, ModuleLoadedEventArgs e);
	}
}
