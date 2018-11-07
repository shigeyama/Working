using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//-----------------------------------------------
// 棚を管理するスクリプト
// プレイヤーからのアクセスと
// AIからのアクセスを取ります
//-----------------------------------------------

public class ItemStockEvent : MonoBehaviour, IGimmick
{
    // 最大ストック数、初期値
    const int defaultStock = 5;

    // 現在のストック数
    int stock;

    [SerializeField, Header("Icon")]
    GameObject eventAlertIconPrefab;
    //---------------------------------

    // AIが移動可能かどうか
    bool isAIMove;

    // 各場所が空いているかどうか
    bool[] isAIMoves;

    // 移動先の座標
    GameObject[] aiMovePosObjects;

    // 移動先保存変数
    GameObject aiMovePosObj;

    GameObject eventAlertIcon;

    //----------------------------------

    // Use this for initialization
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

        // ストックの初期値
        stock = defaultStock;
    }

    /// <summary>
    /// AIが呼び出す用のメソッド、指定した数ストック数が減る
    /// </summary>
    /// <param name="decrement"> 一度に減らすストック数 </param>
    public void StockDecrement(int decrement)
    {
        if (stock < 1) return;

        // ストックの減少
        stock -= decrement;

        // ストック数が0を下回ったら
        if (stock < 1)
        {
            // ストック数の下限は0にする
            stock = 0;
            // 補充しないとやばいってのを伝える
            StartCoroutine(EventTimer());
        }
    }

    /// <summary>
    /// 空いているかどうかチェックするメソッド(AI側でアクセス)
    /// </summary>
    /// <returns></returns>
    public bool CheckPos()
    {
        isAIMove = false;

        if (stock > 0)
        {
            for (int i = 0; i < isAIMoves.Length; i++)
            {
                if (!isAIMoves[i])
                {
                    // 移動可能と判断
                    isAIMove = true;
                    break;
                }
            }
        }

        return isAIMove;
    }

    /// <summary>
    /// 空いてるオブジェクトを返す
    /// </summary>
    public GameObject AIMovePosObj()
    {
        for (int i = 0; i < aiMovePosObjects.Length; i++)
        {
            if (!isAIMoves[i])
            {
                isAIMoves[i] = true;
                aiMovePosObj = aiMovePosObjects[i];

                break;
            }
        }

        return aiMovePosObj;
    }

    /// <summary>
    /// AIがイベントを終了したら移動可能にする
    /// </summary>
    /// <param name="movePosObj"></param>
    public void IsAIMoveRelease(GameObject movePosObj)
    {
        for (int i = 0; i < aiMovePosObjects.Length; i++)
        {
            if (movePosObj == aiMovePosObjects[i])
            {
                isAIMoves[i] = false;
            }
        }
    }

    /// <summary>
    /// プレイヤーが補充を行うまでの時間を計測するコルーチン
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// プレイヤーがギミック発動させるときに呼ばれるメソッド
    /// </summary>
    /// <param name="player"> プレイヤーオブジェクト </param>
    public void PlayGimmick(GameObject player)
    {
        if (stock < 1 && Vector3.Distance(player.transform.position, gameObject.transform.position) < 2)
        {
            player.GetComponent<PlayerSystem>().IsEvent = true;
            StartCoroutine(AddStock(player));
        }
    }

    /// <summary>
    /// ストック数を増やすコルーチン、プレイヤーが行動することにより呼び出される
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    IEnumerator AddStock(GameObject player)
    {
        Debug.Log("Addstock");

        float timeInterval = 2;

        StartCoroutine(eventAlertIcon.GetComponent<AlertIconManager>().PlayerActionTimer(timeInterval));
        yield return new WaitForSeconds(timeInterval);

        // 補充完了
        stock = defaultStock;

        Destroy(eventAlertIcon);
        eventAlertIcon = null;

        // プレイヤーが動けるか判断する変数ギミック終了時「false」に設定
        player.GetComponent<PlayerSystem>().IsEvent = false;
    }
}
