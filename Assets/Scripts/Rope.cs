using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer lineRender;
    [SerializeField] private int segmentCount;
    public ConfigurableJoint segmentPrefab;
    private ConfigurableJoint lastSegment;
    [SerializeField] private Vector3 segmentOffset;

    private List<ConfigurableJoint> nodes = new List<ConfigurableJoint>();
    public Rigidbody rootRigid;

    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();

        var firstSegment = Instantiate(segmentPrefab, rootRigid.transform.position + segmentOffset, Quaternion.identity, transform);
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

        var lastRigid = lastSegment.GetComponent<Rigidbody>();
        lastRigid.isKinematic = true;
        lastRigid.useGravity = false;
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
}
