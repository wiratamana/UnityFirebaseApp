using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    [AttributeUsage(AttributeTargets.Method)]
    public class InvokeByUnityButton : Attribute { }
}
