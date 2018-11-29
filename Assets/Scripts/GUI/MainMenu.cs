using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Panels { MainMenu = 0, SelectVehicle = 1, SelectLevel = 2, Settings = 3 }

public class MainMenu : MonoBehaviour
{
    private int gameScore { get; set; }

    public float cameraRotateSpeed = 5;
    public Animator FadeBackGround;

    public AudioSource menuMusic;
    public Transform vehicleRoot;
    public Material[] allRestMaterials;

    public MenuPanels menuPanels;
    public MenuGUI menuGUI;
    public VehicleSetting[] vehicleSetting;
    public LevelSetting[] levelSetting;


    [System.Serializable]
    public class MenuGUI
    {
        public Text GameScore;
        public Text VehicleName;
        public Text VehiclePrice;

        public Slider VehicleSpeed;
        public Slider VehicleBraking;
        public Slider VehicleNitro;

        public Slider sensitivity;

        public Toggle audio;
        public Toggle music;
        public Toggle vibrateToggle;
        public Toggle ButtonMode, AccelMode;

        public Image wheelColor, smokeColor;
        public Image loadingBar;

        public GameObject loading;
        public GameObject customizeVehicle;
        public GameObject buyNewVehicle;
    }

    [System.Serializable]
    public class MenuPanels
    {
        public GameObject MainMenu;
        public GameObject SelectVehicle;
        public GameObject SelectLevel;
        public GameObject EnoughMoney;
        public GameObject Settings;
    }

    [System.Serializable]
    public class VehicleSetting
    {
        public string name = "Vehicle 1";

        public int price = 20000;

        public GameObject vehicle;
        public GameObject wheelSmokes;

        public Material ringMat, smokeMat;
        public Transform rearWheels;

        public VehiclePower vehiclePower;

        [HideInInspector]
        public bool Bought = false;

        [System.Serializable]
        public class VehiclePower
        {
            public float speed = 80;
            public float braking = 1000;
            public float nitro = 10;
        }
    }

    [System.Serializable]
    public class LevelSetting
    {
        public bool locked = true;
        public Button panel;
        public Text bestTime;
        public Image lockImage;
        public StarClass stars;

        [System.Serializable]
        public class StarClass
        {
            public Image Star1, Star2, Star3;
        }
    }

    private Panels activePanel = Panels.MainMenu;

    private bool vertical, horizontal;
    private Vector2 startPos;
    private Vector2 touchDeltaPosition;
    private float x, y = 0;

    private VehicleSetting currentVehicle;

    private int currentVehicleNumber = 0;
    private int currentLevelNumber = 0;

    private Color mainColor;
    private bool randomColorActive = false;
    private bool startingGame = false;

    private float menuLoadTime = 0.0f;
    private AsyncOperation sceneLoadingOperation = null;


    //ControlMode//////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ControlModeButtons(Toggle value)
    {
        if (value.isOn)
            PlayerPrefs.SetString("ControlMode", "Buttons");
    }
    public void ControlModeAccel(Toggle value)
    {
        if (value.isOn)
            PlayerPrefs.SetString("ControlMode", "Accel");
    }


    public void DisableVibration(Toggle toggle)
    {
        if (toggle.isOn)
            PlayerPrefs.SetInt("VibrationActive", 0);
        else
            PlayerPrefs.SetInt("VibrationActive", 1);
    }

    //Vehcile Color//////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void ActiveCurrentColor(Image activeImage)
    {

        mainColor = activeImage.color;

        if (menuGUI.wheelColor.gameObject.activeSelf)
        {
            vehicleSetting[currentVehicleNumber].ringMat.SetColor("_Color", mainColor);
            PlayerPrefsX.SetColor("VehicleWheelsColor" + currentVehicleNumber, mainColor);
        }
        else if (menuGUI.smokeColor.gameObject.activeSelf)
        {
            vehicleSetting[currentVehicleNumber].smokeMat.SetColor("_TintColor", new Color(mainColor.r, mainColor.g, mainColor.b, 0.2f));
            PlayerPrefsX.SetColor("VehicleSmokeColor" + currentVehicleNumber, new Color(mainColor.r, mainColor.g, mainColor.b, 0.2f));
        }
    }


    public void ActiveWheelColor(Image activeImage)
    {
        randomColorActive = false;

        activeImage.gameObject.SetActive(true);
        menuGUI.wheelColor = activeImage;
        menuGUI.smokeColor.gameObject.SetActive(false);
    }


    public void ActiveSmokeColor(Image activeImage)
    {
        randomColorActive = false;

        activeImage.gameObject.SetActive(true);
        menuGUI.smokeColor = activeImage;
        menuGUI.wheelColor.gameObject.SetActive(false);
    }


    public void OutCustomizeVehicle()
    {
        randomColorActive = false;
        menuGUI.wheelColor.gameObject.SetActive(false);
        menuGUI.smokeColor.gameObject.SetActive(false);
    }


    public void RandomColor()
    {

        randomColorActive = true;

        menuGUI.wheelColor.gameObject.SetActive(false);
        menuGUI.smokeColor.gameObject.SetActive(false);

        vehicleSetting[currentVehicleNumber].ringMat.SetColor("_Color", new Color(Random.Range(0.0f, 1.1f), Random.Range(0.0f, 1.1f), Random.Range(0.0f, 1.1f)));
        vehicleSetting[currentVehicleNumber].smokeMat.SetColor("_TintColor", new Color(Random.Range(0.0f, 1.1f), Random.Range(0.0f, 1.1f), Random.Range(0.0f, 1.1f), 0.2f));

        PlayerPrefsX.SetColor("VehicleWheelsColor" + currentVehicleNumber, vehicleSetting[currentVehicleNumber].ringMat.GetColor("_Color"));
        PlayerPrefsX.SetColor("VehicleSmokeColor" + currentVehicleNumber, vehicleSetting[currentVehicleNumber].smokeMat.GetColor("_TintColor"));
    }


    //Share//////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void SettingActive(bool activePanel)
    {
        menuPanels.Settings.gameObject.SetActive(activePanel);
    }

    public void ClickExitButton()
    {
        Application.Quit();
    }


    //GamePanels//////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void CurrentPanel(int current)
    {

        activePanel = (Panels)current;

        if (currentVehicleNumber != PlayerPrefs.GetInt("CurrentVehicle"))
        {
            currentVehicleNumber = PlayerPrefs.GetInt("CurrentVehicle");

            foreach (VehicleSetting VSetting in vehicleSetting)
            {

                if (VSetting == vehicleSetting[currentVehicleNumber])
                {
                    VSetting.vehicle.SetActive(true);
                    currentVehicle = VSetting;
                }
                else
                {
                    VSetting.vehicle.SetActive(false);
                }
            }
        }

        switch (activePanel)
        {

            case Panels.MainMenu:
                menuPanels.MainMenu.SetActive(true);
                menuPanels.SelectVehicle.SetActive(false);
                menuPanels.SelectLevel.SetActive(false);
                if (menuGUI.wheelColor) menuGUI.wheelColor.gameObject.SetActive(true);

                break;
            case Panels.SelectVehicle:
                menuPanels.MainMenu.gameObject.SetActive(false);
                menuPanels.SelectVehicle.SetActive(true);
                menuPanels.SelectLevel.SetActive(false);
                break;
            case Panels.SelectLevel:
                menuPanels.MainMenu.SetActive(false);
                menuPanels.SelectVehicle.SetActive(false);
                menuPanels.SelectLevel.SetActive(true);
                break;
            case Panels.Settings:
                menuPanels.MainMenu.SetActive(false);
                menuPanels.SelectVehicle.SetActive(false);
                menuPanels.SelectLevel.SetActive(false);
                break;
        }
    }


    //Vehicles Switch//////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void BuyVehicle()
    {
        if ((gameScore >= vehicleSetting[currentVehicleNumber].price) && !vehicleSetting[currentVehicleNumber].Bought)
        {
            PlayerPrefs.SetInt("BoughtVehicle" + currentVehicleNumber.ToString(), 1);
            gameScore -= vehicleSetting[currentVehicleNumber].price;
            if (gameScore <= 0) { gameScore = 1; }
            PlayerPrefs.SetInt("GameScore", gameScore);
            vehicleSetting[currentVehicleNumber].Bought = true;
        }
        else
        {
            menuPanels.EnoughMoney.SetActive(true);
        }
    }



    public void NextVehicle()
    {
        if (menuGUI.wheelColor) { menuGUI.wheelColor.gameObject.SetActive(false); }

        currentVehicleNumber++;
        currentVehicleNumber = (int)Mathf.Repeat(currentVehicleNumber, vehicleSetting.Length);

        foreach (VehicleSetting VSetting in vehicleSetting)
        {

            if (VSetting == vehicleSetting[currentVehicleNumber])
            {
                VSetting.vehicle.SetActive(true);
                currentVehicle = VSetting;
            }
            else
            {
                VSetting.vehicle.SetActive(false);

            }
        }
    }


    public void PreviousVehicle()
    {
        if (menuGUI.wheelColor) { menuGUI.wheelColor.gameObject.SetActive(false); }

        currentVehicleNumber--;
        currentVehicleNumber = (int)Mathf.Repeat(currentVehicleNumber, vehicleSetting.Length);

        foreach (VehicleSetting VSetting in vehicleSetting)
        {
            if (VSetting == vehicleSetting[currentVehicleNumber])
            {
                VSetting.vehicle.SetActive(true);
                currentVehicle = VSetting;
            }
            else
            {
                VSetting.vehicle.SetActive(false);
            }
        }
    }


    //GameSettings//////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void QualitySetting(int quality)
    {
        QualitySettings.SetQualityLevel(quality - 1, true);
        PlayerPrefs.SetInt("QualitySettings", quality);
    }

    public void EditSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity", menuGUI.sensitivity.value);
    }

    public void DisableAudioButton(Toggle toggle)
    {
        if (toggle.isOn)
        {
            AudioListener.volume = 1;
            PlayerPrefs.SetInt("AudioActive", 0);
        }
        else
        {
            AudioListener.volume = 0;
            PlayerPrefs.SetInt("AudioActive", 1);
        }
    }


    public void DisableMusicButton(Toggle toggle)
    {
        if (toggle.isOn)
        {
            menuMusic.GetComponent<AudioSource>().mute = false;
            PlayerPrefs.SetInt("MusicActive", 0);
        }
        else
        {
            menuMusic.GetComponent<AudioSource>().mute = true;
            PlayerPrefs.SetInt("MusicActive", 1);
        }
    }


    public void EraseSave()
    {
        PlayerPrefs.DeleteAll();
        currentVehicleNumber = 0;
        Application.LoadLevel(0);

        foreach (Material mat in allRestMaterials)
            mat.SetColor("_Color", new Color(0.7f, 0.7f, 0.7f));
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void StartGame()
    {
        if (startingGame) return;
        FadeBackGround.SetBool("FadeOut", true);
        StartCoroutine(LoadLevelGame(1.5f));
        startingGame = true;
    }


    IEnumerator LoadLevelGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        menuGUI.loading.SetActive(true);
        StartCoroutine(LoadLevelAsync());

    }

    IEnumerator LoadLevelAsync()
    {

        yield return new WaitForSeconds(0.4f);

        sceneLoadingOperation = Application.LoadLevelAsync(currentLevelNumber + 1);
        sceneLoadingOperation.allowSceneActivation = false;

        while (!sceneLoadingOperation.isDone || sceneLoadingOperation.progress < 0.9f)
        {
            menuLoadTime += Time.deltaTime;

            yield return 0;
        }
    }


    public void currentLevel(int current)
    {

        for (int i = 0; i < levelSetting.Length; i++)
        {
            if (i == current)
            {
                currentLevelNumber = current;
                levelSetting[i].panel.image.color = Color.white;
                levelSetting[i].panel.enabled = true;
                levelSetting[i].lockImage.gameObject.SetActive(false);
                PlayerPrefs.SetInt("CurrentLevelNumber", currentLevelNumber);
            }
            else if (levelSetting[i].locked == false)
            {
                levelSetting[i].panel.image.color = new Color(0.3f, 0.3f, 0.3f);
                levelSetting[i].panel.enabled = true;
                levelSetting[i].lockImage.gameObject.SetActive(false);
            }
            else
            {
                levelSetting[i].panel.image.color = new Color(1.0f, 0.5f, 0.5f);
                levelSetting[i].panel.enabled = false;
                levelSetting[i].lockImage.gameObject.SetActive(true);
            }

            if (levelSetting[i].bestTime)
            {
                if (PlayerPrefs.GetFloat("BestTime" + (i + 1).ToString()) != 0.0f)
                {
                    if (PlayerPrefs.GetInt("LevelStar" + (i + 1)) == 1)
                    {
                        levelSetting[i].stars.Star1.color = Color.white;
                    }
                    else if (PlayerPrefs.GetInt("LevelStar" + (i + 1)) == 2)
                    {
                        levelSetting[i].stars.Star1.color = Color.white;
                        levelSetting[i].stars.Star2.color = Color.white;
                    }
                    else if (PlayerPrefs.GetInt("LevelStar" + (i + 1)) == 3)
                    {
                        levelSetting[i].stars.Star1.color = Color.white;
                        levelSetting[i].stars.Star2.color = Color.white;
                        levelSetting[i].stars.Star3.color = Color.white;
                    }

                    levelSetting[i].bestTime.text = "BEST : " + GetComponent<FormatSecondsScript>().FormatSeconds(PlayerPrefs.GetFloat("BestTime" + (i + 1))).ToString();
                }
            }
        }
    }




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {

        AudioListener.pause = false;
        Time.timeScale = 1.0f;


        menuGUI.vibrateToggle.isOn = (PlayerPrefs.GetInt("VibrationActive") == 0) ? true : false;


        gameScore = 999999;
        CurrentPanel(0);

        if (PlayerPrefs.GetInt("QualitySettings") == 0)
        {
            PlayerPrefs.SetInt("QualitySettings", 4);
            QualitySettings.SetQualityLevel(3, true);
        }
        else
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualitySettings") - 1, true);
        }

        if (PlayerPrefs.GetFloat("Sensitivity") == 0.0f)
        {
            menuGUI.sensitivity.value = 1.0f;
            PlayerPrefs.SetFloat("Sensitivity", 1.0f);
        }
        else
        {
            menuGUI.sensitivity.value = PlayerPrefs.GetFloat("Sensitivity");
        }


        switch (PlayerPrefs.GetString("ControlMode"))
        {
            case "":
                menuGUI.ButtonMode.isOn = true;
                break;
            case "Buttons":
                menuGUI.ButtonMode.isOn = true;
                break;
            case "Accel":
                menuGUI.AccelMode.isOn = true;
                break;
        }


        currentLevelNumber = PlayerPrefs.GetInt("CurrentLevelNumber");

        for (int lvls = 0; lvls < levelSetting.Length; lvls++)
        {
            if (lvls <= PlayerPrefs.GetInt("CurrentLevelUnlocked"))
                levelSetting[lvls].locked = false;

        }


        currentLevel(currentLevelNumber);


        switch (PlayerPrefs.GetString("ControlMode"))
        {
            case "":
                PlayerPrefs.SetString("ControlMode", "Buttons");
                menuGUI.ButtonMode.isOn = true;
                break;
            case "Buttons":
                menuGUI.ButtonMode.isOn = true;
                break;
            case "Accel":
                menuGUI.AccelMode.isOn = true;
                break;
        }


        PlayerPrefs.SetInt("BoughtVehicle0", 1);


        //audio and music Toggle
        menuGUI.audio.isOn = (PlayerPrefs.GetInt("AudioActive") == 0) ? true : false;
        AudioListener.volume = (PlayerPrefs.GetInt("AudioActive") == 0) ? 1.0f : 0.0f;

        menuGUI.music.isOn = (PlayerPrefs.GetInt("MusicActive") == 0) ? true : false;
        menuMusic.mute = (PlayerPrefs.GetInt("MusicActive") == 0) ? false : true;

        currentVehicleNumber = PlayerPrefs.GetInt("CurrentVehicle");
        currentVehicle = vehicleSetting[currentVehicleNumber];


        int i = 0;

        foreach (VehicleSetting VSetting in vehicleSetting)
        {

            if (PlayerPrefsX.GetColor("VehicleWheelsColor" + i) == Color.clear)
            {
                vehicleSetting[i].ringMat.SetColor("_DiffuseColor", Color.white);
            }
            else
            {
                vehicleSetting[i].ringMat.SetColor("_DiffuseColor", PlayerPrefsX.GetColor("VehicleWheelsColor" + i));
            }



            if (PlayerPrefsX.GetColor("VehicleSmokeColor" + i) == Color.clear)
            {
                vehicleSetting[i].smokeMat.SetColor("_TintColor", new Color(0.8f, 0.8f, 0.8f, 0.2f));
            }
            else
            {
                vehicleSetting[i].smokeMat.SetColor("_TintColor", PlayerPrefsX.GetColor("VehicleSmokeColor" + i));
            }



            if (PlayerPrefs.GetInt("BoughtVehicle" + i.ToString()) == 1)
            {
                VSetting.Bought = true;

                if (PlayerPrefs.GetInt("GameScore") == 0)
                {
                    PlayerPrefs.SetInt("GameScore", gameScore);
                }
                else
                {
                    gameScore = PlayerPrefs.GetInt("GameScore");
                }
            }


            if (VSetting == vehicleSetting[currentVehicleNumber])
            {
                VSetting.vehicle.SetActive(true);
                currentVehicle = VSetting;
            }
            else
            {
                VSetting.vehicle.SetActive(false);
            }

            i++;
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Update()
    {


        if (sceneLoadingOperation != null)
        {
            menuGUI.loadingBar.fillAmount = Mathf.MoveTowards(menuGUI.loadingBar.fillAmount, sceneLoadingOperation.progress + 0.2f, Time.deltaTime * 0.5f);

            if (menuGUI.loadingBar.fillAmount > sceneLoadingOperation.progress)
                sceneLoadingOperation.allowSceneActivation = true;
        }


        if (menuGUI.smokeColor.gameObject.activeSelf || randomColorActive)
        {
            vehicleSetting[currentVehicleNumber].rearWheels.Rotate(1000 * Time.deltaTime, 0, 0);
            vehicleSetting[currentVehicleNumber].wheelSmokes.SetActive(true);
        }
        else
        {
            vehicleSetting[currentVehicleNumber].wheelSmokes.SetActive(false);
        }



        menuGUI.VehicleSpeed.value = vehicleSetting[currentVehicleNumber].vehiclePower.speed / 100.0f;
        menuGUI.VehicleBraking.value = vehicleSetting[currentVehicleNumber].vehiclePower.braking / 100.0f;
        menuGUI.VehicleNitro.value = vehicleSetting[currentVehicleNumber].vehiclePower.nitro / 100.0f;
        menuGUI.GameScore.text = gameScore.ToString();


        if (vehicleSetting[currentVehicleNumber].Bought)
        {
            menuGUI.customizeVehicle.SetActive(true);
            menuGUI.buyNewVehicle.SetActive(false);

            menuGUI.VehicleName.text = vehicleSetting[currentVehicleNumber].name;
            menuGUI.VehiclePrice.text = "BOUGHT";
            PlayerPrefs.SetInt("CurrentVehicle", currentVehicleNumber);
        }
        else
        {
            menuGUI.customizeVehicle.SetActive(false);
            menuGUI.buyNewVehicle.SetActive(true);

            menuGUI.VehicleName.text = vehicleSetting[currentVehicleNumber].name;
            menuGUI.VehiclePrice.text = "COST: " + vehicleSetting[currentVehicleNumber].price.ToString();
        }



#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR

        if (Input.GetMouseButton(0) && activePanel != Panels.SelectLevel)
        {
            x = Mathf.Lerp(x, Mathf.Clamp(Input.GetAxis("Mouse X"), -2, 2) * cameraRotateSpeed, Time.deltaTime * 5.0f);
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 50, 60);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 50, Time.deltaTime);
        }
        else {
            x = Mathf.Lerp(x, cameraRotateSpeed * 0.01f, Time.deltaTime * 5.0f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime);
        }


#elif UNITY_ANDROID||UNITY_IOS



        if (Input.touchCount == 1&& activePanel!=Panels.SelectLevel)
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Moved:
                    x = Mathf.Lerp(x, Mathf.Clamp(Input.GetTouch(0).deltaPosition.x, -2, 2) * cameraRotateSpeed, Time.deltaTime*3.0f);
                    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 50, 60);
                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 50, Time.deltaTime);
                    break;
            }

        }
        else {
            x = Mathf.Lerp(x, cameraRotateSpeed * 0.02f, Time.deltaTime*3.0f);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime);
        }

#endif

        transform.RotateAround(vehicleRoot.position, Vector3.up, x);


    }

}
