using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public class CardSortInfo
{
    public Dictionary<int,List<CardObj>>  blocks;

    public List<List<CardObj>> groupedCards = new();
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

        List<CardObj> sortedCards = new List<CardObj>();
        foreach (var cards in sortInfo.groupedCards)
            sortedCards.AddRange(cards);
        
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
                sortInfo.groupedCards.Add(block.Value);
            
            else
                sortInfo.discardedCards.AddRange(block.Value);
        }

        List<CardObj> sortedCards = new List<CardObj>();
        foreach (var cards in sortInfo.groupedCards)
            sortedCards.AddRange(cards);
        
        sortedCards.AddRange(sortInfo.discardedCards);
        
        sortInfo.penalty = GetPenaltyPoint(sortInfo);
        sortInfo.sortedCards = sortedCards;
        return sortInfo;
    }

    public CardSortInfo SmartSort(List<CardObj> cardList)
    {
        CardSortInfo sortedBySequence = SortBySequences(cardList);
        CardSortInfo sortedByGroup = SortByGroups(cardList);

        List<List<CardObj>> potentialBlocks = new();

        foreach (var cards in sortedByGroup.groupedCards)
        {
            potentialBlocks.AddRange(GetSubBlocksOfGroups(cards));
        }
        
        foreach (var cards in sortedBySequence.groupedCards)
        {
            potentialBlocks.AddRange(GetSubBlocksOfSequence(cards));
        }

        Dictionary<int, CardSortInfo> sorts = new Dictionary<int, CardSortInfo>();
        List<CardObj> sortedCards = new List<CardObj>();
        CardSortInfo sortInfo =  sortedBySequence;
        
        sorts.Add(GetPenaltyPoint(sortInfo), sortInfo);
        
        foreach (var firstBlockList in potentialBlocks)
        {
            sortInfo = new CardSortInfo();
            foreach (var secondBlockList in potentialBlocks)
            {
                sortInfo = new CardSortInfo();
                if(IsMutualCardExist(firstBlockList, secondBlockList))
                    continue;
                
                foreach (var thirdBlockList in potentialBlocks)
                {
                    sortInfo = new CardSortInfo();
                    if(IsMutualCardExist(firstBlockList, thirdBlockList))
                        continue;
                    
                    if(IsMutualCardExist(secondBlockList, thirdBlockList))
                        continue;
                    
                    sortInfo.groupedCards.Add(firstBlockList);
                    sortInfo.groupedCards.Add(secondBlockList);
                    sortInfo.groupedCards.Add(thirdBlockList);

                    sortedCards = new List<CardObj>();
                    sortedCards.AddRange(firstBlockList);
                    sortedCards.AddRange(secondBlockList);
                    sortedCards.AddRange(thirdBlockList);

                    sortInfo.discardedCards = GetRemainingCards(cardList, sortedCards);
                    
                    sortedCards.AddRange(sortInfo.discardedCards);
                    sortInfo.sortedCards = sortedCards;
                    if (sorts.ContainsKey(GetPenaltyPoint(sortInfo)))
                        sorts[GetPenaltyPoint(sortInfo)] = sortInfo;
                    else
                        sorts.Add(GetPenaltyPoint(sortInfo), sortInfo);

                }
                
                sortInfo.groupedCards.Add(firstBlockList);
                sortInfo.groupedCards.Add(secondBlockList);

                sortedCards = new List<CardObj>();
                sortedCards.AddRange(firstBlockList);
                sortedCards.AddRange(secondBlockList);

                sortInfo.discardedCards = GetRemainingCards(cardList, sortedCards);
                    
                sortedCards.AddRange(sortInfo.discardedCards);
                sortInfo.sortedCards = sortedCards;
                if (sorts.ContainsKey(GetPenaltyPoint(sortInfo)))
                    sorts[GetPenaltyPoint(sortInfo)] = sortInfo;
                else
                    sorts.Add(GetPenaltyPoint(sortInfo), sortInfo);
                
            }
            
            sortInfo.groupedCards.Add(firstBlockList);

            sortedCards = new List<CardObj>();
            sortedCards.AddRange(firstBlockList);

            sortInfo.discardedCards = GetRemainingCards(cardList, sortedCards);
                    
            sortedCards.AddRange(sortInfo.discardedCards);
            sortInfo.sortedCards = sortedCards;
            if (sorts.ContainsKey(GetPenaltyPoint(sortInfo)))
                sorts[GetPenaltyPoint(sortInfo)] = sortInfo;
            else
                sorts.Add(GetPenaltyPoint(sortInfo), sortInfo);
            
        }

        CardSortInfo bestSort = sorts[sorts.Keys.Min()];
        bestSort.penalty = GetPenaltyPoint(bestSort);
        return bestSort;
    }
    
    private List<CardObj> GetRemainingCards(List<CardObj> allList, List<CardObj> addedItems)
    {
        List<CardObj> result = new();
        result.AddRange(allList);
        
        foreach (var card in addedItems)
            result.Remove(card);

        return result;
    }
    
    private List<List<CardObj>> GetSubBlocksOfSequence(List<CardObj> cards)
    {
        List<List<CardObj>> finalList = new();

        if (cards.Count == MinCardToGroup) 
            finalList.Add(cards);
        else if(cards.Count > MinCardToGroup)
        {
            finalList.Add(cards);
            for (int i = 0; i < cards.Count - MinCardToGroup; i++)
                finalList.Add(cards.GetRange(i,MinCardToGroup));
        }

        return finalList;
    }


    private List<List<CardObj>> GetSubBlocksOfGroups(List<CardObj> cards)
    {
        List<List<CardObj>> finalList = new();
        if (cards.Count == MinCardToGroup) 
            finalList.Add(cards);
        else if(cards.Count > MinCardToGroup)
        {
            finalList.Add(cards);
            foreach (var card in cards)
            {
                List<CardObj> tempList = new List<CardObj>();
                tempList.AddRange(cards);
                tempList.Remove(card);
                finalList.Add(tempList);
            }
        }

        return finalList;
    }

    private bool IsMutualCardExist(List<CardObj> first, List<CardObj> second)
    {
        foreach (var card in first)
            if (second.Contains(card))
                return true;

        return false;
    }

    /*public CardSortInfo SmartSort(List<CardObj> cardList)
    {
        CardSortInfo sortedBySequence = SortBySequences(cardList);
        CardSortInfo sortedByGroup = SortByGroups(cardList);

        List<CardObj> mutualCards = new();

        List<CardObj> sequencedCards = new();
        foreach (var cards in sortedBySequence.groupedCards)
            sequencedCards.AddRange(cards);
        
        
        List<CardObj> groupedCards = new();
        foreach (var cards in sortedBySequence.groupedCards)
            groupedCards.AddRange(cards);

        foreach (var card in groupedCards)
            if (sequencedCards.Contains(card))
                mutualCards.Add(card);
        

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
            foreach (var cards in sortedBySequence.groupedCards)
                sortedCards.AddRange(cards);
            foreach (var cards in groupCardsOfDiscardedSequence.groupedCards)
                sortedCards.AddRange(cards);
            sortedCards.AddRange(groupCardsOfDiscardedSequence.discardedCards);
        
            groupCardsOfDiscardedSequence.penalty = GetPenaltyPoint(groupCardsOfDiscardedSequence);
            groupCardsOfDiscardedSequence.sortedCards = sortedCards;
            
            return groupCardsOfDiscardedSequence;
        }
        else
        {
            foreach (var cards in sortedByGroup.groupedCards)
                sortedCards.AddRange(cards);
            foreach (var cards in sequenceCardsOfDiscardedGroups.groupedCards)
                sortedCards.AddRange(cards);
            sortedCards.AddRange(sequenceCardsOfDiscardedGroups.discardedCards);
        
            sequenceCardsOfDiscardedGroups.penalty = GetPenaltyPoint(sequenceCardsOfDiscardedGroups);
            sequenceCardsOfDiscardedGroups.sortedCards = sortedCards;
            
            return sequenceCardsOfDiscardedGroups;
        }
    }*/
    

    private int GetPenaltyPoint(CardSortInfo sortInfo)
    {
        int penalty = 0;

        foreach (var card in sortInfo.discardedCards)
            penalty += Mathf.Min(card.value,10);

        return penalty;
    }
}
