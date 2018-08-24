using System;
using System.IO;
using UnityEngine;
using UnityStandardAssets.Cameras;

namespace UnityStandardAssets.Characters.ThirdPerson
{

    public enum Group
    {
        Red, Blue
    };

	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class Character : MonoBehaviour
	{

        [SerializeField] bool m_Active = true;
        [SerializeField] int m_ID = 0;
        [SerializeField] Group m_Group = Group.Red;
        [SerializeField] float m_MaxHealth = 1.0f;
        [SerializeField] int m_Lives = 2;
        [SerializeField] float m_MaxSpeed = 1f;
        [SerializeField] float m_AnimTransitionMultiplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_TurnSmoothing = 1.0f;
        [SerializeField] float m_MoveDamping = 1f;
        [SerializeField] float m_GroundCheckDistance = 0.1f;
        

        Animator m_Animator;
        Rigidbody m_Rigidbody;
        CapsuleCollider m_Capsule;
        BoxCollider[] m_Colliders;
        Action m_Action;
        GameObject m_CameraRig;

        float m_Health;
        public float m_SpeedMultiplier;
        bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
        Vector3 m_Direction;
        Vector3  m_MoveTarget;
        Vector3 m_RotateTarget;
        float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;


        private void Awake()
        {

            m_Health = m_MaxHealth;
            m_SpeedMultiplier = 1.0f;

            if (GetComponentInChildren<NeoCam>() != null)
                m_CameraRig = GetComponentInChildren<NeoCam>().gameObject;

        }


        void Start()
		{

            m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
            m_Colliders = GetComponentsInChildren<BoxCollider>();
            m_Action = GetComponent<Action>();

            m_Direction = new Vector3(0, 0, 0);
            m_MoveTarget = new Vector3(0, 0, 0);
            m_RotateTarget = new Vector3(0, 0, 1);
            m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
            
        }


        public void SetID(int id)
        {

            m_ID = id;

        }


        public int GetID()
        {

            return m_ID;

        }


        public void SetSpeedMultiplier(float mult)
        {

            m_SpeedMultiplier = mult;

        }


        public float GetMaxSpeed()
        {

            return m_MaxSpeed;

        }


        public void SetActive(bool active)
        {

            m_Active = active;

        }


        public bool IsActive()
        {

            return m_Active;

        }


        public Group GetGroup()
        {

            return m_Group;

        }


        public float GetHealth()
        {

            return m_Health / m_MaxHealth;

        }


        public bool HasHealth()
        {

            return m_Health > 0;

        }


        public bool HasLives()
        {

            return m_Lives > 0;

        }


        public void ResetHealth()
        {
            
            m_Lives--;

            if (HasLives())
                m_Health = m_MaxHealth;

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

        }


        public void SetCameraRotation(float look)
        {

            if (m_CameraRig != null)
                m_CameraRig.GetComponent<NeoCam>().SetLookAngle(look);//.transform.rotation = //Quaternion.Euler(rotation);

        }


        public void SetRotateTarget(Vector3 rotation)
        {

            m_RotateTarget = rotation;

        }


        public void Rotate()
        {

            var currentRotation = m_Rigidbody.rotation;

            m_Rigidbody.rotation = Quaternion.Slerp(
                m_Rigidbody.rotation, 
                Quaternion.LookRotation(m_RotateTarget), 
                m_TurnSmoothing * Time.deltaTime
                );
            
            
            var forwardA = currentRotation * Vector3.forward;
            var forwardB = m_Rigidbody.rotation * Vector3.forward;
            var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
            var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;
            var angleDiff = Mathf.DeltaAngle(angleA, angleB);

            m_Animator.SetFloat("Turn", angleDiff / 2, 0.1f, Time.deltaTime);

        }


        public void SetMoveTarget(Vector3 direction)
        {

            m_MoveTarget = direction;

        }


        public void Move()
		{
            
            m_Direction = Vector3.Lerp(
                m_Direction, m_MoveTarget * m_MaxSpeed * m_SpeedMultiplier, m_MoveDamping * Time.deltaTime);
            m_Direction.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = m_Rigidbody.transform.rotation * m_Direction;


            Vector3 localDirection = transform.InverseTransformDirection(m_Rigidbody.velocity);
            
            m_Animator.SetFloat("Forward", localDirection.z * m_AnimTransitionMultiplier, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Sidestep", localDirection.x * m_AnimTransitionMultiplier, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("MoveSpeed", localDirection.magnitude * m_AnimSpeedMultiplier);

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
