using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHumanManager : MonoBehaviour
{
    [SerializeField] private Text SleepTime;
    [SerializeField] private Text AwakeTime;
    [SerializeField] private Slider sliderHealth;
    [SerializeField] private Transform IconsParent;

    [Header("Icons")]
    [SerializeField] private Sprite virus;
    [SerializeField] private Sprite mask; 
    [SerializeField] private Sprite sleep;

    private Dictionary<string, GameObject> icons = new Dictionary<string, GameObject>();
    
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
    }

    internal void SetHealthBar(float health)
    {
        sliderHealth.value = health / 100;
    }

    internal void UpdateSleepTime(int time)
    {
        SleepTime.text = time.ToString();
    }

    internal void UpdateAwakeTime(int time)
    {
        AwakeTime.text = time.ToString();
    }

    internal void UpdateSliderHealth(float x)
    {
        sliderHealth.value = x;
    }

    private void AddIcon(Sprite sprite, string name)
    {
        if (!icons.ContainsKey(name))
        {
            Image img = Instantiate(Manager.instance.Icon, IconsParent).GetComponent<Image>();
            img.sprite = sprite;
            icons[name] = img.gameObject;
        }
    }

    internal void AddVirus()
    {
        AddIcon(virus, "virus");
    }

    internal void AddSleep()
    {
        AddIcon(sleep, "sleep");
    }

    internal void AddMask()
    {
        AddIcon(mask, "mask");
    }

    internal void RemoveIcon(string name)
    {
        if (icons.ContainsKey(name))
        {
            GameObject.Destroy(icons[name]);
            icons.Remove(name);
        }
    }
}
