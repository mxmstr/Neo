using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Menu : MonoBehaviour {

    private Phase m_Phase;

	void Start () {
        
        m_Phase = GameObject.Find("EventManager").GetComponent<Phase>();

	}


    public void StartGame()
    {
        
        m_Phase.NextPhase();

    }


    public void ExitGame()
    {

        Application.Quit();

    }


    void Update () {
		


	}

}
