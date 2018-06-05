using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;
using System;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Character))]
    public class AI : MonoBehaviour
    {

        public float m_PunchRange = 1.0f;
        public float m_KickRange = 1.0f;

        private Character m_Character;
        private Action m_Action;
        private Branch m_Branch_Primary;
        private Branch m_Branch_Block;
        private Branch m_Branch_Secondary;
        private Branch m_Branch_Jump;
        private Vector3 m_Move;

        private GameObject m_Target;
        private Root m_Tree;


        private void Start()
        {
            
            m_Character = GetComponent<Character>();
            m_Action = GetComponent<Action>();
            m_Branch_Primary = Array.Find(
                GetComponents<Branch>(),
                delegate (Branch b) { return b.GetName() == "B_Punch_Default"; }
                );
            m_Branch_Block = Array.Find(
                GetComponents<Branch>(),
                delegate (Branch b) { return b.GetName() == "B_Block_Default"; }
                );
            m_Branch_Secondary = Array.Find(
                GetComponents<Branch>(),
                delegate (Branch b) { return b.GetName() == "B_Kick_Default"; }
                );
            m_Branch_Jump = Array.Find(
                GetComponents<Branch>(),
                delegate (Branch b) { return b.GetName() == "B_Jump_Default"; }
                );


            FindPlayerCharacter();
            m_Tree = BT.Root();

            m_Tree.OpenBranch(
                BT.If(InRange).OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Call(Punch)
                    )
                ),
                BT.If(OutOfRange).OpenBranch(
                    BT.Sequence().OpenBranch(
                        BT.Call(RunTo)
                    )
                )
            );

        }
        

        private void FindPlayerCharacter()
        {

            foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
                if (character.name == "Neo")
                    m_Target = character;

        }


        private bool InRange()
        {

            float dist = Vector3.Distance(transform.position, m_Target.transform.position);
            return dist <= m_PunchRange;

        }


        private bool OutOfRange()
        {

            return !InRange();

        }


        private void Punch()
        {
            
            m_Branch_Primary.StartAction("Primary", "AnyDirection", "AnySpeed");
            //RunTo();

        }


        private void RunTo()
        {

            m_Action.Rotate(m_Target.transform.position - transform.position);
            m_Action.Move(transform.forward.normalized);

        }


        private void FixedUpdate()
        {

            m_Tree.Tick();

        }

    }
}
