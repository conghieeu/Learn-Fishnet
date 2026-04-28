using UnityEngine;

namespace MellowAbelson.Core.Save
{
    public class SaveManager : MonoBehaviour
    {
        public void Save<T>(string key, T data)
        {
            ES3.Save(key, data);
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            return ES3.KeyExists(key) ? ES3.Load<T>(key) : defaultValue;
        }

        public bool HasKey(string key)
        {
            return ES3.KeyExists(key);
        }

        public void Delete(string key)
        {
            if (ES3.KeyExists(key))
                ES3.DeleteKey(key);
        }

        public void DeleteAll()
        {
            ES3.DeleteCachedFile();
        }
    }
}
