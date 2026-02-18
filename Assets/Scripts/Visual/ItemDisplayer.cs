using UnityEngine;

public class ItemDisplayer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private float randomMoveUp;
    private float randomStartHorizontal;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private float displayerTimer = 0f;

    private const float minMoveUp = 0.4f;
    private const float maxMoveUp = 0.8f;
    private const float minScale = 0.8f;
    private const float maxMoveHorizontal = 0.5f;
    private const float displayDuration = 0.3f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        displayerTimer = 0f;
        transform.localScale = Vector3.one;

        GetRandomNumber();

        transform.position = startPosition + new Vector3(randomStartHorizontal, 0 , 0);

        endPosition = startPosition + new Vector3(0, randomMoveUp, 0);
    }

    private void Update()
    {
        displayerTimer += Time.deltaTime;
        float progress = displayerTimer / displayDuration;

        transform.position = Vector3.Lerp(transform.position, endPosition, progress);
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * minScale, progress);

        if (displayerTimer >= displayDuration)
        {
            gameObject.SetActive(false);
        }
    }

    private void GetRandomNumber()
    {
        randomMoveUp = Random.Range(minMoveUp, maxMoveUp);
        randomStartHorizontal = Random.Range(-maxMoveHorizontal, maxMoveHorizontal);
    }
}
