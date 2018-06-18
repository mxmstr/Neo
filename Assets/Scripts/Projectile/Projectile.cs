using System.Collections;
using UnityEngine;


public class Projectile : MonoBehaviour {

    public float m_Lifetime;
    public float m_Damage;
    public string m_ReactHit;
    public string m_ReactKO;

    Action m_Action;
    ArrayList m_Contacts;
    Collider m_Shooter;


    void Start ()
    {
        
        m_Contacts = new ArrayList();

    }


    void OnTriggerStay(Collider other)
    {

        m_Contacts.Add(other);
        
    }


    public void SpawnParticle(string particleName, Vector3 position, Quaternion rotation)
    {

        ParticleSystem hit = Instantiate(
            Resources.Load("Particles/" + particleName),
            position,
            rotation
            ) as ParticleSystem;

    }


    public Collider GetShooter()
    {

        return m_Shooter;

    }


    public float GetDamage()
    {

        return m_Damage;

    }


    public string GetReactHit()
    {

        return m_ReactHit;

    }


    public string GetReactKO()
    {

        return m_ReactKO;

    }


    public void SetShooter(Collider shooter)
    {

        m_Shooter = shooter;

    }


    public ArrayList GetContacts()
    {

        return m_Contacts;

    }


    private void Update()
    {

        m_Lifetime -= Time.deltaTime;
        if (m_Lifetime < 0)
            Destroy(this.gameObject);

    }


    void LateUpdate ()
    {
        
        m_Contacts.Clear();

    }

}
