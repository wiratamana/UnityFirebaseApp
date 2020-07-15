using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// 1234567890ABCDEF を 1234-5678-90AB-CDEF に変換し返す。
    /// </summary>
    /// <param name="id">Unity の Input UI から入力された文字列型の値。</param>
    /// <returns>変換した ID を返す</returns>
    public static string GetUserUniqueIDFromInput(string id)
    {
        if (id.Length < 12)
        {
            return null;
        }

        var sb = new StringBuilder();
        int stride = 3;
        foreach (var c in id)
        {
            sb.Append(c);
            stride--;

            if (stride == 0 && sb.Length < 15)
            {
                sb.Append('-');
                stride = 3;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// キーボードが出るときに、キーボードの高さを変えす。
    /// </summary>
    /// <param name="includeInput">キーボードが出るときに、入力した文字列のプレビュー欄の高さを含みますか？</param>
    /// <returns>キーボードの高さ</returns>
    public static float GetTouchScreenKeyboardHeight(bool includeInput)
    {
#if UNITY_ANDROID
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
            var view = unityPlayer.Call<AndroidJavaObject>("getView");

            var result = 0;

            if (view != null)
            {
                using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    view.Call("getWindowVisibleDisplayFrame", rect);
                    result = Display.main.systemHeight - rect.Call<int>("height");
                }

                if (includeInput)
                {
                    var dialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
                    var decorView = dialog?.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");

                    if (decorView != null)
                    {
                        var decorHeight = decorView.Call<int>("getHeight");
                        result += decorHeight;
                    }
                }
            }

            return result;
        }
#else
        return 0.0f;
#endif
    }

    /// <summary>
    /// Unity の ボタンから呼び出される。マークのことです。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InvokeByUnity : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class InheritedByUnityInterface : Attribute { } 
}
