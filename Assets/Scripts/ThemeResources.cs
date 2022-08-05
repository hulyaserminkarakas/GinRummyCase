using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeResources : MonoBehaviour
{
    [SerializeField] private List<Sprite> whiteThemeCards;
    [SerializeField] private List<Sprite> blueThemeCards;

    public Sprite GetCardImage(ThemeManager.ThemeType theme, int cardID)
    {
        if (theme == ThemeManager.ThemeType.BLUE)
            return blueThemeCards[cardID];
        if(theme == ThemeManager.ThemeType.WHITE)
            return whiteThemeCards[cardID];

        throw new Exception("Error while setting theme!");
    }
}
