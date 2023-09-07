using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Train : MonoBehaviour
{
    public GameObject trainModel;
    public BoxCollider2D trainCollider;
    public string trainName;

    public Vector3[] linePoints;  // An array of Vector3 points that define the line
    public float speed = 0.5f;    // The speed at which the GameObject moves

    private int currentPoint = 0; // because train starts at 0 position
    public int maxPoint = 0;
    private bool directionForward = true;
    public bool start = false;

    public float minimalDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        maxPoint = linePoints.Length;
        if (maxPoint > 1)
        {
            currentPoint = 1; // because train starts at 0 position
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (directionForward)
            {
                MoveForward();
            }
            if (!directionForward)
            {
                MoveBackward();
            }
        }
        
        
    }

    /// <summary>
    /// Move the GameObject forward along a predefined path represented by an array of points.
    /// It pays attention to the edges of a line.
    /// </summary>
    public void MoveForward()
    {
        if (currentPoint < maxPoint - 1)
        {
            // Move the GameObject towards the current point
            transform.position = Vector3.MoveTowards(transform.position, linePoints[currentPoint], speed * Time.deltaTime);

            // Check if the GameObject has reached the current point
            if (Vector3.Distance(transform.position, linePoints[currentPoint]) < minimalDistance)
            {
                // Move to the next point
                currentPoint++;
            }
        }
        if (currentPoint == maxPoint - 1) // max position, go back
        {
            directionForward = false;
        }
        else
        {
            return;
        }
    }
    /// <summary>
    /// Move the GameObject backward along a predefined path represented by an array of points.
    /// /// It pays attention to the edges of a line.
    /// </summary>
    public void MoveBackward()
    {
        if(currentPoint > 0)
        {
            // Move the GameObject backwards from the current point
            transform.position = Vector3.MoveTowards(transform.position, linePoints[currentPoint], speed * Time.deltaTime);

            // Check if the GameObject has reached the current point
            if (Vector3.Distance(transform.position, linePoints[currentPoint]) < minimalDistance)
            {
                // Move to the previous point
                currentPoint--;
            }
        }
        if (currentPoint == 0) // min position, go back
        {
            directionForward = true;
        }
        else
        {
            return;
        }
    }

}
