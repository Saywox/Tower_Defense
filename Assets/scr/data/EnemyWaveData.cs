using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Data")]
public class EnemyWaveData : ScriptableObject
{
    [SerializeField] private int scoutCount;
    [SerializeField] private int trooperCount;
    [SerializeField] private int tankCount;

    public int ScoutCount => scoutCount;
    public int TrooperCount => trooperCount;
    public int TankCount => tankCount;
}
