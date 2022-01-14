using System.Collections.Generic;
using UnityEngine;

public class FindingPathCode
{
    public static int iCol;
    public static int iRow;
    public static int[,] lMap;
    private static void SetMap(int col, int row, int[,] mapList)
    {
        iCol = col;
        iRow = row;
        lMap = mapList;
    }

    public static Vector2[] Find()
    {
        int col = 4;
        int row = 4;
        int[,] map = new int[col, row];
        for(int i = 0; i < col; i++)
        {
            for(int j = 0; j < row; j++)
            {
                map[i, j] = 0;
            }
        }
        Find(new Vector2(2, 2), new Vector2(0, 0), col, row, map);

        List<Vector2> res = new List<Vector2>();
        return res.ToArray();
    }

    static readonly List<Vector2> lHadEnqueued = new List<Vector2>();
    static readonly Queue<Vector2> queueCheck = new Queue<Vector2>();
    public static List<Vector2> Find(Vector2 start, Vector2 end, int col, int row, int[,] mapList)
    {
        List<Vector2> res = new List<Vector2>();

        lHadEnqueued.Clear();
        queueCheck.Clear();
        if (start == end)
        {
            res.Add(start);res.Add(end);return res;
        }
        //尾部压入队列
        queueCheck.Enqueue(start);
        lHadEnqueued.Add(start);
        while (queueCheck.Count > 0)
        {
            var currentNode = queueCheck.Dequeue();
            Debug.LogError(currentNode);
            if(currentNode != end)
            {
                var childNodes = FindingNodeChildren(currentNode, col, row, mapList, lHadEnqueued);
                foreach(var node in childNodes)
                {
                    lHadEnqueued.Add(node);
                    queueCheck.Enqueue(node);
                }
            }
            else
            {
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// 返回该点的周围合法节点
    /// </summary>
    /// <param name="node">目标节点</param>
    /// <param name="col">地图总列数</param>
    /// <param name="row">地图总行数</param>
    /// <param name="mapList">地图信息</param>
    /// <param name="hadCheckNode">已经检查过的节点</param>
    /// <returns></returns>
    private static List<Vector2> FindingNodeChildren(Vector2 node, int col, int row, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y;
        bool bUpViable = IsUpViable(node, mapList, hadCheckNode);
        bool bDownViable = IsDownViable(node, row, mapList, hadCheckNode);
        bool bLeftViable = IsLeftViable(node, mapList, hadCheckNode);
        bool bRightViable = IsRightViable(node, col, mapList, hadCheckNode);

        List<Vector2> res = new List<Vector2>();
        if (bUpViable)
            res.Add(new Vector2(x, y - 1));
        if (bLeftViable)
            res.Add(new Vector2(x - 1, y));
        if (bUpViable && bLeftViable)
            res.Add(new Vector2(x - 1, y - 1));
        if (bDownViable)
            res.Add(new Vector2(x, y + 1));
        if (bLeftViable && bDownViable)
            res.Add(new Vector2(x - 1, y + 1));
        if (bRightViable)
            res.Add(new Vector2(x + 1, y));
        if (bDownViable && bRightViable)
            res.Add(new Vector2(x + 1, y + 1));
        if (bRightViable && bUpViable)
            res.Add(new Vector2(x + 1, y - 1));
        return res;
    }

    /// <summary>
    /// 上部的节点是否可行
    /// </summary>
    private static bool IsUpViable(Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y - 1;
        if (y >= 0 && mapList[x, y] == 0 && !hadCheckNode.Contains(new Vector2(x, y)))
            return true;
        return false;
    }
    private static bool IsDownViable(Vector2 node, int row, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y + 1;
        if (y < row && mapList[x, y] == 0 && !hadCheckNode.Contains(new Vector2(x, y)))
        {
            return true;
        }
        return false;
    }
    private static bool IsLeftViable(Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x - 1;
        var y = (int)node.y;
        if (x >= 0 && mapList[x, y] == 0 && !hadCheckNode.Contains(new Vector2(x, y)))
            return true;
        return false;
    }
    private static bool IsRightViable(Vector2 node, int col, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x + 1;
        var y = (int)node.y;
        if (x < col && mapList[x, y] == 0 && !hadCheckNode.Contains(new Vector2(x, y)))
            return true;
        return false;
    }
}
