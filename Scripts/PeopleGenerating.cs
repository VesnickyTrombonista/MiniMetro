using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow;

public class PeopleGenerating : MonoBehaviour
{
    // scaling of the station
    private float scaleX = 0.45f;
    private float scaleY = 0.5f;

    public float spacing = 0.8f;
    public float alignment = 3f;

    private string[] peopleNames = new string[7] { "circle", "square", "triangle", "hexagon", "rectangular", "pentagon", "star" };

    private void Start()
    {

    }


    public void GeneratePerson(Dictionary<string, Transform> people, Transform station, Transform peopleQueue, List<string> spawnedStationsTypes)
    {
        Station stationQueue = station.GetComponent<Station>();
        string stationType = stationQueue.name;
        string peopleType = GeneratePeopleType(stationType, spawnedStationsTypes);
        Vector3 position = GetStationPosition(station) + new Vector3(stationQueue.passengerQueue.Count * spacing - alignment, 0, 0);
        GameObject newPerson = Instantiate(people[peopleType].gameObject, position, Quaternion.identity);
        newPerson.transform.parent = peopleQueue;
        newPerson.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        newPerson.GetComponent<Person>().name = peopleType;
        newPerson.transform.position = position;
        
        stationQueue.passengerQueue.Add(newPerson);
    }
    private string GeneratePeopleType(string stationName, List<string> spawnedStationsTypes)
    {
        int index = Random.Range(0, spawnedStationsTypes.Count);
        string peopleName = peopleNames[index];
        if (stationName == peopleName)
        {
            peopleName = GeneratePeopleType(stationName, spawnedStationsTypes);
        }
        return peopleName;
    }
    Vector3 GetStationPosition(Transform station)
    {
        return station.GetChild(0).GetComponent<Canvas>().transform.position;
    }
}
