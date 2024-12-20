using UnityEngine;

public class SkillSO : ScriptableObject
{
    public int id; // 스킬 판별 아이디
    public SkillName skillName; // 스킬 이름
    public SkillType skillType; // 스킬 유형 (Attack, Debuff);
    public float power; // 스킬 공격력
    public float range; // 스킬 범위
    public float cooldown; // 스킬 쿨타임
    public float percentage; // 스킬 효과 능력치(%)
    public float duration; // 스킬 효과 지속 시간
}
