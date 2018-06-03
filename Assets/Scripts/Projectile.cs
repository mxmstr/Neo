using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] float m_Lifetime;

    Action m_Action;
    ArrayList m_Contacts;
    Collider m_Shooter;
    float m_Damage;
    string m_ReactHit;
    string m_ReactKO;


    void Start ()
    {
        
        m_Contacts = new ArrayList();

    }


    void OnTriggerStay(Collider other)
    {

        m_Contacts.Add(other);
        
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


    public void SetShooterInfo(Collider shooter, float damage, string react_hit, string react_ko)
    {

        m_Shooter = shooter;
        m_Damage = damage;
        m_ReactHit = react_hit;
        m_ReactKO = react_ko;

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
