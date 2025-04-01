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
            target.AddMember(player.transform, 1, 2);
        //}
    }


}
