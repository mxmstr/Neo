﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class DeathCam : MonoBehaviour
{
    [SerializeField] float m_Speed = -10.0f;
    [SerializeField] Vector3 m_Offset = new Vector3(0, 0, 3);
    [SerializeField] Vector3 m_TargetPos = new Vector3(0, 1, 0);

    private Camera m_Cam;

    /*{
                "name": "MenuCam",
                "params": [
                    [ "m_Distance", 10.0 ],
                    "m_Speed"
                    :
                    5.0,
                    "m_TargetPos"
                    :
                ]
            },*/


    void Start() {
        
        m_Cam = GetComponentInChildren<Camera>();
        m_Cam.transform.localPosition = m_Offset;

        Transform player = Array.Find(
                GameObject.FindGameObjectsWithTag("Character"),
                    delegate (GameObject go) { return go.GetComponent<Character>().GetID() == 0; }
                    ).transform;
        
        m_TargetPos += player.position;
        transform.rotation = player.rotation;
        transform.Rotate(new Vector3(0, 90f, 0));

    }


    void Update() {

        transform.position = m_TargetPos;
        transform.RotateAround(m_TargetPos, new Vector3(0, 1, 0), m_Speed * Time.deltaTime);

        m_Cam.transform.rotation = Quaternion.LookRotation(
            m_TargetPos - m_Cam.transform.position,
            new Vector3(0, 1, 0)
            );

    }

}