﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject goTileTemplate;
    public Transform tfTileParent;

    GameObject[,] lGOTile;
    int[,] lMap;
    public void ShowTile(int col, int row)
    {
        var count = tfTileParent.childCount;
        for(int i = 0; i < count; i++)
        {
            var child = tfTileParent.GetChild(i);
            Destroy(child);
        }

        clickState = ClickState.None;
        lGOTile = new GameObject[col, row];
        lMap = new int[col, row];
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                lMap[i, j] = 0;
                var goTile = Instantiate(goTileTemplate);
                goTile.name = i + "_" + j;
                goTile.transform.parent = tfTileParent;
                goTile.SetActive(true);
                goTile.transform.position = new Vector3(i, 0, j);
                lGOTile[i, j] = goTile;
            }
        }
    }

    Color colorStart = Color.red;
    Color colorEnd = Color.green;
    Color colorNormal = Color.white;
    Color colorObstacle = Color.gray;

    string szCol = "10", szRow = "10";

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("列:", GUILayout.Width(20));
        szCol = GUILayout.TextField(szCol, GUILayout.Width(40));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("行:", GUILayout.Width(20));
        szRow = GUILayout.TextField(szRow, GUILayout.Width(40));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("创建地图"))
        {
            ShowTile(int.Parse(szCol),int.Parse(szRow));
        }
        GUILayout.Space(6);
        if(GUILayout.Button("<color=red>选择起点</color>"))
        {
            clickState = ClickState.Start;
        }
        GUILayout.Space(6);
        if (GUILayout.Button("<color=green>选择终点</color>"))
        {
            clickState = ClickState.End;
        }
        GUILayout.Space(6);
        if (GUILayout.Button("<color=grey>添加障碍</color>"))
        {
            clickState = ClickState.Obstacle;
        }
        GUILayout.Space(6);
        if (GUILayout.Button("寻路"))
        {
            
        }
    }

    GameObject goStart, goEnd;
    enum ClickState{ None, Start, End, Obstacle };
    ClickState clickState = ClickState.None;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            object ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
            RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
            if (isHit)
            {
                SetTile(hit.collider.gameObject);
            }
        }
    }

    private void SetTile(GameObject goTile)
    {
        var name = goTile.name;
        var lszName = name.Split('_');
        var col = int.Parse(lszName[0]);
        var row = int.Parse(lszName[1]);
        switch(clickState)
        {
            case ClickState.Start:
                if (null != goStart)
                    SetTileColor(goStart, ref colorNormal);
                goStart = goTile;
                SetTileColor(goStart, ref colorStart);
                break;
            case ClickState.End:
                if (null != goEnd)
                    SetTileColor(goEnd, ref colorNormal);
                goEnd = goTile;
                SetTileColor(goEnd, ref colorEnd);
                break;
            case ClickState.Obstacle:
                if(goTile == goStart)
                {
                    goStart = null;
                }
                else if(goTile == goEnd)
                {
                    goEnd = null;
                }
                SetTileColor(goTile, ref colorObstacle);
                lMap[col, row] = 1;
                break;
        }
    }

    private void SetTileColor(GameObject goTile, ref Color color)
    {
        var meshRender = goTile.GetComponent<MeshRenderer>();
        meshRender.material.color = color;
    }
}
