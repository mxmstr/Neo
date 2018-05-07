using UnityEngine;
using System.IO;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Branch : ScriptableObject
    {

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
        
        private BranchTable table;
        private BranchData branch;
        private string filename = "Branches.json";


        public Branch(string branchName)
        {

            LoadBranchData(branchName);
            

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
                    break;
                }
            }

        }

    }

}