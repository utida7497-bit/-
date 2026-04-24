using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ReturnMove : MonoBehaviour
{
    private Vector2 startPosition;  //èâä˙à íu

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StepBack()
    {
        if (isJustMoved || BlockMove.isMoving || moveHistory.Count == 0)
        {
            //isJustMoved = false;
            return;
        }

        if ((Vector2)transform.position == startPosition) return;

        int lastIndex = moveHistory.Count - 1;
        Vector2 lastDir = moveHistory[lastIndex];

        Vector2 BackDir = -lastDir;
        Vector2 BackPos = (Vector2)transform.position + BackDir;

        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;

        //[èåèÇP]
        Collider2D hit = Physics2D.OverlapCircle(BackPos, 0.4f, obstacleLayer);

        if (myCollider != null) myCollider.enabled = true;

        if (hit == null)
        {
            BlockMove.isMoving = true;

            moveHistory.RemoveAt(lastIndex);

            transform.DOMove(BackPos, 0.4f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    BlockMove.isMoving = false;

                    if (moveHistory.Count == 0)
                    {
                        transform.position = startPosition;
                    }
                    var manager = Object.FindAnyObjectByType<TurnManager>();
                    if (manager != null)
                    {
                        manager.CheckAllGoals();
                    }
                });
        }
        else
        {
            BlockMove.isMoving = false;
            Debug.Log($"{name} : ñﬂÇËêÊÇ™ç«Ç™Ç¡ÇƒÇ¢ÇÈÇΩÇﬂë“ã@ÇµÇ‹Ç∑ÅB");
        }
    }
}
