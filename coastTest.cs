using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coastTest : MonoBehaviour
{
    //[SerializeField]
    public LeoIsland leoIslandScript;
    public Vector2[] boundaries;
    public float epsilon = 1;

    // Start is called before the first frame update
    public void Start()
    {
        leoIslandScript = GameObject.Find("LeoIsland").GetComponent<LeoIsland>();
        boundaries = leoIslandScript.boundaries;
        gameObject.SetActive(this.FarEnough());

    }

    public void AdjustEpsilon(float inputEpsilon)
    {
        Debug.Log(inputEpsilon);
        epsilon = inputEpsilon;   //this doesn't work
        //gameObject.SetActive(this.FarEnough());
        //Debug.Log(isActiveAndEnabled);
        Debug.Log(epsilon);
    }

    // Update is called once per frame
    void Update()
    {
        //Epsilon will be controlled here by the player

        //This ruins the performance
        //gameObject.SetActive(this.FarEnough());

    }

    private bool FarEnough() {
        GFG.Point[] points = new GFG.Point[boundaries.Length];
        for (int i = 0; i < boundaries.Length; i++)
        {
            //Check for every side if can put close to it
            points[i] = new GFG.Point(boundaries[i][0], boundaries[i][1]);
            int k = (i + 1) % boundaries.Length;
            Vector3 lineStart = new Vector3(boundaries[i][0], 0, boundaries[i][1]);
            Vector3 lineEnd = new Vector3(boundaries[k][0], 0, boundaries[k][1]);
            Vector3 pos3d = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            if (!CanPutTreeByLine(lineStart, lineEnd, pos3d))
            {
                Debug.Log("false");
                return false;
            }
        }
        Debug.Log("true");

        return true;
    }

    public bool CanPutTreeByLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 closestOnLine = FindClosestPointOnLineSegment(lineStart, lineEnd, point);
        Debug.Log("epsilon = " + epsilon);
        if ((closestOnLine - point).magnitude >= this.epsilon)
        {
            return true;
        }
        return false;
    }

    public static Vector3 FindClosestPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 line = lineEnd - lineStart;
        Vector3 dir = point - lineStart;
        float d = Vector3.Dot(line, dir) / line.sqrMagnitude;
        d = Mathf.Clamp01(d);
        return Vector3.Lerp(lineStart, lineEnd, d);
    }
}
