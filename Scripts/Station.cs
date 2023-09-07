using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.CoreUtils;
using static UnityEngine.Rendering.DebugUI.Table;

public class Station : MonoBehaviour
{
    // station's properties
    public List<GameObject> passengerQueue = new List<GameObject>(); // for checking count
    public StationGenerating generator;
    public TimePlanning planner;
    public Transform visibleQueue;
    public string stationName;
    
    public Vector3 centre;
    public float spacing = 0.01f;
    public float distanceFromOthers = 2f;

    TimePlanning timePlanner;
    StationGenerating stationGenerator;
    // Start is called before the first frame update
    void Start()
    {        
        centre = this.GetComponent<Transform>().position;
        timePlanner = TimePlanning.singletone;
        stationGenerator = StationGenerating.singletone;
    }
    // Update is called once per frame
    void Update()
    {
        OnCollisionEnter();
    }
    void OnCollisionEnter()
    {
        if (passengerQueue.Count == 0 && 
            Physics.OverlapSphere(this.gameObject.transform.position, distanceFromOthers).Length > 0)
        {
            Destroy(this.gameObject);
            stationGenerator.GenerateStationAsync(timePlanner.stations, timePlanner.stationsGeneratedList, timePlanner.currentWeek, timePlanner.stationsQueues);
        }
        
    }
    // OnCollisionEnter2D is called when the train collides
    /*void OnCollisionEnter2D(Collision2D collider)
    {
        Debug.Log("OnCollisionEnter2D station with> " + collider);
    }*/

    /// <summary>
    /// Adds a passenger to the station's queue and instantiates a new person object in the visible queue.
    /// </summary>
    /// <param name="passenger">The GameObject representing the passenger to be added.</param>
    public void AddPassenger(GameObject passenger)
    {
        passengerQueue.Add(passenger);
        GameObject person = Instantiate(passenger.gameObject, visibleQueue.transform);
        float xPos = passengerQueue.Count * spacing;
        person.transform.position = new Vector3(xPos, 0f, 0f);
    }
    /// <summary>
    /// Remove passenger from the queue.
    /// </summary>
    public void ProcessNextCustomer()
    {
        if (passengerQueue.Count > 0)
        {
            GameObject passenger = passengerQueue.ToArray()[0];
            passengerQueue.Remove(passenger);
        }
    }
}
