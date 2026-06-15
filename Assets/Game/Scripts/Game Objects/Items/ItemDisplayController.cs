using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDisplayController : ItemController
{
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private Animator anim;
    [SerializeField] private Animator dmgAnimator;
    private Material _material;

    // Dissolve Variables
    private float _dissolveDuration = 0.5f;
    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public AnimatorStateInfo stateInfo;

    private bool _damageBonusAdded= false;

    private void Start()
    {
        _material = icon.material;
        _material.SetFloat(_dissolveAmount, 0f);
    }

    public void ResetDissolve()
    {
        StopCoroutine("Dissolve");
        _material = icon.material;
        _material.SetFloat(_dissolveAmount, 0f);
    }

    public void SetDamageDisplay(int value)
    {
        damageTxt.text = value.ToString();

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
        if (_damageBonusAdded)
        {
            dmgAnimator.SetTrigger("Shake");
            _damageBonusAdded = false;
        }
    }

    public void SetDamageWithBonus(int baseD, UpgradeFightingManager.DamageBonus bonus)
    {
        if (bonus.amount > 0)
        {
            damageTxt.text = $"{baseD}  <color=yellow>+ {bonus.amount} {bonus.source}</color>";
            _damageBonusAdded = true;
        }
        else if (bonus.amount < 0)
        {
            damageTxt.text = $"{baseD}  <color=red>{bonus.amount} {bonus.source}</color>";
            _damageBonusAdded = true;
        }
    }

    public void ResetDisplay()
    {
        damageTxt.gameObject.SetActive(true);
        playableText.gameObject.SetActive(true);
        dmgBanner.SetActive(true);
        playableBanner.SetActive(true);
        ResetDissolve();
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
        if (_material.GetFloat(_dissolveAmount) < 1.0f)
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
            damageTxt.gameObject.SetActive(false);
            playableText.gameObject.SetActive(false);
            dmgBanner.SetActive(false);
            playableBanner.SetActive(false);
        }
    }
}
