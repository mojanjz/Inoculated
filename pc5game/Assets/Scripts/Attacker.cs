using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Attacker : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ObjectRaycaster raycaster;

    public bool IsAttacking { get; private set; } = false;

    public UnityEvent OnAttackStartEvent;
    public UnityEvent OnAttackEndEvent;

    private void Awake()
    {
        if (OnAttackStartEvent == null)
            OnAttackStartEvent = new UnityEvent();

        if (OnAttackEndEvent == null)
            OnAttackEndEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Attack()
    {
        RaycastHit2D hit = raycaster.ObjectRaycast();

        IsAttacking = true;

        /* Invokes methods in other relevant scripts as CharacterAudioController. */
        OnAttackStartEvent.Invoke();

        /* Play player attack animation. */
        animator.SetBool("attack", true);

        /* Switch isAttacking state after the attack animation has completed. */
        float myTime = animator.GetCurrentAnimatorStateInfo(2).length;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(2).length);
        IsAttacking = false;

        OnAttackEndEvent.Invoke();

        /* If something was hit, check if it's attackable. */
        if (hit.collider != null)
        {
            var attackable = hit.collider.GetComponent<Attackable>();

            if (attackable)
            {
                attackable.OnAttack();
            }
        }
    }
}
