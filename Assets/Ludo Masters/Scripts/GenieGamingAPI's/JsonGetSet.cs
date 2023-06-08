using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreateGameJsonGetSet
{
    public string[] players = new string[2];
    public string room_id;
}

[Serializable]
public class EndGameJsonGetSet
{
    public string action;
    public string winner;
}