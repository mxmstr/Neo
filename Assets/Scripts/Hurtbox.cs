using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine;


public class Hurtbox : MonoBehaviour {

    Action m_Action;
    ArrayList m_Contacts;
    Collider m_Shooter;
    float m_Lifetime;
    float m_Damage;
    string m_ReactHit;
    string m_ReactKO;


    void Start ()
    {
        
        m_Action = GetComponent<Action>();
        m_Contacts = new ArrayList();

    }


    void OnTriggerStay(Collider other)
    {

        if (other.tag == "Character")
            m_Contacts.Add(other);
        
    }


    public void SetAttributes(Collider shooter, float lifetime, float damage, string react_hit, string react_ko)
    {

        m_Shooter = shooter;
        m_Lifetime = lifetime;
        m_Damage = damage;
        m_ReactHit = react_hit;
        m_ReactKO = react_ko;

    }


    public ArrayList GetContacts()
    {

        return m_Contacts;

    }


    void LateUpdate ()
    {
        
        int numHits = 0;
        foreach (Collider c in m_Contacts)
        {
            if (c != m_Shooter)
            {
                c.GetComponent<Action>().ReceiveDamage(
                    m_Damage, transform.forward, m_ReactHit, m_ReactKO
                    );
                numHits++;
            }
        }
        
        m_Contacts.Clear();


        m_Lifetime -= Time.deltaTime;
        if (m_Lifetime < 0 || numHits > 0)
            Destroy(this.gameObject);

    }

}
