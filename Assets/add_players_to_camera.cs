using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class add_players_to_camera : MonoBehaviour
{
    GameObject[] players;
    CinemachineTargetGroup target;
    PlayerInputManager playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        target = gameObject.GetComponent<CinemachineTargetGroup>();
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerInputManager>();
    }
    void Start()
    {
        FindPlayers();
        //AddPlayersToTargetGroup();
    }


    // Update is called once per frame
    void Update()
    {
        FindPlayers();
    }

    private void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void AddPlayersToTargetGroup(GameObject player)
    {
        //foreach (GameObject player in players){
        if (IsPlayerInTargetGroup(player))
        {
            target.AddMember(player.transform, 1, 2);
        }
            
        //}
    }
    bool IsPlayerInTargetGroup(GameObject player)
    {
        // Iterate through the targets in the target group
        for (int targetItem = 0; targetItem <= target.Targets.Count; targetItem++)
        {
            if (target.Targets[targetItem] == player)
            {
                return true; // Player is already in the target group
            }
        }
        return false; // Player is not in the target group

    }
}
