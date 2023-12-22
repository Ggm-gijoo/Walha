using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SO/BossSO", menuName ="SO/BossSO")]
public class BossBaseSO : EnemyBaseSO
{
    [SerializeField] private string bossName;
    [SerializeField] private string bossDesc;

    public string BossName { get { return bossName; } }
    public string BossDesc { get { return bossDesc; } }
}
