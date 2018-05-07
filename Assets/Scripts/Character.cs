using UnityEngine;


namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class Character : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f;
        [SerializeField] float m_MoveDamping = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

        float m_Health;

		Animator m_Animator;
        Rigidbody m_Rigidbody;
        CapsuleCollider m_Capsule;
        BoxCollider[] m_Colliders;
        Action m_Action;
        bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
        float m_SidestepAmount;
        float m_ForwardAmount;
        Vector3 m_Direction;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		
		bool m_Crouching;

        Branch b_Primary;
        Branch b_Secondary;
        Branch b_Jump;


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

            m_Action.ResetAction();

            b_Primary = new Branch("B_Primary");
            b_Secondary = new Branch("B_Secondary");

        }
        

        public BoxCollider GetBoneCollider(string bone)
        {

            foreach (BoxCollider c in m_Colliders)
                if (c.name == bone)
                    return c;

            return null;

        }


        public void ReceiveDamage(float damage)
        {

            m_Health -= damage;
            m_Action.StartAction("React1");

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


        public void Move(Vector3 direction, float maxSpeed, bool crouch, bool jump)
		{
            
            m_Direction = Vector3.Lerp(m_Direction, direction, m_MoveDamping * Time.deltaTime);
            m_Direction.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = m_Direction;


            var localDirection = transform.InverseTransformDirection(m_Rigidbody.velocity);
            
            m_Animator.SetFloat("Forward", localDirection.z / maxSpeed, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Sidestep", localDirection.x / maxSpeed, 0.1f, Time.deltaTime);
            m_Animator.SetBool("Crouch", crouch);
            m_Animator.SetBool("OnGround", !jump);

        }


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}


		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(float forward, float sidestep, float turn, bool crouch, bool jump)
		{
			
			m_Animator.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Sidestep", sidestep, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", crouch);
			m_Animator.SetBool("OnGround", !jump);

			if (!m_IsGrounded)
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);

		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}


		void ApplyExtraTurnRotation()
		{
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}
 

		void CheckGroundStatus()
		{
			RaycastHit hitInfo;

            #if UNITY_EDITOR
			    Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
            #endif

			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = false;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}

	}
}
