using UnityEngine;

public class CardMovementController : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    [SerializeField] private Card card;

    void OnMouseDown()
    {                                                                                                                                                                                                  
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y/6, screenPoint.z));
    }
 
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y / 6, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

        if (card.cardObj.order > 0 && curPosition.x < ServiceLocator.instance.gameController.cardPositions[card.cardObj.order - 1].x)
        {
            SwapCards(ServiceLocator.instance.gameController.GetCardObjectByOrder(card.cardObj.order - 1));
        }
        
        if (card.cardObj.order < ServiceLocator.instance.gameController.cardPositions.Count - 1 &&  curPosition.x > ServiceLocator.instance.gameController.cardPositions[card.cardObj.order + 1].x)
        {
            SwapCards(ServiceLocator.instance.gameController.GetCardObjectByOrder(card.cardObj.order + 1));
        }

    }

    void OnMouseUp()
    {
        transform.position = ServiceLocator.instance.gameController.cardPositions[card.cardObj.order];
    }


    private void SwapCards(Card swapCard)
    {
        int newOrder = swapCard.cardObj.order;
        swapCard.cardObj.order = card.cardObj.order;
        swapCard.cardImage.sortingOrder = card.cardObj.order;
        swapCard.transform.position = ServiceLocator.instance.gameController.cardPositions[card.cardObj.order];

        card.cardObj.order = newOrder;
        card.cardImage.sortingOrder = newOrder;
        card.transform.position = ServiceLocator.instance.gameController.cardPositions[newOrder];

    }
}
