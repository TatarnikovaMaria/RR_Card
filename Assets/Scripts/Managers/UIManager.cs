using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text gameBtnText;
    public void ClickGameBtn()
    {
        if(GameManager.instance.GameStatus == GameStatus.Preparing)
        {
            GameManager.instance.GameStatus = GameStatus.Game;
            gameBtnText.text = "Change value";
        }
        else
        {
            GameManager.instance.ChangeRandomCardValue();
        }
    }
}
