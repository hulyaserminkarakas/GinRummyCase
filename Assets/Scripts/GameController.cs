using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    
    [SerializeField] private Transform cardHolder;
    [SerializeField] private Text penaltyText;

    public int cardCount = 11;
    
    public List<Vector3> cardPositions = new();
    public List<Card> gameCards = new();

    public DataModel dataModel;

    private Dictionary<int, bool> cardUsageList = new();
    private List<int> ownCardIDList = new();

    private Sorter sorter;
    void Start()
    {
        ServiceLocator.instance.gameController = this;
        ServiceLocator.instance.pool.SetDefault();

        sorter = new Sorter();
        
        ParseCardDeckData();
        DetermineCardList();
        PlaceCards();
    }

    private void ParseCardDeckData()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/Data/CardDeck.json");
        dataModel = new DataModel(jsonString);

        foreach (DataModel.CardData card in dataModel.cards.cardStats)
        {
            cardUsageList.Add(card.cardID,false);
        }
    }

    private void DetermineCardList()
    {
        for (int i = 0; i < cardCount; i++)
        {
            ownCardIDList.Add(GetRandomID());
        }
    }

    private int GetRandomID()
    {
        int id = Random.Range(0, dataModel.cards.cardStats.Count);
        if (ownCardIDList.Contains(id))
            return GetRandomID();
        return id;
    }


    private void PlaceCards()
    {
        float initPos = -4.4f;
        float step = - initPos /5f;

        Card cardObject;

        for (int i = 0; i < cardCount; i++)
        {
            cardObject = ServiceLocator.instance.pool.GetPooledCard();

            DataModel.CardData data = dataModel.cards.cardStats[ownCardIDList[i]];
            cardObject.cardObj = new CardObj((CardObj.Suit)Enum.Parse(typeof(CardObj.Suit), data.type) , data.cardID % 13 + 1);
            cardObject.cardObj.data = data;
            cardObject.transform.SetParent(cardHolder);
            cardObject.gameObject.SetActive(true);
            cardObject.transform.position = new Vector3(initPos + step * i,0 , 0);
            
            
            // cardObject.transform.position = new Vector3(initPos + step * i, - Mathf.Abs((initPos + step * i )* 0.12f), 0);
            // cardObject.transform.Rotate(Vector3.back * (initPos + step * i) * 3) ;
            
            
            cardObject.cardObj.order = i; 
            
            cardObject.cardImage.sortingOrder = i;
            cardObject.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(data.cardID );
            
            cardUsageList[data.cardID] = true;
            
            gameCards.Add(cardObject);
            
            cardPositions.Add(new Vector3(initPos + step * i, 0, 0));
        }
    }

    
    public Card GetCardObjectByOrder(int order)
    {
        foreach (var card in gameCards)
        {
            if (card.cardObj.order == order)
                return card;
        }

        throw new Exception("Card not found, check order!");
    }
    
    private List<CardObj> GetCardObjList(List<Card> gameCards)
    {
        List<CardObj> cards = new List<CardObj>();

        foreach (var card in gameCards)
            cards.Add(card.cardObj);
        
        return cards;
    }


    public void ChangeTheme()
    {
        ServiceLocator.instance.themeManager.ToggleTheme();
        
        foreach (var card in gameCards)
        {
            card.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(card.cardObj.data.cardID);
        }
    }

    private void ReorderCards(List<CardObj> cards)
    {
        Card cardObject;
        
        for (int i = 0; i < cards.Count; i++)
        {
            cardObject = GetCardByID(cards[i].data.cardID);
            
            cardObject.cardObj.data = dataModel.cards.cardStats[cards[i].data.cardID];
            cardObject.gameObject.SetActive(true);
            cardObject.transform.position = cardPositions[i];
            cardObject.cardObj.order = i; 
            
            cardObject.cardImage.sortingOrder = i;
            cardObject.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(cardObject.cardObj.data.cardID );
        }
    }

    private Card GetCardByID(int id)
    {
        foreach (var card in gameCards)
        {
            if (card.cardObj.data.cardID == id)
                return card;
        }

        throw new Exception("Card could not found by id");
    }
    
    public void On123SortButtonPressed()
    {
        var calcCards = sorter.SortBySequences(GetCardObjList(gameCards));
        penaltyText.text = "Penalty: " + calcCards.penalty;
        ReorderCards(calcCards.sortedCards);
    }



    public void On777SortButtonPressed()
    {
        var calcCards = sorter.SortByGroups(GetCardObjList(gameCards));
        penaltyText.text = "Penalty: " + calcCards.penalty;
        ReorderCards(calcCards.sortedCards);
    }
    
    public void OnSmartSortButtonPressed()
    {
        var calcCards = sorter.SmartSort(GetCardObjList(gameCards));
        penaltyText.text = "Penalty: " + calcCards.penalty;
        ReorderCards(calcCards.sortedCards);
        
        /*List<CardObj> cardDeck;
        cardDeck = new List<CardObj>();
        
        cardDeck.Add(new CardObj(CardObj.Suit.HEARTS, 1));
        cardDeck.Add(new CardObj(CardObj.Suit.SPADES, 2));
        cardDeck.Add(new CardObj(CardObj.Suit.DIAMONDS, 5));
        cardDeck.Add(new CardObj(CardObj.Suit.HEARTS, 4));
        cardDeck.Add(new CardObj(CardObj.Suit.SPADES, 1));
        cardDeck.Add(new CardObj(CardObj.Suit.DIAMONDS, 3));
        cardDeck.Add(new CardObj(CardObj.Suit.CLUBS, 4));
        cardDeck.Add(new CardObj(CardObj.Suit.SPADES, 4));
        cardDeck.Add(new CardObj(CardObj.Suit.DIAMONDS, 1));
        cardDeck.Add(new CardObj(CardObj.Suit.SPADES, 3));
        cardDeck.Add(new CardObj(CardObj.Suit.DIAMONDS, 4));
        
        CardSortInfo sortedList = sorter.SmartSort(cardDeck);*/
    }
    
    public void OnChangeThemeButtonPressed()
    {
        ChangeTheme();
    }

    public void OnShuffleCardsButtonPressed()
    {
        gameCards.Clear();
        ownCardIDList.Clear();
        ServiceLocator.instance.pool.SetDefault();
        
        DetermineCardList();
        PlaceCards();
    }
}
