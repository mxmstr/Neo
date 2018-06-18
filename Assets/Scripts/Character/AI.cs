using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


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
        private float m_OutOfRangeTime;


        private void Start()
        {
            
            StartCoroutine(LateStart());

        }


        IEnumerator LateStart()
        {

            yield return new WaitForFixedUpdate();

            m_OutOfRangeTime = 0;

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
            
        }


        private void FindPlayerCharacter()
        {

            foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
                if (character.name == "Neo")
                    m_Target = character;

        }


        public void ResetRangeDelay()
        {

            m_OutOfRangeTime = m_OutOfRangeDelay;

        }


        public void DecrementRangeTime()
        {
            
            m_OutOfRangeTime -= Time.deltaTime;
            
        }


        public bool TargetHasHealth()
        {

            return m_Target.GetComponent<Character>().HasHealth();

        }


        public bool TargetHasLives()
        {

            return m_Target.GetComponent<Character>().HasLives();

        }


        public bool RangeTimeOut()
        {

            return m_OutOfRangeTime <= 0;

        }


        public bool RangeTimeIn()
        {

            return !RangeTimeOut();

        }


        public bool InRange()
        {
            
            float dist = Vector3.Distance(transform.position, m_Target.transform.position);
            return dist <= m_PunchRange;

        }


        public bool OutOfRange()
        {

            float dist = Vector3.Distance(transform.position, m_Target.transform.position);
            return dist > m_PunchRange;

        }


        public bool InRangeOrRangeTimeIn()
        {

            return InRange() || RangeTimeIn();

        }


        public bool OutOfRangeAndRangeTimeOut()
        {

            return OutOfRange() && RangeTimeOut();

        }


        public bool ActionPlaying()
        {

            return !m_Action.IsReset();

        }


        public void Punch()
        {

            m_Branch_Primary.StartAction("Primary", "AnyDirection", "AnySpeed");
            
        }


        public void Kick()
        {

            m_Branch_Secondary.StartAction("Secondary", "AnyDirection", "AnySpeed");
            
        }


        public void JumpForward()
        {

            m_Branch_Jump.StartAction("Jump", "Forward", "AnySpeed");

        }


        public void JumpLeft()
        {

            m_Branch_Jump.StartAction("Jump", "Left", "AnySpeed");

        }


        public void JumpRight()
        {

            m_Branch_Jump.StartAction("Jump", "Right", "AnySpeed");

        }


        public void JumpBackward()
        {

            m_Branch_Jump.StartAction("Jump", "Backward", "AnySpeed");

        }


        public void Stand()
        {

            m_Action.Move(new Vector3(0, 0, 0));

        }


        public void Run()
        {

            m_Action.Move(transform.forward.normalized);

        }


        public void SideStepRight()
        {
            
            m_Action.Move(transform.right.normalized);

        }


        public void SideStepLeft()
        {

            m_Action.Move(transform.right.normalized * -1);

        }


        public void RotateTo()
        {
            
            m_Action.Rotate(m_Target.transform.position - transform.position);

        }

    }
}
