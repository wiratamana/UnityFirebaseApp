using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Firebase.Functions;

/// <summary>
/// Cloud Function を呼び出すための関数の集まり。
/// </summary>
public static class FBFunctions
{
    private const string TOKYO = "asia-northeast1";

    /// <summary>
    /// Cloud Function を呼び出す。
    /// </summary>
    /// <typeparam name="T">基本的に プリミティブ 型や <see cref="Dictionary{string, object}"/></typeparam>
    /// <param name="functionName">Cloud Function の関数名</param>
    /// <param name="parameters">Cloud Function　に渡す引数</param>
    /// <param name="region">レギオンです。</param>
    /// <returns>T で指定した変数型を返す</returns>
    private static async Task<T> CallAsyncTask<T>(string functionName, Dictionary<string, object> parameters = null, string region = null)
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

    /// <summary>
    /// 空のユーザー情報を作成する。
    /// </summary>
    /// <returns>ユーザーID</returns>
    public static async Task<string> CreateEmptyUser()
    {
        const string FUNCTION_NAME = "createEmptyUser";
        return await CallAsyncTask<string>(FUNCTION_NAME, null, TOKYO);
    }

    /// <summary>
    /// メッセージを送る
    /// </summary>
    /// <returns>ID 付きの <see cref="ChatObject"/> を返す</returns>
    public static async Task<Dictionary<string, string>> SendMessageAsyncTask(Dictionary<string, object> data)
    {
        const string FUNCTION_NAME = "sendMessage";
        return await CallAsyncTask<Dictionary<string, string>>(FUNCTION_NAME, data, TOKYO);
    }
}
