using System;
using UnityEngine;
using YG;

public class PlayerTank : HealthBase, IObserveNum
{
    
    [SerializeField] private Transform tankT;
    [SerializeField] private Rigidbody tankRb;
    [SerializeField] private PlayerTankCombat playerTankCombat;
    [SerializeField] private TankEffects tankEffects;

    [Space]
    
    [SerializeField] private float enginePower;
    [SerializeField] private float rotationPower;
    [SerializeField] private float maxSpeed = 20;
    [SerializeField] private float maxRotation = 15;
    [SerializeField] private float driftModeOnSpeed = 5;
    [SerializeField] private float onFlyMovementSpeedModifier = 0.05f;
    [SerializeField] private bool isMovementBlocked;
    [SerializeField] private float mobileRotationMultiplier = 1f;
    
    public bool IsMovementBlocked => isMovementBlocked;
    
    [Space] 
    
    [SerializeField] private float fuel;
    [SerializeField] private float maxFuel;
    [SerializeField] private float fuelEatSpeed;
    [SerializeField] private float onStopFuelEatMultiplier;

    [Space] 
    
    [SerializeField] private Transform tankHead;
    [SerializeField] private float tankHeadRotationSpeed;
    
    [Space] 
    
    [SerializeField] private Collider[] tankWheelColliders;
    
    [SerializeField] private PhysicMaterial normalMaterial;
    [SerializeField] private PhysicMaterial driftMaterial;

    [Space] 
    
    [SerializeField] private Transform[] groundCheckT;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isFly;
    [SerializeField] private float groundCheckDistance = 0.2f;

    public static PlayerTank instance;
    
    private bool isDriftModOn;
    
    private bool isMobileManageOn;
    private float mobileManageAxisVertical;
    private float mobileManageAxisHorizontal;

    private Vector2 tankHeadLookDirectionMobile;
    private float tankHeadJoystickMagnitude;

    private Transform cameraT;

    public bool IsMobileManageOn => isMobileManageOn;
    public float MobileManageAxisVertical => mobileManageAxisVertical;
    public float MobileManageAxisHorizontal => mobileManageAxisHorizontal;

    public float TankHeadJoystickMagnitude => tankHeadJoystickMagnitude;

    public Transform TankT => tankT;
    public Rigidbody TankRb => tankRb;

    public PlayerTankCombat PlayerTankCombat => playerTankCombat;
    public TankEffects TankEffects => tankEffects;

    public event Action OnTankHit;

    public float Fuel
    {
        get => fuel;

        set
        {
            if(value < 0)
                throw new ArgumentException("VALUE BE LESS THAN ZERO!!!!111");

            if (value > maxFuel)
            {
                fuel = maxFuel;
                return;
            }

            fuel = value;
        }
    }

    public float MaxFuel => maxFuel;
    
    public bool IsFly => isFly;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cameraT = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        var vertical = GetMovementAxis("Vertical");
        var horizontal = GetMovementAxis("Horizontal");
        
        //if (vertical < 0)
            //horizontal *= -1;
        
        float GetMovementAxis(string axis)
        {
            if (!isMobileManageOn)
                return Input.GetAxisRaw(axis);

            switch (axis)
            {
                case "Vertical":
                {
                    var preValue = mobileManageAxisVertical * 2f;


                    if (Mathf.Abs(preValue) <= 0.15f)
                        return 0;

                    preValue = Mathf.Clamp(preValue, -1, 1);
                    return preValue;
                }
                case "Horizontal":
                {
                    var preValue = mobileManageAxisHorizontal;
                    
                    if (Mathf.Abs(preValue) < 0.95f)
                    {
                        preValue *= 1.45f;

                        var clamp = 0.5f;
                        if (Mathf.Abs(preValue) <= clamp)
                            return 0;

                        switch (preValue)
                        {
                            case > 0:
                                preValue -= clamp;
                                break;
                            case < 0:
                                preValue += clamp;
                                break;
                        }

                        preValue *= mobileRotationMultiplier;
                    }
                    else preValue *= 2;
                    
                    preValue = Mathf.Clamp(preValue, -1, 1);
                    return preValue;
                }
            }

            throw new ArgumentException($"Axis \"{axis}\" doesn't exist!" );
        }
        
        CheckFlying();
        void CheckFlying()
        {
            isFly = true;

            if (tankRb.velocity.magnitude < 0.15f)
            {
                isFly = false;
                return;
            }
            
            foreach (var groundCheck in groundCheckT)
                if (groundCheck.Raycast(groundCheck.forward, groundCheckDistance, groundMask).distance != 0)
                {
                    isFly = false;
                    break;
                }
        }
        
        if (isFly)
        {
            vertical *= onFlyMovementSpeedModifier;
            horizontal *= onFlyMovementSpeedModifier;
        }
        
        Movement();
        void Movement()
        {
            Vector3 GetMoveVector()
            {
                var resultMoveVector = Vector3.zero;

                resultMoveVector = tankT.forward * vertical * enginePower * Time.deltaTime * 100;
                
                return resultMoveVector; 
            }

            Vector3 GetRotationVector()
            {
                var resultVector = Vector3.zero;

                resultVector.y = horizontal * rotationPower * Time.deltaTime * 100;

                // bool IsTankMoveBackwards()
                // {
                //     return vertical < 0;
                // }
                //
                // if (IsTankMoveBackwards())
                //     return -resultVector;
                
                return resultVector;
            }
            
            if(isMovementBlocked)
                return;
            
            tankRb.AddForce(GetMoveVector(),ForceMode.Acceleration);
            tankRb.AddTorque(GetRotationVector(),ForceMode.Acceleration);
            
            tankRb.velocity = tankRb.velocity.ClampMagnitude(maxSpeed);
            tankRb.angularVelocity = tankRb.angularVelocity.ClampMagnitude(maxRotation);
        }

        DriftModeChange();
        void DriftModeChange()
        {
            void ChangePhysMaterial(PhysicMaterial physicMaterial)
            {
                foreach (var collider in tankWheelColliders)
                    collider.material = physicMaterial;
            }
            
            bool IsDriftModeOn()
            {
                return horizontal != 0 && tankRb.velocity.magnitude >= driftModeOnSpeed;
            }

            isDriftModOn = IsDriftModeOn();
            
            ChangePhysMaterial(isDriftModOn ? driftMaterial : normalMaterial);
        }

        TankHeadRotation();
        void TankHeadRotation()
        {
            if(!isMobileManageOn)
                ManagePC();
            else
                ManageMobile();    
            
            void ManagePC()
            {
                var direction = 0;

                if (Input.GetKey(KeyCode.Q))
                    direction = -1;
                else if (Input.GetKey(KeyCode.E))
                    direction = 1;
                else return;

                var euler = tankHead.localEulerAngles;
                var smooth = 0.01f;

                tankHead.localEulerAngles =
                    new Vector3(euler.x, euler.y + tankHeadRotationSpeed * smooth * direction, euler.z);
            }

            void ManageMobile()
            {
                var joystickDirection = tankHeadLookDirectionMobile;
                if (joystickDirection.magnitude == 0)
                    return;
                
                var currentRotation = tankHead.localRotation;
                var targetRotation = GetTargetRotation();
                Quaternion GetTargetRotation()
                {
                    var realDirection = 
                        cameraT.forward * joystickDirection.y + cameraT.right * joystickDirection.x;
                    realDirection.y = 0;

                    var tankRotationEuler = tankT.eulerAngles;
                    tankRotationEuler.x = 0;
                    tankRotationEuler.z = 0;

                    return Quaternion.Euler(Quaternion.LookRotation(realDirection).eulerAngles - tankRotationEuler);
                }

                var timeStep = Time.deltaTime * tankHeadRotationSpeed;
                
                tankHead.localRotation = Quaternion.RotateTowards(currentRotation,targetRotation,timeStep);
            }
        }
        
        EatFuel();
        void EatFuel()
        {
            if (fuel <= 0)
            {
                if(!isDie)
                    TakeDamage(Health+1);
                
                return;
            }
            
            var isTankEngineWork = (vertical != 0 || horizontal != 0) && !isFly;
            var smooth = 0.01f;

            var fuelEat = fuelEatSpeed * smooth;

            if (!isTankEngineWork)
                fuelEat *= onStopFuelEatMultiplier;
                
            fuel -= fuelEat;
        }
    }

    public override void Died()
    {
        isMovementBlocked = true;
    }

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        isDie = false;
        health = maxHealth;
        tankRb.velocity = Vector3.zero;
        tankRb.angularVelocity = Vector3.zero;

        fuel = maxFuel;
        isMovementBlocked = false;
        
        tankHead.localEulerAngles = 
            new Vector3(tankHead.localEulerAngles.x,0,tankHead.localEulerAngles.z);

        playerTankCombat.Reset();
        tankEffects.Reset();

        isMobileManageOn = YandexGame.EnvironmentData.isMobile;
        
        OnBarParamChange?.Invoke();
    }

    public float GetNum(int id)
    {
        switch (id)
        {
            case 1:
                return health;
                break;
            
            case 2:
                return fuel;
                break;
            
            case 3:
                return playerTankCombat.GunCooldownTimer; 
                break;
            
            default:
                throw new Exception("This num id is not exist!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTankHit?.Invoke();
    }

    public (float min, float max) GetBarParam(int id)
    {
        switch (id)
        {
            case 1:
                return (0,maxHealth);
            
            case 2:
                return (0,maxFuel);
            
            case 3:
                return (0,playerTankCombat.GunCooldown); 
            
            default:
                throw new Exception("This num id is not exist!");
        }
    }

    public void SetTankCharacteristics(ShopData tankShopData, SavesYG.TankSaveData tankSaveData)
    {
        SetEngine();
        void SetEngine()
        {
            var engineImprovement = tankSaveData.engineImprovement-1;
            
            maxSpeed = tankShopData.EngineParam1PerLevel[engineImprovement];
            maxRotation = tankShopData.EngineParam2PerLevel[engineImprovement];
            
            enginePower *= tankShopData.EngineMultiplier[engineImprovement];
        }

        SetGuns();
        void SetGuns()
        {
            var gunsImprovement = tankSaveData.gunsImprovement-1;
            
            playerTankCombat.SetGunsDamage(tankShopData.GunsParam1PerLevel[gunsImprovement],
                tankShopData.GunsParam2PerLevel[gunsImprovement]);
        }
        
        SetFuel();
        void SetFuel()
        {
            var fuelImprovement = tankSaveData.fuelImprovement-1;

            maxFuel = tankShopData.FuelParam1PerLevel[fuelImprovement];
            fuel = tankShopData.FuelParam1PerLevel[fuelImprovement];
        }
    }
    
    public void SetMobileMovementAxis(string axis,float value)
    {
        if (!isMobileManageOn)
            throw new ArgumentException($"The mobile manage state is not enable!" ); ;

        switch (axis)
        {
            case "Vertical":
            {
                mobileManageAxisVertical = value;
            }
                break;
            case "Horizontal":
            {
                mobileManageAxisHorizontal = value;
            }
                break;
            
            default:
                throw new ArgumentException($"Axis \"{axis}\" doesn't exist!" );
        }
    }

    public void SetMobileTankHeadManageData(Vector2 lookDirection, float joystickMagnitude)
    {
        tankHeadLookDirectionMobile = lookDirection;
        tankHeadJoystickMagnitude = joystickMagnitude;
    }

    public event Action OnBarParamChange;
    public event Action<int,int> OnObserveNumChange;
}
