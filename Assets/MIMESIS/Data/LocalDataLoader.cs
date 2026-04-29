using System.IO;
using System.Linq;

public class LocalDataLoader : IDataLoader
{
	private string m_LocalPath;

	public LocalDataLoader(string localPath)
	{
		m_LocalPath = localPath;
	}

	public override string[]? GetFiles(string path)
	{
		string targetPath = Path.Combine(m_LocalPath, path);
		return Directory.GetFiles(targetPath).Select(delegate(string filename)
		{
			if (filename.StartsWith(targetPath))
			{
				filename = filename.Substring(targetPath.Length);
			}
			if (filename.StartsWith("\\") | filename.StartsWith("/"))
			{
				filename = filename.Substring(1);
			}
			return filename;
		}).ToArray();
	}

	public override string GetData(string path)
	{
		return Path.GetFullPath(Path.Combine(m_LocalPath, path));
	}

	public override string GetDataFullPath(string fullPath)
	{
		return Path.GetFullPath(fullPath);
	}
}
