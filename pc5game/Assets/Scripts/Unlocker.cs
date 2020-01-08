using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : MonoBehaviour
{
    [SerializeField] private ObjectRaycaster raycaster;
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for unlocking
    [SerializeField] private LayerMask rayLayer = 1 << 8; // Layer that contains the unlockable objects (touchable by raycast).

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Unlock()
    {
        RaycastHit2D hit = raycaster.ObjectRaycast(maxCastDist, rayLayer);

        /* If something was hit, check if it's unlockable. */
        if (hit.collider != null)
        {
            var unlockable = hit.collider.GetComponent<Unlockable>();

            if (unlockable)
            {
                unlockable.OnUnlock();
            }
        }
    }
}
