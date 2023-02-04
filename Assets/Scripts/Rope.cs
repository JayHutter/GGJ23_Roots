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
    private ConfigurableJoint lastSegment;
    private Rigidbody lastRigid;
    public float segmentLength = 1;
    [SerializeField] private Vector3 segmentOffset;

    private List<ConfigurableJoint> nodes = new List<ConfigurableJoint>();
    public Rigidbody rootRigid;
    public Transform staticPoint;

    float maxLength;

    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        segmentOffset.Normalize();
        segmentOffset*= segmentLength;

        var firstSegment = Instantiate(segmentPrefab, rootRigid.transform.position, Quaternion.identity, transform);
        firstSegment.connectedBody = rootRigid;
        nodes.Add(firstSegment);
        lastSegment = firstSegment;

        for (int i=0; i<segmentCount; i++)
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

        maxLength = segmentCount * segmentLength;


        lastSegment.transform.position = staticPoint.transform.position;
    }

    private void FixedUpdate()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        lineRender.positionCount = nodes.Count;

        for (int i = 0; i < nodes.Count; i++)
        {
            lineRender.SetPosition(i, nodes[i].transform.position);
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
       
        maxLength = segmentCount * segmentLength;
    }
}
