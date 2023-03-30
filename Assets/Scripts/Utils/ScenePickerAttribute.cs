using UnityEngine;
public class ScenePickerAttribute : PropertyAttribute
{ 
    public static string GetName(string scenePath)
    {
        return System.IO.Path.GetFileNameWithoutExtension(scenePath);
    }
}