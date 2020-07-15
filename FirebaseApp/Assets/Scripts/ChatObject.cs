using System.Collections.Generic;
using Tamana;

/// <summary>
/// 送るメッセージの内容。
/// </summary>
public readonly struct ChatObject
{
    public readonly string MessageUniqueID;
    public readonly string UserUniqueID;
    public readonly string Message;
    public readonly string DateUTC;

    public string ElapsedTimeSinceMessageSentString
    {
        get
        {
            if (string.IsNullOrEmpty(DateUTC))
            {
                return "送信中...";
            }

            var time = TimeSpanSinceMessageSent;
            if ((int)time.TotalDays > 30)
            {
                return "昔から";
            }
            else if ((int)time.TotalDays > 0)
            {
                return $"{(int)System.Math.Floor(time.TotalDays)} 日前";
            }
            else if ((int)time.TotalHours > 0)
            {
                return $"{(int)System.Math.Floor(time.TotalHours)} 時間前";
            }
            else if ((int)time.TotalMinutes > 0)
            {
                return $"{(int)System.Math.Floor(time.TotalMinutes)} 分前";
            }
            else
            {
                return "たった今";
            }
        }
    }

    public System.TimeSpan TimeSpanSinceMessageSent 
    {
        get
        {
            var dateUTC = System.DateTime.Parse(DateUTC);
            var now = System.DateTime.Now;
            return now - dateUTC;
        }
    }

    public ChatObject(string message, string userUniqueID)
    {
        MessageUniqueID         = null;
        Message                 = message;
        UserUniqueID            = userUniqueID;
        DateUTC                 = null;
    }

    public ChatObject(Dictionary<string, string> data)
    {
        MessageUniqueID         = data.GetValueIfExist(nameof(MessageUniqueID));
        Message                 = data.GetValueIfExist(nameof(Message));
        UserUniqueID            = data.GetValueIfExist(nameof(UserUniqueID));
        DateUTC                 = data.GetValueIfExist(nameof(DateUTC));
    }

    public override string ToString()
    {
        return $"MessageUniqueID : {MessageUniqueID} | UserUniqueID : {UserUniqueID} | Message : {Message} | Date : {ElapsedTimeSinceMessageSentString}";
    }

    public static bool operator ==(ChatObject a, ChatObject b)
    {
        return a.MessageUniqueID == b.MessageUniqueID;
    }

    public static bool operator !=(ChatObject a, ChatObject b)
    {
        return a.MessageUniqueID != b.MessageUniqueID;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(ChatObject))
        {
            return false;
        }

        return (ChatObject)obj == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}



