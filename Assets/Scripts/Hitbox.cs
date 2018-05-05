using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    Action m_Action;
    ArrayList m_Contacts;

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


    public ArrayList GetContacts()
    {

        return m_Contacts;

    }


    void LateUpdate ()
    {

        m_Contacts.Clear();

    }

}
