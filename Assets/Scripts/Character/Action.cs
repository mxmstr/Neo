using UnityEngine;
using System.IO;
using UnityStandardAssets.Characters.ThirdPerson;
//using UnityEditor;
using System;
using System.Reflection;


public class Action : MonoBehaviour
{
    
    [System.Serializable]
    public class PowerData
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(PowerData);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
            set
            {
                GetType().GetField(propertyName).SetValue(this, value);
            }
        }

        public string name;
        public string punch;
        public string kick;
        public string power;
        public string block;
        public string[] react_light;
        public string[] react_heavy;
        public string[] knockout_light;
        public string[] knockout_heavy;
        public string[] knockout_power;
        public string[] death_default;
        public string[] death_power;

    }

    [System.Serializable]
    public class PowerTable
    {
        public PowerData[] powertypes;
    }

    [System.Serializable]
    public class ActionData : ICloneable
    {

        public string name;
        public string animation;
        public bool rotation;
        public bool movement;
        public bool blendlegs;
        public bool invulnerable;
        public Vector3 velocity;
        public float speed;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }

    [System.Serializable]
    public class ActionTable
    {
        public ActionData[] actions;
    }
    
    public string m_ActionSource = "Actions.json";
    public string m_PowerSource = "PowerTypes.json";
    public string m_PowerType = "Normal";
    public float m_LayerBlendTime = 1.0f;
    public float m_SlotBlendTime = 1.0f;

    private Character m_Character;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private AudioSource m_Audio;
    private AnimatorOverrideController m_AnimatorOverride;
    private ActionTable m_ActionTable;
    private ActionData m_ActionData;
    private PowerTable m_PowerTable;
    private PowerData m_PowerData;
    private int slot = 1;
    

    void Start()
    {

        m_Character = GetComponent<Character>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_AnimatorOverride = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
        m_Animator.runtimeAnimatorController = m_AnimatorOverride;

        LoadActionData();
        LoadPowerData();
        ResetAction();

    }


    public bool IsReset()
    {

        return m_ActionData.name == "Default";

    }


    public bool IsInvulnerable()
    {

        return m_ActionData.invulnerable;

    }


    private void LoadActionData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, m_ActionSource);
        m_ActionTable = JsonUtility.FromJson<ActionTable>(File.ReadAllText(filePath));
        
    }


    private void LoadPowerData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, m_PowerSource);
        m_PowerTable = JsonUtility.FromJson<PowerTable>(File.ReadAllText(filePath));

        foreach (PowerData data in m_PowerTable.powertypes)
        {
            if (data.name == m_PowerType)
            {
                m_PowerData = data;
                break;
            }
        }

    }


    public void EnableRotation(string enable)
    {

        m_ActionData.rotation = bool.Parse(enable);

    }


    public void EnableMovement(string enable)
    {

        m_ActionData.movement = bool.Parse(enable);

    }


    public void EnableInvulnerable(string enable)
    {

        m_ActionData.invulnerable = bool.Parse(enable);

    }


    public void ResetVelocity()
    {

        Vector3 v = m_Rigidbody.velocity;
        m_Rigidbody.velocity = new Vector3(0, v.y, 0);

    }


    public void SetStateFrame(string args)
    {

        string state = args.Split(' ')[0];
        int layer = int.Parse(args.Split(' ')[1]);
        float frame = float.Parse(args.Split(' ')[2]);

        m_Animator.Play(state, layer, frame);

    }


    private void LoadSlotAnimation(string currentslot, string newslot)
    {
        
        m_AnimatorOverride[currentslot].events = new AnimationEvent[0];

        AnimationClip clip_src = Resources.Load(
            "Animations/" + m_ActionData.animation, typeof(AnimationClip)) as AnimationClip;
        AnimationClip clip_new = Instantiate(clip_src);
        //new AnimationClip();
        //EditorUtility.CopySerialized(clip_src, clip_new);
        

        /*FieldInfo[] fields = typeof(AnimationClip).GetFields(); 
        foreach (FieldInfo field in fields)
            field.SetValue(clip_new, field.GetValue(clip_src));*/


        m_AnimatorOverride[newslot] = clip_new;

    }


    public void StartAction(string actionName)
    {

        foreach (ActionData data in m_ActionTable.actions)
        {
            if (data.name == actionName)
            {
                m_ActionData = (Action.ActionData)data.Clone();
                break;
            }
        }
        
        if (m_ActionData.name != "Default")
        {

            var rotation = m_Rigidbody.transform.rotation;
            var actionVelocity = rotation * m_ActionData.velocity;
            m_Rigidbody.velocity = actionVelocity;

            slot++;
            if (slot > 3)
                slot = 1;

            switch (slot)
            {

                case 1:
                    LoadSlotAnimation("neo_reference_skeleton|Action_Slot3", "neo_reference_skeleton|Action_Slot1");
                    m_Animator.CrossFade("Action1", m_SlotBlendTime, 1);
                    m_Animator.CrossFade("Action1", m_SlotBlendTime, 2);
                    break;
                case 2:
                    LoadSlotAnimation("neo_reference_skeleton|Action_Slot1", "neo_reference_skeleton|Action_Slot2");
                    m_Animator.CrossFade("Action2", m_SlotBlendTime, 1);
                    m_Animator.CrossFade("Action2", m_SlotBlendTime, 2);
                    break;
                case 3:
                    LoadSlotAnimation("neo_reference_skeleton|Action_Slot2", "neo_reference_skeleton|Action_Slot3");
                    m_Animator.CrossFade("Action3", m_SlotBlendTime, 1);
                    m_Animator.CrossFade("Action3", m_SlotBlendTime, 2);
                    break;

            }
            
            m_Animator.SetFloat("ActionSpeed", m_ActionData.speed);
            m_Animator.SetBool("ActionPlaying", true);

        }
        else
            m_Animator.SetBool("ActionPlaying", false);

    }


    public void ResetAction()
    {
        
        StartAction("Default");

    }


    public void Rotate(Vector3 rotation)
    {

        if (m_ActionData.rotation)
            m_Character.SetRotateTarget(rotation);

    }


    public void Move(Vector3 move)
    {

        if (m_ActionData.movement)
            m_Character.SetMoveTarget(move);
        else
            m_Character.SetMoveTarget(new Vector3(0, 0, 0));

        if (m_ActionData.blendlegs)
        {
            if (m_Rigidbody.velocity.magnitude < 0.1f)
                m_Animator.SetFloat("MoveSpeed", 1.0f);
        }
        else
            m_Animator.SetFloat("MoveSpeed", 0.0f);

    }


    public void Recover(string actionName)
    {

        m_Character.ResetHealth();

        if (m_Character.HasHealth())
            StartAction(actionName);
        
    }


    public void ApplyDamage(string bone)
    {
        
        /*var collider = m_Character.GetBoneCollider(bone);
        var contacts = collider.GetComponent<Hitbox>().GetContacts();

        foreach (Collider c in contacts)
            c.GetComponent<Character>().ReceiveDamage(action.damage, action.react_hit, action.react_ko);
            */

    }


    public void FireProjectile(string args)
    {
        
        string bone = args.Split(' ')[0];
        string projectileName = args.Split(' ')[1];
        
        Collider collider = m_Character.GetBoneCollider(bone);
        GameObject obj = Instantiate(
            Resources.Load((string)m_PowerData[projectileName]),
            transform
            //gameObject
            ) as GameObject;

        obj.GetComponent<Projectile>().SetShooter(GetComponent<CapsuleCollider>());
        obj.transform.position = collider.transform.position;
        obj.transform.rotation = m_Rigidbody.transform.rotation;

    }
    

    public void ReceiveDamage(float damage, Vector3 direction, string react_hit, string react_ko)
    {

        if (m_ActionData.invulnerable)
            return;

        m_Character.ReceiveDamage(damage);

        try
        {

            System.Random rand = new System.Random();

            if (m_Character.HasHealth())
            {
                string[] reactions = (string[])m_PowerData[react_hit];
                StartAction(reactions[rand.Next(reactions.Length)]);
            }
            else
            {
                string[] reactions = (string[])m_PowerData[react_ko];
                StartAction(reactions[rand.Next(reactions.Length)]);
                m_Rigidbody.rotation = Quaternion.LookRotation(direction * -1);
            }
            
        }
        catch (NullReferenceException e) { }

    }


    private void BlendLayer(int layer, int amount)
    {

        float weight = m_Animator.GetLayerWeight(layer);

        m_Animator.SetLayerWeight(
                    layer,
                    Mathf.Clamp(weight + amount * m_LayerBlendTime * Time.deltaTime, 0.0f, 1.0f)
                    );

    }


    private void FixedUpdate()
    {

        m_Character.Rotate();
        
        if (m_ActionData.velocity.magnitude == 0)
        {
            m_Character.Move();
        }

    }


    void Update()
    {

        if (m_ActionData.name != "Default")
        {
            if (m_ActionData.blendlegs && m_Rigidbody.velocity.magnitude > 0.1f)
            {
                //m_Animator.SetFloat("MoveSpeed", 1.0f);
                BlendLayer(1, -1);
                BlendLayer(2, 1);
            }
            else
            {
                //m_Animator.SetFloat("MoveSpeed", 0.0f);
                BlendLayer(1, 1);
                BlendLayer(2, -1);
            }
        }
        else
        {
            //m_Animator.SetFloat("MoveSpeed", 1.0f);
            BlendLayer(1, -1);
            BlendLayer(2, -1);
        }
        
    }


}