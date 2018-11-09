using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGenerator : SingletonMonoBehaviour<AIGenerator>
{
    [SerializeField, Header("AIPrefab")]
    GameObject aiObjPre;

    Vector3 entryPos;

    List<GameObject> AIList = new List<GameObject>();

    float timer = 0;
    float timeInterval;

    // 店の中にいるAIの最大数
    const int MAXCUSTOMER = 8;

    // Use this for initialization
    void Start()
    {
        entryPos = GameObject.Find("EntryPos").transform.position;
        timeInterval = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (AIList.Count < MAXCUSTOMER)
        {
            timer += Time.deltaTime;

            if (timer > timeInterval)
            {
                AIList.Add(Instantiate(aiObjPre, entryPos, Quaternion.identity));
                timer = 0;
                timeInterval = Random.Range(1.0f, 6.0f);
            }
        }
    }

    public void RemoveList(GameObject removeObj)
    {
        for (int i = 0; i < AIList.Count; i++)
        {
            if (AIList[i] == removeObj)
            {
                AIList.RemoveAt(i);
            }
        }
    }
}
