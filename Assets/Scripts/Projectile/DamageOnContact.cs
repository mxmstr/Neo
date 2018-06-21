using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class DamageOnContact : MonoBehaviour {

    [SerializeField] string m_TagName = "Character";
    [SerializeField] GameObject m_ContactParticle;
    [SerializeField] string m_ContactSound;

    Projectile m_Projectile;
    Sound m_Sound;


    void Start()
    {

        m_Projectile = GetComponent<Projectile>();
        m_Sound = GetComponent<Sound>();

    }


    void Update ()
    {
        
        Group group = m_Projectile.GetShooter().GetComponent<Character>().GetGroup();

        foreach (Collider c in m_Projectile.GetContacts())
        {
            if (c != m_Projectile.GetShooter() && c.tag == m_TagName && c.GetComponent<Character>().GetGroup() != group)
            {
                c.GetComponent<Action>().ReceiveDamage(
                    m_Projectile.GetDamage(), transform.forward, m_Projectile.GetReactHit(), m_Projectile.GetReactKO());

                if (m_ContactSound != null)
                    c.GetComponent<Sound>().PlaySound(m_ContactSound);

                if (m_ContactParticle != null)
                    Instantiate(
                        m_ContactParticle,
                        c.transform.position + new Vector3(0, ((CapsuleCollider)c).height * 0.9f, 0),
                        c.transform.rotation
                        //Quaternion.Euler(new Vector3(-90, 0, 0))
                        );
            }
        }
        
    }

}
