using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardSortInfo
{
    public Dictionary<int,List<Card>>  blocks;

    public List<List<Card>> groupedCards = new();
    public List<Card> discardedCards= new();
    
    public CardSortInfo()
    {
        blocks = new Dictionary<int, List<Card>>();

    }
}

public class Sorter
{
    private const int MinCardToGroup = 3;

    public List<Card> SortByCompleteSequences(List<Card> cardList)
    {
        CardSortInfo sortInfo = new CardSortInfo();
        foreach (var card in cardList)
        {
            if (sortInfo.blocks.ContainsKey((int)card.suit))
                sortInfo.blocks[(int)card.suit].Add(card);

            else
                sortInfo.blocks.Add((int)card.suit, new List<Card> { card });
        }

        foreach (var suitBlock in sortInfo.blocks)
        {
            List<Card> singleSuitList = suitBlock.Value.OrderByDescending(x => x.value).ToList();
            List<Card> groupableCards = new List<Card>();

            Dictionary<int, Card> tempDict = new Dictionary<int, Card>();


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
                    groupableCards = new List<Card>();
                }
                else
                {
                    groupableCards.Add(tempDict[i]);
                }
            }
        }
        
        List<Card> sortedCards = new();
        
        foreach (var cards in sortInfo.groupedCards)
        {
            sortedCards.AddRange(cards);
        }
        sortedCards.AddRange(sortInfo.discardedCards);

        return sortedCards;
    }


    public List<Card> SortByCompleteGroups(List<Card> cardList)
    {
        CardSortInfo sortInfo = new CardSortInfo();
        foreach (var card in cardList)
        {
            if (sortInfo.blocks.ContainsKey(card.value))
                sortInfo.blocks[card.value].Add(card);
            
            else
                sortInfo.blocks.Add(card.value, new List<Card>{card});
            
        }

        sortInfo.blocks = sortInfo.blocks.OrderByDescending(x => x.Key).Reverse().ToDictionary(x => x.Key, x => x.Value);
 
        foreach (var block in sortInfo.blocks)
        {
            if (block.Value.Count >= MinCardToGroup)
                sortInfo.groupedCards.Add(block.Value);
            
            else
                sortInfo.discardedCards.AddRange(block.Value);
            
        }
        List<Card> sortedCards = new();
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
