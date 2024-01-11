using UnityEngine;

public class BrokenRing : MonoBehaviour
{
    private ParticleSystemRenderer brokenEffectRenderer;
    [SerializeField] private ParticleSystem brokenEffect;

    private void Awake()
    {
        brokenEffectRenderer = brokenEffect.GetComponent<ParticleSystemRenderer>();
    }

    public void PlayEffect(Color color)
    {
        brokenEffectRenderer.material = GameManager.Instance.ParticleMaterial(color);
        brokenEffect.Play();
    }
}
