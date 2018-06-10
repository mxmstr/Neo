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
        public float m_OutOfRangeDelay = 1.0f;

        private Character m_Character;
        private Action m_Action;
        private Branch m_Branch_Primary;
        private Branch m_Branch_Block;
        private Branch m_Branch_Secondary;
        private Branch m_Branch_Jump;
        private Vector3 m_Move;

        private GameObject m_Target;
        private Root m_Tree;
        private float m_OutOfRangeTime;


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
            m_OutOfRangeTime = 0;
            int[] attackWeights = {5, 10, 1, 1};

            m_Tree.OpenBranch(

                BT.Call(DecrementRangeTime),

                BT.If(InRangeOrRangeTimeIn).OpenBranch(

                    BT.If(InRange).OpenBranch(
                        BT.Call(ResetRangeDelay)
                    ),
                    BT.RandomSequence(attackWeights).OpenBranch(
                        BT.Sequence().OpenBranch(
                            BT.Call(Punch),
                            BT.While(ActionPlaying).OpenBranch(
                                //BT.Call(Run),
                                BT.Call(RotateTo)
                            )
                        ),
                        BT.Sequence().OpenBranch(
                            BT.Call(Kick),
                            BT.While(ActionPlaying).OpenBranch(
                                //BT.Call(Run),
                                BT.Call(RotateTo)
                            )
                        ),
                        BT.Sequence().OpenBranch(
                            BT.Repeat(30).OpenBranch(
                                BT.Call(SideStepRight),
                                BT.Call(RotateTo)
                            ),
                            BT.Call(Stand)
                        ),
                        BT.Sequence().OpenBranch(
                            BT.Repeat(30).OpenBranch(
                                BT.Call(SideStepLeft),
                                BT.Call(RotateTo)
                            ),
                            BT.Call(Stand)
                        )
                    )

                ),

                BT.If(OutOfRangeAndRangeTimeOut).OpenBranch(
                    BT.Call(Run),
                    BT.Call(RotateTo)
                )

            );

        }


        private void FindPlayerCharacter()
        {

            foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
                if (character.name == "Neo")
                    m_Target = character;

        }


        private void ResetRangeDelay()
        {

            m_OutOfRangeTime = m_OutOfRangeDelay;

        }


        private void DecrementRangeTime()
        {
            
            m_OutOfRangeTime -= Time.deltaTime;
            
        }


        private bool RangeTimeOut()
        {

            return m_OutOfRangeTime <= 0;

        }


        private bool RangeTimeIn()
        {

            return !RangeTimeOut();

        }


        private bool InRange()
        {
            
            float dist = Vector3.Distance(transform.position, m_Target.transform.position);
            return dist <= m_PunchRange;

        }


        private bool OutOfRange()
        {

            float dist = Vector3.Distance(transform.position, m_Target.transform.position);
            return dist > m_PunchRange;

        }


        private bool InRangeOrRangeTimeIn()
        {

            return InRange() || RangeTimeIn();

        }


        private bool OutOfRangeAndRangeTimeOut()
        {

            return OutOfRange() && RangeTimeOut();

        }


        private bool ActionPlaying()
        {

            return !m_Action.IsReset();

        }


        private void Punch()
        {

            m_Branch_Primary.StartAction("Primary", "AnyDirection", "AnySpeed");
            
        }


        private void Kick()
        {

            m_Branch_Secondary.StartAction("Secondary", "AnyDirection", "AnySpeed");
            
        }


        private void Stand()
        {

            m_Action.Move(new Vector3(0, 0, 0));

        }


        private void Run()
        {

            m_Action.Move(transform.forward.normalized);

        }


        private void SideStepRight()
        {
            
            m_Action.Move(transform.right.normalized);

        }


        private void SideStepLeft()
        {

            m_Action.Move(transform.right.normalized * -1);

        }


        private void RotateTo()
        {
            
            m_Action.Rotate(m_Target.transform.position - transform.position);

        }


        private void FixedUpdate()
        {

            m_Tree.Tick();
            //Debug.Log(m_Tree.Children()[m_Tree.ActiveChild()]);

        }

    }
}
