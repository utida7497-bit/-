using UnityEngine;

public class GoalArea : MonoBehaviour
{
    // 今、自分の真上に「誰か」がいるかを返すだけの機能
    public bool IsOccupied()
    {
        // 自分の位置(transform.position)に「Block」レイヤーがいるか調べる
        Collider2D hit = Physics2D.OverlapPoint(transform.position, 1 << LayerMask.NameToLayer("Block"));
        return hit != null;
    }
}
