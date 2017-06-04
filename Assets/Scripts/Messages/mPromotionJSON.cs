using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mPromotionJSON : JSONType
{
    int pawnId;
    string pawnType;
    
    public mPromotionJSON(
        int pawnId,
        string pawnType
    )
    {
        this.pawnId = pawnId;
        this.pawnType = pawnType;
    }

    public object fromJson(string json)
    {
        return JsonUtility.FromJson<mPromotionJSON>(json);
    }

    public string toJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
}
