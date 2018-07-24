using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkTransformSync : NetworkBehaviour
{
    [SerializeField] private float sendInterval = 0.1f;
    [SerializeField] private float positionLerp = 0.1f;
    [SerializeField] private float rotationLerp = 0.1f;

    [SyncVar] public float lastWorldClock;
    [SyncVar] public Vector3 lastPosition;
    [SyncVar] public float lastRotation;
    [SyncVar] public float lastSpeed;
    [SyncVar] public float lastAngularVelocity;

    public static bool enableReckoning = true;
    public static bool enableInterpolation = true;

    private Vector3 updatePos;
    private Quaternion updateRot;

    private struct Movement
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
    }

    private readonly List<Movement> movementLog = new List<Movement>();

    private float intervalTimer = 0f;

    // Use this for initialization
    void Start () {
		
	}
    public override void OnStartServer()
    {
        // server 側で最初の配置位置を syncvar のpos/rot に設定しておく。
        // ここで設定すれば、OnStartClient() 「全員」に間に合う。

        lastPosition = transform.position;
        lastRotation = transform.rotation.eulerAngles.y;
        lastSpeed = 0f;
    }

    public override void OnStartClient()
    {
        // lastPosition/lastRotation は配置位置のはず。
        updatePos = lastPosition;
        updateRot = Quaternion.Euler(0f, lastRotation, 0f);
        SetTransform();
    }


    // Update is called once per frame
    void Update ()
    {
		if(NetworkGameManager.isClockAvailable)
        {
            return;
        }

        if(hasAuthority)
        {
            if (3 < movementLog.Count)
            {
                movementLog.RemoveRange(0, movementLog.Count - 3);
            }
            movementLog.Add(new Movement { position = transform.position, rotation = transform.rotation, time = NetworkGameManager.sInstance.worldClock });
            intervalTimer += Time.deltaTime;
            if (intervalTimer >= sendInterval)
            {
                intervalTimer = 0f;
                UpdateSyncVariables();
                CmdSetSyncVariables(lastWorldClock, lastPosition, lastRotation, lastSpeed, lastAngularVelocity);
            }
        }
        else
        {
            updatePos = lastPosition;
            updateRot = Quaternion.Euler(0f, lastRotation, 0f);
            Reckoning();
            Interpolation();
            SetTransform();
        }
	}

    private void UpdateSyncVariables()
    {
        var logNum = movementLog.Count;
        var curr = movementLog[logNum - 1];
        lastWorldClock = NetworkGameManager.sInstance.worldClock;
        lastPosition = curr.position;
        lastRotation = curr.rotation.eulerAngles.y;
        if (2 <= logNum)
        {
            var prev = movementLog[logNum - 2];
            var dt = curr.time - prev.time;
            lastSpeed = (curr.position - prev.position).magnitude / dt;
            lastAngularVelocity = Mathf.DeltaAngle(curr.rotation.y, prev.rotation.y) / dt;
        }
    }

    [Command]
    public void CmdSetSyncVariables(float clock, Vector3 pos, float yrot, float speed, float angularVelocity)
    {
        lastWorldClock = clock;
        lastPosition = pos;
        lastRotation = yrot;
        lastSpeed = speed;
        lastAngularVelocity = angularVelocity;
    }

    private void Reckoning()
    {
        if (!enableReckoning)
        {
            return;
        }
        var t = NetworkGameManager.sInstance.worldClock - lastWorldClock;
        var charDir = updateRot * Vector3.forward;
        updatePos += charDir * lastSpeed * t;
        updateRot = Quaternion.Euler(0f, lastRotation + lastAngularVelocity * t, 0f);
    }

    private void Interpolation()
    {
        if (!enableInterpolation)
        {
            return;
        }
        updatePos = Vector3.Lerp(transform.position, updatePos, positionLerp);
        updateRot = Quaternion.Lerp(transform.rotation, updateRot, rotationLerp);
    }
    private void SetTransform()
    {
        transform.position = updatePos;
        transform.rotation = updateRot;
    }
}
