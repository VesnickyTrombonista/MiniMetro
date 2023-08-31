using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow;

public class PeopleGenerating : MonoBehaviour
{
    public static PeopleGenerating singletone;
    // scaling of the station
    private float scaleX = 0.45f;
    private float scaleY = 0.5f;

    public float spacing = 0.8f;
    public float alignment = 3f;

    private string[] peopleNames = new string[7] { "circle", "square", "triangle", "hexagon", "rectangular", "pentagon", "star" };

    // Awake is called when the script is initialized and when a Scene loads
    void Awake()
    {
        singletone = this;
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    /// <summary>
    /// Generates a new person at a given station, considering the type of the station and avoiding the same type as the station's name.
    /// </summary>
    /// <param name="people">A dictionary of people types and their respective prefabs.</param>
    /// <param name="station">The station where the person will be generated.</param>
    /// <param name="peopleQueue">The transform representing the queue of people at the station.</param>
    /// <param name="spawnedStationsTypes">A list of already spawned station types.</param>
    public void GeneratePerson(Dictionary<string, Transform> people, Transform station, Transform peopleQueue, List<string> spawnedStationsTypes)
    {
        Station stationQueue = station.GetComponent<Station>();
        string stationType = stationQueue.name;
        string peopleType = GeneratePeopleType(stationType, spawnedStationsTypes);
        if (peopleType == "")
        {
            return;
        }
        Vector3 position = GetStationPosition(station) + new Vector3(stationQueue.passengerQueue.Count * spacing - alignment, 0, 0);
        GameObject newPerson = Instantiate(people[peopleType].gameObject, position, Quaternion.identity);
        newPerson.transform.parent = peopleQueue;
        newPerson.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        newPerson.GetComponent<Person>().name = peopleType;
        newPerson.transform.position = position;
        
        stationQueue.passengerQueue.Add(newPerson);
    }
    /// <summary>
    /// Generates a type of people for a given station, avoiding the same type as the station's name.
    /// </summary>
    /// <param name="stationName">The name of the station.</param>
    /// <param name="spawnedStationsTypes">A list of already spawned station types.</param>
    /// <returns>A name representing the type of people for the station.</returns>
    private string GeneratePeopleType(string stationName, List<string> spawnedStationsTypes)
    {
        int stationsCount = spawnedStationsTypes.Count;
        /*if (stationsCount <= 1)
        {
            return "";
        }*/
        int index = Random.Range(0, spawnedStationsTypes.Count);
        string peopleName = peopleNames[index];
        if (stationName == peopleName)
        {
            peopleName = GeneratePeopleType(stationName, spawnedStationsTypes);
        }
        return peopleName;
    }
    /// <summary>
    /// Returns the position of a station
    /// </summary>
    /// <param name="station">The transform of the station object.</param>
    /// <returns>The position of the station.</returns>
    Vector3 GetStationPosition(Transform station)
    {
        return station.GetChild(0).GetComponent<Canvas>().transform.position;
    }
}
