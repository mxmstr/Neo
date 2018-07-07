using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;


public class HUD : MonoBehaviour {

    public Text m_HealthDisplay;

    private GameObject m_Player;


    void Start()
    {

        StartCoroutine(LateStart());

    }


    IEnumerator LateStart()
    {

        yield return new WaitUntil(delegate ()
        {

            m_Player = Array.Find(
                GameObject.FindGameObjectsWithTag("Character"),
                delegate (GameObject go) { return go.GetComponent<Character>().GetID() == 0; }
                );

            return m_Player != null;

        });
        
    }


    void Update () {

        m_HealthDisplay.text = m_Player.GetComponent<Character>().GetHealth().ToString();

	}

}
