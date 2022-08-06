using System;
using System.Collections.Generic;
using System.Linq;

public class SortingAlgorithms 
{
    
    private List<Card> SortForSet(List<Card> cardList)
    {
        int ValueOrder(Card c1, Card c2)
        {
            int result = c1.value.CompareTo(c2.value);
            return result == 0 ? c1.data.type.CompareTo(c2.data.type) : result;
        }

        cardList.Sort(ValueOrder);
        return cardList;
    }

    private List<Card> SortForRun(List<Card> cardList)
    {
        int SuitToValue(Card c1, Card c2)
        {
            int result = c1.data.type.CompareTo(c2.data.type);
            return result == 0 ? c1.value.CompareTo(c2.value) : result;
        }

        cardList.Sort(SuitToValue);
        return cardList;
    }

    public SortableCardGroup SortByCompleteSequences(List<Card> cardList)
    {
        cardList = SortForRun(cardList);
        
        List<Card> sequences = new List<Card>();
        List<Card> nonSequences = new List<Card>();
        List<CardGroup> sequenceGroups = new List<CardGroup>();
        
        int count = 0;
        int startingIndex = 0;
        
        for (int i = 0; i < cardList.Count; i++)
        {
            if (count == 0)
            {
                startingIndex = i;
                count = 1;
                nonSequences.Add(cardList[i]);
            }
            else if (cardList[i].value == cardList[startingIndex + count].value && cardList[i].data.type == cardList[startingIndex + count].data.type && cardList[startingIndex + count] != null)
            {
                count++;
                nonSequences.Add(cardList[i]);
                if (count == 3)
                {
                    CardGroup group = new CardGroup(GroupType.CompleteSequence);
                    for (int j = startingIndex; j < startingIndex + count; j++)
                    {
                        sequences.Add(cardList[j]);
                        group.AddCard(cardList[j]);

                        if (nonSequences.Contains(cardList[j])) nonSequences.Remove(cardList[j]);
                    }
                    sequenceGroups.Add(group);
                }
                else if (count > 3)
                {
                    CardGroup group = new CardGroup(GroupType.CompleteSequence);
                    group = sequenceGroups[sequenceGroups.Count -1 ];
                    group.AddCard(cardList[i]);
                    sequences.Add(cardList[i]);
                    if (nonSequences.Contains(cardList[i])) nonSequences.Remove(cardList[i]);
                }
            }
            else
            {
                nonSequences.Add(cardList[i]);
                count = 1;
                startingIndex = i;
            }
        }
        
        SortableCardGroup sortableCardGroup = new SortableCardGroup(sequences, nonSequences, sequenceGroups);
        return sortableCardGroup;
    }


    public SortableCardGroup SortByCompleteGroups(List<Card> cardList)
    {
        cardList = SortForSet(cardList);
        List<Card> groups = new List<Card>();
        List<Card> nonGroups = new List<Card>();
        List<CardGroup> groupCardGroups = new List<CardGroup>();
        int count = 0;
        int startingIndex = 0;
        for (int i = 0; i < cardList.Count; i++)
        {
            if (count == 0)
            {
                startingIndex = i;
                count = 1;
                nonGroups.Add(cardList[i]);
            }
            else if (cardList[i].data.type == cardList[startingIndex + count].data.type  && cardList[i].value == cardList[startingIndex + count].value && cardList[startingIndex + count] != null)
            {
                count++;
                nonGroups.Add(cardList[i]);
                if (count == 3)
                {
                    CardGroup group = new CardGroup(GroupType.CompleteGroup);
                    for (int j = startingIndex; j < startingIndex + count; j++)
                    {
                        groups.Add(cardList[j]);
                        group.AddCard(cardList[j]);

                        if (nonGroups.Contains(cardList[j])) nonGroups.Remove(cardList[j]);
                    }
                    groupCardGroups.Add(group);
                }
                else if (count > 3)
                {
                    CardGroup group = new CardGroup(GroupType.CompleteGroup);
                    group = groupCardGroups[groupCardGroups.Count - 1];
                    group.AddCard(cardList[i]);
                    groups.Add(cardList[i]);
                    if (nonGroups.Contains(cardList[i])) nonGroups.Remove(cardList[i]);
                }
            }
            else
            {
                nonGroups.Add(cardList[i]);
                count = 1;
                startingIndex = i;
            }
        }
        SortableCardGroup sortableCardGroup = new SortableCardGroup(groups, nonGroups, groupCardGroups);
        return sortableCardGroup;
    }



    public SortableCardGroup SmartSort(List<Card> listOfCards)
    {
        List<Card> cardList = new List<Card>();
        cardList.AddRange(listOfCards);

        SortableCardGroup completeSequences = SortByCompleteSequences(cardList);
        List<CardGroup> completeSequenceGroups = completeSequences.cardGroup;
        SortableCardGroup completeGroups = SortByCompleteGroups(completeSequences.unsorted);
        List<CardGroup> partialGroups = FindPartialGroupsOrderedByRank(completeGroups.unsorted);

        cardList = completeSequences.sorted;
        cardList.AddRange(completeGroups.sorted);

        foreach (CardGroup sequence in completeSequenceGroups)   
        {
            foreach (CardGroup partialGroup in partialGroups)
            {
                if(sequence.Containsvalue(partialGroup.GetCardvalue()))
                {
                    Card cardToSwap = sequence.FindCardWithvalue(partialGroup.GetCardvalue());
                    
                    if (partialGroup.GetRank() > CalculateBrokenSequence(sequence, cardToSwap))
                    {
                        cardList.Remove(cardToSwap);
                        partialGroup.AddCard(cardToSwap);
                        cardList.AddRange(partialGroup.group);
                        partialGroups.Remove(partialGroup);

                        break;
                    } 
                }
            }
        }

        foreach (var c in listOfCards.Where(c => !cardList.Contains(c)))
        {
            cardList.Add(c);
        }
            

        SortableCardGroup returnSortableCard = new SortableCardGroup(cardList, null);
        return returnSortableCard;
    }

    private int CalculateBrokenSequence(CardGroup sequence, Card card)
    {
        List<Card> sequenceCards = new List<Card>();
        sequenceCards.AddRange(sequence.group);
        sequenceCards.Remove(card);

        int value = 0;
        int count = 0;
        int startingIndex = 0;
        for (int i = 0; i < sequenceCards.Count; i++)
        {
            if (count == 0)
            {
                startingIndex = i;
                count = 1;
                value += sequenceCards[i].value;
            } else if(count == 1)
            {
                startingIndex = i - 1;
                count++;
                value += sequenceCards[i].value;
            }
            else if (sequenceCards[i].value == sequenceCards[startingIndex].value + count)
            {
                count++;
                if (count == 3)
                {
                    value += sequenceCards[i].value;
                    for (int j = startingIndex; j < startingIndex + count; j++)
                    {
                        value -= sequenceCards[j].value;
                    }
                }
            }
            else
            {
                value += sequenceCards[i].value;
                count = 1;
            }
        }
        return value;
    }

    private List<CardGroup> FindPartialGroupsOrderedByRank(List<Card> cardList)
    {
        cardList = SortForSet(cardList);
        List<CardGroup> partialGroups = new List<CardGroup>();

        for (int i = 0; i < cardList.Count; i++)
        {
            for (int j = i+1, length = Math.Min(i + 4, cardList.Count); j < length; j++)
            {
                if (cardList[j] != null && cardList[i].data.type != cardList[j].data.type && cardList[i].value == cardList[j].value)
                {
                    CardGroup group = new CardGroup(GroupType.PartialGroup);
                    group.AddCard(cardList[i]);
                    group.AddCard(cardList[j]);
                    partialGroups.Add(group);
                }
            }
        }

        partialGroups.Sort();
        return partialGroups;
    }
}

public class SortableCardGroup
{
    public List<Card> sorted;
    public List<Card> unsorted;
    public List<CardGroup> cardGroup;

    public SortableCardGroup(List<Card> sorted, List<Card> unsorted, List<CardGroup> cardGroup = null)
    {
        this.sorted = sorted;
        this.unsorted = unsorted;
        this.cardGroup = cardGroup;
    }
}

public class CardGroup : IComparable
{
    GroupType type;
    public List<Card> group { get; set; }

    private int rank;

    public CardGroup(GroupType type)
    {
        this.type = type;
        group = new List<Card>();
    }

    public void AddCard(Card c)
    {
        group.Add(c);
        rank += c.data.penaltyPoint;
    }

    public int GetRank()
    {
        return rank;
    }

    public int GetCardvalue()
    {
        if (type == GroupType.CompleteGroup || type == GroupType.PartialGroup)
            return group[0].value;
        else return -1;
    }

    public bool Containsvalue(int value)
    {
        if (type == GroupType.CompleteGroup || type == GroupType.PartialGroup)
        {
            return value == group[0].value;
        } else
        {
            return group.Any(c => value == c.value);
        }
    }   

    public bool Contains(CardGroup cg)
    {
        return cg.group.Any(c => group.Contains(c));
    }

    public Card FindCardWithvalue(int value)
    {
        if(type == GroupType.CompleteSequence || type == GroupType.PartialSequence)
        {
            return group.FirstOrDefault(c => c.value == value);
        }
        return null;
    }

    public int CompareTo(object obj)
    {
        return -1;
    }
}

public enum GroupType
{
    CompleteGroup,
    CompleteSequence,
    PartialGroup,
    PartialSequence
}