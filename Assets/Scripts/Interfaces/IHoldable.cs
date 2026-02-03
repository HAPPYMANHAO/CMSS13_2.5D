using UnityEngine;

public interface IHoldable
{
    bool allowUseOfBothHands { get; }

    Sprite holdInLeftHandSprite { get; }
    Sprite holdInRightHandSprite { get; }
    Sprite holdInBothHandsSprite { get; }
}
