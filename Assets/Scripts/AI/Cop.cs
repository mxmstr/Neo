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

        private AI m_AI;
        private Root m_Tree;


        void Start() {

            m_AI = GetComponent<AI>();
            m_Tree = BT.Root();
            int[] attackWeights = { m_PunchWeight, m_KickWeight, m_StrafeRightWeight, m_StrafeLeftWeight };

            m_Tree.OpenBranch(

                BT.Call(m_AI.DecrementRangeTime),

                    //BT.If(TargetHasLives).OpenBranch(

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

                //),

                BT.If(m_AI.OutOfRangeAndRangeTimeOut).OpenBranch(
                    BT.Call(m_AI.Run),
                    BT.Call(m_AI.RotateTo)
                )

            );

        }


        private void FixedUpdate()
        {

            //try
            //{
                m_Tree.Tick();
                //Debug.Log(m_Tree.Children()[m_Tree.ActiveChild()]);
            //}
            //catch (NullReferenceException e) { }

        }

    }

}