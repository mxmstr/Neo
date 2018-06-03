using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour {

    [SerializeField] string m_TagName = "Character";

    Projectile m_Projectile;


    void Start()
    {

        m_Projectile = GetComponent<Projectile>();

    }


    void Update ()
    {
        
        foreach (Collider c in m_Projectile.GetContacts())
            if (c != m_Projectile.GetShooter() && c.tag == m_TagName)
                c.GetComponent<Action>().ReceiveDamage(m_Projectile.GetDamage(), transform.forward, m_Projectile.GetReactHit(), m_Projectile.GetReactKO());

    }

}
