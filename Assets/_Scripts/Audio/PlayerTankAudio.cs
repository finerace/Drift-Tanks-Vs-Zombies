using System;
using UnityEngine;

public class PlayerTankAudio : MonoBehaviour
{

    private AudioPoolService audioPoolService;
    
    [SerializeField] private PlayerTank tankMain;

    [SerializeField] private TankCrusher tankCrusher;
    private PlayerTankCombat tankCombat;
    private TankEffects tankEffects;
    
    [Space]
    
    [SerializeField] private AudioCastData mainGunShot;
    [SerializeField] private AudioCastData machineGunShot;

    [Space] 
    
    [SerializeField] private AudioCastData tankHit;
    [SerializeField] private AudioCastData boxHit;

    [Space]

    [SerializeField] private AudioSource engineRun;
    [SerializeField] private AudioSource engineIdle;
    [SerializeField] private AudioSource drift;

    private void Start()
    {
        audioPoolService = AudioPoolService.audioPoolServiceInstance;
        tankCombat = tankMain.PlayerTankCombat;
        tankEffects = tankMain.TankEffects;
        
        tankCombat.onGunShot += vector3 =>
        {
            audioPoolService.CastAudio(mainGunShot);
        };

        tankCombat.onMachineGunShot += vector3 =>
        {
            audioPoolService.CastAudio(machineGunShot);
        };

        tankMain.OnTankHit += () =>
        {
            audioPoolService.CastAudio(tankHit);
        };

        tankCrusher.OnCrushEvent += () =>
        {
            audioPoolService.CastAudio(boxHit);
        };
    }

    private void Update()
    {
        EngineRunWork();
        void EngineRunWork()
        {
            engineRun.volume = 0 + GetEngineVolume(0.035f);
            engineRun.pitch = 0 + GetEngineVolume(0.05f);
        }
        
        EngineIdleWork();
        void EngineIdleWork()
        {
            engineIdle.volume = 0.2f + GetEngineVolume(0.015f);
            engineIdle.pitch = 0.5f + GetEngineVolume(0.05f);
        }

        DriftWork();
        void DriftWork()
        {
            drift.pitch = 1f + GetEngineVolume(0.1f);

            drift.volume = tankEffects.IsTankDrifting ? 1 : 0;
        }

        float GetEngineVolume(float cof)
        {
            return tankMain.TankRb.velocity.LengthXZ() * cof;
        }
    }
}
