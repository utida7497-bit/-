using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BlockMove : MonoBehaviour
{
    public float moveSpeed = 3f; // 1秒間に進むマス数
    public GameObject arrowSet;
    public LayerMask obstacleLayer;
    static public bool isMoving = false;

    [HideInInspector]
    public List<Vector2> moveHistory = new List<Vector2>();
    
    [HideInInspector]
    public bool isJustMoved = false;

    void Start()
    {
         

        if(TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.freezeRotation = true;
        }
    }

    // 矢印（SelectDirection）から呼ばれる移動開始関数
    public void Move(Vector2 dir)
    {
        if (isMoving) return;

        isMoving = true;

        Vector2 targetPos = CalculateTarget(dir);

        float distance = Vector2.Distance(transform.position, targetPos);

        if (distance <= 0.1f)
        {
            isMoving = false;
            isJustMoved = false;
            return;
        }
        if(arrowSet != null) arrowSet.SetActive(false);
        //移動履歴の保存処理
        int steps = Mathf.RoundToInt(distance);
        for (int i = 0; i < steps; i++)
        {
            moveHistory.Add(dir);
        }

        // 移動するのにかかる時間を計算
        float duration = distance / moveSpeed;

        // DOTweenで移動開始
        transform.DOMove(targetPos, duration)
            .SetEase(Ease.OutQuad) //止まる直前に少し減速
            .OnComplete(() => {
                isMoving = false;
                isJustMoved = true;

                var manager = Object.FindAnyObjectByType<TurnManager>();
                if (manager != null) 
                {
                    manager.OnBlockMoveCompleted();
                }
            });
    }

    // 壁を探す「目」の役割
    private Vector2 CalculateTarget(Vector2 dir)
    {
        //自分自身のコライダーを取得
        Collider2D myCollider = GetComponent<Collider2D>();

        if(myCollider != null) myCollider.enabled = false;

        // Physics2D.Raycast(今の位置, 方向, 飛ばす距離, 対象レイヤー)
        Vector2 origin = (Vector2)transform.position + (dir * 0.51f);
        RaycastHit2D hit = Physics2D.Raycast(origin,dir,100f,obstacleLayer);
        
        if (myCollider != null) myCollider.enabled = true;

        if (hit.collider != null)
        {
            float targetX = Mathf.Round(hit.point.x - dir.x * 0.5f);
            float targetY = Mathf.Round(hit.point.y - dir.y * 0.5f);
            return new Vector2(targetX, targetY);
        }
        return origin;
    }
}
