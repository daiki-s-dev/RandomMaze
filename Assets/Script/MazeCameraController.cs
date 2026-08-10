using UnityEngine;

/// <summary>
/// 迷路全体が画面に収まるように、カメラの位置とサイズ（Orthographic Size）を自動調整する。
/// MazeGameManager が迷路生成の直後に AdjustCamera() を呼び出す想定。
/// （MonoBehaviour の Start() 実行順には依存しない）
/// </summary>
[RequireComponent(typeof(Camera))]
public class MazeCameraController : MonoBehaviour
{
    [Header("参照")]
    public MazeGenerator mazeGenerator;

    [Header("余白")]
    public float padding = 2f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (!cam.orthographic)
        {
            Debug.LogWarning("MazeCameraController: Camera が Orthographic になっていません。orthographicSize が反映されません。");
        }
    }

    /// <summary>
    /// 迷路全体がフレームに収まるようカメラを調整する。
    /// MazeGenerator.Generate() の後に呼び出すこと。
    /// </summary>
    public void AdjustCamera()
    {
        if (mazeGenerator == null)
        {
            Debug.LogError("MazeCameraController: mazeGenerator が設定されていません。Inspector を確認してください。");
            return;
        }

        float mazeWidth = mazeGenerator.width * mazeGenerator.cellSize;
        float mazeHeight = mazeGenerator.height * mazeGenerator.cellSize;

        // 迷路の中心にカメラを配置
        Vector3 mazeCenter = mazeGenerator.transform.position +
            new Vector3(
                (mazeWidth - mazeGenerator.cellSize) / 2f,
                (mazeHeight - mazeGenerator.cellSize) / 2f,
                0f
            );

        transform.position = new Vector3(
            mazeCenter.x,
            mazeCenter.y,
            transform.position.z
        );

        // 縦方向に必要なサイズ
        float verticalSize = (mazeHeight / 2f) + padding;

        // 横方向に必要なサイズを考慮
        float horizontalSize = (mazeWidth / 2f + padding) / cam.aspect;

        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}