using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;
public class wagon_storage : MonoBehaviour{
    public int maxCapacity = 50; // amount of packages that can be stored
    private int totalPackagedPackages = 0;
    private Stack<GameObject> storedPackages = new Stack<GameObject>();
    public Transform storagePoint;
    public GameObject packagePrefab;
    [SerializeField] private List<GameObject> _packagesToInit;

    public bool IsFull() => storedPackages.Count >= maxCapacity;
    public bool IsEmpty() => storedPackages.Count == 0;

    void Start()
    {
        if(game_events.current != null){
            game_events.current.onNeighborhoodGenerated += AddPackagesFromNewZones;
        }
        int packagesToSpawn = level_create.Instance.GetTotalPackagesToDeliver();
        Debug.Log($"[Wagon] At Start, asking for {packagesToSpawn} packages");
        for (int i = 0; i < packagesToSpawn && i < maxCapacity; i++){
            GameObject package = Instantiate(packagePrefab);
            AddPackage(package);
        }
    }
    private void FillWagonAtStart(){
        for(int i = 0; i < _packagesToInit.Count && i < maxCapacity; i++){
            GameObject package = Instantiate(_packagesToInit[i]);
            AddPackage(package);
        }
    }

    public void FillWagonNewZone(int packageAmount){
        for(int i = 0; i < packageAmount; i++){
            GameObject package = Instantiate(_packagesToInit[i + totalPackagedPackages]);
            AddPackage(package);
            totalPackagedPackages++;
        }
    }

    public void AddPackage(GameObject package){
        if (storedPackages.Count < maxCapacity){
            package.transform.SetParent(storagePoint); // parent it to the wagon
            float newy = storedPackages.TryPeek(out var top) ? (top.transform.localPosition.y + (top.TryGetComponent<BoxCollider>(out var collider)?collider.size.y:0.4f)) : 0.4f;
            // Fix position (Magic)
            package.transform.localPosition = new Vector3((new float[]{0.1f,0f,-0.1f,0f})[storedPackages.Count%4], (storedPackages.Count == 0 ? 0 : newy ) , (new float[]{0f,0.1f,0f,-0.1f})[storedPackages.Count%4]);
            // Fix rotation
            package.transform.eulerAngles =new Vector3(0,new int[]{0,90,180,270}[storedPackages.Count % 4],0);
            package.SetActive(true);
            storedPackages.Push(package);
            Rigidbody rb = package.GetComponent<Rigidbody>();
            if (rb != null) // gör så paketen stannar i vagnen☠️
            {
                rb.isKinematic = true;
            }
            Collider wagonCollider = GetComponent<Collider>();
            Collider[] packageColliders = package.GetComponentsInChildren<Collider>();
            Physics.IgnoreCollision(packageColliders[0], wagonCollider, true);
            packageColliders[1].enabled = false;
        }
        else{
            Debug.Log("Wagon is full!");
        }
    }

    public GameObject RemovePackage(){
        if(storedPackages.Count > 0){
            GameObject package = storedPackages.Pop();
            package.transform.SetParent(null); // remove parent wagon
            Rigidbody rb = package.GetComponent<Rigidbody>();
            if (rb != null) // unfreeezea paketen☠️
            {
                rb.isKinematic = false; 
            }
            Collider wagonCollider = GetComponent<Collider>();
            Collider[] packageColliders = package.GetComponentsInChildren<Collider>();
            Physics.IgnoreCollision(packageColliders[0], wagonCollider, false);
            packageColliders[1].enabled = true;
            return package;
        }
        return null;   
    }

    void OnDisable()
    {
        game_events.current.onNeighborhoodGenerated -= AddPackagesFromNewZones;       
    }

    private void AddPackagesFromNewZones(){

        int totalNeeded = level_create.Instance.GetTotalPackagesToDeliver();
        Debug.Log(totalNeeded);
        int packagesToFill = Mathf.Min(totalNeeded - storedPackages.Count, maxCapacity - storedPackages.Count);
        Debug.Log(packagesToFill);
        for(int i = 0; i < packagesToFill; i++){
            GameObject package = Instantiate(packagePrefab);
            AddPackage(package);
        }
    }
}