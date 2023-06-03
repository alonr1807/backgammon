using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    Ray ray;

    RaycastHit hit;

    private Vector3 mOffset;

    private float mZCoord;

    private bool isBeingDragged = false;

    [SerializeField] LayerMask SpikeLayer;
    [SerializeField] LayerMask EscapeLayer;

    private void Start()
    {
        //SpikeLayer = LayerMask.NameToLayer("Spike");
        //EscapeLayer = LayerMask.NameToLayer("Escape");
    }

    void OnMouseDown()
    {
        gameObject.SendMessage("OnCustomDragStart");

        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

        isBeingDragged = true;
    }
    private bool handleSpikeMove()
    {
        if(Physics.Raycast(ray, out hit, 10f, SpikeLayer))
        {
            GameObject targetSpikeObject = hit.collider.gameObject;
            SpikeData spikeData = targetSpikeObject.GetComponent<SpikeData>();
            int moveTo = spikeData.getIndex();
            DataPackage dataPackage = new DataPackage(moveTo, gameObject);
            gameObject.SendMessageUpwards("GetMoveData", dataPackage);
            return true;
        }
        return false;
    }
    private bool handleEscapeMove()
    {
        if (Physics.Raycast(ray, out hit, 10f, EscapeLayer))
        {
            GameObject escapeObject = hit.collider.gameObject;
            EscapeData escapeData = escapeObject.GetComponent<EscapeData>();
            //question: do u handle the if statement of checker type here like i did or do it in the main file
            if (escapeData.getEscapeKind() == gameObject.GetComponent<CheckerData>().getKind())
            {
                DataPackage dataPackage = new DataPackage(true, gameObject, escapeObject);
                gameObject.SendMessageUpwards("GetMoveData", dataPackage);
                return true;
            }
        }
        return false;
    }

    private void handleNoMove()
    {
        DataPackage dataPackage = new DataPackage(new Vector3(), false);
        gameObject.SendMessage("OnCustomDragEnd", dataPackage);
    }

    void OnMouseUp()
    {
        if (isBeingDragged)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);
            if(!handleSpikeMove() && !handleEscapeMove())
            {
                handleNoMove();
            }


            // We just dragged this object and released it.
            //  gameObject.SendMessage("OnCustomDragEnd");

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