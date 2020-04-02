using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class SessionHandler : MonoBehaviour
{
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
                    if (manager.sessionChecker?.value == true)
                    {
                        manager.sessionChecker.value = false;
                    }
            }
        }
    }
}
#endif
