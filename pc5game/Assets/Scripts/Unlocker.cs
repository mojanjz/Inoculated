using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unlocker : MonoBehaviour
{
    [SerializeField] private ObjectRaycaster raycaster;
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for unlocking
    [SerializeField] private LayerMask rayLayer = 1 << 8; // Layer that contains the unlockable objects (touchable by raycast).

    public UnityEvent OnUnlockStartEvent;
    public UnityEvent OnUnlockEndEvent;

    private void Awake()
    {
        if (OnUnlockStartEvent == null)
            OnUnlockStartEvent = new UnityEvent();

        if (OnUnlockEndEvent == null)
            OnUnlockEndEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Unlock(GameObject key)
    {
        bool result = false;
        RaycastHit2D hit = raycaster.ObjectRaycast(maxCastDist, rayLayer);

        /* If something was hit, check if it's unlockable. */
        if (hit.collider != null)
        {
            var unlockable = hit.collider.GetComponent<Unlockable>();

            if (unlockable)
            {
                result = unlockable.OnUnlock(key);
            }
        }

        OnUnlockEndEvent.Invoke();

        return result;
    }
}
