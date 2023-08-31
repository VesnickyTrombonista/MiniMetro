using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    // Start is called before the first frame update
    void Start()
    {        
        //TimePlanning.singletone.
        centre = this.GetComponent<Transform>().position;
    }
    // Update is called once per frame
    void Update()
    {
    }
    /* void OnCollision()
    {
        generator.transform.localPosition = Vector3.zero + generator.CheckValidPosition(generator.transform.localPosition, planner.stationsGeneratedList);
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
