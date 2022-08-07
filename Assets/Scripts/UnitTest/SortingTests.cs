using System.Collections.Generic;
using NUnit.Framework;


public class SortingTests
{
    private Sorter sorter = new Sorter();
    private  List<CardObj> cardDeck;

    private void InitCardDeck()
    {
        cardDeck = new List<CardObj>();
        
        cardDeck.Add(new CardObj(Suit.HEARTS, 1));
        cardDeck.Add(new CardObj(Suit.SPADES, 2));
        cardDeck.Add(new CardObj(Suit.DIAMONDS, 5));
        cardDeck.Add(new CardObj(Suit.HEARTS, 4));
        cardDeck.Add(new CardObj(Suit.SPADES, 1));
        cardDeck.Add(new CardObj(Suit.DIAMONDS, 3));
        cardDeck.Add(new CardObj(Suit.CLUBS, 4));
        cardDeck.Add(new CardObj(Suit.SPADES, 4));
        cardDeck.Add(new CardObj(Suit.DIAMONDS, 1));
        cardDeck.Add(new CardObj(Suit.SPADES, 3));
        cardDeck.Add(new CardObj(Suit.DIAMONDS, 4));
    }
    
    
    [Test]
    public void SequenceSortTest()
    {
        InitCardDeck();

        List<CardObj> expectedOutput = new List<CardObj>();
        
        expectedOutput.Add(new CardObj(Suit.SPADES, 1));
        expectedOutput.Add(new CardObj(Suit.SPADES, 2));
        expectedOutput.Add(new CardObj(Suit.SPADES, 3));
        expectedOutput.Add(new CardObj(Suit.SPADES, 4));
        
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 3));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 4));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 5));
        
        expectedOutput.Add(new CardObj(Suit.HEARTS, 1));
        expectedOutput.Add(new CardObj(Suit.HEARTS, 4));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 1));
        expectedOutput.Add(new CardObj(Suit.CLUBS, 4));
        
        
        CardSortInfo sortedList = sorter.SortBySequences(cardDeck);
        CollectionAssert.AreEqual(expectedOutput, sortedList.sortedCards);
    }
    
    [Test]
    public void GroupSortTest()
    {
        InitCardDeck();

        List<CardObj> expectedOutput = new List<CardObj>();
        
        expectedOutput.Add(new CardObj(Suit.HEARTS, 1));
        expectedOutput.Add(new CardObj(Suit.SPADES, 1));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 1));

        expectedOutput.Add(new CardObj(Suit.HEARTS, 4));
        expectedOutput.Add(new CardObj(Suit.CLUBS, 4));
        expectedOutput.Add(new CardObj(Suit.SPADES, 4));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 4));
        
        expectedOutput.Add(new CardObj(Suit.SPADES, 2));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 3));
        expectedOutput.Add(new CardObj(Suit.SPADES, 3));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 5));
        
        CardSortInfo sortedList = sorter.SortByGroups(cardDeck);
        CollectionAssert.AreEqual(expectedOutput, sortedList.sortedCards);
    }
    
    [Test]
    public void SmartSortTest()
    {
        InitCardDeck();

        List<CardObj> expectedOutput = new List<CardObj>();
        
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 3));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 4));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 5));
        
        expectedOutput.Add(new CardObj(Suit.SPADES, 1));
        expectedOutput.Add(new CardObj(Suit.SPADES, 2));
        expectedOutput.Add(new CardObj(Suit.SPADES, 3));

        expectedOutput.Add(new CardObj(Suit.HEARTS, 4));
        expectedOutput.Add(new CardObj(Suit.CLUBS, 4));
        expectedOutput.Add(new CardObj(Suit.SPADES, 4));
        
        expectedOutput.Add(new CardObj(Suit.HEARTS, 1));
        expectedOutput.Add(new CardObj(Suit.DIAMONDS, 1));
        
        CardSortInfo sortedList = sorter.SmartSort(cardDeck);
        CollectionAssert.AreEqual(expectedOutput, sortedList.sortedCards);
    }
}
