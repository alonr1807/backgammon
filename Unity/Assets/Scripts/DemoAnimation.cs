using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoAnimation : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;

    private bool shouldAnimate = false;
    private float startTime;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Started Animation");
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldAnimate)
        {
            // Go towards end position (simple but buggy)
            // transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime / 2);

            // Go towards end position (more correct)
            transform.position = Vector3.SmoothDamp(transform.position, endPos, ref velocity, 2f);

            //float timePassed = Time.time - startTime;
            //transform.position = startPos + (endPos - startPos) * Mathf.Clamp(timePassed / 2, 0, 1);
        }
        //transform.position += Vector3.right * 0.1f * Time.deltaTime;
    }

    public void OnCustomDragStart()
    {
        //Debug.Log("Got drag start event");
        startPos = transform.position;
    }
    
    public void OnCustomDragEnd(DataPackage datapack)
    {
        //Debug.Log("Got drag end event");
        if (datapack.valid)
        {
            endPos = datapack.finalPos;
            transform.position = startPos;
            shouldAnimate = true;
        }
        else
        {
            transform.position = startPos;
        }
    
        //startTime = Time.time;
    }
}
