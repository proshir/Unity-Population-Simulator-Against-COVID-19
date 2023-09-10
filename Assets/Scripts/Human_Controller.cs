using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human_Controller : MonoBehaviour
{
    internal Spawner home;
    private NavMeshAgent agent;
    private Animator animator;
    private float range = 10f;
    private float delay = 7f;
    private float min_speed = 1.5f;
    internal bool awake = false;
    internal int sleepTime;
    internal int awakeTime;

    private float awakeSpent = 0f;
    private CanvasHumanManager canvasHumanManager;

    [SerializeField] internal bool gotVirus = false;

    private float health = 100;

    void Start()
    {
        health = 100;
        agent = GetComponent<NavMeshAgent>();
        gameObject.tag = "Human";
        animator = GetComponent<Animator>();
        agent.stoppingDistance = 1f;
        canvasHumanManager = Instantiate(Manager.instance.CanvasHuman, transform).GetComponent<CanvasHumanManager>();
        SetAwakePerson(false);
        canvasHumanManager.UpdateSleepTime(sleepTime);
        canvasHumanManager.UpdateAwakeTime(awakeTime);
        StartCoroutine(UpdateDestination());
    }

    private void NegHealth(float healthD)
    {
        health -= healthD;
        if (health < 0)
        {
            health = 0;
            if (awake && awakeSpent > 0.5f)
            {
                GameObject grave = Instantiate(Manager.instance.gravePrefab, transform.position, Quaternion.identity, Manager.instance.graveParent);
                grave.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            Destroy(gameObject);
            return;
        }
        canvasHumanManager.SetHealthBar(health);
    }

    private void SetAwakePerson(bool status)
    {
        if (status && !CheckCanAwake())
            return;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(status);
        }
        GetComponent<Animator>().enabled = status;
        GetComponent<CapsuleCollider>().enabled = status;
        GetComponent<NavMeshAgent>().enabled = status;
        awake = status;
        if (!awake)
            awakeSpent = 0;
    }

    private bool CheckCanAwake()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider nearbyCollider in nearbyColliders)
            if (nearbyCollider.tag == "Human" && nearbyCollider.gameObject != gameObject)
                return false;
        return true;
    }

    private Coroutine myCoroutine;

    void Update()
    {
        animator.SetBool("walk", agent.velocity.magnitude >= min_speed);
        if (gotVirus && myCoroutine == null)
            myCoroutine = StartCoroutine(checkVirusStatus());
    }
    private List<Human_Controller> GetNearHumans(float range)
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, range);
        List<Human_Controller> result = new List<Human_Controller>();
        foreach (Collider nearbyCollider in nearbyColliders)
        {
            Human_Controller human = nearbyCollider.GetComponent<Human_Controller>();
            if (human != null && human != this)
                result.Add(human);
        }
        return result;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Manager.instance.rangeVirus);
    }

    private void ActiveVirus()
    {
        this.gotVirus = true;
        canvasHumanManager.AddVirus();
    }

    private void CheckCanGiveVirus()
    {
        List<Human_Controller> near_humans = GetNearHumans(Manager.instance.rangeVirus);
        foreach(Human_Controller near_human in near_humans)
        {
            if (!near_human.gotVirus)
                near_human.ActiveVirus();
        }
    }

    IEnumerator checkVirusStatus()
    {
        while (gotVirus)
        {
            if (awake && awakeSpent > 0.1f)
                CheckCanGiveVirus();
            NegHealth(Manager.instance.pDead / 1000f);
            yield return new WaitForSeconds(1f);
        }
        myCoroutine = null;
    }

    IEnumerator UpdateDestination()
    {
        while (true)
        {
            if (awake)
            {
                if (awakeSpent > awakeTime)
                {
                    canvasHumanManager.AddSleep();
                    if (agent.SetDestination(home.transform.position))
                        yield return new WaitUntil(() => Vector3.Distance(gameObject.transform.position, home.transform.position) <= agent.stoppingDistance);
                    if (Vector3.Distance(gameObject.transform.position, home.transform.position) <= agent.stoppingDistance)
                        SetAwakePerson(false);
                    yield return new WaitForSeconds(1);
                }
                else
                {
                    NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * range, out NavMeshHit hit, range, NavMesh.AllAreas);
                    agent.SetDestination(hit.position);
                    yield return new WaitForSeconds(delay);
                    awakeSpent += delay;
                }
            }
            else
            {
                yield return new WaitForSeconds(sleepTime);
                canvasHumanManager.RemoveIcon("sleep");
                SetAwakePerson(true);
            }
        }
    }

    IEnumerator UpdateDestinationTool()
    {
        while (true)
        {
            NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * range, out NavMeshHit hit, range, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            yield return new WaitForSeconds(delay);
        }
    }
}
