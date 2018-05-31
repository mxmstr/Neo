using UnityEngine;


namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class Character : MonoBehaviour
	{
        [SerializeField] float m_MoveDamping = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

        float m_Health;
        float m_Lives;

        Animator m_Animator;
        Rigidbody m_Rigidbody;
        CapsuleCollider m_Capsule;
        BoxCollider[] m_Colliders;
        Action m_Action;
        
        bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
        Vector3 m_Direction;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;


        void Start()
		{

            m_Health = 100f;

            m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
            m_Colliders = GetComponentsInChildren<BoxCollider>();
            
            m_Action = GetComponent<Action>();
            m_Direction = new Vector3(0, 0, 0);
            m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
            
        }
        

        public BoxCollider GetBoneCollider(string bone)
        {

            foreach (BoxCollider c in m_Colliders)
                if (c.name == bone)
                    return c;

            return null;

        }


        public void ReceiveDamage(float damage, string react_hit, string react_ko)
        {

            m_Health -= damage;

            if (react_hit != null)
                m_Action.StartAction(react_hit);

        }


        public void Rotate(Vector3 rotation, float smoothing)
        {

            var currentRotation = m_Rigidbody.rotation;

            m_Rigidbody.rotation = Quaternion.Slerp(
                m_Rigidbody.rotation, Quaternion.LookRotation(rotation), smoothing * Time.deltaTime);
            
            
            var forwardA = currentRotation * Vector3.forward;
            var forwardB = m_Rigidbody.rotation * Vector3.forward;
            var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
            var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;
            var angleDiff = Mathf.DeltaAngle(angleA, angleB);

            m_Animator.SetFloat("Turn", angleDiff / 2, 0.1f, Time.deltaTime);

        }


        public void Move(Vector3 direction, float maxSpeed)
		{
            
            m_Direction = Vector3.Lerp(m_Direction, direction, m_MoveDamping * Time.deltaTime);
            m_Direction.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = m_Direction;


            var localDirection = transform.InverseTransformDirection(m_Rigidbody.velocity);
            
            m_Animator.SetFloat("Forward", localDirection.z / maxSpeed, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Sidestep", localDirection.x / maxSpeed, 0.1f, Time.deltaTime);

        }
 

		private void CheckGroundStatus()
		{

			RaycastHit hitInfo;

            #if UNITY_EDITOR
			    Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
            #endif

            bool hit = Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance);
            

            if (hit)// && hitInfo.collider.tag == "Ground")
			{
                //Debug.Log(hitInfo.collider.tag);
                m_Animator.SetBool("OnGround", true);
			}
			else
			{
                m_Animator.SetBool("OnGround", false);
			}

		}


        void Update()
        {
            
            CheckGroundStatus();

        }

    }
}
