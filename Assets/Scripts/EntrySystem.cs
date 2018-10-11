using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class EntrySystem : SingletonMonoBehaviour<EntrySystem>
{
    public enum PLAYERNUM
    {
        ONE,
        TWO,
        THREE,
        FOUR
    }

    // 実際は-1スタート
    public static int[] playerNumber = { 0, 1, 2, 3 };

    int playerCount = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCount != 4)
        {
            for (int i = 0; i < 4; i++)
            {
                //各プレイヤーの対応コントローラーを設定する
                if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)i))
                {
                    SetPlayerNumber((PLAYERNUM)i);
                }
            }
        }
    }

    private void SetPlayerNumber(PLAYERNUM _player)
    {
        bool flg = false;

        //登録済みのコントローラーかを調べる
        foreach (int _number in playerNumber)
        {
            //登録済みの場合は、登録できない
            if (_number == (int)_player)
            {
                flg = true;

                Debug.Log("登録できません");
            }
        }

        if (!flg)
        {
            //コントローラー番号を1Pから割り当てる
            playerNumber[playerCount] = (int)_player;

            Debug.Log((PLAYERNUM)playerCount + " Player 登録完了");

            playerCount++;
        }
    }
}
