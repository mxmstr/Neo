using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.IO;
using UnityEditor;

public class Action : MonoBehaviour
{

    [System.Serializable]
    private class ActionData
    {

        public string name;
        public string animation;
        public bool rotation;
        public bool movement;
        public bool blendlegs;
        public Vector3 velocity;
        public float speed;
        public float damage;

    }

    [System.Serializable]
    private class ActionTable
    {
        public ActionData[] actions;
    }

    private Character m_Character;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private AnimatorOverrideController m_AnimatorOverride;
    private ActionTable table;
    private ActionData action;
    private AnimationClip clip;
    private string filename = "Actions.json";


    void Start ()
    {

        m_Character = GetComponent<Character>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();

        m_AnimatorOverride = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
        m_Animator.runtimeAnimatorController = m_AnimatorOverride;

    }


    private void LoadActionData(string actionName)
    {
        
        
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        table = JsonUtility.FromJson<ActionTable>(File.ReadAllText(filePath));
        
        foreach (ActionData data in table.actions)
        {
            if (data.name == actionName)
            {
                action = data;
                return;
            }
        }
        
    }


    public void StartAction(string actionName)
    {

        LoadActionData(actionName);
        
        clip = Resources.Load(action.animation, typeof(AnimationClip)) as AnimationClip;
        m_AnimatorOverride["neo_reference_skeleton|Action_Default"] = clip;

        if (action.name == "Default")
        {
            m_Animator.SetLayerWeight(1, 0.0f);
            m_Animator.SetLayerWeight(2, 0.0f);
        }
        else
        {
            if (action.blendlegs)
                m_Animator.SetLayerWeight(2, 1.0f);
            else
                m_Animator.SetLayerWeight(1, 1.0f);
        }
        
        m_Character.EnableMovement(action.movement);
        m_Character.EnableRotation(action.rotation);
        m_Animator.SetFloat("ActionSpeed", action.speed);
        

    }


    void Update ()
    {

        var rotation = m_Rigidbody.transform.rotation;
        var v1 = rotation * action.velocity;
        var v2 = m_Rigidbody.velocity;

        for (int i = 0; i < 3; i++)
            if (v1[i] != 0)
                v2[i] = v1[i];

        m_Rigidbody.velocity = v2;

    }


}
