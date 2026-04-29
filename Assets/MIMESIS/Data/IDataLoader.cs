using System;
using System.IO;
using System.Threading.Tasks;

public abstract class IDataLoader
{
	public Stream GetStream(string path)
	{
		return new FileStream(GetData(path), FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	public Stream GetStreamFullPath(string path)
	{
		return new FileStream(GetDataFullPath(path), FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	public abstract string[]? GetFiles(string path);

	public abstract string GetData(string path);

	public abstract string GetDataFullPath(string path);

	public virtual async Task<Stream> GetStreamAsync(string path)
	{
		await Task.Yield();
		throw new NotImplementedException();
	}

	public virtual async Task<Stream> GetStreamFullPathAsync(string path)
	{
		await Task.Yield();
		throw new NotImplementedException();
	}
}
