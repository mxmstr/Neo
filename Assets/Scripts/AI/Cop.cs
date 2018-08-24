using UnityEngine;
using BTAI;
using System;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Cop : MonoBehaviour {

        public int m_PunchWeight = 1;
        public int m_KickWeight = 1;
        public int m_StrafeRightWeight = 1;
        public int m_StrafeLeftWeight = 1;
        public int m_StrafeLength = 30;
        
        private Character m_Character;
        private AI m_AI;
        private Root m_Tree;


        void Start() {

            m_Character = GetComponent<Character>();
            m_AI = GetComponent<AI>();
            m_Tree = BT.Root();
            int[] attackWeights = { m_PunchWeight, m_KickWeight, m_StrafeRightWeight, m_StrafeLeftWeight };

            m_Tree.OpenBranch(

                BT.If(m_AI.HasScript).OpenBranch(
                    BT.Call(m_AI.RunScript)
                ),

                BT.If(m_AI.NoScript).OpenBranch(

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
                                    BT.Repeat(m_StrafeLength).OpenBranch(
                                        BT.Call(m_AI.SideStepRight),
                                        BT.Call(m_AI.RotateTo)
                                    ),
                                    BT.Call(m_AI.Stand)
                                ),
                                BT.Sequence().OpenBranch(
                                    BT.Repeat(m_StrafeLength).OpenBranch(
                                        BT.Call(m_AI.SideStepLeft),
                                        BT.Call(m_AI.RotateTo)
                                    ),
                                    BT.Call(m_AI.Stand)
                                )
                            )

                        )

                    ),

                    BT.If(m_AI.OutOfRangeAndRangeTimeOut).OpenBranch(
                        BT.Call(m_AI.Run),
                        BT.Call(m_AI.RotateTo)
                    )

                )

            );

        }


        private void Update()
        {
            
            if (!m_Character.IsActive())
            {
                m_AI.Stand();
                return;
            }
            
            m_Tree.Tick();

        }

    }

}