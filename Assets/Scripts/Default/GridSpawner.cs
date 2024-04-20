using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    private GameObject grid;
    private RectTransform gridTransform;
    public readonly List<List<GameObject>> gridList = new();

    private void Awake()
    {
        grid = Resources.Load<GameObject>("Prefabs/Grid");
        
        gridTransform = GetComponent<RectTransform>();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void MakeGridEditor()
    {
        grid = Resources.Load<GameObject>("Prefabs/Grid");
        
        gridTransform = GetComponent<RectTransform>();
        
        var tempList = transform.Cast<Transform>().ToList();
        foreach(var child in tempList)
        {
            if (child.CompareTag("Grid"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        gridList.Clear();

        var parentTransform = gridTransform.parent.GetComponent<RectTransform>();
        
        gridTransform.sizeDelta = parentTransform.sizeDelta;
        var gridTransformRect = gridTransform.rect;
        
        var x = gridTransformRect.size.x;
        var y = gridTransformRect.size.y;
        
        var x1 = Mathf.RoundToInt(x/100)*100;
        var y1 = Mathf.RoundToInt(y/100)*100;
        
        var sizex = x/x1;
        var sizey = y/y1;
        
        var difx = x - x1;
        var dify = y - y1;
        
        gridTransform.localScale = new Vector2(sizex, sizey);
        gridTransform.anchoredPosition = parentTransform.anchoredPosition + new Vector2(difx/2, dify/2);
        //gridTransform.position = new Vector3(x/2+difx/2, y/2+dify/2, 0);
        
        for (var i = 0; i < x1/100; i++)
        {
            gridList.Add(new List<GameObject>());
            for (var j = 0; j < y1/100; j++)
            {
                var obj = Instantiate(grid, transform);
                var rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i*100+50, j*100+50);
                rectTransform.sizeDelta = new Vector2(100, 100);
                gridList[i].Add(obj.transform.GetChild(0).gameObject);
            }
        }
    }

    public void MakeGrid()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        gridList.Clear();
        
        var parentTransform = gridTransform.parent.GetComponent<RectTransform>();
        
        gridTransform.sizeDelta = parentTransform.sizeDelta;
        var gridTransformRect = gridTransform.rect;
        
        var x = gridTransformRect.size.x;
        var y = gridTransformRect.size.y;
        
        var x1 = Mathf.RoundToInt(x/100)*100;
        var y1 = Mathf.RoundToInt(y/100)*100;
        
        var sizex = x/x1;
        var sizey = y/y1;
        
        var difx = x - x1;
        var dify = y - y1;
        
        gridTransform.localScale = new Vector2(sizex, sizey);
        gridTransform.anchoredPosition = parentTransform.anchoredPosition + new Vector2(difx/2, dify/2);
        //gridTransform.position = new Vector3(x/2+difx/2, y/2+dify/2, 0);

        for (var i = 0; i < x1/100; i++)
        {
            gridList.Add(new List<GameObject>());
            for (var j = 0; j < y1/100; j++)
            {
                var obj = Instantiate(grid, transform);
                var rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i*100+50, j*100+50);
                rectTransform.sizeDelta = new Vector2(100, 100);
                gridList[i].Add(obj.transform.GetChild(0).gameObject);
            }
        }
    }
}
