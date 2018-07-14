using BTAI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class WaitForArrest : ScriptedBehavior {

    public int m_ArresterID = 10;
    public float m_PlayerEscapeDist = 3.0f;

    private AI m_AI;
    private Root m_Tree;
    private GameObject m_Arrester;
    private GameObject m_Player;
    private Vector3 m_PlayerInitPos;


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

        yield return new WaitUntil(delegate ()
        {

            m_Player = Array.Find(
                GameObject.FindGameObjectsWithTag("Character"),
                delegate (GameObject go) { return go.GetComponent<Character>().GetID() == 0; }
                );

            return m_Player != null;

        });

        m_PlayerInitPos = m_Player.transform.position;
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


    public bool PlayerEscaping()
    {

        return (m_Player.transform.position - m_PlayerInitPos).magnitude > m_PlayerEscapeDist;

    }


    public override void Tick()
    {
        
        m_Tree.Tick();

        if (ArresterAttacked() || PlayerEscaping())
            End();

    }

}
