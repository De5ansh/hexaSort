using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Collider collider;
    public HexStack hex { get; private set; }

    private rocketPowerUp rocketPowerUpScript; // Add reference to rocket power-up script

    public Color color
    {
        get => renderer.material.color;
        set => renderer.material.color = value;
    }

    void Start()
    {
        rocketPowerUpScript = FindObjectOfType<rocketPowerUp>(); // Find the rocket power-up script in the scene
    }

    public void Configure(HexStack hexStack)
    {
        hex = hexStack;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void MoveLocal(Vector3 targetPos)
    {
        LeanTween.cancel(gameObject);
        float delay = transform.GetSiblingIndex() * .01f;
        LeanTween.moveLocal(gameObject, targetPos, .2f).setEase(LeanTweenType.easeInOutSine).setDelay(delay);

        Vector3 direction = (targetPos - transform.localPosition).With(y: 0).normalized;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        LeanTween.rotateAround(gameObject, rotationAxis, 180, 0.2f).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
    }

    public void Vanish(float delay)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.zero, .2f).setEase(LeanTweenType.easeInBack).setDelay(delay).setOnComplete(() => Destroy(gameObject));
    }

    public void DisableCollider() => collider.enabled = false;

    public void EnableCollider()
    {
        // Enable collider if power-up is active
        collider.enabled = true;
    }
}
