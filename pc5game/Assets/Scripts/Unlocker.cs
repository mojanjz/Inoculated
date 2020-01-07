using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : MonoBehaviour
{
    [SerializeField] private ObjectRaycaster raycaster;

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
        RaycastHit2D hit = raycaster.ObjectRaycast();

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
