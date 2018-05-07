using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.IO;


public class Branch : MonoBehaviour {

    [System.Serializable]
    public class ResultData
    {

        public string[] Any;
        public string[] Stand;
        public string[] Forward;
        public string[] ForwardLeft;
        public string[] ForwardRight;
        public string[] Left;
        public string[] Right;
        public string[] Backward;
        public string[] BackwardLeft;
        public string[] BackwardRight;

    }

    [System.Serializable]
    public class BranchData
    {

        public string name;
        public string[] actions;
        public ResultData results;

    }
    
    [System.Serializable]
    public class BranchTable
    {
        public BranchData[] branches;
    }
    
    BranchData branch;

    private Character m_Character;
    private BranchTable table;
    private string filename = "Branches.json";


    void Start ()
    {

        m_Character = GetComponent<Character>();

        LoadBranchData("B_Primary");

        Debug.Log(branch.results.Any[0]);

    }


    private void LoadBranchData(string branchName)
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        table = JsonUtility.FromJson<BranchTable>(File.ReadAllText(filePath));

        foreach (BranchData data in table.branches)
        {
            if (data.name == branchName)
            {
                branch = data;
                return;
            }
        }

    }


    void Update ()
    {

        
		
	}

}
