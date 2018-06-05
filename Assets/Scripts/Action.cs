using UnityEngine;
using System.IO;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEditor;
using System;


public class Action : MonoBehaviour
{

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
        public float damage;
        public string react_hit;
        public string react_ko;

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

    [SerializeField] float m_BlendTime = 1.0f;

    public string filename = "Actions.json";

    private Character m_Character;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private AnimatorOverrideController m_AnimatorOverride;
    private ActionTable table;
    private ActionData action;
    private int slot = 1;
    

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


    public void EnableRotation(string enable)
    {

        action.rotation = bool.Parse(enable);

    }


    public void EnableMovement(string enable)
    {

        action.movement = bool.Parse(enable);

    }


    public void EnableInvulnerable(string enable)
    {

        action.invulnerable = bool.Parse(enable);

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
            "Animations/" + action.animation, typeof(AnimationClip)) as AnimationClip;
        AnimationClip clip_new = new AnimationClip();
        EditorUtility.CopySerialized(clip_src, clip_new);

        m_AnimatorOverride[newslot] = clip_new;
        
    }


    public void StartAction(string actionName)
    {

        foreach (ActionData data in table.actions)
        {
            if (data.name == actionName)
            {
                action = (Action.ActionData)data.Clone();
                break;
            }
        }
        

        if (action.name != "Default")
        {

            var rotation = m_Rigidbody.transform.rotation;
            var actionVelocity = rotation * action.velocity;

            m_Rigidbody.velocity = actionVelocity;

            /*for (int i = 0; i < 3; i++)
                if (actionVelocity[i] != 0)
                    m_Rigidbody.velocity[i] = actionVelocity[i];*/


            m_Animator.SetInteger("ActionSlot", -1 * m_Animator.GetInteger("ActionSlot"));

            if (m_Animator.GetInteger("ActionSlot") == 1)
                LoadSlotAnimation("neo_reference_skeleton|Action_Slot2", "neo_reference_skeleton|Action_Slot1");
            else
                LoadSlotAnimation("neo_reference_skeleton|Action_Slot1", "neo_reference_skeleton|Action_Slot2");

            m_Animator.SetFloat("ActionSpeed", action.speed);
            m_Animator.SetBool("ActionPlaying", true);

        }
        else
            m_Animator.SetBool("ActionPlaying", false);

    }


    public void ResetAction()
    {
        
        StartAction("Default");

    }


    public void Rotate(Vector3 camForward)
    {

        if (action.rotation)
            m_Character.Rotate(camForward);

    }


    public void Move(Vector3 move)
    {

        if (action.movement)
            m_Character.Move(move);

    }


    public void GetUp(string actionName)
    {
        
        if (m_Character.HasLives())
        {
            m_Character.ResetHealth();
            StartAction(actionName);
        }
        
    }


    public void ApplyDamage(string bone)
    {

        FireProjectile(bone + " Punch");
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
            Resources.Load("Prefabs/Projectiles/" + projectileName),
            transform
            ) as GameObject;

        obj.GetComponent<Projectile>().SetShooterInfo(
            GetComponent<CapsuleCollider>(), action.damage, action.react_hit, action.react_ko);
        obj.transform.position = collider.transform.position;
        obj.transform.rotation = m_Rigidbody.transform.rotation;

    }


    public void SpawnParticle(string particleName, Vector3 position, Quaternion rotation)
    {

        ParticleSystem hit = Instantiate(
            Resources.Load("Prefabs/Particles/" + particleName),
            position,
            rotation
            ) as ParticleSystem;

    }


    public void ReceiveDamage(float damage, Vector3 direction, string react_hit, string react_ko)
    {

        if (action.invulnerable)
            return;

        m_Character.ReceiveDamage(damage);

        if (m_Character.HasHealth())
        {
            if (react_ko != null)
            {
                StartAction(react_ko);
                m_Rigidbody.rotation = Quaternion.LookRotation(direction * -1);
            }
        }
        else if (react_hit != null)
            StartAction(react_hit);


        SpawnParticle(
            "HitContact", 
            transform.position + new Vector3(0, GetComponent<CapsuleCollider>().height * 0.9f, 0),
            Quaternion.Euler(new Vector3(-90, 0, 0))
            );

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