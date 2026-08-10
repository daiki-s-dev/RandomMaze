using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 迷路の生成本体。
/// 再帰的バックトラッキング法（スタックを使った反復DFS）で通路を掘り、
/// スタートから最も遠いセルにゴールを、行き止まりを優先してアイテムを配置する。
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("迷路サイズ")]
    public int width = 15;
    public int height = 15;
    public float cellSize = 2f;

    [Header("プレハブ")]
    public MazeCell cellPrefab;
    public GameObject itemPrefab;
    public GameObject goalPrefab;

    [Header("アイテム設定")]
    public int itemCount = 5;

    [Header("スタート地点")]
    public Vector2Int startCell = Vector2Int.zero;

    // 各セルの接続状況（true = その方向に通路がある）
    private bool[,,] connected;
    private bool[,] visited;
    private MazeCell[,] cellInstances;

    /// <summary>
    /// 迷路生成後のスタート地点のワールド座標。
    /// </summary>
    public Vector3 StartWorldPosition => CellToWorldPosition(startCell);

    /// <summary>
    /// 迷路生成後のゴール地点のワールド座標。
    /// </summary>
    public Vector3 GoalWorldPosition { get; private set; }

    /// <summary>
    /// 迷路を生成する。MazeGameManager など外部から呼び出す想定。
    /// </summary>
    public void Generate()
    {
        connected = new bool[width, height, 4];
        visited = new bool[width, height];
        cellInstances = new MazeCell[width, height];

        CarvePassages();
        SpawnCells();

        Vector2Int goalCell = FindFarthestCell(startCell);
        PlaceGoal(goalCell);
        PlaceItems(goalCell);
    }

    #region 通路掘り（再帰的バックトラッキング法）

    private void CarvePassages()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int current = startCell;
        visited[current.x, current.y] = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Peek();

            List<Direction> candidates = GetUnvisitedNeighborDirections(current);
            if (candidates.Count == 0)
            {
                stack.Pop();
                continue;
            }

            Direction dir = candidates[Random.Range(0, candidates.Count)];
            Vector2Int next = current + DirectionUtil.Offsets[(int)dir];

            // 現在セルと隣接セルを双方向に接続
            connected[current.x, current.y, (int)dir] = true;
            connected[next.x, next.y, (int)DirectionUtil.Opposite(dir)] = true;

            visited[next.x, next.y] = true;
            stack.Push(next);
        }
    }

    private List<Direction> GetUnvisitedNeighborDirections(Vector2Int cell)
    {
        List<Direction> result = new List<Direction>();

        for (int i = 0; i < 4; i++)
        {
            Vector2Int neighbor = cell + DirectionUtil.Offsets[i];

            if (!IsInBounds(neighbor)) continue;
            if (visited[neighbor.x, neighbor.y]) continue;

            result.Add((Direction)i);
        }

        return result;
    }

    private bool IsInBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    #endregion

    #region セル生成

    private void SpawnCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);

                MazeCell cell = Instantiate(
                    cellPrefab,
                    CellToWorldPosition(gridPos),
                    Quaternion.identity,
                    transform
                );

                cell.Init(gridPos);

                bool[] cellConnections =
                {
                    connected[x, y, 0],
                    connected[x, y, 1],
                    connected[x, y, 2],
                    connected[x, y, 3],
                };
                cell.ApplyWalls(cellConnections);

                cellInstances[x, y] = cell;
            }
        }
    }

    private Vector3 CellToWorldPosition(Vector2Int cell)
    {
        return transform.position + new Vector3(cell.x * cellSize, cell.y * cellSize, 0f);
    }

    #endregion

    #region ゴール配置（スタートから最も遠いセルをBFSで探索）

    private Vector2Int FindFarthestCell(Vector2Int from)
    {
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        distance[from] = 0;
        queue.Enqueue(from);

        Vector2Int farthest = from;

        while (queue.Count > 0)
        {
            Vector2Int cell = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                if (!connected[cell.x, cell.y, i]) continue;

                Vector2Int next = cell + DirectionUtil.Offsets[i];
                if (distance.ContainsKey(next)) continue;

                distance[next] = distance[cell] + 1;
                queue.Enqueue(next);

                if (distance[next] > distance[farthest])
                    farthest = next;
            }
        }

        return farthest;
    }

    private void PlaceGoal(Vector2Int goalCell)
    {
        GoalWorldPosition = CellToWorldPosition(goalCell);

        if (goalPrefab != null)
            Instantiate(goalPrefab, GoalWorldPosition, Quaternion.identity, transform);
    }

    #endregion

    #region アイテム配置（行き止まりを優先）

    private void PlaceItems(Vector2Int goalCell)
    {
        if (itemPrefab == null || itemCount <= 0) return;

        List<Vector2Int> deadEnds = new List<Vector2Int>();
        List<Vector2Int> others = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (cell == startCell || cell == goalCell) continue;

                int connectionCount = CountConnections(cell);

                if (connectionCount == 1)
                    deadEnds.Add(cell);
                else
                    others.Add(cell);
            }
        }

        Shuffle(deadEnds);
        Shuffle(others);

        List<Vector2Int> placement = new List<Vector2Int>();
        placement.AddRange(deadEnds);
        placement.AddRange(others);

        int count = Mathf.Min(itemCount, placement.Count);
        for (int i = 0; i < count; i++)
        {
            Instantiate(itemPrefab, CellToWorldPosition(placement[i]), Quaternion.identity, transform);
        }
    }

    private int CountConnections(Vector2Int cell)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (connected[cell.x, cell.y, i]) count++;
        }
        return count;
    }

    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    #endregion
}