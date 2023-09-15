using UnityEngine;
using System.IO;

/// <summary>
/// Handles file management
/// </summary>
public class FileManager : MonoBehaviour
{
    public static string GetDocumentsPath()
    {

        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        path = path.Substring(0, path.LastIndexOf('/'));
#if UNITY_EDITOR
        if (!Directory.Exists(path + "/Assets/Text Adventure/Assets/Resources"))
            System.IO.Directory.CreateDirectory(path + "/Assets/Text Adventure/Assets/Resources/");

        return path + "/Assets/Text Adventure/Assets/Resources";
#endif
    }
}
