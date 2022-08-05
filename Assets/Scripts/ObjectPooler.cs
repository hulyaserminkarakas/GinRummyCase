using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectPooler : MonoBehaviour
{
    public List<Card> pooledCards;
    public GameObject cardObject;
    public int amountToPool;
    
    void Awake()
    {
        pooledCards = new List<Card>();
        GameObject temp;
        for(int i = 0; i < amountToPool; i++)
        {
            temp = Instantiate(cardObject);
            temp.SetActive(false);
            pooledCards.Add(temp.GetComponent<Card>());
        }
    }
    
    public Card GetPooledCard()
    {
        for(int i = 0; i < amountToPool; i++)
        {
            if(!pooledCards[i].gameObject.activeInHierarchy)
            {
                return pooledCards[i];
            }
        }
        return null;
    }

    public void SetDefault()
    {
        foreach (var card in pooledCards)
        {
            card.gameObject.SetActive(false);
            //card.cardFilter.SetActive(false);
            card.transform.position = Vector3.zero;
            card.cardImage.sortingOrder = 0;
        }
    }
}
