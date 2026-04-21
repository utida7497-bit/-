using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

/// <summary>
/// Unity Editor上でシーンとアセットを自動保存するスクリプト。
/// Assets/Editor フォルダに配置してください。
/// </summary>
[InitializeOnLoad]
public class AutoSave
{
    private static double _nextSaveTime;
    private const double IntervalMinutes = 5.0; // 保存間隔（分）

    static AutoSave()
    {
        _nextSaveTime = EditorApplication.timeSinceStartup + (IntervalMinutes * 60);
        EditorApplication.update += Update;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        Debug.Log($"[AutoSave] 自動保存が有効になりました（間隔: {IntervalMinutes}分）");
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // EditモードからPlayモードに切り替わる直前（保存のタイミング）
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("[AutoSave] Playモード開始前の自動保存を実行します...");
            Save(true);
        }
    }

    private static void Update()
    {
        if (EditorApplication.timeSinceStartup > _nextSaveTime)
        {
            Save(false);
            _nextSaveTime = EditorApplication.timeSinceStartup + (IntervalMinutes * 60);
        }
    }

    private static void Save(bool force)
    {
        // forceがfalseの場合のみ、再生中などのチェックを行う
        if (!force && (UnityEngine.Application.isPlaying || EditorApplication.isCompiling || EditorApplication.isUpdating))
        {
            return;
        }

        // 開いているすべてのシーンを保存
        EditorSceneManager.SaveOpenScenes();
        // 未保存のアセット（プレハブ等）を保存
        AssetDatabase.SaveAssets();

        Debug.Log($"[AutoSave] 自動保存完了: {System.DateTime.Now:HH:mm:ss}");

        // Sceneビューに通知を表示
        SceneView.lastActiveSceneView?.ShowNotification(new GUIContent($"AutoSave Completed\n{System.DateTime.Now:HH:mm:ss}"), 2.0f);
    }
}
