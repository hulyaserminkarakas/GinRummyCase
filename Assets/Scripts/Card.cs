using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public SpriteRenderer cardImage;
    public GameObject cardFilter;
    [HideInInspector] public DataModel.CardData data;
    [HideInInspector] public int value;
    
    [HideInInspector] public int order;

}
