using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;


public class Phase : MonoBehaviour {

    private MethodInfo this[string propertyName]
    {
        get
        {
            Type myType = typeof(Phase);
            MethodInfo myPropInfo = myType.GetMethod(propertyName);
            return myPropInfo == null ? null : myPropInfo;
        }
    }

    [System.Serializable]
    public class ScriptData
    {

        public string prefab;

    }

    [System.Serializable]
    public class CharacterData
    {

        public string prefab;
        public int id;
        public bool active;
        public string[] scripts;
        public bool transform;
        public Vector3 position;
        public Vector3 rotation;

    }

    [System.Serializable]
    public class PhaseData
    {

        public string name;
        public bool cursor_visible;
        public int cursor_lock;
        public string music;
        public string camera;
        public string canvas;
        public float freecam_timer;
        public Vector3 freecam_pos;
        public Vector3 freecam_rot;
        public string freecam_script;
        public string next_trigger;
        public CharacterData[] spawned;
        public CharacterData[] existing;

    }

    [System.Serializable]
    public class PhaseTable
    {
        public PhaseData[] phases;
    }

    [SerializeField] string m_Source = "Phases.json";
    [SerializeField] NavMeshData m_NavMeshData;
    [SerializeField] List<string> m_Phases = new List<string>();
    [SerializeField] List<string> m_Events = new List<string>();

    private GameObject m_CameraRig;
    private GameObject m_Canvas;
    private Sound m_Sound;
    private string m_SoundName;
    private PhaseTable m_PhaseTable;
    private PhaseData m_PhaseData;
    private IDictionary<string, MethodInfo> m_EventList;
    private GameObject m_Characters;
    private delegate bool Trigger();
    private Trigger m_NextTrigger;


    void Start () {

        m_Sound = GetComponent<Sound>();

        NavMesh.AddNavMeshData(m_NavMeshData);
        NavMesh.CalculateTriangulation();

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).name.Contains("FreeCamRig"))
            {
                m_CameraRig = transform.GetChild(i).gameObject;
                break;
            }
        }

        m_EventList = new Dictionary<string, MethodInfo>();

        foreach (string e in m_Events) {
            string eventName = e.Split(' ')[0];
            string phaseName = e.Split(' ')[1];
            
            m_EventList.Add(phaseName, this[eventName]);
        }


        LoadPhasesData();
        NextPhase();
        
    }


    void LoadPhasesData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, m_Source);
        m_PhaseTable = JsonUtility.FromJson<PhaseTable>(File.ReadAllText(filePath));

    }


    public void NextPhase()
    {

        if (m_Phases.Count == 0)
            return;

        string phaseName = m_Phases[0];
        m_Phases.RemoveAt(0);

        StartPhase(phaseName);

    }


    public void StartPhase(string phaseName)
    {

        foreach (PhaseData data in m_PhaseTable.phases)
        {
            if (data.name == phaseName)
            {
                m_PhaseData = data;
                break;
            }
        }


        if (m_PhaseData.next_trigger != null)
        {
            m_NextTrigger = delegate ()
                {
                    return (bool)(this[m_PhaseData.next_trigger]).Invoke(this, null);
                };
        }
        else
        {
            m_NextTrigger = delegate ()
                {
                    return false;
                };
        }


        if (m_PhaseData.existing != null)
        {
            foreach (CharacterData data in m_PhaseData.existing)
            {
                
                List<GameObject> characters = new List<GameObject>();

                if (data.id == -1)
                    characters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Character"));
                else
                    characters.Add(Array.Find(
                        GameObject.FindGameObjectsWithTag("Character"),
                        delegate (GameObject go) { return go.GetComponent<Character>().GetID() == data.id; }
                        ));

                foreach (GameObject character in characters)
                {
                    
                    if (data.transform)
                    {
                        character.transform.position = data.position;
                        character.transform.rotation = Quaternion.Euler(data.rotation);
                    }
                    
                    character.GetComponent<Character>().SetCameraRotation(data.rotation.y);
                    character.GetComponent<Character>().SetActive(data.active);

                    foreach (string script in data.scripts)
                    {
                        Type t = Type.GetType(script);
                        character.AddComponent(t);
                    }
                }

            }
        }


        if (m_PhaseData.spawned != null)
        {
            foreach (CharacterData data in m_PhaseData.spawned)
            {
                
                GameObject character = Instantiate(
                    Resources.Load(data.prefab),
                    data.position,
                    Quaternion.Euler(data.rotation)
                    ) as GameObject;

                character.GetComponent<Character>().SetCameraRotation(data.rotation.y);
                character.GetComponent<Character>().SetActive(data.active);
                character.GetComponent<Character>().SetID(data.id);

                foreach (string script in data.scripts)
                {
                    Type t = Type.GetType(script);
                    character.AddComponent(t);
                }

            }
        }


        Cursor.visible = m_PhaseData.cursor_visible;
        Cursor.lockState = (CursorLockMode)m_PhaseData.cursor_lock;


        if (m_SoundName != m_PhaseData.music)
        {
            m_Sound.StopSound();
            if (m_PhaseData.music != null)
            {
                m_Sound.PlaySound(m_PhaseData.music);
                m_SoundName = m_PhaseData.music;
            }
        }


        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.tag == "MainCamera")
            {
                if (obj.name.Contains(m_PhaseData.camera))
                {
                    obj.GetComponent<Camera>().enabled = true;
                    obj.GetComponent<AudioListener>().enabled = true;
                }
                else
                {
                    obj.GetComponent<Camera>().enabled = false;
                    obj.GetComponent<AudioListener>().enabled = false;
                }
            }
        }


        SetCanvas(m_PhaseData.canvas);


        m_CameraRig.GetComponentInChildren<CameraBase>().SetTimer(m_PhaseData.freecam_timer);
        m_CameraRig.transform.position = m_PhaseData.freecam_pos;
        m_CameraRig.transform.rotation = Quaternion.Euler(m_PhaseData.freecam_rot);
        m_CameraRig.GetComponentInChildren<Camera>().transform.localPosition = new Vector3(0, 0, 0);
        m_CameraRig.GetComponentInChildren<Camera>().transform.localRotation = new Quaternion();


        foreach (MonoBehaviour script in m_CameraRig.GetComponents<MonoBehaviour>())
            DestroyImmediate(script);
        
        if (m_PhaseData.freecam_script != null)
        {
            Type t = Type.GetType(m_PhaseData.freecam_script);
            m_CameraRig.AddComponent(t);
        }
        

    }


    public void SetCanvas(string canvasName)
    {

        if (m_Canvas != null)
        {
            if (!(canvasName != null && canvasName.Contains(m_Canvas.name)))
            {
                DestroyImmediate(m_Canvas);
                m_Canvas = null;
            }
        }

        if (canvasName != null &&
            !(m_Canvas != null && canvasName.Contains(m_Canvas.name)))
            m_Canvas = Instantiate(Resources.Load(canvasName)) as GameObject;
        
    }


    public bool FreeCamTimedOut()
    {

        return m_CameraRig.GetComponentInChildren<CameraBase>().IsTimedOut();

    }


    public bool PlayerDead()
    {

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Character"))
        {
            Character character = go.GetComponent<Character>();
            if (character.GetID() == 0)
                return !character.HasLives() && !character.HasHealth();
        }

        return false;

    }


    public bool AllBlueDead()
    {

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Character"))
        {
            Character character = go.GetComponent<Character>();
            if (character.GetGroup() == Group.Blue && (character.HasLives() || character.HasHealth()))
                return false;
        }

        return true;

    }


    void Update () {

        if (m_NextTrigger())
            NextPhase();

        foreach (string phaseName in m_EventList.Keys)
        {
            if (phaseName != m_PhaseData.name && (bool)m_EventList[phaseName].Invoke(this, null))
                    StartPhase(phaseName);
        }


    }

}
