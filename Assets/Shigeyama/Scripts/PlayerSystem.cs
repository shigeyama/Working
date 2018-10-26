using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField, Header("Playerのナンバリング")]
    EntrySystem.PLAYERNUM PlayerNum;

    int gamePadNumber;

    Vector3 moveDirection;

    [Header("静止状態からの回転量")]
    [SerializeField]
    [Range(0, 360)]
    float m_StationaryTurnSpeed;

    [Header("動いている状態からの回転量")]
    [SerializeField]
    [Range(0, 360)]
    float m_MovingTurnSpeed;

    Rigidbody rd;

    float m_TurnAmount;

    float m_ForwardAmount;

    float moveSpeed = 7;

    public static Camera PlayerCam;

    GameObject DummyPlayerCam;

    //-----------------------------------------------
    GameObject hitItem;

    GameObject catchItem;

    bool isCatch = false;

    bool isEvent = false;

    Vector3 itemLocalPosition;
    Vector3 itemLocalRotation;

    Vector3 playerColliderSize;
    Vector3 playerColliderCenter;

    // Use this for initialization
    void Start()
    {
        gamePadNumber = EntrySystem.playerNumber[(int)GetPlayerNumber()];

        PlayerCam = Camera.main;

        rd = GetComponent<Rigidbody>();

        DummyPlayerCam = new GameObject();
        DummyPlayerCam.name = "PLAYERCAM_" + gamePadNumber + "_DUMMY";
        DummyPlayerCam.transform.position = PlayerCam.transform.position;
        DummyPlayerCam.transform.eulerAngles = new Vector3(0f, PlayerCam.transform.eulerAngles.y, 0f);
        DummyPlayerCam.transform.parent = PlayerCam.transform;

        playerColliderSize = gameObject.GetComponent<BoxCollider>().size;
        playerColliderCenter = gameObject.GetComponent<BoxCollider>().center;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEvent)
        {
            return;
        }
        ButtonInput();
    }

    void FixedUpdate()
    {
        if (isEvent)
        {
            return;
        }
        StickInput();
    }

    private void ButtonInput()
    {
        // つかむ
        if (GamePad.GetButtonDown(GamePad.Button.A, (GamePad.Index)gamePadNumber))
        {
            Debug.Log("OnButton");
            // アイテムを持っていない場合
            if (!isCatch && hitItem != null && hitItem.GetComponent<IItem>() != null)
            {
                CatchItem(hitItem);
            }
            // 持っている場合
            else if (isCatch)
            {
                ReleaseItem();
            }
        }
        // アクション
        if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)gamePadNumber))
        {
            if (hitItem != null && hitItem.GetComponent<IGimmick>() != null)
            {
                hitItem.GetComponent<IGimmick>().PlayGimmick(gameObject);
            }
        }
        // ポーズ
        if (GamePad.GetButtonDown(GamePad.Button.Start, (GamePad.Index)gamePadNumber))
        {

        }
    }

    private void StickInput()
    {
        //カメラの方向ベクトルを取得
        Vector3 forward = DummyPlayerCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = DummyPlayerCam.transform.TransformDirection(Vector3.right);

        //Axisにカメラの方向ベクトルを掛ける
        moveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, (GamePad.Index)gamePadNumber, true).x * right +
                        GamePad.GetAxis(GamePad.Axis.LeftStick, (GamePad.Index)gamePadNumber, true).y * forward;

        //１以上ならば、正規化(Normalize)をし、1にする
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();

        float RightXAxis = -GamePad.GetAxis(GamePad.Axis.RightStick, (GamePad.Index)gamePadNumber, true).x;
        float RightYAxis = -GamePad.GetAxis(GamePad.Axis.RightStick, (GamePad.Index)gamePadNumber, true).y;

        //ワールド空間での方向をローカル空間に逆変換する
        //※ワールド空間でのカメラは、JoyStickと逆の方向ベクトルを持つため、Inverseをしなければならない
        Vector3 C_move = transform.InverseTransformDirection(moveDirection);

        //アークタンジェントをもとに、最終的になる角度を求める
        m_TurnAmount = Mathf.Atan2(C_move.x, C_move.z);

        //最終的な前方に代入する
        m_ForwardAmount = C_move.z;

        //最終的な前方になるまでの時間を計算する
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);

        //Y軸を最終的な角度になるようにする
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);

        //移動スピードを掛ける
        moveDirection *= moveSpeed * Time.deltaTime;

        moveDirection.y = 0;

        //プレイヤーを移動させる
        rd.MovePosition(transform.position + moveDirection);
    }

    public EntrySystem.PLAYERNUM GetPlayerNumber()
    {
        return PlayerNum;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<IItem>() != null || collider.gameObject.GetComponent<IGimmick>() != null)
        {
            hitItem = collider.gameObject;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        hitItem = null;
    }

    /// <summary>
    /// アイテムをつかむ
    /// </summary>
    private void CatchItem(GameObject itemObject)
    {
        isCatch = true;

        catchItem = itemObject;

        catchItem.transform.parent = transform;

        catchItem.GetComponent<Rigidbody>().useGravity = false;
        catchItem.GetComponent<Rigidbody>().isKinematic = true;
        catchItem.GetComponent<BoxCollider>().enabled = false;

        catchItem.transform.localRotation = Quaternion.Euler(catchItem.GetComponent<IItem>().LocalRotation());
        catchItem.transform.localPosition = catchItem.GetComponent<IItem>().LocalPosition();

        gameObject.GetComponent<BoxCollider>().size = catchItem.GetComponent<IItem>().PlayerColliderSize();
        gameObject.GetComponent<BoxCollider>().center = catchItem.GetComponent<IItem>().PlayerColliderCenter();

        // アイテムの機能を発動(なんとなくプレイヤーを渡しています)
        catchItem.GetComponent<IItem>().PlayItem(gameObject);
    }

    /// <summary>
    /// アイテムを離す
    /// </summary>
    private void ReleaseItem()
    {
        isCatch = false;

        catchItem.GetComponent<Rigidbody>().useGravity = true;
        catchItem.GetComponent<Rigidbody>().isKinematic = false;
        catchItem.GetComponent<BoxCollider>().enabled = true;

        gameObject.GetComponent<BoxCollider>().size = playerColliderSize;
        gameObject.GetComponent<BoxCollider>().center = playerColliderCenter;

        catchItem.transform.parent = null;

        catchItem = null;
    }

    public bool IsEvent
    {
        set { isEvent = value; }
    }

    // アイテムクラスでプレイヤーが持つ際のローカルの座標と回転の値を渡す
    public Vector3 ItemLocalPosition
    {
        set { itemLocalPosition = value; }
    }
    public Vector3 ItemLocalRotation
    {
        set { itemLocalRotation = value; }
    }
}
