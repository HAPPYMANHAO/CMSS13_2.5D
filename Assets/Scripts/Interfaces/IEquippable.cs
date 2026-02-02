using UnityEngine;

public interface IEquippable
{
    void OnEquip(CurrentPartyMemberInfo character);
    void OnUnEquip(CurrentPartyMemberInfo character);
}
