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
    
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        CardObj other = obj as CardObj;
        if ((object)other == null)
            return false;


        return this.value == other.value
               && this.suit.Equals(other.suit);
    }
}
public enum Suit
{
    SPADES,
    DIAMONDS,
    HEARTS,
    CLUBS
}