﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface JSONType {

    string toJson();
    object fromJson(string json);

}
