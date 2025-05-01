using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class add_players_to_camera : MonoBehaviour
{
    GameObject[] players;
    CinemachineTargetGroup target;
    PlayerInputManager playerManager;

    private void Awake()
    {
        target = gameObject.GetComponent<CinemachineTargetGroup>();
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerInputManager>();
    }
    void Start()
    {
        FindPlayers();

    }

    void Update()
    {
        FindPlayers();
    }

    private void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        AddPlayersToTargetGroup();
    }

    public void AddPlayersToTargetGroup()
    {
        //For all the players in the game, see if at is already added to the target group, if not, add it to the group
        foreach (GameObject player in players){
            
            if (!IsPlayerInTargetGroup(player))
            {
                target.AddMember(player.transform, 1, 2);
            }
            
        }
    }
    bool IsPlayerInTargetGroup(GameObject player)
    {
            if(target.FindMember(player.transform) == -1)
            {
                return false; // Player is not in the target group

            }

        return true; // Player is in the target group

    }
}
