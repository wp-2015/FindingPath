using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindingPathWithViewShow
{
    static readonly List<Vector2> lHadEnqueued = new List<Vector2>();
    //static readonly Queue<NodeInfo> queueCheck = new Queue<NodeInfo>();
    //static readonly Stack<NodeInfo> stackCheck = new Stack<NodeInfo>();
    static readonly List<NodeInfo> lCheck = new List<NodeInfo>();
    public static List<NodeInfo> lNodeInfo = new List<NodeInfo>();
    public static void Clear()
    {
        lHadEnqueued.Clear();
        //queueCheck.Clear();
        //stackCheck.Clear();
        lCheck.Clear();
        lNodeInfo.Clear();
    }

    public static void Find(Vector2 start, Vector2 end, int col, int row, int[,] mapList, MonoBehaviour mono, Action<NodeInfo> cbFinish, Action<int, int> cbTravel = null)
    {
        lHadEnqueued.Clear();
        //queueCheck.Clear();
        //stackCheck.Clear();
        lCheck.Clear();
        lNodeInfo.Clear();
        mono.StartCoroutine(Find(start, end, col, row, mapList, cbFinish, cbTravel));
    }

    private static float GetF(NodeInfo node, ref Vector2 start, ref Vector2 end)
    {
        var currentPos = node.currentPos;
        var sD = Vector2.Distance(start, currentPos);
        var eD = Vector2.Distance(end, currentPos);
        return sD + eD;
    }
    private static NodeInfo PopTheLowestF(ref Vector2 start, ref Vector2 end)
    {
        if (lCheck.Count < 1)
            return null;

        float lowestDis = GetF(lCheck[0], ref start, ref end);
        NodeInfo res = lCheck[0];
        for (int i = 1; i < lCheck.Count; i++)
        {
            var f = GetF(lCheck[i], ref start, ref end);
            if(f < lowestDis)
            {
                res = lCheck[i];
                lowestDis = f;
            }
        }
        return res;
    }

    static IEnumerator Find(Vector2 start, Vector2 end, int col, int row, int[,] mapList, Action<NodeInfo> cbFinish, Action<int, int> cbTravel = null)
    {
        var startNode = new NodeInfo() { currentPos = start, parentNode = null };
        if (start == end)
        {
            cbFinish?.Invoke(new NodeInfo() { currentPos = end, parentNode = startNode });
        }
        //尾部压入队列
        //queueCheck.Enqueue(startNode);
        //stackCheck.Push(startNode);
        lCheck.Add(startNode);
        lNodeInfo.Add(startNode);
        lHadEnqueued.Add(start);
        //lNodeInfo.Add(startNode);
        while (lCheck.Count > 0)
        {
            //var currentNode = queueCheck.Dequeue();
            var currentNode = PopTheLowestF(ref start, ref end);
            lCheck.Remove(currentNode);
            if (currentNode.currentPos != end)
            {
                lNodeInfo.Add(currentNode);
                cbTravel?.Invoke((int)currentNode.currentPos.x, (int)currentNode.currentPos.y);
                var childNodes = FindingNodeChildren(currentNode.currentPos, col, row, mapList, lHadEnqueued);
                
                childNodes.Sort((x, y) =>
                {
                    var dis = Vector2.Distance(y, end) - Vector2.Distance(x, end);
                    if (dis > 0)
                        return 1;
                    else if (dis < 0)
                        return -1;
                    return 0;
                });

                foreach (var node in childNodes)
                {
                    lHadEnqueued.Add(node);
                    var childNode = new NodeInfo() { currentPos = node, parentNode = currentNode };
                    //queueCheck.Enqueue(childNode);
                    //stackCheck.Push(childNode);
                    lCheck.Add(childNode);
                }
            }
            else
            {
                cbFinish?.Invoke(currentNode);
                yield break;
            }
            if(cbTravel != null)
                yield return new WaitForSeconds(0.5f);
        }
        yield return null;
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
        {
            var ix = x - 1;
            var iy = y - 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode))
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        if (bDownViable)
            res.Add(new Vector2(x, y + 1));
        if (bLeftViable && bDownViable)
        {
            var ix = x - 1;
            var iy = y + 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode))
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        if (bRightViable)
            res.Add(new Vector2(x + 1, y));
        if (bDownViable && bRightViable)
        {
            var ix = x + 1;
            var iy = y + 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode))
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        if (bRightViable && bUpViable)
        {
            var ix = x + 1;
            var iy = y - 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode))
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        return res;
    }

    public static bool IsPosViable(ref Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        if (!hadCheckNode.Contains(node) && mapList[(int)node.x, (int)node.y] == 0)
            return true;
        return false;
    }

    /// <summary>
    /// 上部的节点是否可行
    /// </summary>
    private static bool IsUpViable(Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y - 1;
        var tp = new Vector2(x, y);
        if (y >= 0 && IsPosViable(ref tp, mapList, hadCheckNode))
            return true;
        return false;
    }
    private static bool IsDownViable(Vector2 node, int row, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y + 1;
        var tp = new Vector2(x, y);
        if (y < row && IsPosViable(ref tp, mapList, hadCheckNode))
        {
            return true;
        }
        return false;
    }
    private static bool IsLeftViable(Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x - 1;
        var y = (int)node.y;
        var tp = new Vector2(x, y);
        if (x >= 0 && IsPosViable(ref tp, mapList, hadCheckNode))
            return true;
        return false;
    }
    private static bool IsRightViable(Vector2 node, int col, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x + 1;
        var y = (int)node.y;
        var tp = new Vector2(x, y);
        if (x < col && IsPosViable(ref tp, mapList, hadCheckNode))
            return true;
        return false;
    }

    public class NodeInfo
    {
        public NodeInfo parentNode;
        public Vector2 currentPos;
    }
}
