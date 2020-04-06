using UnityEngine;

[ExecuteInEditMode]
public class SessionHandler : MonoBehaviour
{
#if UNITY_EDITOR
    private TestSceneManager manager;

    private void Update()
    {
        if (Application.isEditor)
        {
            if (manager == null)
                manager = FindObjectOfType<TestSceneManager>();

            if (!Application.isPlaying)
            {
                if (manager != null)
                {
                    if (manager.sessionChecker?.value == true)
                    {
                        manager.sessionChecker.value = false;
                    }
                    if (manager.conditionChecker?.value == true)
                    {
                        manager.conditionChecker.value = false;
                    }
                }
            }
        }
    }
#endif
}