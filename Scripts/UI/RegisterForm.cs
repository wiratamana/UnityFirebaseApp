using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegisterForm : MonoBehaviour
{
    public static event System.Action<string, Gender, System.DateTime> AccountCreated;
    
    public TMP_InputField username;
    public Image female;
    public Image male;

    public Color colorFemale;
    public Color colorMale;
    public Color inactive;

    public TMP_InputField day;
    public TMP_InputField month;
    public TMP_InputField year;

    public bool IsOnScreen => Mathf.Approximately(GetComponent<RectTransform>().localPosition.x, 0);

    [Utils.InvokeByUnityButton]
    public void OnClick_Next()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            return;
        }

        if (female.color == inactive && male.color == inactive)
        {
            // TODO : Show error
            return;
        }

        var gender = female.color == colorFemale ? Gender.Female : Gender.Male;

        var intDayParseResult = int.TryParse(day.text, out int intDay);
        var intMonthParseResult = int.TryParse(month.text, out int intMonth);
        var intYearParseResult = int.TryParse(year.text, out int intYear);

        if (intDayParseResult == false || intMonthParseResult == false || intYearParseResult == false)
        {
            // TODO : Show error
            return;
        }

        var dateString = $"{intMonth}/{intDay}/{intYear}";
        var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
        var dateTimeParseResult = System.DateTime.TryParse(dateString, culture, System.Globalization.DateTimeStyles.None, out System.DateTime birthday);
        if (dateTimeParseResult == false)
        {
            // TODO : Show error
            return;
        }

        AccountCreated?.Invoke(username.text, gender, birthday);
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_ChangeGender(int gender)
    {
        if ((Gender)gender == Gender.Female)
        {
            female.color = colorFemale;
            male.color = inactive;
        }
        else if ((Gender)gender == Gender.Male)
        {
            female.color = inactive;
            male.color = colorMale;
        }
    }
}
