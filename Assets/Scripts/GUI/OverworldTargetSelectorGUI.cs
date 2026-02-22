using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldTargetSelectorGUI : MonoBehaviour
{
    [SerializeField] private Camera overworldCamera;
    [SerializeField] private LayerMask interactableLayer;

    private RaycastHit[] currentHits;

    public IInteractable currentTarget { get; private set; }
    public Collider currentTargetCollider;

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray mouseRay = overworldCamera.ScreenPointToRay(mousePos);

        currentHits = Physics.RaycastAll(mouseRay, 100f, interactableLayer);
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
                if (currentTarget != interactable)
                {
                    SelectTarget(interactable);
                }
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
