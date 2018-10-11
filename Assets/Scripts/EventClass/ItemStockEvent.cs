using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStockEvent : MonoBehaviour, IGimmick
{
    // イベントごとに時間を決める
    [SerializeField, Header("Time")]
    float eventTimer = 20;

    // 最大ストック数、初期値
    const int defaultStock = 10;

    // 現在のストック数
    int stock;

    [SerializeField, Header("Icon")]
    GameObject eventAlertIconPrefab;

    // プレイヤーがギミックを動かしているか判断するフラグ
    bool isEvent;

    // Use this for initialization
    void Start()
    {
        stock = defaultStock;
    }

    // Update is called once per frame
    void Update()
    {
        // テスト用インプット
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StockDecrement(5);
        }
    }

    /// <summary>
    /// AIが呼び出す用のメソッド、指定した数ストック数が減る
    /// </summary>
    /// <param name="decrement"> 一度に減らすストック数 </param>
    public void StockDecrement(int decrement)
    {
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

        Debug.Log("Stock : " + stock);
    }


    /// <summary>
    /// プレイヤーが補充を行うまでの時間を計測するコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator EventTimer()
    {
        GameObject eventAlertIcon = Instantiate(eventAlertIconPrefab, transform.position + new Vector3(0, 2.5f, 0), Quaternion.Euler(60, 0, 0));

        float timer = 0;

        // 指定した時間までタイマーを加算する、またプレイヤーが行動を始めた場合もタイマーを止める
        while (timer < eventTimer)
        {
            timer += Time.deltaTime;

            if (timer > eventTimer)
            {
                timer = eventTimer;
            }

            if (isEvent)
            {
                break;
            }
            eventAlertIcon.transform.GetChild(1).GetComponent<Image>().fillAmount = timer / eventTimer;
            
            yield return null;
        }

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
        StartCoroutine(AddStock(player));
    }

    /// <summary>
    /// ストック数を増やすコルーチン、プレイヤーが行動することにより呼び出される
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    IEnumerator AddStock(GameObject player)
    {
        Debug.Log("Addstock");

        // プレイヤーが行動を開始
        isEvent = true;

        // タイマー、時間を図る変数
        float timer = 0;

        // 補充にかかる時間
        float addTimer = 2;

        // 補充までにかかる時間
        // 補充アニメーションもここで流す
        while (timer < addTimer)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        // 補充完了
        stock = defaultStock;

        // プレイヤーが動けるか判断する変数ギミック終了時「false」に設定
        player.GetComponent<PlayerSystem>().IsEvent = false;

        // プレイヤーが行動を終了
        isEvent = false;
    }
}
