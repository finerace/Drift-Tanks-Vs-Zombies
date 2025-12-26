using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TankCrusher : MonoBehaviour
{
    [SerializeField] private Transform tankT;
    [SerializeField] private Rigidbody tankRb;

    [SerializeField] private float toCrushNeededPower = 10;
    [SerializeField] private float crushDamage;
    [SerializeField] private float crushPower;
    [SerializeField] private bool toCrusherRotation = true;
    
    private PlayerMoneyXpService playerMoneyXpService = PlayerMoneyXpService.instance;
    private LevelScoreCounter levelScores = LevelScoreCounter.instance;

    public event Action OnCrushEvent;
    
    private void Start()
    {
        playerMoneyXpService = PlayerMoneyXpService.instance;
        levelScores = LevelScoreCounter.instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Crush(collision.transform);
        //OnCrushEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        Crush(other.transform);
        OnCrushEvent?.Invoke();
    }

    void Crush(Transform objectT)
    {
        if(tankRb.velocity.magnitude < toCrushNeededPower)
            return;

        if (objectT.gameObject.TryGetComponent(out Rigidbody rb))
        {
            var punchDirection = -(tankT.position - rb.transform.position).normalized;
            rb.AddForce((punchDirection * crushPower) + tankRb.velocity,ForceMode.Impulse);

            if(toCrusherRotation)
                RotateTargetToTank();
            void RotateTargetToTank()
            {
                var toTankLookRotation = Quaternion.LookRotation(-punchDirection).eulerAngles;

                toTankLookRotation.x = 0;
                toTankLookRotation.z = 0;

                rb.transform.rotation = Quaternion.Euler(toTankLookRotation);
            }
        }

        if (objectT.gameObject.TryGetComponent(out HealthBase health))
        {
            health.TakeDamage(crushDamage);
            var layer = objectT.gameObject.layer;

            health.OnDie += AddBonus;
            void AddBonus()
            {
                if (layer == 6)
                {
                    if (RandomChance(250))
                        playerMoneyXpService.PlayerMoney += Random.Range(5, 26);

                    if (RandomChance(2))
                        playerMoneyXpService.PlayerDonateMoney += 1;

                    playerMoneyXpService.PlayerXp += 25;
                    
                    levelScores.AddEnemyKilledDestructionScore(50);
                }
                else
                {
                    playerMoneyXpService.PlayerXp += 10;
                    
                    levelScores.AddEnvironmentDestructionScore(10);
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
