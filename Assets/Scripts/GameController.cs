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
    void Start()
    {
        ServiceLocator.instance.gameController = this;
        ServiceLocator.instance.pool.SetDefault();
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
        float initPos = -5.5f;
        float step = 1.1f;

        for (int i = 0; i < cardCount; i++)
        {
            Card cardObject = ServiceLocator.instance.pool.GetPooledCard();
            
            cardObject.id = ownCardIDList[i];
            cardObject.transform.SetParent(cardHolder);
            cardObject.gameObject.SetActive(true);
            cardObject.transform.position = new Vector3(initPos + step * i, 0, 0);
            cardObject.order = i; 
            
            cardObject.cardImage.sortingOrder = i;
            cardObject.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(cardObject.id );
            
            cardUsageList[cardObject.id ] = true;
            
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
            card.cardImage.sprite = ServiceLocator.instance.themeManager.GetCardImage(card.id);
        }
    }
    
    public void On123SortButtonPressed()
    {
        
    }
    
    public void On777SortButtonPressed()
    {
        
    }
    
    public void OnSmartSortButtonPressed()
    {
        
    }
    
    public void OnChangeThemeButtonPressed()
    {
        ChangeTheme();
    }
}
