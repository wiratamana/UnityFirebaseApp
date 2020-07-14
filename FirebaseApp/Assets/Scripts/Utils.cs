using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
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

    [AttributeUsage(AttributeTargets.Method)]
    public class InvokeByUnity : Attribute { }
}
