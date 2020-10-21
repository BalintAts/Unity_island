using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class LeoTest : MonoBehaviour
{
    public TestCase[] testCases =
    {
         new TestCase { fileName = "Island1.txt", epsilon = 0.01f, expectedResult = 0 },
         new TestCase { fileName = "Island2.txt", epsilon = 0.01f, expectedResult = 1 },
         new TestCase { fileName = "Island3.txt", epsilon = 0.01f, expectedResult = 16 },
         new TestCase { fileName = "Island4.txt", epsilon = 0.01f, expectedResult = 39 },
         new TestCase { fileName = "Island1.txt", epsilon = 1.1f,  expectedResult = 0 },
         new TestCase { fileName = "Island2.txt", epsilon = 1.1f,  expectedResult = 0 },
         new TestCase { fileName = "Island3.txt", epsilon = 1.1f,  expectedResult = 4 },
         new TestCase { fileName = "Island4.txt", epsilon = 1.1f,  expectedResult = 16 },
    };

    [ContextMenu("Run tests")]
    public void RunTest()
    {
        foreach (var tc in testCases)
        {
            string path = Path.Combine(Application.streamingAssetsPath, tc.fileName);
            //using (var stream = File.OpenRead(path))
            using (StreamReader sr = new StreamReader(path))
            {
                var island = LeoIsland.LoadIsland(sr);
                int result = LeoIsland.GetNumTrees(island, tc.epsilon);
                Assert.AreEqual(result, tc.expectedResult, string.Format("Invalid result in test: {0}, E={1}", tc.fileName, tc.epsilon));
            }
        }
    }

    [Serializable]
    public struct TestCase
    {
        public string fileName;
        public float epsilon;
        public int expectedResult;
    }
}
