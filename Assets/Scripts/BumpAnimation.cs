using UnityEngine;
using System.Collections;

public class BumpAnimation : MonoBehaviour
{

    public float BUMP_DURATION = 0.15f;
    public float BUMP_PERCENT = 1.3f;
    private float HALF_DURATION;
    private float startBump;
    private bool inAnimation;

    private Vector3 initialScale;

    private Collider collider;

    public bool ManageCollider { set; get; }

    // Use this for initialization
    void Start()
    {
        initialScale = transform.localScale;
        HALF_DURATION = BUMP_DURATION / 2.0f;
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateBump();
    }

    public void StartBump()
    {
        startBump = Time.time;
        inAnimation = true;
        if (collider != null && ManageCollider) collider.enabled = false;
    }

    private void AnimateBump()
    {
        if (inAnimation && Time.time > startBump && Time.time < (startBump + BUMP_DURATION))
        {
            if (Time.time < startBump + HALF_DURATION)
            {
                transform.localScale = Vector3.Lerp(initialScale, initialScale * BUMP_PERCENT, (Time.time - startBump) / HALF_DURATION);
            }
            else
            {
                transform.localScale = Vector3.Lerp(initialScale * BUMP_PERCENT, initialScale, (Time.time - (startBump + HALF_DURATION)) / HALF_DURATION);
            }
        } else {
            if (inAnimation) {
                transform.localScale = initialScale;
                inAnimation = false;
                if (collider != null && ManageCollider) collider.enabled = true;
            }
        }
    }
}
