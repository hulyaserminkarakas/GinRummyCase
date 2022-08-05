using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardMovementController : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    [SerializeField] private Card card;



    private void Awake()
    {
    }


    void OnMouseDown()
    {                                                                                                                                                                                                  
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y/6, screenPoint.z));
    }
 
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y / 6, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

        if (card.order > 0 && curPosition.x < ServiceLocator.instance.gameController.cardPositions[card.order - 1].x)
        {
            SwapCards(ServiceLocator.instance.gameController.GetCardObjectByOrder(card.order - 1));
        }
        
        if (card.order < ServiceLocator.instance.gameController.cardPositions.Count - 1 &&  curPosition.x > ServiceLocator.instance.gameController.cardPositions[card.order + 1].x)
        {
            SwapCards(ServiceLocator.instance.gameController.GetCardObjectByOrder(card.order + 1));
        }

    }

    void OnMouseUp()
    {
        transform.position = ServiceLocator.instance.gameController.cardPositions[card.order];
    }


    private void SwapCards(Card swapCard)
    {
        int newOrder = swapCard.order;
        swapCard.order = card.order;
        swapCard.cardImage.sortingOrder = card.order;
        swapCard.transform.position = ServiceLocator.instance.gameController.cardPositions[card.order];

        card.order = newOrder;
        card.cardImage.sortingOrder = newOrder;
        card.transform.position = ServiceLocator.instance.gameController.cardPositions[newOrder];

    }
}
