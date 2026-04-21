using UnityEngine;
using UnityEngine.EventSystems;

public class BlockClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject ArrowSet;

    // クリックされたときに呼び出されるメソッド
    // BlockClick.cs の OnPointerClick の中身
    public void OnPointerClick(PointerEventData eventData)
    {
        var manager = Object.FindAnyObjectByType<TurnManager>();
        if (manager != null)
        {
            // 1. 今の自分の表示状態を記録
            bool isArrowActive = ArrowSet.activeSelf;

            // 2. マネージャーに頼んで、画面上の全矢印を一旦消してもらう
            manager.HideAllArrowSets();

            // 3. もし自分が元々「非表示」だったなら、自分だけを表示にする
            if (!isArrowActive)
            {
                ArrowSet.SetActive(true);
            }
        }
    }


}