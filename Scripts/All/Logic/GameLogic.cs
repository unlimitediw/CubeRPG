using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic{

	//升级经验计算
    public static float ExperienceForNextLevel(int currentLevel)
    {
        if (currentLevel == 0) return 0;
        return (currentLevel * currentLevel * 100 + currentLevel * 200 + currentLevel * currentLevel * currentLevel + 99);
    }

    //基础攻击模块
    public static float CalculatePlayerBaseAttackDamage(MyRpgController myRpgController)
    {
        float baseDamage = myRpgController.strength + Mathf.Floor(myRpgController.strength / 10) * 3;
        return baseDamage;
    }

    public static float CalculatePlayerBaseAttackSpeed(MyRpgController myRpgController)
    {
        float baseAttackSpeed = myRpgController.agility * 0.02f; 
        return baseAttackSpeed;
    }
}
