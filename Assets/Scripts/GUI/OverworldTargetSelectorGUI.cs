using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldTargetSelectorGUI : MonoBehaviour
{
    [SerializeField] private Camera overworldCamera;
    [SerializeField] private LayerMask combinedMask; //两个可交互层
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask playerLayer;

    private RaycastHit[] currentHits;
    public bool isPlayer { get; private set; } // 标记是否为玩家

    public IInteractable currentTarget { get; private set; }
    public Collider currentTargetCollider;

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray mouseRay = overworldCamera.ScreenPointToRay(mousePos);

        currentHits = Physics.RaycastAll(mouseRay, 100f, combinedMask);
       
        CheckTarget();
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

    public void CheckTarget()
    {
        if (currentHits.Length > 0)
        {
            RaycastHit closestHit = GetClosestHit(currentHits);
            currentTargetCollider = closestHit.collider;
            IInteractable interactable = currentTargetCollider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if(isPlayer = closestHit.collider.gameObject.layer == playerLayer)
                {
                    isPlayer = true;
                }
                else
                {
                    isPlayer = false;
                }
                SelectTarget(interactable);
            }
            else
            {
                isPlayer = false;
                currentTarget = null;
                currentTargetCollider = null;
            }
        }
        else
        {
            currentTarget = null;
            currentTargetCollider = null;
        }
    }


    private void SelectTarget(IInteractable interactable)
    {
        currentTarget = interactable;
    }

    public IInteractable GetCurrentTarget()
    {
        return currentTarget;
    }

    public Collider GetCurrentCollider()
    {
        return currentTargetCollider;
    }
}
