using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    [Header("Background blur while paused")]
    public bool blurWhenPaused = true;
    [Range(0.5f, 1.5f)] public float blurStrength = 1.5f;

    private bool isPaused = false;

    private Volume blurVolume;
    private VolumeProfile blurProfile;
    private UniversalAdditionalCameraData cameraData;
    private bool cameraHadPostProcessing;

    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (blurWhenPaused)
        {
            CreateBlurVolume();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        SetBlur(true);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SetBlur(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Builds a global post-processing volume that blurs and darkens the 3D scene.
    // The pause canvas is Screen Space - Overlay, which is drawn after post-processing,
    // so the panel and text stay sharp.
    void CreateBlurVolume()
    {
        Camera cam = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
        if (cam == null)
        {
            return;
        }
        cameraData = cam.GetUniversalAdditionalCameraData();

        // The game cameras only read volumes on the Default layer
        GameObject volumeObject = new GameObject("PauseBlurVolume");
        volumeObject.layer = 0;

        blurProfile = ScriptableObject.CreateInstance<VolumeProfile>();

        DepthOfField dof = blurProfile.Add<DepthOfField>();
        dof.mode.Override(DepthOfFieldMode.Gaussian);
        dof.gaussianStart.Override(0f);
        dof.gaussianEnd.Override(1f);
        dof.gaussianMaxRadius.Override(blurStrength);
        dof.highQualitySampling.Override(true);

        ColorAdjustments colorAdjustments = blurProfile.Add<ColorAdjustments>();
        colorAdjustments.postExposure.Override(-0.6f);
        colorAdjustments.saturation.Override(-35f);

        blurVolume = volumeObject.AddComponent<Volume>();
        blurVolume.isGlobal = true;
        blurVolume.priority = 100f;
        blurVolume.sharedProfile = blurProfile;
        blurVolume.enabled = false;
    }

    void SetBlur(bool on)
    {
        if (blurVolume == null || cameraData == null)
        {
            return;
        }

        if (on)
        {
            // Some scenes have post-processing off on the camera; turn it on only while paused
            cameraHadPostProcessing = cameraData.renderPostProcessing;
            cameraData.renderPostProcessing = true;
            blurVolume.enabled = true;
        }
        else
        {
            blurVolume.enabled = false;
            cameraData.renderPostProcessing = cameraHadPostProcessing;
        }
    }

    void OnDestroy()
    {
        if (blurProfile != null)
        {
            Destroy(blurProfile);
        }
    }
}
