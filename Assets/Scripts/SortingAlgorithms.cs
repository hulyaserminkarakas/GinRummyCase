using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public class CardSortInfo
{
    public Dictionary<int,List<CardObj>>  blocks;

    public List<CardObj> groupedCards = new();
    public List<CardObj> discardedCards = new();
    public List<CardObj> sortedCards = new();

    public int penalty = 0;
    
    public CardSortInfo()
    {
        blocks = new Dictionary<int, List<CardObj>>();

    }
}

public class Sorter
{
    private const int MinCardToGroup = 3;

    public CardSortInfo SortBySequences(List<CardObj> cardList)
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
                        sortInfo.groupedCards.AddRange(groupableCards);
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

        List<CardObj> sortedCards = new List<CardObj>();
        sortedCards.AddRange(sortInfo.groupedCards);
        sortedCards.AddRange(sortInfo.discardedCards);
        
        sortInfo.penalty = GetPenaltyPoint(sortInfo);
        sortInfo.sortedCards = sortedCards;
        return sortInfo;
    }


    public CardSortInfo SortByGroups(List<CardObj> cardList)
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
                sortInfo.groupedCards.AddRange(block.Value);
            
            else
                sortInfo.discardedCards.AddRange(block.Value);
        }

        List<CardObj> sortedCards = new List<CardObj>();
        sortedCards.AddRange(sortInfo.groupedCards);
        sortedCards.AddRange(sortInfo.discardedCards);
        
        sortInfo.penalty = GetPenaltyPoint(sortInfo);
        sortInfo.sortedCards = sortedCards;
        return sortInfo;
    }


    public CardSortInfo SmartSort(List<CardObj> cardList)
    {
        CardSortInfo sortedBySequence = SortBySequences(cardList);
        CardSortInfo sortedByGroup = SortByGroups(cardList);

        List<CardObj> mutualCards = new();

        foreach (var card in sortedBySequence.groupedCards)
        {
            if (sortedByGroup.groupedCards.Contains(card))
            {
                mutualCards.Add(card);
            }
        }

        CardSortInfo bestSort =  SmartSortAlgorithm(cardList);
        
        if (mutualCards.Count > 0)
        {
            foreach (var card in mutualCards)
            {
                cardList.Remove(card);
                CardSortInfo result = SmartSortAlgorithm(cardList, card);
                if (result.penalty < bestSort.penalty)
                    bestSort = result;
                cardList.Add(card);
            }
        }

        return bestSort;

    }

    private CardSortInfo SmartSortAlgorithm(List<CardObj> cardList, CardObj removedCard = null)
    {
        CardSortInfo sortedBySequence = SortBySequences(cardList);
        CardSortInfo sortedByGroup = SortByGroups(cardList);
        
        //Option1, sequence sort first then group
        if(removedCard != null) sortedBySequence.discardedCards.Add(removedCard);
        CardSortInfo groupCardsOfDiscardedSequence = SortByGroups(sortedBySequence.discardedCards);
        groupCardsOfDiscardedSequence.penalty = GetPenaltyPoint(groupCardsOfDiscardedSequence);
        
        //Option1, group sort first then sequence
        if(removedCard != null) sortedByGroup.discardedCards.Add(removedCard);
        CardSortInfo sequenceCardsOfDiscardedGroups = SortBySequences(sortedByGroup.discardedCards);
        sequenceCardsOfDiscardedGroups.penalty = GetPenaltyPoint(sequenceCardsOfDiscardedGroups);

        List<CardObj> sortedCards = new List<CardObj>();
        
        if (groupCardsOfDiscardedSequence.penalty < sequenceCardsOfDiscardedGroups.penalty)
        {
            sortedCards.AddRange(sortedBySequence.groupedCards);
            sortedCards.AddRange(groupCardsOfDiscardedSequence.groupedCards);
            sortedCards.AddRange(groupCardsOfDiscardedSequence.discardedCards);
        
            groupCardsOfDiscardedSequence.penalty = GetPenaltyPoint(groupCardsOfDiscardedSequence);
            groupCardsOfDiscardedSequence.sortedCards = sortedCards;
            
            return groupCardsOfDiscardedSequence;
        }
        else
        {
            sortedCards.AddRange(sortedByGroup.groupedCards);
            sortedCards.AddRange(sequenceCardsOfDiscardedGroups.groupedCards);
            sortedCards.AddRange(sequenceCardsOfDiscardedGroups.discardedCards);
        
            sequenceCardsOfDiscardedGroups.penalty = GetPenaltyPoint(sequenceCardsOfDiscardedGroups);
            sequenceCardsOfDiscardedGroups.sortedCards = sortedCards;
            
            return sequenceCardsOfDiscardedGroups;
        }
    }
    

    private int GetPenaltyPoint(CardSortInfo sortInfo)
    {
        int penalty = 0;

        foreach (var card in sortInfo.discardedCards)
            penalty += Mathf.Min(card.value,10);

        return penalty;
    }
}
