using UnityEngine;
using System.IO;
using UnityStandardAssets.Characters.ThirdPerson;


public class Action : MonoBehaviour
{

    [System.Serializable]
    public class MoveData
    {

        public string Stand;
        public string Walk;
        public string Run;

    }

    [System.Serializable]
    public class ActionData
    {

        public string name;
        public MoveData animations;
        public bool rotation;
        public bool movement;
        public bool blendlegs;
        public Vector3 velocity;
        public float speed;
        public float damage;

    }

    [System.Serializable]
    public class ActionTable
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


    void Start()
    {

        m_Character = GetComponent<Character>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_AnimatorOverride = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
        m_Animator.runtimeAnimatorController = m_AnimatorOverride;

        LoadActionData();

    }


    public bool IsReset()
    {

        return action.name == "Default";

    }


    private void LoadActionData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        table = JsonUtility.FromJson<ActionTable>(File.ReadAllText(filePath));

    }


    public void StartAction(string actionName)
    {

        foreach (ActionData data in table.actions)
        {
            if (data.name == actionName)
            {
                action = data;
                break;
            }
        }


        clip = Resources.Load("Animations/" + action.animations.Stand, typeof(AnimationClip)) as AnimationClip;
        m_AnimatorOverride["neo_reference_skeleton|Action_Default"] = clip;

        m_Animator.Play("Action", 1, 0);
        m_Animator.Play("Action", 2, 0);

        m_Animator.SetFloat("ActionSpeed", action.speed);

    }


    public void ResetAction()
    {

        StartAction("Default");

    }


    public void Rotate(Vector3 m_CamForward, float m_TurnSmoothing)
    {

        if (action.rotation)
            m_Character.Rotate(m_CamForward, m_TurnSmoothing);

    }


    public void Move(Vector3 m_Move, float m_MaxSpeed, bool crouch, bool m_Jump)
    {

        var rotation = m_Rigidbody.transform.rotation;
        var actionVelocity = rotation * action.velocity;

        for (int i = 0; i < 3; i++)
            if (actionVelocity[i] != 0)
                m_Move[i] = actionVelocity[i];


        if (action.movement)
            m_Character.Move(m_Move, m_MaxSpeed, crouch, m_Jump);

    }


    public void ApplyDamage(string bone)
    {

        var collider = m_Character.GetBoneCollider(bone);
        var contacts = collider.GetComponent<Hitbox>().GetContacts();

        foreach (Collider c in contacts)
            c.GetComponent<Character>().ReceiveDamage(action.damage);

    }


    void Update()
    {

        if (action.name != "Default")
        {
            if (action.blendlegs && m_Rigidbody.velocity.magnitude > 0.1f)
            {
                m_Animator.SetLayerWeight(1, 0.0f);
                m_Animator.SetLayerWeight(2, 1.0f);
            }
            else
            {
                m_Animator.SetLayerWeight(1, 1.0f);
                m_Animator.SetLayerWeight(2, 0.0f);
            }
        }
        else
        {
            m_Animator.SetLayerWeight(1, 0.0f);
            m_Animator.SetLayerWeight(2, 0.0f);
        }

    }


}