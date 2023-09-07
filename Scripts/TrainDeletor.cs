using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Net;
using System.Threading;

public class TrainDeletor : MonoBehaviour
{
    public Train myTrain;
    public Vector3[] trainPoints;

    public TimePlanning planner;

    string defaultName = "train";


    private void Awake()
    {
        planner = TimePlanning.singletone;
    }
    // Start is called before the first frame update
    private void Start()
    {
    }
    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// Destroys the GameObject under certain conditions.
    /// </summary>
    /// <param name="timeOfCreation">The time of creation of the GameObject.</param>
    /// <param name="deltaTimeForDestruction">The time interval for potential destruction.</param>
    /// <param name="linePointsLength">The length of the line points attached to the GameObject.</param>
    /// <param name="trainName">The name of the train associated with the GameObject.</param>
    public void Destruction(float timeOfCreation, float deltaTimeForDestruction, int linePointsLength, string trainName) 
    {
        if (planner.currentTimeInWeek > timeOfCreation + deltaTimeForDestruction
                && linePointsLength < 3 && trainName!= defaultName)
        {
                Destroy(this.gameObject);
            
        }
    }


}