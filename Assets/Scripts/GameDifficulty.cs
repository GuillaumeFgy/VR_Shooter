using UnityEngine;

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public static class GameDifficulty
{
    public static DifficultyLevel current = DifficultyLevel.Medium;

    public static float spawnInterval = 3.5f;
    public static int maxAlive = 8;

    public static void Apply(DifficultyLevel level)
    {
        current = level;

        switch (level)
        {
            case DifficultyLevel.Easy:
                spawnInterval = 4.0f;  // slower spawns
                maxAlive = 5;          // fewer enemies
                break;

            case DifficultyLevel.Medium:
                spawnInterval = 3.0f;
                maxAlive = 8;
                break;

            case DifficultyLevel.Hard:
                spawnInterval = 2.0f;  // faster spawns
                maxAlive = 12;         // more enemies
                break;
        }

        Debug.Log($"Difficulty set to {current} | spawnInterval={spawnInterval}, maxAlive={maxAlive}");
    }
}
