using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examiner : MonoBehaviour
{
    [SerializeField] private ObjectRaycaster raycaster;
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for examination
    [SerializeField] private LayerMask rayLayer = 1 << 8; // Layer that contains the examineable objects (touchable by raycast).

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Examine()
    {
        RaycastHit2D hit = raycaster.ObjectRaycast(maxCastDist, rayLayer);

        /* If something was hit, check if it's examinable. */
        if (hit.collider != null)
        {
            var examineable = hit.collider.GetComponent<Examinable>();

            if (examineable)
            {
                examineable.OnExamine();
            }
        }
    }
}
