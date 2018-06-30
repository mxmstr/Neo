using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class MenuCam : MonoBehaviour
{
    [SerializeField] float m_Speed = -10.0f;
    [SerializeField] Vector3 m_Offset = new Vector3(0, 25, -60);
    [SerializeField] Vector3 m_TargetPos = new Vector3(0, 0, 0);

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