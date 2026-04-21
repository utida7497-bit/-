using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{

    public GameObject clearTextUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool isProcessing = false;

    // TurnManager.cs の中に追加
    public void HideAllArrowSets()
    {
        // シーン内の全ブロックを検索
        BlockMove[] allBlocks = Object.FindObjectsByType<BlockMove>(FindObjectsSortMode.None);
        foreach (var b in allBlocks)
        {
            // 各ブロックが持っている矢印セットを非表示にする
            if (b.arrowSet != null) b.arrowSet.SetActive(false);
        }
    }


    public void OnBlockMoveCompleted()
    {
        if (isProcessing) return;
        StartCoroutine(ProcessTurnSequence());
    }

    IEnumerator ProcessTurnSequence()
    {
        // 1. 全ブロックを取得
        List<BlockMove> allBlocks = Object.FindObjectsByType<BlockMove>(FindObjectsSortMode.None).ToList();
        for (int i = 0; i < 2; ++i)
        {

            foreach (BlockMove block in allBlocks)
            {
                block.StepBack();
                yield return new WaitForSeconds(0.1f);
            }
        }
            
        // 2. 全員の移動（0.4秒設定ならそれ以上）が終わるまで待つ
        yield return new WaitForSeconds(0.5f);


        foreach (var b in allBlocks)
        {
            b.ResetJustMovedFlag(); // ここで全員のフラグを削除
        }

        // 3. 最後にクリア判定を行う
        CheckAllGoals();
    }
    public void CheckAllGoals()
    {
        // シーン内のすべてのゴールを探す
        GoalArea[] allGoals = Object.FindObjectsByType<GoalArea>(FindObjectsSortMode.None);

        if (allGoals.Length == 0) return;

        bool allClear = true;

        foreach (GoalArea goal in allGoals)
        {
            // 一つでも「IsOccupied」がfalseならクリアではない
            if (!goal.IsOccupied())
            {
                allClear = false;
                break;
            }
        }
        if (allClear)
        {
            Debug.Log("★★★ オールクリア！ ★★★");
            SnowClearEffect();
            // ここでクリア演出などを呼び出す
        }
    }

    void SnowClearEffect()
    {

        if (clearTextUI != null && !clearTextUI.activeSelf) //textUIが存在していて、なおかつ非表示であるとき
        {
            clearTextUI.SetActive(true); // 表示！

            // 4. DOTweenで「跳ねるように」出す演出（オプション）
            clearTextUI.transform.localScale = Vector3.zero;
            clearTextUI.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}

