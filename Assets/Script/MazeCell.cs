using UnityEngine;

/// <summary>
/// 迷路の方向（北/東/南/西）を表す。
/// 迷路生成・セルの壁管理で共通して使う。
/// </summary>
public enum Direction
{
    North,
    East,
    South,
    West
}

/// <summary>
/// 迷路の方向に関する共通ユーティリティ。
/// </summary>
public static class DirectionUtil
{
    /// <summary>
    /// 各方向に対応するグリッド上のオフセット（North = +y）。
    /// </summary>
    public static readonly Vector2Int[] Offsets =
    {
        new Vector2Int(0, 1),  // North
        new Vector2Int(1, 0),  // East
        new Vector2Int(0, -1), // South
        new Vector2Int(-1, 0), // West
    };

    /// <summary>
    /// 反対方向を取得する（North <-> South, East <-> West）。
    /// </summary>
    public static Direction Opposite(Direction dir)
    {
        return (Direction)(((int)dir + 2) % 4);
    }
}

/// <summary>
/// 迷路1マス分のセル。
/// 4方向の壁オブジェクトのON/OFFを、隣接セルとの接続状況に応じて切り替える。
/// </summary>
public class MazeCell : MonoBehaviour
{
    [Header("壁オブジェクト（North, East, South, West の順）")]
    public GameObject[] walls = new GameObject[4];

    [Header("床（見た目用・任意）")]
    public GameObject floor;

    /// <summary>
    /// このセルのグリッド座標。MazeGenerator から設定される。
    /// </summary>
    public Vector2Int GridPosition { get; private set; }

    public void Init(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
    }

    /// <summary>
    /// 接続状況に応じて壁の表示/非表示を切り替える。
    /// connected[i] が true の方向は壁を消す（通路になる）。
    /// </summary>
    public void ApplyWalls(bool[] connected)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] == null) continue;
            walls[i].SetActive(!connected[i]);
        }
    }
}