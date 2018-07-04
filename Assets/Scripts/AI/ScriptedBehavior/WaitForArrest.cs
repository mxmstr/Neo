using BTAI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class WaitForArrest : ScriptedBehavior {

    public int m_ArresterID = 10;

    private AI m_AI;
    private Root m_Tree;
    private GameObject m_Arrester;
    

    public override void Start()
    {

        StartCoroutine(LateStart());

    }


    IEnumerator LateStart()
    {
        
        yield return new WaitUntil(delegate ()
        {

            m_Arrester = Array.Find(
                GameObject.FindGameObjectsWithTag("Character"),
                    delegate (GameObject go) { return go.GetComponent<Character>().GetID() == m_ArresterID; }
                    );

            return m_Arrester != null;

        });

        m_AI = GetComponent<AI>();
        m_Tree = BT.Root();

        m_Tree.OpenBranch(

            BT.Call(m_AI.Stand),
            BT.Call(m_AI.RotateTo)

        );

    }


    public bool ArresterAttacked()
    {

        return !m_Arrester.GetComponent<Character>().HasHealth();

    }


    public override void Tick()
    {
        
        m_Tree.Tick();

        if (ArresterAttacked())
            End();

    }

}
