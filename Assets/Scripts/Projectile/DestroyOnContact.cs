using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour {

    [SerializeField] string m_TagName = "Character";

    Projectile m_Projectile;


    void Start () {

        m_Projectile = GetComponent<Projectile>();

	}
	

	void Update () {
        
        foreach (Collider c in m_Projectile.GetContacts())
            if (c != m_Projectile.GetShooter() && c.tag == m_TagName)
                Destroy(c.gameObject);

    }

}
