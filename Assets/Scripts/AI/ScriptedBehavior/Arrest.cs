using BTAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class Arrest : ScriptedBehavior {

    private Character m_Character;
    private AI m_AI;
    private Root m_Tree;

    public override void Start()
    {

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


    public override void Tick()
    {
        
        m_Tree.Tick();

        if (Attacked())
            End();

    }

}
