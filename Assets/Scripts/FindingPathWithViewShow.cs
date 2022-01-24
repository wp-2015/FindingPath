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

    private static NodeInfo PopTheLowestF(ref Vector2 start, ref Vector2 end)
    {
        if (lCheck.Count < 1)
            return null;

        float lowestDis = lCheck[0].iDis;
        NodeInfo res = lCheck[0];
        for (int i = 1; i < lCheck.Count; i++)
        {
            var f = lCheck[i].iDis;
            if(f < lowestDis)
            {
                res = lCheck[i];
                lowestDis = f;
            }
        }
        return res;
    }

    private static int GetDis(ref Vector2 a, ref Vector2 b)
    {
        return Math.Abs((int)a.x - (int)b.x) + Math.Abs((int)a.y - (int)b.y);
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
                    var xDis = GetDis(ref end, ref x);
                    var yDis = GetDis(ref end, ref y);

                    var dis = xDis - yDis;//Vector2.Distance(y, end) - Vector2.Distance(x, end);
                    if (dis > 0)
                        return 1;
                    else if (dis < 0)
                        return -1;
                    return 0;
                });

                for(int i = 0; i < childNodes.Count; i++)
                {
                    var node = childNodes[i];
                    lHadEnqueued.Add(node);
                    var dis = GetDis(ref start, ref node) + GetDis(ref end, ref node);
                    var childNode = new NodeInfo() { currentPos = node, parentNode = currentNode, iDis = dis };
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
        var bUpViable = IsUpViable(node, mapList, hadCheckNode);
        var bDownViable = IsDownViable(node, row, mapList, hadCheckNode);
        var bLeftViable = IsLeftViable(node, mapList, hadCheckNode);
        var bRightViable = IsRightViable(node, col, mapList, hadCheckNode);

        List<Vector2> res = new List<Vector2>();
        if (bUpViable == ViableType.Viable)
            res.Add(new Vector2(x, y - 1));
        if (bLeftViable == ViableType.Viable)
            res.Add(new Vector2(x - 1, y));
        if (bUpViable >= ViableType.HadChecked && bLeftViable >= ViableType.HadChecked)
        {
            var ix = x - 1;
            var iy = y - 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode) == ViableType.Viable)
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        if (bDownViable == ViableType.Viable)
            res.Add(new Vector2(x, y + 1));
        if (bLeftViable >= ViableType.HadChecked && bDownViable >= ViableType.HadChecked)
        {
            var ix = x - 1;
            var iy = y + 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode) == ViableType.Viable)
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        if (bRightViable == ViableType.Viable)
            res.Add(new Vector2(x + 1, y));
        if (bDownViable >= ViableType.HadChecked && bRightViable >= ViableType.HadChecked)
        {
            var ix = x + 1;
            var iy = y + 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode) == ViableType.Viable)
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        if (bRightViable >= ViableType.HadChecked && bUpViable >= ViableType.HadChecked)
        {
            var ix = x + 1;
            var iy = y - 1;
            var tp = new Vector2(ix, iy);
            if (IsPosViable(ref tp, mapList, hadCheckNode) == ViableType.Viable)
            {
                res.Add(new Vector2(ix, iy));
            }
        }
        return res;
    }

    public static ViableType IsPosViable(ref Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        if (mapList[(int)node.x, (int)node.y] != 0)
            return ViableType.CannotMove;
        if (hadCheckNode.Contains(node))
            return ViableType.HadChecked;

        return ViableType.Viable;
    }

    public enum ViableType { OutRange, CannotMove, HadChecked, Viable };
    /// <summary>
    /// 上部的节点是否可行
    /// </summary>
    private static ViableType IsUpViable(Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y - 1;
        var tp = new Vector2(x, y);
        if (y < 0)
            return ViableType.OutRange;

        return IsPosViable(ref tp, mapList, hadCheckNode);
    }
    private static ViableType IsDownViable(Vector2 node, int row, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x;
        var y = (int)node.y + 1;
        var tp = new Vector2(x, y);
        if(y >= row)
            return ViableType.OutRange;

        return IsPosViable(ref tp, mapList, hadCheckNode);
    }
    private static ViableType IsLeftViable(Vector2 node, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x - 1;
        var y = (int)node.y;
        var tp = new Vector2(x, y);
        if(x < 0)
            return ViableType.OutRange;

        return IsPosViable(ref tp, mapList, hadCheckNode);
    }
    private static ViableType IsRightViable(Vector2 node, int col, int[,] mapList, List<Vector2> hadCheckNode)
    {
        var x = (int)node.x + 1;
        var y = (int)node.y;
        var tp = new Vector2(x, y);
        if(x >= col)
            return ViableType.OutRange;

        return IsPosViable(ref tp, mapList, hadCheckNode);
    }

    public class NodeInfo
    {
        public NodeInfo parentNode;
        public Vector2 currentPos;
        public int iDis;
    }
}
