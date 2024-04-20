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
        
        yield return StartCoroutine(SpinCoroutine(name, true));
        
        
        
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
                var newobj = InstantiateNewObject(obj, selectedGridObj[i]);
                if (objtext)
                {
                    var txt = newobj.GetComponentInChildren<TextMeshProUGUI>();
                    txt.text = objtext.text.Substring(i * cnt, cnt);
                }
                newobj.name = obj.name;
            }
        }
        
        foreach (var obj in objList[1])
        {
            var selectedGridObj = GetSelectedGridObjects(obj);
            var startPos = selectedGridObj[0].transform.position;
            var lastPos = selectedGridObj[^1].transform.position;
            var size = new Vector2(
                lastPos.x - startPos.x + 100,
                lastPos.y - startPos.y + 100
                );
            var anchoredPosition = new Vector2(
                size.x / 2 - 50,
                size.y / 2 - 50
                );
            // ReSharper disable once UnusedVariable
            var newobj = InstantiateCustomObject(obj,selectedGridObj[0], size, anchoredPosition);
            newobj.name = obj.name;
        }
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
                    selectedGridObj.Add(gridObj);
                }
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
        newobj.SetActive(false);
        return newobj;
    }
    
    private GameObject InstantiateCustomObject(GameObject obj, GameObject gridobj, Vector2 sizeDelta, Vector2 anchoredPosition)
    {
        var newobj = Instantiate(obj, gridobj.transform);
        newobj.transform.localPosition = Vector3.zero;
        var newRect = newobj.GetComponent<RectTransform>();
        newRect.sizeDelta = sizeDelta;
        newRect.pivot = new Vector2(0.5f, 0.5f);
        newRect.anchorMin = new Vector2(0.5f, 0.5f);
        newRect.anchorMax = new Vector2(0.5f, 0.5f);
        newRect.anchoredPosition = anchoredPosition;
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
        var time = 1f;
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
    
    private IEnumerator Spin(GameObject obj, bool isHalf = false)
    {
        var time = 0.5f;
        yield return StartCoroutine(SpinWhileCoroutine(time, obj.transform.rotation, Quaternion.Euler(-45, -90, 45), obj));

        foreach (Transform child in obj.transform)
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
        
        if (isHalf)
        {
            yield break;
        }
        
        yield return StartCoroutine(SpinWhileCoroutine( time, obj.transform.rotation, Quaternion.Euler(0, 0, 0), obj));
    }

    private IEnumerator SpinWhileCoroutine( float time, Quaternion startRot, Quaternion endRot, GameObject obj)
    {
        var startTime = Time.time;
        while (startTime + time > Time.time)
        {
            var seq = (Time.time - startTime) / time*50;
            for (var i = 0; i < seq; i++)
            {
                obj.transform.rotation = Quaternion.Slerp(startRot, endRot, i/50f);
            }
            yield return null;
        }
        obj.transform.rotation = endRot;
    }
}
