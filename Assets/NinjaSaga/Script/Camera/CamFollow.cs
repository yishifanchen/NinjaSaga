using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

    public Transform target;
    [Header("Follow Settings")]
    public float distanceToTarget = 5;
    public float heightOffest = 5;//the height offest of the camera relative to it's target
    public float viewAngle = 10;//a downwards rotation
    public Vector3 additionalOffest;//ant additional offest
    public bool followZAxis;//enable or disable the camera following the z axis 

    [Header("Damp Settings")]
    public float DampX = 3;
    public float DampY = 3;
    public float DampZ = 3;

    [Header("View Area")]
    public float MinLeft;
    public float MaxRight;

    [Header("Wave Area collider")]
    public bool UseWaveAreaCollider;
    public BoxCollider CurrentAreaCollider;
    public float AreaColliderViewOffest;
    private void Start()
    {
        if (!target) SetPlayerAsTarget();
        if (target)
        {
            Vector3 playerPos = target.transform.position;
            transform.position = new Vector3(playerPos.x, playerPos.y - heightOffest, playerPos.z + distanceToTarget);
        }
    }
    private void Update()
    {
        if (target)
        {
            float currentX = transform.position.x;
            float currentY = transform.position.y;
            float currentZ = transform.position.z;
            Vector3 playerPos = target.transform.position;

            currentX = Mathf.Lerp(currentX, playerPos.x, DampX * Time.deltaTime);
            currentY = Mathf.Lerp(currentY, playerPos.y - heightOffest, DampY * Time.deltaTime);

            if (followZAxis)
            {
                currentZ = Mathf.Lerp(currentZ, playerPos.z + distanceToTarget, DampZ * Time.deltaTime);
            }
            else
            {
                currentZ = distanceToTarget;
            }
            if (CurrentAreaCollider == null) UseWaveAreaCollider = false;
            if (!UseWaveAreaCollider)
            {
                transform.position = new Vector3(Mathf.Clamp(currentX, MaxRight, MinLeft), currentY, currentZ) + additionalOffest;
            }
            else
            {
                transform.position = new Vector3(Mathf.Clamp(currentX, CurrentAreaCollider.transform.position.x + AreaColliderViewOffest, MinLeft), currentY, currentZ) + additionalOffest;
            }
            transform.rotation = new Quaternion(0, 180f, viewAngle, 0);
        }
    }
    void SetPlayerAsTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
}
