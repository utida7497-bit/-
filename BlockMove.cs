using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BlockMove : MonoBehaviour
{
    public float moveSpeed = 3f; // 1秒間に進むマス数
    public GameObject arrowSet;
    public LayerMask obstacleLayer;
    private bool isMoving = false;

    private Vector2 startPosition;  //初期位置
    private List<Vector2> moveHistory = new List<Vector2>();

    private bool isJustMoved = false;

    void Start()
    {
        startPosition = transform.position;    

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

    public void StepBack()
    {
        if (isJustMoved || isMoving || moveHistory.Count == 0)
        { 
            //isJustMoved = false;
            return ;
        }

        if ((Vector2)transform.position == startPosition) return;

        int lastIndex = moveHistory.Count - 1;
        Vector2 lastDir = moveHistory[lastIndex];

        Vector2 BackDir = -lastDir;
        Vector2 BackPos = (Vector2)transform.position + BackDir;
    
        Collider2D myCollider = GetComponent<Collider2D>();
        if(myCollider != null) myCollider.enabled = false;

        //[条件１]
        Collider2D hit = Physics2D.OverlapCircle(BackPos, 0.4f, obstacleLayer);

        if (myCollider != null) myCollider.enabled = true;

        if (hit == null)
        {
            isMoving = true;

            moveHistory.RemoveAt(lastIndex);

            transform.DOMove(BackPos, 0.4f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    isMoving = false;

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
            isMoving = false;
            Debug.Log($"{name} : 戻り先が塞がっているため待機します。");
        }
    }
    public void ResetJustMovedFlag()
    {
        isJustMoved = false;
    }

    // 壁を探す「目」の役割
    private Vector2 CalculateTarget(Vector2 dir)
    {
        //自分自身のコライダーを取得
        Collider2D myCollider = GetComponent<Collider2D>();

        if(myCollider != null) myCollider.enabled = false;

        // Physics2D.Raycast(今の位置, 方向, 飛ばす距離, 対象レイヤー)
        // 自分のコライダーに当たらないよう、少しだけ(0.6f)外側から飛ばすのがコツです
        Vector2 origin = (Vector2)transform.position + (dir * 0.51f);
        RaycastHit2D hit = Physics2D.Raycast(origin,dir,100f,obstacleLayer);
        
        if (myCollider != null) myCollider.enabled = true;

        if (hit.collider != null)
        {
            // ヒット地点の座標を四捨五入して「マス目」に合わせるのがパズルでは確実です
            float targetX = Mathf.Round(hit.point.x - dir.x * 0.5f);
            float targetY = Mathf.Round(hit.point.y - dir.y * 0.5f);
            return new Vector2(targetX, targetY);
        }

        // 壁がなければとりあえず遠くへ
        return (Vector2)transform.position + (dir * 100f);
    }
}
