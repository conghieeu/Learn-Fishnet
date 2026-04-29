using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlagImageLoader : MonoBehaviour
{
	private Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();

	public Texture2D LoadFlagImage(string locale)
	{
		if (imageCache.ContainsKey(locale))
		{
			return imageCache[locale];
		}
		string path = "flag_" + locale + ".png";
		string path2 = Path.Combine(Application.streamingAssetsPath, "Flag", path);
		if (File.Exists(path2))
		{
			byte[] data = File.ReadAllBytes(path2);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			imageCache[locale] = texture2D;
			return texture2D;
		}
		return null;
	}

	public void ClearCache()
	{
		foreach (Texture2D value in imageCache.Values)
		{
			if (value != null)
			{
				Object.Destroy(value);
			}
		}
		imageCache.Clear();
	}

	public void RemoveFromCache(string locale)
	{
		if (imageCache.ContainsKey(locale))
		{
			if (imageCache[locale] != null)
			{
				Object.Destroy(imageCache[locale]);
			}
			imageCache.Remove(locale);
		}
	}

	private void OnDestroy()
	{
		ClearCache();
	}
}
