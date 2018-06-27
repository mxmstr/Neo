using UnityEngine;
using BTAI;
using System;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Swat : MonoBehaviour
    {

        public int m_AttackPunchWeight = 1;
        public int m_AttackKickWeight = 1;
        public int m_AttackJumpRightWeight = 1;
        public int m_AttackJumpLeftWeight = 1;
        public int m_RunWeight = 1;
        public int m_RunRollWeight = 1;

        private Character m_Character;
        private AI m_AI;
        private Root m_Tree;


        void Start()
        {

            m_Character = GetComponent<Character>();
            m_AI = GetComponent<AI>();
            m_Tree = BT.Root();
            int[] attackWeights = { m_AttackPunchWeight, m_AttackKickWeight, m_AttackJumpRightWeight, m_AttackJumpLeftWeight };
            int[] jumpWeights = { m_RunWeight, m_RunRollWeight };

            m_Tree.OpenBranch(

                BT.Call(m_AI.DecrementRangeTime),

                BT.If(m_AI.InRangeOrRangeTimeIn).OpenBranch(

                    BT.If(m_AI.TargetHasHealth).OpenBranch(

                        BT.If(m_AI.InRange).OpenBranch(
                            BT.Call(m_AI.ResetRangeDelay)
                        ),
                        BT.RandomSequence(attackWeights).OpenBranch(
                            BT.Sequence().OpenBranch(
                                BT.Call(m_AI.Punch),
                                BT.While(m_AI.ActionPlaying).OpenBranch(
                                    BT.Call(m_AI.RotateTo)
                                )
                            ),
                            BT.Sequence().OpenBranch(
                                BT.Call(m_AI.Kick),
                                BT.While(m_AI.ActionPlaying).OpenBranch(
                                    BT.Call(m_AI.RotateTo)
                                )
                            ),
                            BT.Sequence().OpenBranch(
                                BT.Call(m_AI.JumpLeft)
                            ),
                            BT.Sequence().OpenBranch(
                                BT.Call(m_AI.JumpRight)
                            )
                        )

                    )

                ),

                BT.If(m_AI.OutOfRangeAndRangeTimeOut).OpenBranch(
                    BT.RandomSequence(jumpWeights).OpenBranch(
                        BT.Sequence().OpenBranch(
                            BT.Call(m_AI.Run),
                            BT.Call(m_AI.RotateTo)
                        ),
                        BT.Call(m_AI.JumpForward)
                    )
                )

            );

        }


        private void FixedUpdate()
        {
            
            if (!m_Character.IsActive())
                return;

            m_Tree.Tick();

        }

    }

}