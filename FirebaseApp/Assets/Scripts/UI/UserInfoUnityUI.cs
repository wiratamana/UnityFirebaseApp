using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ユーザー名、誕生日、スコア、性別、などの UnityUI リファレンスの集まり。
/// </summary>
[Serializable]
public class UserInfoUnityUI
{
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI birthday;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Image gender;

    [SerializeField] private Sprite spriteMale;
    [SerializeField] private Sprite spriteFemale;

    [SerializeField] private Color colorMale;
    [SerializeField] private Color colorFemale;

    public TextMeshProUGUI Username => username;
    public TextMeshProUGUI Birthday => birthday;
    public TextMeshProUGUI Score => score;
    public Image Gender => gender;


    public Sprite SpriteMale => spriteMale;
    public Sprite SpriteFemale => spriteFemale;


    public Color ColorMale => colorMale;
    public Color ColorFemale => colorFemale;

    /// <summary>
    /// 値を代入する。
    /// </summary>
    /// <param name="userInfoData">ユーザー情報</param>
    public void SetValue(UserInfoData userInfoData)
    {        
        Username.text = userInfoData.Username;
        Birthday.text = userInfoData.BirthdayStringJP;
        Gender.color = userInfoData.Gender == global::Gender.Female ? ColorFemale : ColorMale;
        Gender.sprite = userInfoData.Gender == global::Gender.Female ? SpriteFemale : SpriteMale;
        Score.text = userInfoData.Score.ToString();
    }
}
