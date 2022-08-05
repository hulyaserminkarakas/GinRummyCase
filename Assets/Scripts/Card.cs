using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public SpriteRenderer cardImage;
    public GameObject cardFilter;
    [HideInInspector] public int id;

    [HideInInspector] public int order;

}
