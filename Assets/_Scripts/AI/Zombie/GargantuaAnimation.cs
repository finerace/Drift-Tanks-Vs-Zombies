using System;
using UnityEngine;

public class GargantuaAnimation : MonoBehaviour
{

    [SerializeField] private ZombieAI zombieAI;
    [SerializeField] private Animator zombieMeshAnimator;
    
    private static readonly int IsAnnoyed = Animator.StringToHash("IsAnnoyed");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttack");
    private static readonly int IsDie = Animator.StringToHash("IsDie");

    private bool isAnnoyed;
    private bool isDie;
    
    private void Start()
    {
        zombieAI.onAnnoyedChange += () =>
        {
            isAnnoyed = zombieAI.IsAnnoyed;
        };
        
        zombieAI.OnDie += () =>
        {
            isDie = zombieAI.IsDie;
        };
    }

    private void Update()
    {
        if(!zombieMeshAnimator.gameObject.activeSelf)
            return;
            
        var isAttacking = zombieAI.isAttacking;

        if(zombieMeshAnimator.GetBool(IsAnnoyed) != isAnnoyed)
            zombieMeshAnimator.SetBool(IsAnnoyed,isAnnoyed);
        
        if(zombieMeshAnimator.GetBool(IsAttacking) != isAttacking)
            zombieMeshAnimator.SetBool(IsAttacking,isAttacking);

        if(zombieMeshAnimator.GetBool(IsDie) != isDie)
            zombieMeshAnimator.SetBool(IsDie,isDie);
    }
}
