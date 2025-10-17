using UnityEngine;

[CreateAssetMenu(menuName = "VR Shooter/Enemy Config", fileName = "EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Core")]
    public int maxHP = 50;
    public float moveSpeed = 1.8f;
    public float turnSpeed = 360f;

    [Header("Offense")]
    public int contactDamage = 10;      // damage if touches the player
    public int projectileDamage = 5;    // damage if shooting
    public float attackRate = 1.2f;     // attacks per second (ranged), or bite rate
    public float attackRangeDistance = 6f;
    public float attackRangeMelee = 1f;
    public float projectileSpeed = 12f;

    [Header("Behavior")]
    public float strafeRadius = 3.5f;
    public float zigZagAmplitude = 1.2f;
    public float zigZagFrequency = 2.2f;
    public float pushBackForce = 4f;
}
