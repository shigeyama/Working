using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardboardItem : MonoBehaviour, IItem
{
    Rigidbody rd;

    // 各アイテムの持った際の座標と回転を設定する変数
    Vector3 catchPosition;
    Vector3 catchRotation;

    // Use this for initialization
    void Start()
    {
        rd = GetComponent<Rigidbody>();

        catchPosition = new Vector3(0, -0.5f, 0.7f);
        catchRotation = new Vector3(0, 90, 0);
    }

    // 段ボール側で何か機能をつける場合はここに書くといいはず
    public void PlayItem(GameObject player)
    {

    }

    public Vector3 LocalPosition()
    {
        return catchPosition;
    }

    public Vector3 LocalRotation()
    {
        return catchRotation;
    }

}
