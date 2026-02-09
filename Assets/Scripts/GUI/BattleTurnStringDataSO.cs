using UnityEngine;
using UnityEngine.Localization;
[CreateAssetMenu]
public class BattleStringDataSO : ScriptableObject 
{
        [SerializeField] public LocalizedString playerTurnStartText;
        [SerializeField] public LocalizedString playerEndTurnText;
}
