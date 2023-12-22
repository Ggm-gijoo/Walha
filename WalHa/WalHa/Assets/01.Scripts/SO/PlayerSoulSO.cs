using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Icon
{
    CHARACTER = 0,
    SKILL,
    PARRY,
}

[CreateAssetMenu(fileName = "SO/PlayerSoul", menuName ="SO/PlayerSoul")]
public class PlayerSoulSO : ScriptableObject
{
    [SerializeField] private string soulName;
    [SerializeField] private SerializableDictionary<Icon, Sprite> soulSprites;

    public Sprite soulCharIcon { get { return soulSprites[Icon.CHARACTER]; } }
    public Sprite soulSkillIcon { get { return soulSprites[Icon.SKILL]; } }
    public Sprite soulParryIcon { get { return soulSprites[Icon.PARRY]; } }
}
