using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemDisplayController : ItemController
{
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private Animator anim;
    private Image _Image;
    private Material _material;

    // Dissolve Variables
    private float _dissolveDuration = 0.5f;
    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Start()
    {
        _Image = transform.GetChild(1).GetComponent<Image>();
        _material = _Image.material;
        _material.SetFloat(_dissolveAmount, 0f);
    }

    public void Reset()
    {
        if (_material != null)
        {
            _material.SetFloat(_dissolveAmount, 0f);
        }
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void Success()
    {
        anim.SetTrigger("Success");
        confetti.Play();
    }

    public void Fail()
    {
        anim.SetTrigger("Fail");
    }

    public void HideImage()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void Disappear()
    {
        StartCoroutine(Dissolve());
    }

    IEnumerator Dissolve()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveDuration)
        {
            elapsedTime += Time.deltaTime;

            var percentage = elapsedTime / _dissolveDuration;

            float lerpedDissolve = Mathf.Lerp(0f, 1.1f, percentage);

            _material.SetFloat(_dissolveAmount, lerpedDissolve);
            yield return null;
        }
        Debug.Log("Dissolve Complete");
    }
}
