using UnityEngine;
using System.IO;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEditor;

public class Action : MonoBehaviour
{

    [System.Serializable]
    public class ActionData
    {

        public string name;
        public string animation;
        public bool rotation;
        public bool movement;
        public bool blendlegs;
        public Vector3 velocity;
        public float speed;
        public float damage;
        public string react_hit;
        public string react_ko;

    }

    [System.Serializable]
    public class ActionTable
    {
        public ActionData[] actions;
    }

    [SerializeField] float m_BlendTime = 1.0f;

    private Character m_Character;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private AnimatorOverrideController m_AnimatorOverride;
    private ActionTable table;
    private ActionData action;
    private int slot = 1;
    private string filename = "Actions.json";


    void Start()
    {

        m_Character = GetComponent<Character>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_AnimatorOverride = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
        m_Animator.runtimeAnimatorController = m_AnimatorOverride;

        LoadActionData();
        ResetAction();

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


    private void LoadSlotAnimation(string currentslot, string newslot)
    {
        
        m_AnimatorOverride[currentslot].events = new AnimationEvent[0];

        AnimationClip clip_src = Resources.Load(
            "Animations/" + action.animation, typeof(AnimationClip)) as AnimationClip;
        AnimationClip clip_new = new AnimationClip();
        EditorUtility.CopySerialized(clip_src, clip_new);

        m_AnimatorOverride[newslot] = clip_new;
        
    }


    public void StartAction(string actionName)
    {
        
        string currentName;
        
        if (action == null)
            currentName = "Action_Default";
        else
            currentName = action.animation;


        foreach (ActionData data in table.actions)
        {
            if (data.name == actionName)
            {
                action = data;
                break;
            }
        }


        if (!action.movement)
        {
            var velocity = m_Rigidbody.velocity;
            m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        }


        var rotation = m_Rigidbody.transform.rotation;
        var actionVelocity = rotation * action.velocity;

        m_Rigidbody.velocity = actionVelocity;

        /*for (int i = 0; i < 3; i++)
            if (actionVelocity[i] != 0)
                m_Rigidbody.velocity[i] = actionVelocity[i];*/
        

        if (action.name != "Default")
        {

            m_Animator.SetInteger("ActionSlot", -1 * m_Animator.GetInteger("ActionSlot"));

            if (m_Animator.GetInteger("ActionSlot") == 1)
                LoadSlotAnimation("neo_reference_skeleton|Action_Slot2", "neo_reference_skeleton|Action_Slot1");
            else
                LoadSlotAnimation("neo_reference_skeleton|Action_Slot1", "neo_reference_skeleton|Action_Slot2");

            m_Animator.SetFloat("ActionSpeed", action.speed);

        }

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

        if (action.movement)
            m_Character.Move(m_Move, m_MaxSpeed, crouch, m_Jump);

    }


    public void ApplyDamage(string bone)
    {
        
        var collider = m_Character.GetBoneCollider(bone);
        var contacts = collider.GetComponent<Hitbox>().GetContacts();

        foreach (Collider c in contacts)
            c.GetComponent<Character>().ReceiveDamage(action.damage, action.react_hit, action.react_ko);

    }


    private void BlendLayer(int layer, int amount)
    {

        float weight = m_Animator.GetLayerWeight(layer);

        m_Animator.SetLayerWeight(
                    layer,
                    Mathf.Clamp(weight + amount * m_BlendTime * Time.deltaTime, 0.0f, 1.0f)
                    );

    }


    void Update()
    {
        
        if (action.name != "Default")
        {
            if (action.blendlegs && m_Rigidbody.velocity.magnitude > 0.1f)
            {
                BlendLayer(1, -1);
                BlendLayer(2, 1);
            }
            else
            {
                BlendLayer(1, 1);
                BlendLayer(2, -1);
            }
        }
        else
        {
            BlendLayer(1, -1);
            BlendLayer(2, -1);
        }
        
    }


}