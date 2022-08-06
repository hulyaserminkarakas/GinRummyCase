using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObj 
{
    public DataModel.CardData data;
    public int value;
    public Suit suit;
    public int order;
    
    public CardObj(Suit suit, int value)
    {
        this.suit = suit;
        this.value = value;
    }
    
    public enum Suit
    {
        SPADES,
        DIAMONDS,
        HEARTS,
        CLUBS
    }
}
