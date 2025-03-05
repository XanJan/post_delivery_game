using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delivery_zone : MonoBehaviour
{
    private HashSet<GameObject> detectedPackages = new HashSet<GameObject>();
    private Renderer areaRenderer;
    public Color red = Color.red;
    public Color green = Color.green;

    void Start(){
     areaRenderer = GetComponent<Renderer>();
     areaRenderer.material.color = red;   
    }

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Package") && !detectedPackages.Contains(other.gameObject)){
            detectedPackages.Add(other.gameObject);
            score_manager.Instance.AddScore();
            areaRenderer.material.color = green;
            StartCoroutine(DisappearEffect(other.gameObject));
        }
    }

    IEnumerator DisappearEffect(GameObject package){
        Renderer packageRenderer = package.GetComponent<Renderer>();

        if (packageRenderer != null)
        {
            float fadeDuration = 0.5f; 
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                if (package == null || packageRenderer == null)
                    yield break; 
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);

                Color newColor = packageRenderer.material.color;
                newColor.a = alpha;
                packageRenderer.material.color = newColor;

                yield return null;
            }
        }

        detectedPackages.Remove(package); 
        if (package != null) Destroy(package); 
        areaRenderer.material.color = red;
    }

}
