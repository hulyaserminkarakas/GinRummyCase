using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    
    [SerializeField] private Transform cardHolder;

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
            
            cardObject.data = dataModel.cards.cardStats[ownCardIDList[i]];
            cardObject.value = cardObject.data.cardID % 13 + 1; 
            cardObject.transform.SetParent(cardHolder);
            cardObject.gameObject.SetActive(true);
            cardObject.transform.position = new Vector3(initPos + step * i,0 , 0);
            cardObject.suit = (Card.Suit)Enum.Parse(typeof(Card.Suit), cardObject.data.type);
            
            
            // cardObject.transform.position = new Vector3(initPos + step * i, - Mathf.Abs((initPos + step * i )* 0.12f), 0);
            // cardObject.transform.Rotate(Vector3.back * (initPos + step * i) * 3) ;
            
            
            cardObject.order = i; 
            
            cardObject.cardImage.sortingOrder = i;
            cardObject.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(cardObject.data.cardID );
            
            cardUsageList[cardObject.data.cardID ] = true;
            
            gameCards.Add(cardObject);
            
            cardPositions.Add(new Vector3(initPos + step * i, 0, 0));
        }
    }

    
    public Card GetCardObjectByOrder(int order)
    {
        foreach (var card in gameCards)
        {
            if (card.order == order)
                return card;
        }

        throw new Exception("Card not found, check order!");
    }


    public void ChangeTheme()
    {
        ServiceLocator.instance.themeManager.ToggleTheme();
        
        foreach (var card in gameCards)
        {
            card.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(card.data.cardID);
        }
    }

    private void ReorderCards(List<Card> cards)
    {
        Card cardObject;
        
        for (int i = 0; i < cards.Count; i++)
        {
            cardObject = cards[i];
            
            cardObject.data = dataModel.cards.cardStats[cards[i].data.cardID];
            cardObject.gameObject.SetActive(true);
            cardObject.transform.position = cardPositions[i];
            cardObject.order = i; 
            
            cardObject.cardImage.sortingOrder = i;
            cardObject.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(cardObject.data.cardID );
        }
    }
    
    public void On123SortButtonPressed()
    {
        var calcCards = sorter.SortByCompleteSequences(gameCards);
        ReorderCards(calcCards);
    }

    
    
    public void On777SortButtonPressed()
    {
        var calcCards = sorter.SortByCompleteGroups(gameCards);
        ReorderCards(calcCards);
    }
    
    public void OnSmartSortButtonPressed()
    {
       
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
