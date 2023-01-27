using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class RouteSwitch : MonoBehaviour
{
    private SplineFollower _follower;

    public SplineComputer Band_1;
    public SplineComputer Band_2;
    public SplineComputer Band_3;
    public SplineComputer Band_4;
    public SplineComputer Band_5;

    private void Awake()
    {
        _follower = GetComponent<SplineFollower>();
        _follower.enabled = false;
    }

    void Start()
    {
        
        _follower.onNode += OnNodePassed;
        

    }

    // Update is called once per frame
    private void OnNodePassed(List<SplineTracer.NodeConnection> passed)
    {
        
        SplineTracer.NodeConnection nodeConnection = passed[0];
        Debug.Log(nodeConnection.node.name + " at point " + nodeConnection.point);
        double nodePercent = (double)nodeConnection.point / (_follower.spline.pointCount - 1);
        double followerPercent = _follower.UnclipPercent(_follower.result.percent);
        float distancePastNode = _follower.spline.CalculateLength(nodePercent, followerPercent);
        Debug.Log(nodePercent);

        Node.Connection[] connections = nodeConnection.node.GetConnections();
        int rnd = Random.Range(5, connections.Length);
        _follower.spline = connections[rnd].spline;
        double newNodePercent = (double)connections[rnd].pointIndex / (connections[rnd].spline.pointCount - 1);
        double newPercent = connections[rnd].spline.Travel(newNodePercent, distancePastNode, _follower.direction);
        _follower.SetPercent(newPercent);
        
        
    }

    private void Update()
    {
        if (transform.position.y == 5.168582f)
        {
            _follower.enabled = true;
            _follower.follow = true;
            
        }


        if (transform.position.x == -2.045752f)
        {
            _follower.spline = Band_1;
        }

        if (transform.position.x == -0.01242246f)
        {
            _follower.spline = Band_2;
        }

        if (transform.position.x == 1.982074f)
        {
            _follower.spline = Band_3;
        }

        if (transform.position.x == 3.931505f)
        {
            _follower.spline = Band_4;
        }

        if (transform.position.x == 6.038516f)
        {
            _follower.spline = Band_5;
        }
    }

}
