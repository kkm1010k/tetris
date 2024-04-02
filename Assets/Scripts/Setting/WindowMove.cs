using UnityEngine;

public class WindowMove : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private RectTransform rectTransform;
    private void Start()
    {
        mainCamera = Camera.main;
        rectTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void OnMouseDown()
    {
        offset = rectTransform.position - new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
    }

    public void OnMouseDrag()
    {
        rectTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f) + offset;
    }
}
