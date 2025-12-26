using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerTankCombat : MonoBehaviour
{
    private PlayerTank playerTank;
    [SerializeField] private Transform tankT;
    [SerializeField] private Rigidbody tankRb;

    [Space] 
    
    [SerializeField] private Transform headT;
    [SerializeField] private float gunExplosionDamage = 20f;
    [SerializeField] private float gunCooldown = 2f;
    [SerializeField] private float gunExplosionPower = 15f;
    [SerializeField] private float gunExplosionRadius = 10f;
    [SerializeField] private float gunShotDistance = 15f;
    [SerializeField] private LayerMask gunShotLayerMask;
    [SerializeField] private Transform gunShotPoint;
    private float gunCooldownTimer;
    
    [Space]
    
    [SerializeField] private float machineGunDamage = 1;
    [SerializeField] private float machineGunCooldown = 0.25f;
    [SerializeField] private float machineGunShotDistance = 20f;
    [SerializeField] private LayerMask machineGunShotLayerMask;
    [SerializeField] private Transform machineGunShotPoint;
    private float machineGunCooldownTimer;

    public float GunCooldown => gunCooldown;
    public float GunCooldownTimer => gunCooldownTimer;
    
    public float MachineGunCooldown => machineGunCooldown;
    public float MachineGunCooldownTimer => machineGunCooldownTimer;

    public event Action<Vector3> onGunShot;
    public event Action<Vector3> onMachineGunShot;

    private void Start()
    {
        playerTank = GetComponent<PlayerTank>();
    }

    private void Update()
    {
        CooldownTimersWork();
        void CooldownTimersWork()
        {
            if (gunCooldownTimer > 0)
                gunCooldownTimer -= Time.deltaTime;

            if (machineGunCooldownTimer > 0)
                machineGunCooldownTimer -= Time.deltaTime;
        }

        if(!playerTank.IsMobileManageOn)
            ManagePC();
        else
            ManageMobile();
            
        void ManagePC()
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
                UseGun();

            if (Input.GetKey(KeyCode.Space))
                UseMachineGun();
        }
        void ManageMobile()
        {
            var joystickM = playerTank.TankHeadJoystickMagnitude;
            
            if(joystickM >= 0.5f)
                UseMachineGun();
            
            if(joystickM >= 0.95f)
                UseGun();
        }
        
        void UseGun()
        {
            if(gunCooldownTimer > 0)
                return;
            gunCooldownTimer = gunCooldown;
            
            var raycastHit = gunShotPoint.Raycast(gunShotPoint.forward, gunShotDistance, gunShotLayerMask);

            var explosionPos = Vector3.zero;

            if (raycastHit.collider == null)
                explosionPos = gunShotPoint.position + gunShotPoint.forward * gunShotDistance;
            else
                explosionPos = raycastHit.point;
            
            Explosions.Explosion
            (explosionPos,
                gunExplosionPower,gunExplosionRadius,
                gunExplosionDamage,
                0,
                gunShotLayerMask,gunShotLayerMask);
            
            tankRb.AddForce(-gunShotPoint.forward * gunExplosionDamage * gunExplosionPower,ForceMode.Acceleration);
            onGunShot?.Invoke(explosionPos);
        }
        void UseMachineGun()
        {
            if(machineGunCooldownTimer > 0)
                return;
            machineGunCooldownTimer = machineGunCooldown;
            
            var rayCastInfo = machineGunShotPoint.Raycast(machineGunShotPoint.forward, machineGunShotDistance,
                machineGunShotLayerMask);
            
            onMachineGunShot?.Invoke(rayCastInfo.point);
            
            if(rayCastInfo.distance == 0)
                return;

            if(rayCastInfo.collider.TryGetComponent(out IHealth health))
                health.TakeDamage(machineGunDamage);
            
            if (rayCastInfo.collider.gameObject.TryGetComponent(out HealthBase health1))
            {
                var layer = rayCastInfo.collider.gameObject.layer;

                health1.OnDie += AddBonus;
                void AddBonus()
                {
                    var money = PlayerMoneyXpService.instance;
                    var score = LevelScoreCounter.instance;
                                
                    if (layer == 6)
                    {
                        if (RandomChance(150))
                            money.PlayerMoney += Random.Range(1, 11);

                        if (RandomChance(7))
                            money.PlayerDonateMoney += 1;

                        money.PlayerXp += 15;
                    
                        LevelScoreCounter.instance.AddEnemyKilledDestructionScore(25);
                    }
                    else
                    {
                        money.PlayerXp += 5;
                    
                        score.AddEnvironmentDestructionScore(5);
                    }
                }

                bool RandomChance(int chance)
                {
                    var rand = Random.Range(0, 1000);

                    return rand <= chance;
                }
            
            }
            
        }
    }

    public void SetGunsDamage(float main, float second)
    {
        gunExplosionDamage = main;
        machineGunDamage = second;
    }
    
    public void Reset()
    {
        gunCooldownTimer = 0;
        machineGunCooldownTimer = 0;
    }
}
