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
        
        public float m_WalkTimeMax = 0.25f;

        private Character m_Character;
        private Rigidbody m_Rigidbody;
        private Action m_Action;
        private Branch m_Branch;
        private Transform m_Cam;
        private Vector3 m_CamForward;
        private Vector3 m_Move;
        private bool m_Jump;
        private float m_WalkTime = 0;


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
            m_Branch = GetComponent<Branch>();

        }

        
        private string GetDirection(float h, float v)
        {
            
            bool right = h >= 0.5;
            bool left = h <= -0.5;
            bool forward = v >= 0.5;
            bool backward = v <= -0.5;


            if (forward && !right && !left)
                return "Forward";
            else if (forward && right && !left)
                return "ForwardRight";
            else if (forward && !right && left)
                return "ForwardLeft";
            else if (backward && !right && !left)
                return "Backward";
            else if (backward && right && !left)
                return "BackwardRight";
            else if (backward && !right && left)
                return "BackwardLeft";
            else if (right && !forward && !backward)
                return "Right";
            else if (left && !forward && !backward)
                return "Left";

            return "Stand";

        }


        private string GetSpeed(float h, float v)
        {

            bool moving = ((Mathf.Abs(h) + Mathf.Abs(v)) / 2) > 0;

            if (moving)
            {
                m_WalkTime += Time.deltaTime;

                if (m_WalkTime < m_WalkTimeMax)
                    return "Walk";

                return "Run";
            }

            m_WalkTime = 0;

            return "Stand";

        }


        private void InputAction()
        {
            
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool primary = Input.GetKey(KeyCode.Mouse0);
            bool secondary = Input.GetKey(KeyCode.Mouse1);

            string direction = GetDirection(h, v);
            string speed = GetSpeed(h, v);
            

            if (primary)
                m_Branch.StartAction("Primary", direction, speed);
            else if (secondary)
                m_Branch.StartAction("Secondary", direction, speed);

        }


        private void InputMovement()
        {

            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;

            if (m_Cam != null)
                m_Move = v * m_CamForward + h * m_Cam.right;
            else
                m_Move = v * Vector3.forward + h * Vector3.right;

            m_Move = m_Move.normalized * m_MaxSpeed;

            #if !MOBILE_INPUT
                        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
            #endif


            m_Action.Rotate(m_CamForward, m_TurnSmoothing);
            m_Action.Move(m_Move, m_MaxSpeed, crouch, m_Jump);

            m_Jump = false;

        }


        private void Update()
        {
            if (!m_Jump)
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }


        private void FixedUpdate()
        {
            
            InputAction();
            InputMovement();

        }
    }
}
