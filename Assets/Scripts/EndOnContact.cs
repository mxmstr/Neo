using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOnContact : MonoBehaviour {

    [SerializeField] string m_TagName = "Character";

    Projectile m_Projectile;


    void Start()
    {

        m_Projectile = GetComponent<Projectile>();

    }


    void Update () {

        int numHits = 0;

        foreach (Collider c in m_Projectile.GetContacts())
            if (c != m_Projectile.GetShooter() && c.tag == m_TagName)
                numHits++;

        if (numHits > 0)
            Destroy(this.gameObject);

    }

}
