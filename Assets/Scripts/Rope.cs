using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Rope : MonoBehaviour
{
    private LineRenderer lineRender;
    [SerializeField] private int segmentCount;
    public ConfigurableJoint segmentPrefab;
    public ConfigurableJoint rootPrefab;
    private ConfigurableJoint lastSegment;
    private Rigidbody lastRigid;
    public float segmentLength = 1;
    [SerializeField] private Vector3 segmentOffset;

    private List<ConfigurableJoint> nodes = new List<ConfigurableJoint>();
    public Rigidbody rootRigid;
    public Transform lineStart;
    public Transform lineEnd;
    public Transform staticPoint;
    [SerializeField] private float extentionDistance = 0.5f;
    float maxLength;

    public bool createAtStart = true;
    private bool created = false;

    private void Start()
    {
        if (createAtStart) 
        {
            CreateRope();
        }
    }

    public void CreateRope()
    {
        lineRender = GetComponent<LineRenderer>();
        segmentOffset.Normalize();
        segmentOffset *= segmentLength;

        var firstSegment = Instantiate(rootPrefab, rootRigid.transform.position, Quaternion.identity, transform);
        firstSegment.connectedBody = rootRigid;
        nodes.Add(firstSegment);
        lastSegment = firstSegment;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 newPos = lastSegment.transform.position + segmentOffset;
            var newSegment = Instantiate(segmentPrefab, newPos, Quaternion.identity, transform);
            newSegment.connectedBody = lastSegment.GetComponent<Rigidbody>();

            lastSegment = newSegment;
            nodes.Add(newSegment);
        }

        lastRigid = lastSegment.GetComponent<Rigidbody>();
        lastRigid.isKinematic = true;
        lastRigid.useGravity = false;

        maxLength = segmentCount * (segmentLength + extentionDistance);


        lastSegment.transform.position = staticPoint.transform.position;
        created = true;
    }

    private void FixedUpdate()
    {
        if(created)
            UpdateLine();
    }

    private void UpdateLine()
    {
        lineRender.positionCount = nodes.Count+2 + (lineEnd!=null? 1:0);
        lineRender.SetPosition(0, lineStart.position);
        lineRender.SetPosition(1, rootRigid.transform.position);

        for (int i = 0; i < nodes.Count; i++)
        {
            lineRender.SetPosition(i+2, nodes[i].transform.position);
        }

        if (lineEnd)
        {
            lineRender.SetPosition(lineRender.positionCount-1, lineEnd.position);
        }
    }

    public bool IsWithinDistance(Vector3 position)
    {
        float distSqr = (position - lastSegment.transform.position).sqrMagnitude;
        return distSqr < (maxLength * maxLength);
    }

    public Vector3 GetDirectionTowardsEnd(Vector3 fromPos)
    {
        return (lastSegment.transform.position - fromPos).normalized;
    }

    public void AddNode()
    {
        Debug.Log("Adding Node");

        var newNode = Instantiate(segmentPrefab, lastSegment.transform.position + segmentOffset, Quaternion.identity, transform);
        newNode.connectedBody = lastRigid;
        nodes.Add(newNode);

        var newRigid = newNode.GetComponent<Rigidbody>();
        newRigid.isKinematic = true;
        newRigid.useGravity = false;
        newRigid.transform.position = staticPoint.position;

        lastRigid.isKinematic = false;
        lastRigid.useGravity = true;
        lastSegment.transform.position = lastSegment.transform.position + segmentOffset * 3;

        lastSegment = newNode;
        lastRigid = newRigid;
        segmentCount++;
       
        maxLength = segmentCount * (segmentLength + extentionDistance);
    }

    public float GetLength()
    {
        return maxLength;
    }

    public void MoveEndPointBy(Vector3 offset)
    {
        lastSegment.transform.position = lastSegment.transform.position + offset;
    }
}
