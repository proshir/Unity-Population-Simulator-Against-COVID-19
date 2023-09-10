using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    internal void SpawnPerson(GameObject personPrefab, int sleepTime, int awakeTime)
    {
        GameObject person = Instantiate(personPrefab, transform.position, Quaternion.identity, Manager.instance.people_parent);
        person.AddComponent<NavMeshAgent>();
        person.AddComponent<Human_Controller>();
        Human_Controller humanController = person.GetComponent<Human_Controller>();
        humanController.sleepTime = sleepTime;
        humanController.awakeTime = awakeTime;
        humanController.home = this;
    }
}
