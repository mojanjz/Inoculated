using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Examiner : MonoBehaviour
{
    [SerializeField] private ObjectRaycaster raycaster;
    [SerializeField] private float maxCastDist = 1.0f; // Distance that the object must be within for examination
    [SerializeField] private LayerMask rayLayer = 1 << 8; // Layer that contains the examineable objects (touchable by raycast).

    public string dialoguePanelName;

    public UnityEvent OnExamineStartEvent;
    public UnityEvent OnExamineEndEvent;
    public bool IsActive { get; private set; } = false;

    private Examinable examinable = null;

    private void Awake()
    {
        if (OnExamineStartEvent == null)
            OnExamineStartEvent = new UnityEvent();

        if (OnExamineEndEvent == null)
            OnExamineEndEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /* Method to examine an Examineable using ObjectRaycast. */
    public void Examine()
    {
        /* If already in process of examining something, don't continue this call. */
        if (IsActive)
        {
            return;
        }

        IsActive = true;

        /* Cast a ray. */
        RaycastHit2D hit = raycaster.ObjectRaycast(maxCastDist, rayLayer);

        /* If something was hit by the ray, check if it's examinable. */
        if (hit.collider != null)
        {
            examinable = hit.collider.GetComponent<Examinable>();

            if (examinable != null)
            {
                examinable.OnExamineEndEvent.AddListener(OnExamineEnd);
                examinable.OnExamine(this);    

                /* Return so we don't call the end method below. */
                return;
            }
        }

        /* If nothing examineable was hit, call the end method directly. */
        OnExamineEnd(this);
    }

    /* Method to be called when the interaction ends. */
    public void OnExamineEnd(Examiner examiner)
    {
        /* If the examiner was not this instance, don't respond to the call. */
        if (examiner != this)
        {
            return;
        }

        if (examinable != null)
        {
            examinable.OnExamineEndEvent.RemoveListener(OnExamineEnd);
            examinable = null;
        }

        IsActive = false;
        OnExamineEndEvent.Invoke();
    }
}
