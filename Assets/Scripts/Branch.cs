using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.IO;
using System;
using System.Reflection;


public class Branch : MonoBehaviour
{

    [System.Serializable]
    private class ResultSpeed
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ResultSpeed);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
        }

        public string[] AnySpeed;
        public string[] Stand;
        public string[] Walk;
        public string[] Run;

    }

    [System.Serializable]
    private class ResultDirection
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ResultDirection);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
        }

        public ResultSpeed AnyDirection;
        public ResultSpeed Stand;
        public ResultSpeed Forward;
        public ResultSpeed ForwardLeft;
        public ResultSpeed ForwardRight;
        public ResultSpeed Left;
        public ResultSpeed Right;
        public ResultSpeed Backward;
        public ResultSpeed BackwardLeft;
        public ResultSpeed BackwardRight;

    }

    [System.Serializable]
    private class ResultInput
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ResultInput);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
        }

        public ResultDirection AnyInput;
        public ResultDirection Primary;
        public ResultDirection Secondary;
        public ResultDirection Jump;

    }

    [System.Serializable]
    private class BranchData
    {

        public string name;
        public float cooldown;
        public string[] actions;
        public ResultInput results;

    }

    [System.Serializable]
    private class BranchTable
    {
        public BranchData[] branches;
    }

    public string filename = "Branches.json";

    private Action m_Action;
    private BranchTable table;
    private BranchData branch;


    void Start()
    {
        
        m_Action = GetComponent<Action>();

        LoadBranchData();
        ResetBranch();

    }


    private void LoadBranchData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        table = JsonUtility.FromJson<BranchTable>(File.ReadAllText(filePath));
        
    }


    private void StartBranch(string branchName)
    {
        
        foreach (BranchData data in table.branches)
        {
            if (data.name == branchName)
            {
                branch = data;
                break;
            }
        }

    }


    private void ResetBranch()
    {

        StartBranch("B_Default");

    }


    private string GetAction()
    {

        System.Random rand = new System.Random();

        return branch.actions[rand.Next(branch.actions.Length)];

    }


    public void StartAction(string input, string direction, string speed)
    {

        System.Random rand = new System.Random();
        ResultDirection directions;
        ResultSpeed speeds;
        string[] branches;


        if (!m_Action.IsReset())
            return;


        if (branch.results.AnyInput.AnyDirection == null && 
            branch.results.AnyInput[direction] == null)
            directions = (ResultDirection)branch.results[input];
        else
            directions = branch.results.AnyInput;
        

        if (directions.AnyDirection[speed] == null &&
            directions.AnyDirection.AnySpeed == null)
            speeds = (ResultSpeed)directions[direction];
        else
            speeds = directions.AnyDirection;
            

        try
        {
            if (speeds.AnySpeed == null)
                branches = (string[])speeds[speed];
            else
                branches = speeds.AnySpeed;
            
            StartBranch(branches[rand.Next(branches.Length)]);
            m_Action.StartAction(GetAction());
        }
        catch (NullReferenceException e) {}

    }


    public void Update()
    {
        
        if (branch.cooldown != -1.0 && m_Action.IsReset()) {
            branch.cooldown -= Time.deltaTime;
            if (branch.cooldown < 0)
                ResetBranch();
        }

    }

}