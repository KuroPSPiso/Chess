using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mTurnJSON : JSONType
{
    public const string _BLACKCOLOUR = "BLACK";
    public const string _WHITECOLOUR = "WHITE";

    int turnCount;
    string playerColour;

    public mTurnJSON(
        int turnCount,
        string playerColour
    )
    {
        this.turnCount = turnCount;
        this.playerColour = playerColour;
    }

    public static Color GetColour(string playerColour)
    {
        Color colour = Color.white;
        switch(playerColour)
        {
            case _WHITECOLOUR:
                colour = Color.white;
                break;
            case _BLACKCOLOUR:
                colour = Color.black;
                break;
        }

        return colour;
    }

    public static String GetColourString(Color playerColour)
    {
        String colour = _WHITECOLOUR;
        if (playerColour.Equals(Color.black))
        {
            colour = _BLACKCOLOUR;
        }

        return colour;
    }

    public object fromJson(string json)
    {
        return JsonUtility.FromJson<mTurnJSON>(json);
    }

    public string toJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static string toJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
}
