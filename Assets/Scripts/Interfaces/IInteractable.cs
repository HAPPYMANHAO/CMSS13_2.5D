public interface IInteractable
{
    // 返回是否可以被 handItem 交互（用于 UI 提示）
    bool CanInteract(ItemInstance handItem);

    // 执行交互，handItem 为 null 表示空手
    void Interact(ItemInstance handItem, CurrentPartyMemberInfo interactor);
}