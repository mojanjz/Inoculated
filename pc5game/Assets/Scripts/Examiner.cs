using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examiner : MonoBehaviour
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

    public void Examine()
    {
        RaycastHit2D hit = raycaster.ObjectRaycast();

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
