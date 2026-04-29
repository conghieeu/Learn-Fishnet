using System.IO;
using System.Threading.Tasks;

public class ResourceDataHandler
{
	private IDataLoader? _loader;

	public void InitLocal(string path)
	{
		_loader = new LocalDataLoader(path);
	}

	public void Init(IDataLoader loader)
	{
		_loader = loader;
	}

	public Stream GetStream(string path)
	{
		if (_loader == null)
		{
			throw new InvalidDataException();
		}
		return _loader.GetStream(path);
	}

	public Stream GetStreamFullPath(string path)
	{
		if (_loader == null)
		{
			throw new InvalidDataException();
		}
		return _loader.GetStreamFullPath(path);
	}

	public string[]? GetFiles(string path)
	{
		if (_loader == null)
		{
			throw new InvalidDataException();
		}
		return _loader.GetFiles(path);
	}

	public string GetDiskPath(string path)
	{
		if (_loader == null)
		{
			throw new InvalidDataException();
		}
		return _loader.GetData(path);
	}

	public async Task<Stream> GetStreamAsync(string path)
	{
		if (_loader == null)
		{
			throw new InvalidDataException();
		}
		return await _loader.GetStreamAsync(path);
	}

	public async Task<Stream> GetStreamFullPathAsync(string path)
	{
		if (_loader == null)
		{
			throw new InvalidDataException();
		}
		return await _loader.GetStreamFullPathAsync(path);
	}
}
