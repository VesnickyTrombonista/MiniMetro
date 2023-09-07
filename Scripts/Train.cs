using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Train : MonoBehaviour
{
    public GameObject trainModel;
    public BoxCollider2D trainCollider;
    public string trainName;

    public UnityEngine.Vector3[] linePoints;  // An array of Vector3 points that define the line
    public static float speed = 0.5f;    // The speed at which the GameObject moves

    private int currentPoint = 0; // because train starts at 0 position
    public int maxPoint = 0;
    private bool directionForward = true;
    public bool start = false;

    public float minimalDistance = 0.1f;

    public TimePlanning planner;

    public float timeOfCreation; // from planner and its currentTimeInWeek
    public float deltaTimeForDestruction = 0.5f;

    public string[] stationNames = new string[7];
    public List<GameObject> trainRoom = new List<GameObject>();
    public float passengerCapacity = 6;
    public int passengerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        this.AddComponent<TrainDeletor>();
        planner = TimePlanning.singletone;
        timeOfCreation = planner.currentTimeInWeek;
        maxPoint = linePoints.Length;
        if (maxPoint > 1)
        {
            currentPoint = 1; // because train starts at 0 position
        }
        InitializeStationsNames();
    }

    // Update is called once per frame
    void Update()
    {
        //this.GetComponentInParent<TextMeshProUGUI>().text = passengerCount.ToString();
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
        this.GetComponent<TrainDeletor>().Destruction(timeOfCreation, deltaTimeForDestruction, linePoints.Length, trainName);
    }
    // Gets called when the object enters the collider area
    void OnTriggerEnter2D(Collider2D objectCollider)
    {
        string colliderName = objectCollider.name;
        if (IsStationName(colliderName))
        {
            Exit(colliderName);
            Boarding(objectCollider);
        }
        else return;
    }
    /// <summary>
    /// Handles the boarding process of passengers from a station to the train.
    /// </summary>
    /// <param name="objectCollider">The Collider2D of the station where passengers are boarding.</param>
    public void Boarding(Collider2D objectCollider)
    {
        if (objectCollider.GetComponent<Station>().passengerQueue.Count < 1)
        {
            return;
        }
        while (trainRoom.Count < passengerCapacity && objectCollider.GetComponent<Station>().passengerQueue.Count > 0)
        // enough space and people
        {
            trainRoom.Add(objectCollider.GetComponent<Station>().passengerQueue[0]);
            objectCollider.GetComponent<Station>().passengerQueue.RemoveAt(0);

            passengerCount++;
        }
    }
    /// <summary>
    /// Handles the process of passengers exiting the train at a specific station.
    /// </summary>
    /// <param name="stationName">The name of the station where passengers are exiting.</param>
    public void Exit(string stationName)
    {
        if (trainRoom.Count == 0)
        {
            return;
        }
        else
        {
            GameObject passenger = trainRoom[0];
            if (passenger.GetComponent<Person>().personName == stationName)
            {
                
                trainRoom.Remove(passenger);
                passenger.GetComponent<Person>().used = false;
                int count;
                bool parsing = Int32.TryParse(planner.totalPassengersCount.text, out count);
                if (parsing)
                {
                    count++;
                    planner.totalPassengersCount.text = count.ToString();
                    passengerCount--;
                }
                else 
                { 
                    return;
                }
                
            }
        }
    }
    /// <summary>
    /// Checks if a given name corresponds to one of the predefined station names.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if the name matches one of the predefined station names; otherwise, false.</returns>
    public bool IsStationName(string name)
    {
        if (name == stationNames[0] || name == stationNames[1] || name == stationNames[2] || name == stationNames[3] || 
            name == stationNames[4] || name == stationNames[5] || name == stationNames[6]) 
        {
            return true;
        }    
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Initializes the predefined station names.
    /// </summary>
    public void InitializeStationsNames()
    {
        stationNames[0] = "circle";
        stationNames[1] = "square";
        stationNames[2] = "triangle";
        stationNames[3] = "pentagon";
        stationNames[4] = "rectangular";
        stationNames[5] = "hexagon";
        stationNames[6] = "star";
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
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, linePoints[currentPoint], speed * Time.deltaTime);

            // Check if the GameObject has reached the current point
            if (UnityEngine.Vector3.Distance(transform.position, linePoints[currentPoint]) < minimalDistance)
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
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, linePoints[currentPoint], speed * Time.deltaTime);

            // Check if the GameObject has reached the current point
            if (UnityEngine.Vector3.Distance(transform.position, linePoints[currentPoint]) < minimalDistance)
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
