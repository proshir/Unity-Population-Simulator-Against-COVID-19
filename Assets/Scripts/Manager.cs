using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    internal static Manager instance;

    [SerializeField] private GameObject[] person_prefab;
    public Transform people_parent;
    [SerializeField] private int number_pop;
    private Spawner[] spawners;

    [SerializeField] int sleepTime;
    [SerializeField] int awakeTime;
    [SerializeField] internal GameObject CanvasHuman;
    [SerializeField] private float scaleTime = 1f;

    private int day = 0;
    [SerializeField] private int endFrameResult;
    [SerializeField] private float timeDay;
    [SerializeField] internal GameObject Icon;

    [Header("UI")]
    [SerializeField] private Text dayT;
    [SerializeField] Text awakeT;
    [SerializeField] Text sleepT;
    private Human_Controller[] human_Controllers;
    private int awake;
    private int sleep;
    private int virus;
    private int dead;
    private int alive;

    [Header("Virus")]
    [SerializeField] private int dayStart;
    [SerializeField] internal float rangeVirus;
    [SerializeField] Text virusT;
    [SerializeField] private Text deadT;
    [SerializeField] private Text aliveT;
    [SerializeField] internal GameObject gravePrefab;
    [SerializeField] internal Transform graveParent;
    [SerializeField] internal int pDead;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        spawners = FindObjectsOfType<Spawner>();
        SpawnPeople();
        human_Controllers = FindObjectsOfType<Human_Controller>();
        StartCoroutine(UpdateUI());
        StartCoroutine(CheckDay());
    }

    private void SpawnPeople()
    {
        for (int i = 0; i < number_pop; i++)
        {
            float curSleepTime = sleepTime;
            float curAwakeTime = awakeTime;
            spawners[Random.Range(0, spawners.Length)].SpawnPerson(person_prefab[Random.Range(0, person_prefab.Length)], sleepTime, awakeTime);
        }
    }

    private void GiveVirusRandomSomeone()
    {
        Human_Controller human_c = human_Controllers[Random.Range(0, human_Controllers.Length)];
        if (human_c != null)
            human_c.gotVirus = true;
    }

    class MR
    {
        int day;
        int awake;
        int virus;
        int dead;
        int alive;

        public MR(int day, int awake, int virus, int dead, int alive)
        {
            this.day = day;
            this.awake = awake;
            this.virus = virus;
            this.dead = dead;
            this.alive = alive;
        }

        public override string ToString()
        {
            return day + "," + awake + "," + virus + "," + dead + "," + alive;
        }
    }

    IEnumerator CheckDay()
    {
        while (true)
        {
            SetText("Day", dayT, day);
            if (day == dayStart)
                GiveVirusRandomSomeone();
            result.Add(new MR(day, awake, virus, dead, alive));
            if (day == endFrameResult)
            {
                SaveResult();
            }
            yield return new WaitForSeconds(timeDay);
            day++;
        }
    }

    IEnumerator UpdateUI()
    {
        while (true)
        {
            Time.timeScale = scaleTime;
            int awakes = 0;
            int sleeps = 0;
            int viruss = 0;
            int alives = 0;
            int deads = 0;
            foreach (Human_Controller controller in human_Controllers)
            {
                if (controller != null)
                {
                    if (controller.awake)
                        awakes++;
                    else
                        sleeps++;
                    if (controller.gotVirus)
                        viruss++;
                    alives++;
                }
                else
                    deads++;
            }
            awake = awakes;
            sleep = sleeps;
            virus = viruss;
            dead = deads;
            alive = alives;
            SetText("Awake", awakeT, awake);
            SetText("Sleep", sleepT, sleep);
            SetText("Virus", virusT, virus);
            SetText("Dead", deadT, dead);
            SetText("Alive", aliveT, alive);
            yield return new WaitForSeconds(0.5f);
        }
    }

    List<MR> result = new List<MR>();

    private void SaveResult()
    {
        string filePath = Application.dataPath + "/" + "x.csv";

        StreamWriter writer = new StreamWriter(filePath);

        foreach (MR mr in result)
        {
            string line = mr.ToString();
            writer.WriteLine(line);
        }

        writer.Close();

        Debug.Log("CSV file created: " + filePath);
    }

    private void SetText(string name, Text text, float value)
    {
        text.text = name + ": " + value.ToString("F2").TrimEnd('0').TrimEnd('.');
    }

}
