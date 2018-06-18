using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;


public class HUD : MonoBehaviour {

    public Text m_HealthDisplay;

    private Character m_Character;


	void Start () {

        m_Character = GetComponentInParent<Character>();

	}
	

	void Update () {

        m_HealthDisplay.text = m_Character.GetHealth().ToString();

	}

}
