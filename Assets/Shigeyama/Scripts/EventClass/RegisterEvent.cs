using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterEvent : MonoBehaviour, IGimmick
{
    [SerializeField, Header("Icon")]
    GameObject eventAlertIconPrefab;

    // AIが移動可能かどうか
    bool isAIMove;

    // 各場所が空いているかどうか
    bool[] isAIMoves;

    // 移動先の座標
    GameObject[] aiMovePosObjects;

    // 移動先保存変数
    GameObject aiMovePosObj;

    GameObject eventAlertIcon;

    int customerCounter = 0;

    // レジに客がいるかどうか
    bool isCustomer;

    // プレイヤーがアクションを終えたかどうか
    bool isPlayerActionEnd;

    bool updateStop = false;

    void Awake()
    {
        isAIMoves = new bool[transform.childCount];
        aiMovePosObjects = new GameObject[transform.childCount];

        // 配列を子供の数分初期化して長さを決める
        for (int i = 0; i < transform.childCount; i++)
        {
            isAIMoves[i] = false;
            // 座標取得
            aiMovePosObjects[i] = transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        if (updateStop) return;

        if (isCustomer)
        {
            updateStop = true;

            StartCoroutine(EventTimer());
        }
    }

    public bool CheckPos()
    {
        isAIMove = false;

        if (customerCounter < isAIMoves.Length)
        {
            isAIMove = true;
        }

        return isAIMove;
    }

    public int AIMovePosObjNum()
    {
        customerCounter++;
        isAIMoves[customerCounter - 1] = true;
        return customerCounter - 1;
    }

    public GameObject AIMovePosObj()
    {
        return aiMovePosObjects[customerCounter - 1];
    }

    //------------------------------------------------------------

    public bool IsAIMoveLine(int registerNum)
    {
        bool isMove = false;
        if (!isAIMoves[registerNum - 1])
        {
            isMove = true;
        }

        return isMove;
    }

    public GameObject AIMoveLineObj(int registerNum)
    {
        // 自身のいた場所は空に
        isAIMoves[registerNum] = false;
        // 向かう場所は誰も来れないように
        isAIMoves[registerNum - 1] = true;

        return aiMovePosObjects[registerNum - 1];
    }

    IEnumerator EventTimer()
    {
        eventAlertIcon = Instantiate(eventAlertIconPrefab,
            transform.position + new Vector3(0, 2.5f, 0),
            Quaternion.Euler(Camera.main.gameObject.transform.localEulerAngles));

        eventAlertIcon.transform.GetChild(0).GetComponent<Image>().fillAmount = 0;

        eventAlertIcon.GetComponent<AlertIconManager>().GimmickObj = gameObject;

        float timeInterval = 20;

        // 時間を計測
        StartCoroutine(eventAlertIcon.GetComponent<AlertIconManager>().EventAlertTimer(timeInterval));
        yield return new WaitForSeconds(timeInterval);

        // スコアの減少(クレーム)
        //ScoreManager.Instance.ScoreDecrement();

        yield return null;
    }

    //-------------------------------------------------------------

    public void PlayGimmick(GameObject player)
    {
        if (isCustomer && eventAlertIcon != null)
        {
            player.GetComponent<PlayerSystem>().IsEvent = true;

            StartCoroutine(PlayRegister(player));
        }
    }

    IEnumerator PlayRegister(GameObject player)
    {
        float timeInterval = 2;

        StartCoroutine(eventAlertIcon.GetComponent<AlertIconManager>().PlayerActionTimer(timeInterval));
        yield return new WaitForSeconds(timeInterval);

        Destroy(eventAlertIcon);
        eventAlertIcon = null;

        isPlayerActionEnd = true;
        isCustomer = false;
        isAIMoves[0] = false;
        updateStop = false;
        customerCounter--;
        player.GetComponent<PlayerSystem>().IsEvent = false;
        yield return null;
    }

    public bool IsCustomer
    {
        get { return isCustomer; }
        set { isCustomer = value; }
    }

    public bool IsPlayerActionEnd
    {
        get { return isPlayerActionEnd; }
        set { isPlayerActionEnd = value; } 
    }
}
