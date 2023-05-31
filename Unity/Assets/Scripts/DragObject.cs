using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    Ray ray;

    RaycastHit hit;

    public GameObject targetSpike;

    private Vector3 mOffset;

    private float mZCoord;

    private bool isBeingDragged = false;

    public LayerMask layerToHit; 
    void OnMouseDown()
    {
        gameObject.SendMessage("OnCustomDragStart");

        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

        isBeingDragged = true;
    }

    void OnMouseUp()
    {
        if (isBeingDragged)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

            if (Physics.Raycast(ray, out hit, 10f,layerToHit))
            {
                print(hit.collider.name);
                targetSpike = hit.collider.gameObject;
                SpikeData spikeData = targetSpike.GetComponent<SpikeData>();
                print(spikeData.getIndex());
            }

            // We just dragged this object and released it.
            gameObject.SendMessage("OnCustomDragEnd");

            isBeingDragged = false;
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }
}