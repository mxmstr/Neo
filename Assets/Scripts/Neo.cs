using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (Character))]
    public class Neo : MonoBehaviour
    {
        [SerializeField] float m_Sensitivity = 10f;
        [SerializeField] float m_TurnSmoothing = 1.0f;
        [SerializeField] float m_MaxSpeed = 1f;

        private Character m_Character; // A reference to the ThirdPersonCharacter on the object
        private Rigidbody m_Rigidbody;
        private Action m_Action;
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        
        private void Start()
        {

            if (Camera.main != null)
                m_Cam = GetComponentInChildren<Camera>().transform;
            else
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            
            m_Character = GetComponent<Character>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Action = GetComponent<Action>();

        }


        private void Update()
        {
            if (!m_Jump)
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        
        private void FixedUpdate()
        {

            bool crouch = Input.GetKey(KeyCode.C);
            bool primary = Input.GetKey(KeyCode.Mouse0);
            bool secondary = Input.GetKey(KeyCode.Mouse1);
            float h = 0;
            float v = 0;

            if (m_Action.GetAction().movement)
            {
                h = CrossPlatformInputManager.GetAxis("Horizontal");
                v = CrossPlatformInputManager.GetAxis("Vertical");
            }

            if (m_Action.IsReset())
            {
                if (primary)
                    m_Action.StartAction("Punch1");
                if (secondary)
                    m_Action.StartAction("Kick1");
            }


            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            
            if (m_Cam != null)
                m_Move = v * m_CamForward + h * m_Cam.right;
            else
                m_Move = v * Vector3.forward + h * Vector3.right;

            m_Move = m_Move.normalized * m_MaxSpeed;

            #if !MOBILE_INPUT
	            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
            #endif


            var rotation = m_Rigidbody.transform.rotation;
            var actionVelocity = rotation * m_Action.GetAction().velocity;

            for (int i = 0; i < 3; i++)
                if (actionVelocity[i] != 0)
                    m_Move[i] = actionVelocity[i];


            if (m_Action.GetAction().rotation)
                m_Character.Rotate(m_CamForward, m_TurnSmoothing);

            m_Character.Move(m_Move, m_MaxSpeed, crouch, m_Jump);
            m_Jump = false;

        }
    }
}
