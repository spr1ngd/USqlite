
using UnityEngine;

namespace miniMVC
{
    public class FilePath
    {
        public static string prePath
        {
            get
            {
#if UNITY_EDITOR
                return "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
                return "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROUD
                return "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
                return Application.dataPath + "/Raw/";
#endif
            }
        }

        public static string normalPath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + "/StreamingAssets/";
#elif UNITY_STANDALONE_WIN
                return "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROUD
                return "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
                return Application.dataPath + "/Raw/";
#endif
            }
        }
        
        public static string fullPath(string fileName)
        {
            return prePath + fileName;
        }
    }
}