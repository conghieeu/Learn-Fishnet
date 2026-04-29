using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static bool bShutdown;

	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null && !bShutdown)
			{
				T val = Object.FindFirstObjectByType<T>();
				if (val == null)
				{
					val = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
				}
				InstanceInit(val);
			}
			return _instance;
		}
	}

	private static void InstanceInit(Object instance)
	{
		_instance = instance as T;
		_instance.Init();
	}

	public virtual void Init()
	{
		Object.DontDestroyOnLoad(_instance);
	}

	public virtual void OnDestroy()
	{
		_instance = null;
	}

	private void OnApplicationQuit()
	{
		_instance = null;
		bShutdown = true;
	}
}
