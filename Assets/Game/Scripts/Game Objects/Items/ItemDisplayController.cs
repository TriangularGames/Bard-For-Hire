using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
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

    public AnimatorStateInfo stateInfo;

    private void Start()
    {
        _Image = transform.GetChild(0).GetComponent<Image>();
        _material = _Image.material;
        _material.SetFloat(_dissolveAmount, 0f);
    }

    protected override void SetDamageTxt()
    {
        base.SetDamageTxt();

        if (int.Parse(damageTxt.text) < 10)
        {
            damageTxt.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 100, 0f);
        }

        if (int.Parse(damageTxt.text) >= 10)
        {
            damageTxt.GetComponent<RectTransform>().anchoredPosition = new Vector3(-10, 100, 0f);
        }

        if (int.Parse(damageTxt.text) >= 100)
        {
            damageTxt.GetComponent<RectTransform>().anchoredPosition = new Vector3(-25, 100, 0f);
        }
    }

    public void Reset()
    {
        if (_material != null)
        {
            _material.SetFloat(_dissolveAmount, 0f);
        }
        transform.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when Item is Scored Successfully
    /// </summary>
    public void Success()
    {
        anim.SetTrigger("Success");
        confetti.Play();

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    }

    /// <summary>
    /// Called when Item Fails to Score
    /// </summary>
    public void Fail()
    {
        anim.SetTrigger("Fail");

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    }

    /// <summary>
    /// At the end of Fail state, Hide the Image
    /// </summary>
    public void HideImage()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// Animation Event during the Fail state
    /// </summary>
    public void Disappear()
    {
        StartCoroutine(Dissolve());
    }

    /// <summary>
    /// Coroutine for the Dissolve Shader
    /// </summary>
    /// <returns></returns>
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
    }
}
