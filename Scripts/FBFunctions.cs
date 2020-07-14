using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Functions;

public static class FBFunctions
{
    private const string TOKYO = "asia-northeast1";

    public static async Task<T> CallAsyncTask<T>(string functionName, Dictionary<string, object> parameters = null, string region = null)
    {
        var functions = string.IsNullOrEmpty(region) ? FirebaseFunctions.DefaultInstance : FirebaseFunctions.GetInstance(region);

        var function = functions.GetHttpsCallable(functionName);
        try
        {
            var result = await function.CallAsync(parameters);

            if (result.Data == null)
            {
                return default;
            }

            Type dataType = result.Data.GetType();
            bool isDataDict = dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

            var tType = typeof(T);
            bool isTDict = tType.IsGenericType && tType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

            if (isTDict && isDataDict)
            {
                var data = result.Data as Dictionary<object, object>;

                var keyType = tType.GetGenericArguments()[0];
                var valueType = tType.GetGenericArguments()[1];

                Type dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                var dict = Activator.CreateInstance(dictType);
                var addMethod = dict.GetType().GetMethod("Add");

                foreach (var item in data)
                {
                    addMethod.Invoke(dict, new[] { item.Key, item.Value });
                }

                return (T)dict;
            }

            return (T)result.Data;
        }
        catch
        {
            return default;
        }               
    }

    public static async Task<string> CreateEmptyUser()
    {
        const string FUNCTION_NAME = "createEmptyUser";
        return await CallAsyncTask<string>(FUNCTION_NAME, null, TOKYO);
    }
}
