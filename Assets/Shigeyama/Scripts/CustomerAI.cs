using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{

    // AIの状態を管理するステート
    public enum CustomerAIState
    {
        CHECK,
        IDLE,
        MOVE,
        ACTION,
        ACCIDENT,
        REGISTERMOVE,
        REGISTERIDLE,
        REGISTERACTION
    }

    // ステートの宣言
    CustomerAIState state;
    
    // NavMeshAgentの宣言
    NavMeshAgent agent;

    // 全棚を管理する配列
    GameObject[] shelfObj;

    // 全レジを管理する配列
    GameObject[] registerObj;

    // アクションを起こす対象を保存しておく変数
    GameObject actionObj;

    // 実際に移動するポジションを記憶する変数
    GameObject movePosObj;

    // レジの順番
    int registerNum;

    // タイマー
    float timer = 0;

    // 棚から商品を取った数
    int stockCount = 0;

    bool isMoveEnd = false;

    // Use this for initialization
    void Start()
    {
        // 初期化
        agent = GetComponent<NavMeshAgent>();
        // シーン上にある棚オブジェクトを全取得
        shelfObj = GameObject.FindGameObjectsWithTag("Shelf");
        // シーン上にあるレジオブジェクトを全取得
        registerObj = GameObject.FindGameObjectsWithTag("Register");
        // 初期ステートの設定
        state = CustomerAIState.CHECK;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // 各ステートを実行
        switch (state)
        {
            case CustomerAIState.CHECK:
                Check();
                break;
            case CustomerAIState.IDLE:
                Idle();
                break;
            case CustomerAIState.MOVE:
                Move();
                break;
            case CustomerAIState.ACTION:
                Action();
                break;
            case CustomerAIState.ACCIDENT:
                Accident();
                break;
            case CustomerAIState.REGISTERMOVE:
                RegisterMove();
                break;
            case CustomerAIState.REGISTERIDLE:
                RegisterIdle();
                break;
            case CustomerAIState.REGISTERACTION:
                RegisterAction();
                break;
        }

        Debug.Log(state);
    }

    /// <summary>
    /// 移動先の検索とその後の行動を指定
    /// </summary>
    void Check()
    {
        List<GameObject> canMovePosObjcts = new List<GameObject>();
        
        for (int i = 0; i < shelfObj.Length; i++)
        {
            if (shelfObj[i].GetComponent<ItemStockEvent>().CheckPos() && shelfObj[i] != actionObj && stockCount < 4)
            {
                canMovePosObjcts.Add(shelfObj[i]);
            }
        }

        for (int i = 0; i < registerObj.Length; i++)
        {
            if (registerObj[i].GetComponent<RegisterEvent>().CheckPos() && stockCount > 0)
            {
                canMovePosObjcts.Add(registerObj[i]);
            }
        }

        if (canMovePosObjcts.Count > 0)
        {
            int rnd = Random.Range(0, canMovePosObjcts.Count);

            actionObj = canMovePosObjcts[rnd];

            if (actionObj.GetComponent<ItemStockEvent>())
            {
                movePosObj = actionObj.GetComponent<ItemStockEvent>().AIMovePosObj();

                state = CustomerAIState.MOVE;
            }
            else
            {

                registerNum = actionObj.GetComponent<RegisterEvent>().AIMovePosObjNum();
                movePosObj = actionObj.GetComponent<RegisterEvent>().AIMovePosObj();
                state = CustomerAIState.REGISTERMOVE;
            }
        }
        else
        {
            timer = 0;
            state = CustomerAIState.IDLE;
        }
    }

    /// <summary>
    /// 待機状態
    /// </summary>
    void Idle()
    {
        float timeInterval = 2.0f;

        if (timer > timeInterval)
        {
            state = CustomerAIState.CHECK;
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    void Move()
    {
        agent.SetDestination(movePosObj.transform.position);

        if (Vector3.Distance(transform.position, movePosObj.transform.position) < 0.5f)
        {
            timer = 0;
            state = CustomerAIState.ACTION;
        }
    }


    /// <summary>
    /// ギミックの発動
    /// </summary>
    void Action()
    {
        float timeInterval = 2.0f;

        if (timer > timeInterval)
        {
            actionObj.GetComponent<ItemStockEvent>().StockDecrement(1);
            actionObj.GetComponent<ItemStockEvent>().IsAIMoveRelease(movePosObj);

            stockCount++;

            state = CustomerAIState.CHECK;
        }
    }

    /// <summary>
    /// 一定確率で起こるアクシデント(MoveまたはIdle状態から)
    /// </summary>
    void Accident()
    {

    }

    void RegisterMove()
    {
        agent.SetDestination(movePosObj.transform.position);

        if (Vector3.Distance(transform.position, movePosObj.transform.position) < 0.5f)
        {
            if (isMoveEnd)
            {
                AIGenerator.Instance.RemoveList(gameObject);
                Destroy(gameObject);
            }
            else
            {
                state = CustomerAIState.REGISTERIDLE;
            }
        }
    }

    void RegisterIdle()
    {
        if (registerNum <= 0)
        {
            actionObj.GetComponent<RegisterEvent>().IsPlayerActionEnd = false;
            state = CustomerAIState.REGISTERACTION;
        }
        else if (actionObj.GetComponent<RegisterEvent>().IsAIMoveLine(registerNum))
        {
            movePosObj = actionObj.GetComponent<RegisterEvent>().AIMoveLineObj(registerNum);
            registerNum--;
            state = CustomerAIState.REGISTERMOVE;
        }
    }

    void RegisterAction()
    {
        actionObj.GetComponent<RegisterEvent>().IsCustomer = true;

        if (actionObj.GetComponent<RegisterEvent>().IsPlayerActionEnd)
        {
            actionObj.GetComponent<RegisterEvent>().IsCustomer = false;
            isMoveEnd = true;
            movePosObj = GameObject.Find("EntryPos");
            state = CustomerAIState.REGISTERMOVE;
        }
    }
}
