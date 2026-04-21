using UnityEngine;
using UnityEngine.EventSystems;


public class SelectDirection : MonoBehaviour, IPointerClickHandler
{
    public Vector2 direction;
    private BlockMove target;
    void Start()
    {
        target = GetComponentInParent<BlockMove>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("矢印ブロックがクリックされた！");
        if (target == null) return;
        target.Move(direction);
    }

}
