using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public Material skyboxMaterial;
    public bool cycleActive = false, cycleAroundX = false, cycleSunHorizon = true;
    public float speed = 0.1f, sunHeightLimit = 20.0f, exposureModifier = 0.015f;

    private float skyboxAngleZ, skyboxAngleX, skyboxExposure;
    private Color skyboxTintLevel;
    private bool goingUp = true, underLimit, stopAtTop;

    void Start()
    {
        if (skyboxMaterial == null)
            return;

        // skyboxTintLevel = skyboxMaterial.GetColor("_Tint");
        skyboxAngleZ = 0.0f;
        skyboxAngleX = 205.0f;
        skyboxExposure = 1.0f;

        skyboxMaterial.SetFloat("_RotationZ", skyboxAngleZ);
        skyboxMaterial.SetFloat("_RotationX", skyboxAngleX);
        skyboxMaterial.SetFloat("_Exposure", skyboxExposure);
    }

    void LateUpdate()
    {
        if (cycleActive)
        {
            if (skyboxMaterial == null)
                return;

            // skyboxMaterial.SetColor("_Tint", skyboxTintLevel);

            if (cycleSunHorizon)
            {
                skyboxMaterial.SetFloat("_RotationZ", skyboxAngleZ);
                skyboxMaterial.SetFloat("_Exposure", skyboxExposure);
            }

            if (cycleAroundX)
                skyboxMaterial.SetFloat("_RotationX", skyboxAngleX);


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

            skyboxAngleX += Time.deltaTime * speed % 360;

        }
    }

    public void StartSkybox(bool _stopAtTop)
    {
        cycleActive = true;
        stopAtTop = _stopAtTop;
    }
}