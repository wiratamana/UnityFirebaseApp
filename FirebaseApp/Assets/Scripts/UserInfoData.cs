using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public readonly struct UserInfoData
{
    public readonly string UserUniqueID;
    public readonly string Username;
    public readonly DateTime Birthday;
    public readonly Gender Gender;
    public readonly long Score;

    public string BirthdayStringJP => $"{Birthday.Year}年{Birthday.Month}月{Birthday.Day}日";

    public UserInfoData(string userUniqueID, string userName, DateTime birthday, Gender gender, long score)
    {
        UserUniqueID = userUniqueID;
        Username = userName;
        Birthday = birthday;
        Gender = gender;
        Score = score;
    }

    public bool IsNull => string.IsNullOrEmpty(UserUniqueID);
    public static UserInfoData Null => new UserInfoData(null, null, DateTime.MinValue, Gender.Female, 0);

    public override string ToString()
    {
        return $"ID : {UserUniqueID} | Username : {Username} | Birthday : {BirthdayStringJP} | Gender : {Gender} | Score : {Score}";
    }
}
