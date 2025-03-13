using UnityEngine;
using System.Collections.Generic;

public class wagon_storage : MonoBehaviour{
    public int maxCapacity = 10; // amount of packages that can be stored
    private Stack<GameObject> storedPackages = new Stack<GameObject>();
    public Transform storagePoint;
    public GameObject packagePrefab;

    public bool IsFull() => storedPackages.Count >= maxCapacity;
    public bool IsEmpty() => storedPackages.Count == 0;

    void Start()
    {
        FillWagonAtStart();
    }

    private void FillWagonAtStart(){
        for(int i = 0; i < maxCapacity; i++){
            GameObject package = Instantiate(packagePrefab);
            AddPackage(package);
        }
    }

    public void AddPackage(GameObject package){
        if (storedPackages.Count < maxCapacity){
            package.transform.SetParent(storagePoint); // parent it to the wagon
            package.transform.localPosition = new Vector3(0, storedPackages.Count * 0.5f, 0); 
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
}