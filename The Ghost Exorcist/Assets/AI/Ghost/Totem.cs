using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour {
    public int totemID;
    public int ghostID;

    public void setID(int newID){
        totemID = newID;
    }

    public void setGhostID(int newID){
        ghostID = newID;
    }
}
