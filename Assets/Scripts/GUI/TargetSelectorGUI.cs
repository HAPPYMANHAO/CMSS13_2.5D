using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class TargetSelectorGUI : MonoBehaviour
{
    [SerializeField] private Camera battleCamera;
    [SerializeField] private LayerMask battleEntityLayer;

    public BattleEntityBase currentTarget { get; private set; }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray mouseRay = battleCamera.ScreenPointToRay(mousePos);

        RaycastHit[] hits = Physics.RaycastAll(mouseRay, 100f, battleEntityLayer);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ConfirmTarget(hits);
        }
    }

    private RaycastHit GetClosestHit(RaycastHit[] hits)
    {
        RaycastHit closest = hits[0];
        float minDistance = hits[0].distance;

        for (int i = 1; i < hits.Length; i++)
        {
            if (hits[i].distance < minDistance)
            {
                minDistance = hits[i].distance;
                closest = hits[i];
            }
        }
        return closest;
    }

    private void UpdateHoverTarget()
    {
        //ToDo
    }

    private void ConfirmTarget(RaycastHit[] hits)
    {
        if (hits.Length > 0)
        {
            RaycastHit closestHit = GetClosestHit(hits);

            CharacterBattleVisual visual = closestHit.collider.GetComponentInParent<CharacterBattleVisual>();

            if (visual != null)
            {
                if (currentTarget != visual.battleEntity)
                {
                    SelectTarget(visual.battleEntity);
                }
            }
        }
    }


    private void SelectTarget(BattleEntityBase entity)
    {
        currentTarget = entity;
    }

    public BattleEntityBase GetCurrentTarget()
    {
        return currentTarget;
    }
}
