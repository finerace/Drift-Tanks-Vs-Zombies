using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class AuxiliaryFunc
{
    
    public static float LengthXZ(this Vector3 b)
    {
        float returnFloat = Mathf.Abs(b.x)+ Mathf.Abs(b.z);
        return returnFloat;
    }

    public static float LengthY(this Vector3 b)
    {
        float returnFloat = Mathf.Abs(b.y);
        return returnFloat;
    }

    public static bool IsLayerInMask(this LayerMask mask, int layer)
    {
        int maskValue = mask.value;
        int layerValue = 1 << layer;

        if (maskValue < layerValue)
            return false;

        if (maskValue == layerValue)
            return maskValue == layerValue;

        int dynamicMaskValue = maskValue;

        for (int i = 30; i >= 0; i--)
        {
            int localMaskNum = 1 << i;

            if (localMaskNum > maskValue)
                continue;

            if (dynamicMaskValue == layerValue)
                return true;

            if (((layerValue * 2) - 1) >= dynamicMaskValue && layerValue <= dynamicMaskValue)
                return true;

            if ((dynamicMaskValue - localMaskNum) < 0)
                continue;

            
            dynamicMaskValue -= localMaskNum;

            if (((layerValue * 2) - 1) >= dynamicMaskValue && layerValue <= dynamicMaskValue)
                return true;
        }

        return false;

    }

    public static float PointDirection_TargetLocalPosDOT(Vector3 targetLocalPos, Vector3 pointDirection)
    {
        return Vector3.Dot(pointDirection.normalized, targetLocalPos.normalized);
    }
    
    public static float ClampToTwoRemainingCharacters(this float target)
    {
        return (int)(target * 100f) / 100f;
    }
    
    public static string ConvertNumCharacters(this int charactersCount)
    {
        var resultString = charactersCount.ToString();

        if (resultString.Length == 1)
        {
            resultString = "0" + resultString;
        }

        return resultString;
    }

    public static TimeSpan ConvertSecondsToTimeSpan(int seconds)
    {
        return new TimeSpan(0, 0, seconds);
    }
    
    public static string ConvertSecondsToTimer(this float seconds)
    {
        var timeSpan = new TimeSpan(0, 0, (int)seconds);

        var milliseconds = 
            ConvertNumCharacters((int)((seconds.ClampToTwoRemainingCharacters() - (int)seconds) * 100));
        
        return $"{ConvertNumCharacters(timeSpan.Minutes)}:" +
               $"{ConvertNumCharacters(timeSpan.Seconds)}:" +
               $"{milliseconds}";
    }

    public static void SetChildsActive(this Transform objectT, bool active)
    {
        for (int i = 0; i < objectT.childCount; i++)
        {
            objectT.GetChild(i).gameObject.SetActive(active);
        }
    }
    
    public static void DeleteChilds(this Transform objectT)
    {
        var count = objectT.childCount;
        
        for (int i = 0; i < count; i++)
        {
            UnityEngine.Object.DestroyImmediate(objectT.GetChild(0).gameObject);
        }
    }

    public static Vector3 ClampMagnitude(this Vector3 vector, float max)
    {
        if (max == 0)
            return vector;
        
        var magnitudeExcess = max / vector.magnitude;

        if (magnitudeExcess < 1)
            return vector * magnitudeExcess;

        return vector;
    }

    public static RaycastHit Raycast(this Transform origin,Vector3 rayDirection,float distance = Mathf.Infinity,int layerMask = -1)
    {
        RaycastHit raycastHitOut;

        if (layerMask >= 0)
        {
            if (Physics.Raycast(origin.position, rayDirection, out raycastHitOut, distance, layerMask))
                return raycastHitOut;
        }
        else 
            if (Physics.Raycast(origin.position, rayDirection, out raycastHitOut, distance))
                return raycastHitOut;
            

        return new RaycastHit();
    }

    public static Vector2 ToVectorXZ(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static string ConvertToString(this float target)
    {
        return $"{(int)target}." +
               $"{((int)((target.ClampToTwoRemainingCharacters() - (int)target) * 100)).ConvertNumCharacters()}";
    }
    
    public static void PlayP(this ParticleSystem particle)
    {
        if(!particle.isEmitting)
            particle.Play();
    }

    public static string ToShortenInt(this int count)
    {
        var countStr = count.ToString();

        switch (countStr.Length)
        {
            default:
                return countStr;
            
            case 4:
                return $"{countStr[0]}.{countStr[1]}{countStr[2]}k";
            case 5:
                return $"{countStr[0]}{countStr[1]}.{countStr[2]}k";
            case 6:
                return $"{countStr[0]}{countStr[1]}{countStr[2]}k";
            case 7:
                return $"{countStr[0]}.{countStr[1]}{countStr[2]}m";
            case 8:
                return $"{countStr[0]}{countStr[1]}.{countStr[2]}m";
            case 9:
                return $"{countStr[0]}{countStr[1]}{countStr[2]}m";
        }
    }

}

public static class Explosions
{

    public static void Explosion(Vector3 explousionPos, float explosionForce, float explosionRadius, float damage
        ,LayerMask wallsLayerMask, LayerMask damageLayerMask, LayerMask forceLayerMask,float upModify = 0.25f)
    {
        float explosionForceSmoothness = 100f;
        float resultExplosionForce = explosionForce * explosionForceSmoothness;

        //??????????? ??????????? ? ???? ?????????
        Collider[] explousionColliders = Physics.OverlapSphere(explousionPos, explosionRadius);

        foreach (var collider in explousionColliders)
        {
            int colliderLayer = collider.gameObject.layer;

            bool forceAllow = forceLayerMask.IsLayerInMask(colliderLayer);
            bool damageAllow = damageLayerMask.IsLayerInMask(colliderLayer);

            if (!forceAllow && !damageAllow)
                continue;

            Rigidbody bodyRb;
            IHealth health;

            Vector3 trueBulletPos = explousionPos;

            RaycastHit hitInfo;
            Vector3 direction = collider.transform.position - trueBulletPos;
            Ray ray = new Ray(trueBulletPos, direction);

            float maxDistance = Vector3.Distance(explousionPos, collider.transform.position);

            Physics.Raycast(ray, out hitInfo, maxDistance, wallsLayerMask);
            bool raycastTest;

            if (hitInfo.transform != null)
            {
                int hitObjHash = hitInfo.transform.gameObject.GetHashCode();

                int colliderObjHash = collider.gameObject.GetHashCode();

                raycastTest = (hitObjHash == colliderObjHash);
            }
            else raycastTest = true;


            if (raycastTest)
            {
                if (damageAllow)
                    if (collider.gameObject.TryGetComponent(out health))
                    {
                        //?????? ?????
                        Vector3 healthPos = collider.transform.position;

                        float distance = Vector3.Distance(explousionPos, healthPos);
                        float resultDamage = damage * (1 - (distance / explosionRadius));

                        if (distance > explosionRadius)
                            resultDamage = 0;
                        
                        if (collider.gameObject.TryGetComponent(out HealthBase health1))
                        {
                            var layer = collider.gameObject.layer;

                            health1.OnDie += AddBonus;
                            void AddBonus()
                            {
                                var money = PlayerMoneyXpService.instance;
                                var score = LevelScoreCounter.instance;
                                
                                if (layer == 6)
                                {
                                    if (RandomChance(150))
                                        money.PlayerMoney += Random.Range(1, 11);

                                    if (RandomChance(2))
                                        money.PlayerDonateMoney += 1;

                                    money.PlayerXp += 25;
                    
                                    LevelScoreCounter.instance.AddEnemyKilledDestructionScore(50);
                                }
                                else
                                {
                                    money.PlayerXp += 10;
                    
                                    score.AddEnvironmentDestructionScore(10);
                                }
                            }

                            bool RandomChance(int chance)
                            {
                                var rand = Random.Range(0, 1000);

                                return rand <= chance;
                            }
            
                        } 
                        
                        health.TakeDamage(resultDamage);
                    }
                
                if (forceAllow)
                    if (collider.gameObject.TryGetComponent(out bodyRb))
                    {
                        bodyRb.AddExplosionForce(resultExplosionForce, explousionPos, explosionRadius, upModify);
                    }
            }
        }
    }

    public static void DirectedExplosion(Vector3 explousionPos, Vector3 explousionDirection,
        float minDot, float explosionForce, float explosionRadius,LayerMask playerShootingLayerMask,float damage = 0, bool DotScale = false)
    {
        float explousionForceSmoothness = 100f;
        float resultExplousionForce = explosionForce * explousionForceSmoothness;
        float upModify = 0.25f;

        //??????????? ??????????? ? ???? ?????????
        Collider[] explousionColliders = Physics.OverlapSphere(explousionPos, explosionRadius);

        foreach (var collider in explousionColliders)
        {
            Rigidbody body;
            IHealth health;
            
            if(collider.isTrigger)
                continue;
            
            //?????? ???????? ?? ??????? ????
            Vector3 trueBulletPos = explousionPos;

            RaycastHit hitInfo;
            Vector3 direction = collider.transform.position - trueBulletPos;
            Ray ray = new Ray(trueBulletPos, direction);

            float maxDistance = Vector3.Distance(explousionPos, collider.transform.position);

            var layerMask =
                playerShootingLayerMask;
            
            Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
            bool raycastTest;

            if (hitInfo.transform != null)
            {
                int hitObjHash = hitInfo.transform.gameObject.GetHashCode();

                int colliderObjHash = collider.gameObject.GetHashCode();

                raycastTest = (hitObjHash == colliderObjHash);
            }
            else raycastTest = true;

            if (raycastTest)
            {
                bool explousionDirectedTest = false;

                Vector3 localObjPos = (collider.transform.position - explousionPos).normalized;
                float dot = Vector3.Dot(explousionDirection.normalized, localObjPos);

                if (dot >= minDot)
                    explousionDirectedTest = true;


                if (explousionDirectedTest)
                {

                    //??? ??????? ????? ???????? ????
                    if (collider.gameObject.TryGetComponent<Rigidbody>(out body))
                    {
                        if (DotScale)
                        {
                            if (dot > 0) resultExplousionForce *= dot;
                            else resultExplousionForce = 0;
                        }

                        body.AddExplosionForce(resultExplousionForce, explousionPos, explosionRadius, upModify);
                    }

                    //??? ??????? ???????? ??????? ????
                    if (collider.gameObject.TryGetComponent<IHealth>(out health))
                    {
                        //?????? ?????
                        Vector3 healthPos = collider.transform.position;

                        float distance = Vector3.Distance(explousionPos, healthPos);
                        float resultDamage = damage * (1 - (distance / explosionRadius));

                        if (DotScale)
                        {
                            if (dot > 0) resultDamage *= dot;
                            else resultDamage = 0;
                        }

                        health.TakeDamage(resultDamage);
                    }
                }
            }
        }
    }

}