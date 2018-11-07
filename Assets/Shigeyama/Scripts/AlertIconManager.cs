using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertIconManager : MonoBehaviour
{
    GameObject gimmickObj;

    GameObject[] players;

    float scaleSpeed;

    const float playerDistance = 2.0f;

    bool isPlayerNear;

    bool isPlayerAction;

    // Use this for initialization
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        scaleSpeed = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerNear = false;

        for (int i = 0; i < players.Length; i++)
        {
            if (Vector3.Distance(gimmickObj.transform.position, players[i].transform.position) < playerDistance)
            {
                isPlayerNear = true;
                break;
            }
        }

        // プレイヤーが近い(小さく表示)
        if (isPlayerNear || isPlayerAction)
        {
            IconSizeDown();
        }
        // プレイヤーが遠い(大きく表示)
        else
        {
            IconSizeUp();
        }
    }

    void IconSizeUp()
    {
        if (gameObject.transform.localScale.x < 1.5f)
        {
            gameObject.transform.localScale += new Vector3(scaleSpeed, scaleSpeed, 0.0f) * Time.deltaTime;
            if (gameObject.transform.localScale.x > 1.5f)
            {
                gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            }
        }
    }

    void IconSizeDown()
    {
        if (gameObject.transform.localScale.x > 0.5f)
        {
            gameObject.transform.localScale -= new Vector3(scaleSpeed, scaleSpeed, 0.0f) * Time.deltaTime;
            if (gameObject.transform.localScale.x < 0.5f)
            {
                gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            }
        }
    }

    public GameObject GimmickObj
    {
        set { gimmickObj = value; }
    }

    public IEnumerator EventAlertTimer(float eventTimer)
    {
        float timer = 0;

        // 指定した時間までタイマーを加算する、またプレイヤーが行動を始めた場合もタイマーを止める
        while (timer < eventTimer)
        {
            timer += Time.deltaTime;

            if (timer > eventTimer)
            {
                timer = eventTimer;
            }

            if (isPlayerAction)
            {
                break;
            }
            transform.GetChild(2).GetComponent<Image>().fillAmount = timer / eventTimer;

            yield return null;
        }
        yield return null;
    }

    public IEnumerator PlayerActionTimer(float eventTimer)
    {
        float timer = 0;

        isPlayerAction = true;

        // 指定した時間までタイマーを加算する、またプレイヤーが行動を始めた場合もタイマーを止める
        while (timer < eventTimer)
        {
            timer += Time.deltaTime;

            if (timer > eventTimer)
            {
                timer = eventTimer;
            }

            transform.GetChild(0).GetComponent<Image>().fillAmount = timer / eventTimer;

            yield return null;
        }
    }
}
