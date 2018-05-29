using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Character))]
    public class Agent : MonoBehaviour
    {

        [SerializeField] float m_MaxSpeed = 1f;

        private Character m_Character;
        private Action m_Action;
        private Vector3 m_Move;
        private bool m_Jump;


        private void Start()
        {
            
            m_Character = GetComponent<Character>();
            m_Action = GetComponent<Action>();

        }


        private void Update()
        {
            
        }


        private void FixedUpdate()
        {

            m_Action.Move(new Vector3(0, 0, 0), m_MaxSpeed, false, false);

        }
    }
}
