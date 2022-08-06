using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardSortInfo
{
    public Dictionary<int,List<CardObj>>  blocks;

    public List<List<CardObj>> groupedCards = new();
    public List<CardObj> discardedCards= new();
    
    public CardSortInfo()
    {
        blocks = new Dictionary<int, List<CardObj>>();

    }
}

public class Sorter
{
    private const int MinCardToGroup = 3;

    public List<CardObj> SortBySequences(List<CardObj> cardList)
    {
        CardSortInfo sortInfo = new CardSortInfo();
        foreach (var card in cardList)
        {
            if (sortInfo.blocks.ContainsKey((int)card.suit))
                sortInfo.blocks[(int)card.suit].Add(card);

            else
                sortInfo.blocks.Add((int)card.suit, new List<CardObj> { card });
        }

        foreach (var suitBlock in sortInfo.blocks)
        {
            List<CardObj> singleSuitList = suitBlock.Value.OrderByDescending(x => x.value).ToList();
            List<CardObj> groupableCards = new List<CardObj>();

            Dictionary<int, CardObj> tempDict = new Dictionary<int, CardObj>();


            for (int i = 0; i < 14; i++)
                tempDict.Add(i, null);

            foreach (var card in singleSuitList)
                tempDict[card.value - 1] = card;
            
            for (int i = 0; i< tempDict.Values.Count; i++ )
            {
                if (tempDict[i] == null)
                {
                    if (groupableCards.Count >= MinCardToGroup)
                    {
                        sortInfo.groupedCards.Add(groupableCards);
                    }
                    else
                    {
                        sortInfo.discardedCards.AddRange(groupableCards);
                    }
                    groupableCards = new List<CardObj>();
                }
                else
                {
                    groupableCards.Add(tempDict[i]);
                }
            }
        }
        
        List<CardObj> sortedCards = new();
        
        foreach (var cards in sortInfo.groupedCards)
        {
            sortedCards.AddRange(cards);
        }
        sortedCards.AddRange(sortInfo.discardedCards);

        return sortedCards;
    }


    public List<CardObj> SortByGroups(List<CardObj> cardList)
    {
        CardSortInfo sortInfo = new CardSortInfo();
        foreach (var card in cardList)
        {
            if (sortInfo.blocks.ContainsKey(card.value))
                sortInfo.blocks[card.value].Add(card);
            
            else
                sortInfo.blocks.Add(card.value, new List<CardObj>{card});
            
        }

        sortInfo.blocks = sortInfo.blocks.OrderByDescending(x => x.Key).Reverse().ToDictionary(x => x.Key, x => x.Value);
 
        foreach (var block in sortInfo.blocks)
        {
            if (block.Value.Count >= MinCardToGroup)
                sortInfo.groupedCards.Add(block.Value);
            
            else
                sortInfo.discardedCards.AddRange(block.Value);
            
        }
        List<CardObj> sortedCards = new();
        foreach (var cards in sortInfo.groupedCards)
        {
            sortedCards.AddRange(cards);
        }
        sortedCards.AddRange(sortInfo.discardedCards);

        return sortedCards;
    }


    public List<Card> SmartSort(List<Card> cardList)
    {
        return null;

    }
    
}
