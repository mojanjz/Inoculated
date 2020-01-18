using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Attacker : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterAudioController audioController;
    [SerializeField] private AttackData[] attackData; // Owned attacks

    /* Casting settings */
    [SerializeField] private ObjectRaycaster raycaster;
    [SerializeField] private LayerMask rayLayer = 1 << 8; // Layer that contains the attackable objects (touchable by raycast).

    public int AtkIndex { get; private set; } = 0;

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
        /* Set default attack. */
        animator.runtimeAnimatorController = attackData[AtkIndex].animController;
        audioController.attackSound = attackData[AtkIndex].Audio;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator Attack()
    {
        /* Cast a ray forwards from the attacker object, and visually show the ray for debugging purposes. */
        RaycastHit2D hit = raycaster.ObjectRaycast(attackData[AtkIndex].Distance, rayLayer);

        IsAttacking = true;

        /* Invokes methods in other relevant scripts as CharacterAudioController. */
        OnAttackStartEvent.Invoke();

        /* Play attack animation. */
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

    /* Method that switches the attack index to the specified attack. Switches to
     * first attack in attackDataCollection that matches the given name.
     * PARAM: attackName, name of the desired attack
     * PRE: attackName is the name of an attack existing in attackData collection */
    public void SwitchAttack(string attackName)
    {
        for ( int i = 0; i < attackData.Length; i++)
        {
            if ( attackData[i].Name == attackName )
            {
                AtkIndex = i;
                animator.runtimeAnimatorController = attackData[i].animController;
                audioController.attackSound = attackData[i].Audio;
                return;
            }
        }

        /* If the given name was not found in attackData collection, throw error. */
        throw new InvalidAttackName(attackName);
    }

    public class InvalidAttackName : Exception
    {
        public InvalidAttackName(string name) : base(String.Format("Invalid attack name: {0}.", name))
        {
        }
    }
}
