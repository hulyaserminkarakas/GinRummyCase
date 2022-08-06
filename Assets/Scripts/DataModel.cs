
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataModel
{
    public CardDeck cards;
    
    public DataModel(string jsonString)
    {
        cards =  JsonUtility.FromJson<CardDeck>(jsonString);
    }

    [Serializable]
    public class CardDeck
    {
        public List<CardData> cardStats;

        public CardDeck(List<CardData> cardStats)
        {
            this.cardStats = cardStats;
        }
    }

    [Serializable]
    public class CardData {
        public int cardID;
        public string shownName;
        public int penaltyPoint;
        public string type;
        
        public CardData(int cardID, string shownName, int penaltyPoint, string type)
        {
            this.cardID = cardID;
            this.shownName = shownName;
            this.penaltyPoint = penaltyPoint;
            this.type = type;
        }
    }
}
