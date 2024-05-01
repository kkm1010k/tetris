using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridChanger : MonoBehaviour
{
    private GridSpawner gridSpawner;
    private bool isSpinning;
    private void Awake()
    {
        gridSpawner = GetComponentInChildren<GridSpawner>();
    }
    
    private async void Start()
    {
        gridSpawner.MakeGrid();
        await Task.Delay(1000);
        SpinChange("Title");
    }

    public void SpinChange(string name)
    {
        if (isSpinning)
        {
            return;
        }
        
        StartCoroutine(SpinCoroutine(name));
    }

    public void SceneChange(string name)
    {
        StartCoroutine(SceneChangeCoroutine(name));
    }
    
    private IEnumerator SceneChangeCoroutine(string name)
    {
        if (isSpinning)
        {
            yield break;
        }
        
        yield return StartCoroutine(SpinCoroutine("", true));
        
        
        
        SceneManager.LoadScene(name);
    }
    
    private List<List<GameObject>> GetObjList(string name)
    {
        var tempList = transform.GetChild(0).Find(name).Cast<Transform>();
        var objList = new List<GameObject>();
        var multiList = new List<GameObject>();
        foreach (var child in tempList)
        {
            if (child.CompareTag("MultiGrid"))
            {
               multiList.Add(child.gameObject); 
            }
            else
            {
                objList.Add(child.gameObject);
            }
        }

        var list = new List<List<GameObject>>
        {
            objList,
            multiList
        };
        return list;
    }
    
    private void Change(string name)
    {
        var objList = GetObjList(name);
        foreach (var obj in objList[0])
        {
            var selectedGridObj = GetSelectedGridObjects(obj);
            var objtext = obj.GetComponentInChildren<TextMeshProUGUI>();
            var cnt = objtext ? AdjustTextAndGetCount(objtext, selectedGridObj.Count) : 0;
    
            for (var i = 0; i < selectedGridObj.Count; i++)
            {
                SetGridProperties(obj, selectedGridObj[i], objtext, cnt, i);
            }
        }
        
        foreach (var obj in objList[1])
        {
            var selectedGridObj = GetSelectedGridObjects2d(obj);
            for (var i = 0; i < selectedGridObj.Count; i++)
            {
                for (var j = 0; j < selectedGridObj[i].Count; j++)
                {
                    SetMultiGridProperties(obj, selectedGridObj, i, j);
                }
            }
        }
    }

    private void SetGridProperties(GameObject obj, GameObject gridObj, TextMeshProUGUI objtext, int cnt, int i)
    {
        var property = gridObj.GetComponent<GridProperty>();
        property.isActive = true;
        property.endSize = new Vector2(100, 100);
        var rectTransform = gridObj.transform.GetComponent<RectTransform>();
        property.endPosition = rectTransform.position;
    
        var newobj = InstantiateNewObject(obj, gridObj);
        if (objtext)
        {
            var txt = newobj.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = objtext.text.Substring(i * cnt, cnt);
        }
    }

    private void SetMultiGridProperties(GameObject obj, List<List<GameObject>> selectedGridObj, int i, int j)
    {
        var property = selectedGridObj[i][j].GetComponent<GridProperty>();
        var rectTransform = selectedGridObj[i][j].transform.GetComponent<RectTransform>();
        if (i == 0 && j == 0)
        {
            property.isActive = true;
            property.endSize = new Vector2(selectedGridObj.Count * 100, selectedGridObj[i].Count * 100);
            var temp = CalculateTemp(selectedGridObj);
            property.endPosition = rectTransform.position + new Vector3(temp.x, temp.y);
            InstantiateNewObject(obj, selectedGridObj[i][j]);
        }
        else
        {
            property.isActive = false;
            property.endSize = new Vector2(100, 100);
            property.endPosition = rectTransform.position;
        }
    }

    private Vector2 CalculateTemp(List<List<GameObject>> selectedGridObj)
    {
        var temp = Vector2.zero;
        for (var i = 0; i < selectedGridObj.Count; i++)
        {
            for (var j = 0; j < selectedGridObj[i].Count; j++)
            {
                var rect = GetWorldRect(selectedGridObj[i][j].GetComponent<RectTransform>());

                if (i != 0)
                {
                    temp.x += rect.width / 2;
                }
                if (j != 0)
                {
                    temp.y += rect.height / 2;
                }
            }
        }
        return temp;
    }

    private List<GameObject> GetSelectedGridObjects(GameObject obj)
    {
        var selectedGridObj = new List<GameObject>();
        var r1 = GetWorldRect(obj.GetComponent<RectTransform>());

        foreach (var grid in gridSpawner.gridList)
        {
            foreach (var gridObj in grid)
            {
                if (r1.Overlaps(GetWorldRect(gridObj.transform.parent.GetComponent<RectTransform>())))
                {
                    if (!gridObj.GetComponent<GridProperty>())
                    {
                        gridObj.AddComponent<GridProperty>();
                    }
                    selectedGridObj.Add(gridObj);
                }
            }
        }
        return selectedGridObj;
    }
    
    private List<List<GameObject>> GetSelectedGridObjects2d(GameObject obj)
    {
        var selectedGridObj = new List<List<GameObject>>();
        var r1 = GetWorldRect(obj.GetComponent<RectTransform>());

        foreach (var grid in gridSpawner.gridList)
        {
            var tempList = new List<GameObject>();
            foreach (var gridObj in grid)
            {
                if (r1.Overlaps(GetWorldRect(gridObj.transform.parent.GetComponent<RectTransform>())))
                {
                    if (!tempList.Any() && !gridObj.GetComponent<GridProperty>())
                    {
                        gridObj.AddComponent<GridProperty>();
                    }
                    tempList.Add(gridObj);
                }
            }
            if (tempList.Any())
            {
                selectedGridObj.Add(tempList);
            }
        }
        return selectedGridObj;
    }

    private int AdjustTextAndGetCount(TMP_Text objtext, int gridcnt)
    {
        var w = 0;
        while (objtext.text.Length % gridcnt != 0)
        {
            w++;
            if (w > 100)
            {
                return 0;
            }
            objtext.text = "_" + objtext.text + "_";
        }
        return objtext.text.Length / gridcnt;
    }

    private GameObject InstantiateNewObject(GameObject obj, GameObject gridobj)
    {
        var newobj = Instantiate(obj, gridobj.transform);
        newobj.transform.localPosition = Vector3.zero;
        var newRect = newobj.GetComponent<RectTransform>();
        newRect.sizeDelta = new Vector2(100, 100);
        newRect.pivot = new Vector2(0.5f, 0.5f);
        newRect.anchorMin = new Vector2(0.5f, 0.5f);
        newRect.anchorMax = new Vector2(0.5f, 0.5f);
        newRect.anchoredPosition = Vector2.zero;
        newobj.name = obj.name;
        newobj.SetActive(false);
        return newobj;
    }
    
    public Rect GetWorldRect(RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];
        
        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        return new Rect(position, size);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator SpinCoroutine(string name = "", bool isHalf = false)
    {
        isSpinning = true;

        if (name != string.Empty)
        {
            Change(name);
        }
        
        var gridList = gridSpawner.gridList;
        var startTime = Time.time;
        var cnt = 0;
        var time = 0.3f;
        Coroutine coroutine = null;
        
        while (startTime + time > Time.time)
        {
            var seq = (Time.time - startTime) / time*gridList.Count*gridList[0].Count;
            for (var i = cnt; i <= seq; i++)
            {
                var x = i / gridList[0].Count;
                var y = gridList[0].Count - i % gridList[0].Count - 1;
                coroutine = StartCoroutine(Spin(gridList[x][y], isHalf));
                cnt++;
            }
            yield return null;
        }

        for (var i = cnt; i < gridList.Count*gridList[0].Count; i++)
        {
            var x = i / gridList[0].Count;
            var y = gridList[0].Count - i % gridList[0].Count - 1;
            coroutine = StartCoroutine(Spin(gridList[x][y], isHalf));
        }
        
        yield return coroutine;
        isSpinning = false;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator Spin(GameObject gridObj, bool isHalf = false)
    {
        var time = 0.3f;
        var property = gridObj.GetComponent<GridProperty>();
        var rectTransform = gridObj.GetComponent<RectTransform>();
        
        var endSize = property ? property.endSize : rectTransform.sizeDelta;
        var endPosition = property ? property.endPosition : rectTransform.position;
        
        yield return StartCoroutine(SpinWhileCoroutine(time, gridObj, gridObj.transform.rotation, Quaternion.Euler(-45, -90, 45), endSize, endPosition));
        
        gridObj.SetActive(!property || property.isActive);

        foreach (Transform child in gridObj.transform)
        {
            
            if (child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
        
        if (property)
        {
            property.endSize = new Vector2(100, 100);
            property.endPosition = rectTransform.parent.position;
            property.isActive = true;
        }
        
        if (isHalf)
        {
            yield break;
        }
        
        yield return StartCoroutine(SpinWhileCoroutine(time, gridObj, gridObj.transform.rotation, Quaternion.Euler(0, 0, 0), endSize, endPosition));
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator SpinWhileCoroutine(float time, GameObject gridObj, Quaternion startRot, Quaternion endRot, Vector2 endSize, Vector3 endPosition)
    {
        var rectTransform = gridObj.GetComponent<RectTransform>();
        var startTime = Time.time;
        var startSize = rectTransform.sizeDelta;
        var startPosition = rectTransform.position;
        while (startTime + time > Time.time)
        {
            var seq = (Time.time - startTime) / time*50;
            for (var i = 0; i < seq; i++)
            {
                gridObj.transform.rotation = Quaternion.Slerp(startRot, endRot, i / 50f);
                rectTransform.position = Vector3.Lerp(startPosition, endPosition, i / 50f);
                rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, i / 50f);
            }
            yield return null;
        }
        gridObj.transform.rotation = endRot;
        rectTransform.position = endPosition;
        rectTransform.sizeDelta = endSize;
    }
}
