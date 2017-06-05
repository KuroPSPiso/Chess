using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mMovementJSON : JSONType
{
    int pawnId;
    int posX;
    int posY;
    int? pawnIdToDestroy;

    public mMovementJSON(
        int pawnId,
        int posX,
        int posY,
        int? pawnIdToDestroy = null
    )
    {
        this.pawnId = pawnId;
        this.posX = posX;
        this.posY = posY;
        this.pawnIdToDestroy = pawnIdToDestroy;
    }

    public object fromJson(string json)
    {
        return JsonUtility.FromJson<mMovementJSON>(json);
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
