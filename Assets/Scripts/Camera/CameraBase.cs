using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraBase : MonoBehaviour {

    [SerializeField] float m_Timer = 0;


    void Start () {
		
	}


    public void SetTimer(float time)
    {

        m_Timer = time;

    }

    public bool IsTimedOut()
    {

        return m_Timer <= 0;

    }
	

	void Update () {

        m_Timer -= Time.deltaTime;

	}

    
}
