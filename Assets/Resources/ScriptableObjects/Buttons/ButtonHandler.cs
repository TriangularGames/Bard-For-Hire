using UnityEngine;

[CreateAssetMenu(fileName = "ButtonHandler", menuName = "Scriptable Objects/ButtonHandler")]
public class ButtonHandler : ScriptableObject
{
    public void OnClickPerformance()
    {
        SceneLoader.Instance.LoadScene("Performance", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
