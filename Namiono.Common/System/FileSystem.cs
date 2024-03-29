﻿using System;
using System.IO;

namespace Namiono.Common
{
	public class FileSystem : IDisposable
	{
		public string Root { get; set; }

		public FileSystem(string path = "") => Root = CreateDirectory(path);

		private string ReplaceSlashes(string path, string curSlash = "\\", string newSlash = "/")
		{
			while (path.Contains(curSlash))
				path = path.Replace(curSlash, newSlash);

			return path;
		}

		private string Resolve(string path) => ReplaceSlashes(Path.Combine(Root, path.StartsWith("/") ? path.Substring(1) : path).ToLowerInvariant());

		public void Read(out byte[] dest, string path) => dest = Read(path);

		public DateTime GetCreationDate(string path) => new FileInfo(Resolve(path)).LastWriteTime;

		public bool Exists(string path) => File.Exists(Resolve(path));

		public byte[] Read(string path)
		{
			byte[] array = new byte[0];

			using (var fileStream = new BufferedStream(File.Open(Resolve(path), FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				array = new byte[fileStream.Length];
				var newSize = fileStream.Read(array, 0, (int)fileStream.Length);

				if (newSize != array.Length)
					Array.Resize(ref array, newSize);

				fileStream.Close();
			}

			return array;
		}

		public void Read(Stream target, string srcFile)
		{
			using (var fileStream = new BufferedStream(File.Open(Path.Combine(Root, srcFile),
				FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				fileStream.CopyToAsync(target);
				fileStream.Close();
			}
		}

		public void Write(string path, in byte[] buffer, int start = 0)
		{
			using (var fileStream = new BufferedStream(File.Create(Path.Combine(Root, path))))
			{
				fileStream.Write(buffer, start, buffer.Length);
				fileStream.Close();
			}
		}

		public void Write(in Stream src, string outputfile)
		{
			using (src)
			{
				using (var destination = new BufferedStream(File.Create(Path.Combine(Root, outputfile))))
				{
					src.CopyTo(destination);
					destination.Close();
				}

				src.FlushAsync();
				src.Close();
			}
		}

		public string Combine(string path1, string path2)
			=> ReplaceSlashes(Path.Combine(path1, path2));

		public string CreateDirectory(string path)
			=> ReplaceSlashes(Directory.CreateDirectory(!string.IsNullOrEmpty(Root) ? ReplaceSlashes(Path.Combine(Root, path)) : ReplaceSlashes(Path.Combine(Directory.GetCurrentDirectory(), path))).FullName);

		public void Dispose() => Root = null;
	}
}
