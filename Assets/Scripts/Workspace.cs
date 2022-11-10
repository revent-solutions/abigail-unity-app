using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class WorkSpace
{
    public string id;
    public string Uid;
    public string[] tasks = new string[] { "" };
    public bool[] istasks = new bool[] { false };
    public int signal = 0;
    public bool isPlanner = false;
    public bool isSignaler = false;
    public string planner;
    public string signaler;
}
