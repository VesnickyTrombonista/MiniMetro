using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Station : MonoBehaviour
{
    public Queue<GameObject> passengerQueue = new Queue<GameObject>();
    public Transform visibleQueue;
    private int capacityWithNoWaiting = 6;
    private int maximalCapacity = 20;

    public float spacing = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        CheckWaitingPeople();
    }

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
        passengerQueue.Enqueue(passenger);
        GameObject person = Instantiate(passenger.gameObject, visibleQueue.transform);
        float xPos = passengerQueue.Count * spacing;
        person.transform.position = new Vector3(xPos, 0f, 0f);
    }

    public void ProcessNextCustomer()
    {
        if (passengerQueue.Count > 0)
        {
            GameObject passenger = passengerQueue.Dequeue();
        }
    }
}
