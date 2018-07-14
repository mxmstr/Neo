using BTAI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class Arrest : ScriptedBehavior {
    
    public float m_PlayerEscapeDist = 3.0f;

    private Character m_Character;
    private AI m_AI;
    private Root m_Tree;
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

            m_Player = Array.Find(
                GameObject.FindGameObjectsWithTag("Character"),
                delegate (GameObject go) { return go.GetComponent<Character>().GetID() == 0; }
                );

            return m_Player != null;

        });

        m_PlayerInitPos = m_Player.transform.position;
        m_Character = GetComponent<Character>();
        m_AI = GetComponent<AI>();
        m_Tree = BT.Root();

        m_Tree.OpenBranch(

            BT.If(m_AI.InRange).OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Call(m_AI.Punch),
                    BT.While(m_AI.ActionPlaying).OpenBranch(
                        BT.Call(m_AI.RotateTo)
                    )
                )
            ),
            BT.If(m_AI.OutOfRange).OpenBranch(
                BT.Call(m_AI.Walk),
                BT.Call(m_AI.RotateTo)
            )

        );

    }


    public bool Attacked()
    {

        return !m_Character.HasHealth();

    }


    public bool PlayerEscaping()
    {

        return (m_Player.transform.position - m_PlayerInitPos).magnitude > m_PlayerEscapeDist;

    }


    public override void Tick()
    {
        
        m_Tree.Tick();

        if (Attacked() || PlayerEscaping())
            End();

    }

}
