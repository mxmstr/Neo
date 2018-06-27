using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
#if UNITY_EDITOR

#endif

namespace UnityStandardAssets.Cameras
{
    [ExecuteInEditMode]
    public class NeoCam : PivotBasedCameraRig
    {
        
        [SerializeField] float m_Sensitivity = 10f;
        [SerializeField] private float m_TiltMax = 75f;
        [SerializeField] private float m_TiltMin = 75f;
        [SerializeField] private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
        [SerializeField] private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
        [SerializeField] private float m_RollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField] private bool m_FollowVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
        [SerializeField] private bool m_FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
        [SerializeField] private float m_SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        [SerializeField] private float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] private float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation
        
        private float m_LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
        private float m_CurrentTurnAmount; // How much to turn the camera
        private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
        private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )

        private float m_LookAngle;
        private float m_TiltAngle;
        private const float k_LookDistance = 100f;
        private Vector3 m_PivotEulers;
        private Quaternion m_PivotTargetRot;
        private Quaternion m_TransformTargetRot;
        

        protected override void Awake()
        {
            base.Awake();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        protected void Update()
        {

            HandleRotationMovement();

        }


        protected override void FollowTarget(float deltaTime)
        {
            
            var x = CrossPlatformInputManager.GetAxis("Mouse X");
            var y = CrossPlatformInputManager.GetAxis("Mouse Y");
            
            if (!(deltaTime > 0) || m_Target == null)
                return;
            
            var targetForward = m_Target.forward;
            var targetUp = m_Target.up;

            if (m_FollowVelocity && Application.isPlaying)
            {

                if (targetRigidbody.velocity.magnitude > m_TargetVelocityLowerLimit)
                {
                    targetForward = targetRigidbody.velocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);
            }
            else
            {
                var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z)*Mathf.Rad2Deg;
                if (m_SpinTurnLimit > 0)
                {
                    var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle))/deltaTime;
                    var desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit*0.75f, targetSpinSpeed);
                    var turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
                    if (Application.isPlaying)
                    {
                        m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount,
                                                             ref m_TurnSpeedVelocityChange, turnReactSpeed);
                    }
                    else
                    {
                        m_CurrentTurnAmount = desiredTurnAmount;
                    }
                }
                else
                {
                    m_CurrentTurnAmount = 1;
                }
                m_LastFlatAngle = currentFlatAngle;
            }
            
            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime*m_MoveSpeed);
        }


        private void HandleRotationMovement()
        {

            if (Time.timeScale < float.Epsilon)
                return;
            
            var x = CrossPlatformInputManager.GetAxis("Mouse X");
            var y = CrossPlatformInputManager.GetAxis("Mouse Y");
            
            m_LookAngle += x * m_TurnSpeed;
            
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);
            
            m_TiltAngle -= y * m_TurnSpeed;
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
            
            m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);
            
            m_Pivot.localRotation = m_PivotTargetRot;
            transform.rotation = m_TransformTargetRot;

        }


    }
}
