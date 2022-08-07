using System;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public ThemeResources resource;

    private void Start()
    {
        ServiceLocator.instance.themeManager = this;
    }
    
    private void SetPlayerTheme(ThemeType val)
    {
        PlayerPrefs.SetString("user_theme", val.ToString());
    }

    private ThemeType GetPlayerTheme()
    {
        return (ThemeType) Enum.Parse(typeof(ThemeType),  PlayerPrefs.GetString("user_theme", "WHITE"));
    }

    public void ToggleTheme()
    {
        if (GetPlayerTheme() == ThemeType.WHITE)
            SetPlayerTheme(ThemeType.BLUE);
        
        else if (GetPlayerTheme() == ThemeType.BLUE)
            SetPlayerTheme(ThemeType.WHITE);
    }
    
    public Sprite GetCardImage(int cardID)
    {
       return  resource.GetCardImage(GetPlayerTheme(), cardID);
    }
    
    
    
    public enum ThemeType
    {
        WHITE,
        BLUE
    }
}
