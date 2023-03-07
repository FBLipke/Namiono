using Namiono.Common;
using Namiono.Common.Database;
using Namiono.Common.Network;
using Namiono.Common.Network.HTTP;
using Namiono.Common.Provider;
using Namiono.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace Namiono.Module
{
	public class User : IProvider, IManager, ILogin
	{
		public ModulSettings Settings { get; private set; }

		public User()
		{
			CanAdd = true;
			CanEdit = true;
			CanRemove = true;
			IsPublicModule = true;
			Members = new Dictionary<Guid, IMember>();
			FileSystem = new FileSystem("Providers\\User");
			Settings = new ModulSettings("Config\\User\\Config.xml");
			Crypt = new MD5();

			if (VolativeModule)
				return;

			Database = new SqlDatabase(FileSystem, "User.db");
		}

		public IDatabase Database { get; set; }

		public void Start()
		{
			Database.Start();
			Members = Provider.LoadFromDataBase(Database, nameof(User));
			Active = true;
		}

		public void Stop()
		{
			if (!Active)
				return;
			Close();
			Active = false;
		}

		public void HeartBeat()
		{
			if (VolativeModule)
				return;

			Database?.HeartBeat();
			Update();
		}

		private void Update() => Provider.Commit(Members, "User", Database, FileSystem);

		public void Close()
		{
			Update();
			if (VolativeModule)
				return;
			Database?.Close();
		}

		public void Dispose()
		{
			if (Database != null)
			{
				Database?.Dispose();
				Database = null;
			}
			if (Members != null)
			{
				Members.Clear();
				Members = null;
			}
			FileSystem = null;
		}

		public bool VolativeModule { get; set; } = false;

		public Dictionary<Guid, IMember> Members { get; set; }

		public bool CanEdit { get; set; } = true;

		public string FriendlyName { get; set; } = "Mitglieder";

		public string Description { get; set; } = "Dieses Module fügt die Unterstützung für Benutzerkonten hinzu.";

		public bool IsPublicModule { get; set; }

		public bool CanLogin { get; set; } = true;

		public bool CanAdd { get; set; } = true;

		public bool CanRemove { get; set; } = true;

		public FileSystem FileSystem { get; set; }

		public bool Active { get; set; }
		public ICrypto Crypt { get; set; }

		public void Remove(Guid id) => Members.Remove(id);

		public bool Contains(Guid id) => Members.ContainsKey(id);

		public void Bootstrap() => Database.Bootstrap();

		public void Install() => Provider.Install(nameof(User), Members, Database, FileSystem, Crypt);

		public IMember Get_Member(Guid id) => Members.ContainsKey(id) ? Members[id] : null;

		public string Handle_Get_Request(NamionoHttpContext request)
		{
			var stringBuilder = new StringBuilder("<section id =\"user\">");
			stringBuilder.Append("<h2>Mitglieder</h2>");
			stringBuilder.Append("<hr>");
			var members = Members.Values.Where(user => !user.Locked && !user.Service);
			stringBuilder.Append("<ul>");
			foreach (var member in members)
			{
				stringBuilder.Append("<li class=\"userlist-row\">");
				stringBuilder.AppendFormat("<div class=\"left\"> <img src=\"{0}\" width=\"48px\" height=\"48px\" /></div>", member.Image);
				stringBuilder.AppendFormat("<div class=\"right\"><p>Name: {0}</p><p>Level: {1}</p></div>", member.Name, member.Level);
				stringBuilder.Append("</li>");
			}
			stringBuilder.Append("</ul>");
			stringBuilder.Append("</section>");
			return stringBuilder.ToString();
		}

		public string Handle_Add_Request(NamionoHttpContext request)
		{
			var stringBuilder = new StringBuilder("");
			switch (request.Request.Method)
			{
				case "GET":
					stringBuilder.Append(FileSystem.Read("template/User_Add_Form.tpl").GetString_UTF8());
					break;
				case "POST":
					if (request.Request.Parameters.ContainsKey("useremail") && request.Request.Parameters["useremail"].Length > 5 && request.Request.Parameters["useremail"].Contains("@") && !request.Request.Parameters["useremail"].StartsWith("@") && !request.Request.Parameters["useremail"].EndsWith("@") && !request.Request.Parameters["useremail"].StartsWith(".") && !request.Request.Parameters["useremail"].EndsWith(".") && request.Request.Parameters["useremail"].Contains("."))
					{
						var parameter1 = request.Request.Parameters["useremail"];
						if (Database.Count("User", "EMail", parameter1) != 0)
						{
							stringBuilder.Append("Die E-Mail Adresse wird bereits verwendet. Jede E-Mail Adresse darf nur einmal verwendet werden.");
							break;
						}
						if (request.Request.Parameters.ContainsKey("userpass") && request.Request.Parameters["userpass"].Length > Settings.Min_Pass_Length && !request.Request.Parameters["userpass"].ContainsChar(Settings.forbidden_Chars_Username))
						{
							var parameter2 = request.Request.Parameters["userpass"];
							if (request.Request.Parameters.ContainsKey("username") && request.Request.Parameters["username"].Length >= Settings.Min_Name_Length)
							{
								var parameter3 = request.Request.Parameters["username"];
								if (Database.Count("User", "Name", parameter1) != 0)
								{
									stringBuilder.Append("Der Benutzername wird bereits verwendet. Jeder Benutzername darf nur einmal verwendet werden.");
									break;
								}
								if (!request.Request.Parameters["username"].ContainsChar(Settings.forbidden_Chars_Username))
								{
									var flag = false;
									using (var httpClient = new HttpClient())
									{
										httpClient.Timeout = TimeSpan.FromSeconds(5.0);
										var result = httpClient.GetStringAsync(new Uri("http://www.stopforumspam.com/api?email=" + parameter1)).Result;
										if (!string.IsNullOrEmpty(result))
										{
											var xmlDocument = new XmlDocument();
											xmlDocument.LoadXml(result);
											if (xmlDocument.DocumentElement != null)
												flag = xmlDocument.DocumentElement.SelectSingleNode("appears")?.Value == "yes";
										}
									}
									if (flag)
									{
										stringBuilder.Append("Die E-Mail-Adresse " + request.Request.Parameters["useremail"] + ", kann nicht für die Registrierung eines Benutzerkontos verwendet werden!");
										break;
									}

									var member = new Member()
									{
										Id = Guid.NewGuid(),
										Name = parameter3,
										Created = DateTime.Now.AsUnixTimeStamp(),
										Updated = DateTime.Now.AsUnixTimeStamp(),
										Provider = nameof(User),
										IpAddress = request.Request.RemEndpoint.Address.ToString(),
										Port = (ushort)request.Request.RemEndpoint.Port,
										EMail = parameter1,
										Image = Settings.DefaultAvatar,
										Level = 1,
										Moderator = false,
										Locked = false,
										Service = false,
										Design = Settings.DefaultStyle,
										Description = "",
										OutPut = string.Empty,
										Password = Crypt.GetHash(parameter2, string.Empty)
									};

									Members.Add(member.Id, member);
									Update();
									stringBuilder.Append("Die Registrierung ist abgeschlossen!");
									break;
								}
								break;
							}
							break;
						}
						break;
					}
					break;
			}
			return stringBuilder.ToString();
		}

		public string Handle_Edit_Request(NamionoHttpContext request)
		{
			var stringBuilder = new StringBuilder();
			if (!CanEdit)
			{
				stringBuilder.Append("Der Provider unterstützt das bearbeiten dieses Objekts nicht!");
			}
			else
			{
				if (!request.Request.Parameters.Any())
					return null;
				switch (request.Request.Method)
				{
					case "GET":
						stringBuilder.Append(FileSystem.Read("template/User_Edit_Form.tpl").GetString_UTF8());
						break;
				}
			}
			return stringBuilder.ToString();
		}

		public string Handle_Remove_Request(NamionoHttpContext request)
		{
			var stringBuilder = new StringBuilder();
			if (request.Request.User.Level >= 128UL)
				return stringBuilder.ToString();
			stringBuilder.Append("Du hast nicht die nötigen Rechte!");
			return stringBuilder.ToString();
		}

		public string Handle_Info_Request(NamionoHttpContext request) => throw new NotImplementedException();

		public IMember Request(Guid id) => Members.ContainsKey(id) ? Members[id] : null;

		public IMember Handle_Login_Request(NamionoHttpContext request)
		{
			var member1 = (IMember)null;
			if (!request.Request.Parameters.Any())
				return null;
			switch (request.Request.Method)
			{
				case "POST":
					var source = Members.Values.Where(u => !u.Locked && !u.Service);
					var member2 = source.Where(u => u.Name.ToLowerInvariant() == request.Request.Parameters["username"].ToLowerInvariant() && u.Password ==
						Crypt.GetHash(request.Request.Parameters["userpass"], string.Empty)).FirstOrDefault();
					if (member2 != null)
						member1 = member2;
					else if (request.Request.Parameters["username"].Contains("@") && request.Request.Parameters["username"].Contains("."))
						member1 = source.Where(u => u.EMail.ToLowerInvariant() == request.Request.Parameters["username"]
							.ToLowerInvariant() && u.Password == Crypt.GetHash(request.Request.Parameters["userpass"], string.Empty)).FirstOrDefault();
					else
						break;
					if (member1 != null)
					{
						member1.IpAddress = request.Request.RemEndpoint.Address.ToString();
						member1.Port = (ushort)request.Request.RemEndpoint.Port;
						member1.Active = true;
						member1.Updated = DateTime.Now.AsUnixTimeStamp();
						Members[member1.Id] = member1;
						request.Request.User = Members[member1.Id];
						request.Request.SetCookie(new HttpCookie("UserId", request.Request.User.Id.ToString()));
						NamionoCommon.Log("D", FriendlyName, string.Format("Login from {0} for {1}.",
							request.Request.RemEndpoint, member1.Name));
						break;
					}
					break;
			}
			return member1;
		}

		public IMember Handle_Logout_Request(NamionoHttpContext request)
		{
			if (request.Request.HasCookie("UserId"))
			{
				var member = Request(Guid.Parse(request.Request.Get_Cookie("UserId").Value));
				if (member != null)
				{
					member.Active = false;
					member.Updated = DateTime.Now.AsUnixTimeStamp();
					lock (Members)
						Members[member.Id] = member;
				}
				if (request.Request.HasCookie("UserId"))
					request.Request.Get_Cookie("UserId").Remove = true;
			}
			request.Request.User = null;
			return request.Request.User;
		}

		public class ModulSettings
		{
			public string Config { get; private set; }

			public int Min_Name_Length = 5;
			public string DefaultAvatar = "avatar.png";
			public string DefaultStyle = "default";
			public char[] forbidden_Chars_Username = new char[10]
			{
		' ',
		'@',
		'.',
		',',
		':',
		';',
		'"',
		'\'',
		'\\',
		'/'
			};
			public int Min_Pass_Length = 6;

			public ModulSettings(string config)
			{
				Config = config;
			}
		}
	}
}
