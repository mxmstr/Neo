using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour {



    private Phase m_Phase;

	void Start () {
        
        m_Phase = GameObject.Find("EventManager").GetComponent<Phase>();

	}


    public void StartGame()
    {
        
        m_Phase.NextPhase();

    }


    public void ToCredits()
    {

        m_Phase.SetCanvas("Canvases/CreditsMenu");

    }


    public void ToMain()
    {

        m_Phase.SetCanvas("Canvases/MainMenu");

    }


    public void ResetGame()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }


    public void ExitGame()
    {

        Application.Quit();

    }


    void Update () {
		


	}

}
