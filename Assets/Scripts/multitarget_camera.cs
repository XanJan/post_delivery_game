using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class multitarget_camera : MonoBehaviour
{
    private GameObject[] players;
    private PlayerInputManager playerManager;
    private Vector3 velocity;

    public float minZoom;
    public float maxZoom;
    public float zoomLimiter;
    public float zoomTime;

    public float smoothTime;
    public List<Transform> targets;
    [SerializeField] public Vector3 offset;

    private void Awake()
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerInputManager>();
    }

    void Start()
    {
        FindPlayers();
    }

    private void Update()
    {
        FindPlayers();
    }

    private void LateUpdate()
    {
        if (targets.Count == 0) return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPos = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    void ZoomCamera()
    {
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetGreatestDistance() / zoomLimiter);

        offset.y = Mathf.Lerp(offset.y, newZoom, Time.deltaTime * zoomTime);
        offset.z = Mathf.Lerp(offset.z, -newZoom, Time.deltaTime * zoomTime);
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    private void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        AddPlayersToTargetGroup();
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.z;
    }

    public void AddPlayersToTargetGroup()
    {
        //For all the players in the game, see if at is already added to the target group, if not, add it to the group
        foreach (GameObject player in players)
        {
            if (!IsPlayerInTargetGroup(player))
            {
                targets.Add(player.transform);
            }
        }
    }

    bool IsPlayerInTargetGroup(GameObject player)
    {
        return targets.Contains(player.transform);
    }
}
