using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SO/EnemySO", menuName ="SO/EnemySO")]
public class EnemyBaseSO : ScriptableObject
{
    [SerializeField] private float hp;
    [SerializeField] private float spd;
    [SerializeField] private float atk;

    public float Hp { get { return hp; }}
    public float Spd { get { return spd; }}
    public float Atk { get { return atk; }}
}
