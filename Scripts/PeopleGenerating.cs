using System.Collections;
using System.Collections.Generic;
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
        //sorroundings =  0.05f; // todo jen na velikost okna u stanice, todo
    }


    public void GeneratePerson(Dictionary<string, Transform> people, Transform station, Transform peopleQueue, List<string> spawnedStationsTypes)
    {
        Station stationQueue = station.GetComponent<Station>();
        int index = Random.Range(0, spawnedStationsTypes.Count);
        Vector3 position = GetStationPosition(station) + new Vector3(stationQueue.passengerQueue.Count * spacing - alignment, 0, 0);
        GameObject newPerson = Instantiate(people[peopleNames[index]].gameObject, position, Quaternion.identity);
        newPerson.transform.parent = peopleQueue;
        newPerson.transform.localScale = new Vector3(scaleX, scaleY, 0f);
        
        stationQueue.passengerQueue.Enqueue(newPerson);
    }
    Vector3 GetStationPosition(Transform station)
    {
        return station.GetChild(0).GetComponent<Canvas>().transform.position;
    }
}
