using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieAI : HealthBase
{
    [SerializeField] private Transform zombieT;
    [SerializeField] public Rigidbody zombieRb;

    [Space] 
    
    [SerializeField] private float damage;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    
    private IHealth targetHealth;
    private float attackCooldownTimer;
    
    [Space]
    
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    private bool isAnnoyed = false;

    [Space]
    
    [SerializeField] private Transform targetT;
    [SerializeField] private float lookTargetDistance;
    [SerializeField] private float lookTargetDotFov;

    [SerializeField] private bool isGargantua;
    [SerializeField] private Collider collider;

    [SerializeField] private AudioCastData died;
    
    public bool IsAnnoyed => isAnnoyed;
    public bool IsDie => isDie;

    public bool isAttacking => 
        Vector3.Distance(zombieT.position, targetT.position) <= attackDistance && attackCooldown >= 0 && !IsDie;

    public event Action onAnnoyedChange;
    
    private void Start()
    {
        SetStartTarget();
        void SetStartTarget()
        {
            targetT = PlayerTank.instance.transform;
        }
        
        attackCooldownTimer = attackCooldown;
        
        StartCoroutine(LookTargetUpdater());
        IEnumerator LookTargetUpdater()
        {
            var wait = new WaitForSeconds(0.4f + Random.Range(0f,0.65f));
            
            while (true)
            {
                yield return wait;
                
                if(isDie)
                    break;
                
                if(isAnnoyed)
                    continue;
                
                if(Vector3.Distance(zombieT.position,targetT.position) > lookTargetDistance)
                    continue;
                
                var toTargetDirection = (targetT.position - zombieT.position).normalized;
                
                if (Vector3.Dot(zombieT.forward, toTargetDirection) >= lookTargetDotFov)
                {
                    isAnnoyed = true;
                    AddRb();
                    
                    onAnnoyedChange?.Invoke();
                }
            }
        }

        OnTakeDamage += damage =>
        {
            if (isDie)
                return;
            
            isAnnoyed = true;
            AddRb();
            
            onAnnoyedChange?.Invoke();
        };
        
        targetT.TryGetComponent(out targetHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        collider.enabled = true;
        
        if(other.gameObject.layer == 3)
            AddRb();
    }

    private void Update()
    {
        if (!isAnnoyed || isDie) 
            return;
        
        var toTargetDirection = (targetT.position - zombieT.position).normalized;
        
        void MoveToTarget()
        {
            zombieRb.AddForce(toTargetDirection * Time.deltaTime * speed * 100,ForceMode.Acceleration);
        }
        void RotateToTarget()
        {
            var targetRotationEuler = 
                Quaternion.LookRotation(toTargetDirection).eulerAngles;

            targetRotationEuler.x = 0;
            targetRotationEuler.z = 0;

            zombieT.rotation = 
                Quaternion.RotateTowards(zombieT.rotation,  Quaternion.Euler(targetRotationEuler),
                    Time.deltaTime * rotationSpeed  * 100);
        }

        MoveToTarget();
        RotateToTarget();
        
        if (Vector3.Distance(zombieT.position, targetT.position) > attackDistance)
            return;
        
        void AttackTarget()
        {
            if (targetHealth == null) 
                return;

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
                return;    
            }
            
            targetHealth.TakeDamage(damage);
            attackCooldownTimer = attackCooldown;
        }
        
        AttackTarget();
    }
    
    public override void Died()
    {
        AddRb();
        
        DisableAnnoying();
        void DisableAnnoying()
        {
            StartCoroutine(wait());

            IEnumerator wait()
            {
                yield return new WaitForSeconds(0.5f);
                isAnnoyed = false;
                onAnnoyedChange?.Invoke();
            }
        }

        AudioPoolService.audioPoolServiceInstance.CastAudio(died);
        
        gameObject.layer = 8;
        Destroy(gameObject,30f);
    }

    private void AddRb()
    {
        if(zombieRb == null)
            zombieRb = gameObject.AddComponent<Rigidbody>();

        if (!isGargantua)
            zombieRb.mass = 1;
        else
            zombieRb.mass = 6;
        
        zombieRb.drag = 2;
        zombieRb.angularDrag = 0;
        zombieRb.automaticInertiaTensor = true;

        if (!isGargantua)
            zombieRb.useGravity = false;
        else
            zombieRb.useGravity = true;
        
        zombieRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }
}
