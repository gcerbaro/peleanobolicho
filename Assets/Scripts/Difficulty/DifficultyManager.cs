﻿using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public enum Difficulty { Easy, Normal, Hard }
    public static Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

    public static float EnemyHealthMultiplier { get; private set; } = 1f;
    public static float EnemyDamageMultiplier { get; private set; } = 1f;
    public static float PlayerDamageMultiplier { get; private set; } = 1f;

    public static void SetDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;

        switch (difficulty)
        {
            case Difficulty.Easy:
                EnemyHealthMultiplier = 0.75f;
                EnemyDamageMultiplier = 0.75f;
                PlayerDamageMultiplier = 1.25f;
                break;
            case Difficulty.Normal:
                EnemyHealthMultiplier = 1f;
                EnemyDamageMultiplier = 1f;
                PlayerDamageMultiplier = 1f;
                break;
            case Difficulty.Hard:
                EnemyHealthMultiplier = 2f;
                EnemyDamageMultiplier = 2f;
                PlayerDamageMultiplier = 0.75f;
                break;
        }
    }
}