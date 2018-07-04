using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ScriptedBehavior : MonoBehaviour {
    
    
    public void End()
    {

        DestroyImmediate(this);

    }


    public abstract void Start();

    
    abstract public void Tick();
    

}
