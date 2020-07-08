using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMethods : MonoBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("EditorMethods/ClearSaveData")]
    private static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    [UnityEditor.MenuItem("EditorMethods/GetTamanaDll")]
    private static void GetTamanaDll()
    {
        const string tamanaDllPath = @"C:\Users\wirat\source\repos\Tamana\Tamana\bin\Release\Tamana.dll";
        string destination = System.IO.Path.Combine(Application.dataPath, "Tamana.dll");
        System.IO.File.Copy(tamanaDllPath, destination, true);
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
