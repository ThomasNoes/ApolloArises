using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public Material skyboxMaterial;
    public bool cycleActive = false, cycleAroundX = false, cycleSunHorizon = true;
    public float speed = 0.1f, sunHeightLimit = 20.0f, exposureModifier = 0.015f;
    public Color startColor = Color.gray, endColor = Color.blue; 

    private float skyboxAngleZ, skyboxAngleX, skyboxExposure;
    private Color skyboxTintLevel;
    private bool goingUp = true, underLimit, stopAtTop, changingTint;

    void Start()
    {
        if (skyboxMaterial == null)
            return;

        skyboxAngleZ = 0.0f;
        skyboxAngleX = 205.0f;
        skyboxExposure = 1.0f;

        skyboxMaterial.SetFloat("_RotationZ", skyboxAngleZ);
        skyboxMaterial.SetFloat("_RotationX", skyboxAngleX);
        skyboxMaterial.SetFloat("_Exposure", skyboxExposure);
        skyboxMaterial.SetColor("_Tint", startColor);
    }

    void LateUpdate()
    {
        if (cycleActive)
        {
            if (skyboxMaterial == null)
                return;

            if (cycleSunHorizon)
            {
                skyboxMaterial.SetFloat("_RotationZ", skyboxAngleZ);
                skyboxMaterial.SetFloat("_Exposure", skyboxExposure);
            }

            if (cycleAroundX)
            {
                skyboxMaterial.SetFloat("_RotationX", skyboxAngleX);
                skyboxAngleX += Time.deltaTime * speed % 360;
            }

            if (cycleSunHorizon)
            {
                if (goingUp)
                {
                    skyboxAngleZ += Time.deltaTime * speed;
                    skyboxExposure += Time.deltaTime * speed * exposureModifier;

                    if (skyboxAngleZ > sunHeightLimit && !underLimit)
                    {
                        goingUp = false;

                        if (stopAtTop)
                        {
                            cycleActive = false;
                            return;
                        }
                    }

                    if (skyboxAngleZ > 359.9f)
                    {
                        skyboxAngleZ = 0;
                        underLimit = false;
                    }
                }
                else if (!goingUp)
                {
                    skyboxAngleZ -= Time.deltaTime * speed;
                    skyboxExposure -= Time.deltaTime * speed * exposureModifier;

                    if (skyboxAngleZ <= (360 - sunHeightLimit) && underLimit)
                        goingUp = true;

                    if (skyboxAngleZ < 0.1f)
                    {
                        skyboxAngleZ = 360;
                        underLimit = true;
                    }

                }
            }

            if (changingTint)
            {
                skyboxMaterial.SetColor("_Tint", Color.Lerp(startColor, endColor, Time.deltaTime * speed));
            }

        }
    }

    public void StartSkybox(bool _stopAtTop)
    {
        cycleActive = true;
        stopAtTop = _stopAtTop;
    }

    public void StartSkyboxAndTintChange(bool _stopAtTop)
    {
        changingTint = true;
        StartSkybox(_stopAtTop);
    }
}