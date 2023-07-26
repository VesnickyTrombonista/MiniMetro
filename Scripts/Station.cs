using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Station : MonoBehaviour
{
    public List<GameObject> passengerQueue = new List<GameObject>();
    public StationGenerating generator;
    public TimePlanning planner;
    public Transform visibleQueue;
    public string stationName;
    private int capacityWithNoWaiting = 6;
    private int maximalCapacity = 20;

    public float spacing = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        generator = new StationGenerating();
        // AddComponent
        planner = new TimePlanning();
    }
    // Update is called once per frame
    void Update()
    {
        CheckWaitingPeople();
    }
    /* void OnCollision()
    {
        generator.transform.localPosition = Vector3.zero + generator.CheckValidPosition(generator.transform.localPosition, planner.stationsGeneratedList);
    }*/

    /// <summary>
    /// Checks the count of the waiting people in the queue and performs actions: timer or game over.
    /// </summary>
    private void CheckWaitingPeople()
    {
        if (visibleQueue.childCount >= capacityWithNoWaiting)
        {
            // start timer of the station
        }
        if (visibleQueue.childCount >= maximalCapacity)
        {
            // gameover    
        }
    }
    public void AddPassenger(GameObject passenger)
    {
        passengerQueue.Add(passenger);
        GameObject person = Instantiate(passenger.gameObject, visibleQueue.transform);
        float xPos = passengerQueue.Count * spacing;
        person.transform.position = new Vector3(xPos, 0f, 0f);
    }

    public void ProcessNextCustomer()
    {
        if (passengerQueue.Count > 0)
        {
           GameObject passenger = passengerQueue.ToArray()[0];
           passengerQueue.Remove(passenger);
        }
    }
}
