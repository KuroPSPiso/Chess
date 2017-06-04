using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    public Player player;
    public InputField gameCode;

    string currentColour;
    public Button btnColour;
    public Text txtBtnColour;

    // Use this for initialization
    void Start () {
        currentColour = mTurnJSON._WHITECOLOUR;
        setWhite();
        this.txtBtnColour.text = currentColour;
    }

    public void StartGame()
    {
        if(gameCode.text.Length == 0)
        {
            return;
        }

        player.GameCode = gameCode.text;
        player.PlayerColour = mTurnJSON.GetColour(currentColour);

        SceneManager.LoadScene(1);
    }

    public void ChangeColour()
    {
        if(currentColour.Equals(mTurnJSON._WHITECOLOUR))
        {
            setBlack();
        }
        else
        {
            setWhite();
        }
        this.txtBtnColour.text = currentColour;
    }

    void setWhite()
    {
        ColorBlock block = new ColorBlock();
        block.normalColor = Color.white;
        block.highlightedColor = Color.white;
        block.pressedColor = Color.blue;
        block.disabledColor = Color.clear;
        block.colorMultiplier = 1;

        currentColour = mTurnJSON._WHITECOLOUR;
        this.txtBtnColour.color = Color.black;
        this.btnColour.colors = block;
    }

    void setBlack()
    {
        ColorBlock block = new ColorBlock();
        block.normalColor = Color.black;
        block.highlightedColor = Color.black;
        block.pressedColor = Color.blue;
        block.disabledColor = Color.clear;
        block.colorMultiplier = 1;

        currentColour = mTurnJSON._BLACKCOLOUR;
        this.txtBtnColour.color = Color.white;
        this.btnColour.colors = block;
    }
}
