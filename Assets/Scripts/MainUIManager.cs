using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class MainUIManager : MonoBehaviour {
    public Slider healthBar;
    public Image SkillBar;
    public Image HPBarFill;
    public GameObject canvas;
    static GameObject CoverPanel;
    public GameObject trailEffect;
    static GameObject dialogBox;
    static GameObject checkBox;
    static GameObject miniCheckBox;
    static GameObject checkBoxInMainGame;
    static GameObject dialogBoxInMainGame;
    static bool isCreatedDialogBox = false;
    bool isShining = false;
    public static MainUIManager instance;

    public Sprite catSkillUI;
    public Sprite dogSkillUI;
    public Sprite fishSkillUI;
    public GameObject UseSkillUI;
    public Image AnimalIconImg;
    Sprite[] AnimalIcon;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start() {
        InitialAnimalIcon();
    }

    // Update is called once per frame
    void Update() {
        if (MainGameManager.getInstance().player != null)
        {
            LerpSkillBar();
            LerpHPBar();
        }
    }

    void LerpSkillBar()
    {
        float value = MainGameManager.getInstance().player.mp / 100f;
        SkillBar.fillAmount = Mathf.Lerp(SkillBar.fillAmount, value, 8f * Time.deltaTime);
    }

    public void OnSkillUsed(string animal)
    {
        Sprite targetSprite = null;
        if (animal == "cat")
        {
            targetSprite = catSkillUI;
        }
        else if (animal == "dog")
        {
            targetSprite = dogSkillUI;
        }
        else
            targetSprite = fishSkillUI;
        Image ui = Instantiate(UseSkillUI, canvas.transform).GetComponent<Image>();
        ui.sprite = targetSprite;
    }

    void LerpHPBar()
    {
        float value = MainGameManager.getInstance().player.hp / 100f;
        healthBar.value = Mathf.Lerp(healthBar.value, value, 8f * Time.deltaTime);
    }

    public void InitialAnimalIcon()
    {
        string animal = EntireGameManager.getInstance().playerData.nowEquipment.animal;
        AnimalIcon = new Sprite[3];
        AnimalIcon[0] = Resources.Load<Sprite>("Character/IconImage/" + animal);
        AnimalIcon[1] = Resources.Load<Sprite>("Character/IconImage/" + animal + "02");
        AnimalIcon[2] = Resources.Load<Sprite>("Character/IconImage/" + animal + "03");
    }

    public void ChangeAnimalIcon(int index)
    {
        AnimalIconImg.sprite = AnimalIcon[index];
    }

    public void UpdateHealthBar(int hp)
    {
        if (hp > healthBar.value * 100f)
        {
            DoHPBarShiningEffect();
        }
        float value = (float)hp / 100f;
        healthBar.value = value;
    }

    public void UpdateSkillBar(int val)
    {
        float value = (float)val / 100f;
        SkillBar.fillAmount = value;
    }

    public void PlayTrailEffect(RectTransform rect)
    {
        TrailController trail = Instantiate(trailEffect).GetComponent<TrailController>();
        trail.Assign(rect);
    }

    public void DoHPBarShiningEffect()
    {
        if (isShining)
            return;
        isShining = true;
        StartCoroutine(shining());
    }

    IEnumerator shining()
    {
        float targetA = 114 / 225;
        while (HPBarFill.color.a > targetA + 0.02f)
        {
            Color color = HPBarFill.color;
            color.a -= Time.deltaTime * 5;
            HPBarFill.color = color;
            yield return null;
        }
        Color colorr = HPBarFill.color;
        colorr.a = targetA;
        HPBarFill.color = colorr;

        while (HPBarFill.color.a < 0.95)
        {
            Color color = HPBarFill.color;
            color.a += Time.deltaTime * 5;
            HPBarFill.color = color;
            yield return null;
        }
        colorr.a = 1;
        HPBarFill.color = colorr;
        isShining = false;
    }



    public static void CreateDialogBox(UnityAction yes, UnityAction no, string descript)
    {
        if (dialogBox == null)
            dialogBox = Resources.Load<GameObject>("Prefabs/DialogBox");
        if (isCreatedDialogBox)
            return;
        isCreatedDialogBox = true;
        GameObject canvas = GameObject.Find("Canvas");
        DialogBox newDialogBox = Instantiate(dialogBox, canvas.transform).GetComponent<DialogBox>();
        newDialogBox.yesButton.onClick.AddListener(yes);
        newDialogBox.yesButton.onClick.AddListener(() => isCreatedDialogBox = false);
        newDialogBox.yesButton.onClick.AddListener(() => Destroy(newDialogBox.gameObject));
        newDialogBox.noButton.onClick.AddListener(no);
        newDialogBox.noButton.onClick.AddListener(() => isCreatedDialogBox = false);
        newDialogBox.noButton.onClick.AddListener(() => Destroy(newDialogBox.gameObject));
        newDialogBox.descriptText.text = descript;
    }

    public static void CreateDialogBox(UnityAction yes, string descript)
    {
        if (dialogBox == null)
            dialogBox = Resources.Load<GameObject>("Prefabs/DialogBox");
        if (isCreatedDialogBox)
            return;
        isCreatedDialogBox = true;
        GameObject canvas = GameObject.Find("Canvas");
        DialogBox newDialogBox = Instantiate(dialogBox, canvas.transform).GetComponent<DialogBox>();
        newDialogBox.yesButton.onClick.AddListener(yes);
        newDialogBox.yesButton.onClick.AddListener(() => isCreatedDialogBox = false);
        newDialogBox.yesButton.onClick.AddListener(() => Destroy(newDialogBox.gameObject));
        newDialogBox.noButton.onClick.AddListener(() => isCreatedDialogBox = false);
        newDialogBox.noButton.onClick.AddListener(() => Destroy(newDialogBox.gameObject));
        newDialogBox.descriptText.text = descript;
    }

    public static void CreateDialogBoxInGame(UnityAction yes, UnityAction no, string descript)
    {
        if (dialogBoxInMainGame == null)
            dialogBoxInMainGame = Resources.Load<GameObject>("Prefabs/MainGameDialogbox");
        if (isCreatedDialogBox)
            return;
        isCreatedDialogBox = true;
        GameObject canvas = GameObject.Find("Canvas");
        DialogBox newDialogBox = Instantiate(dialogBoxInMainGame, canvas.transform).GetComponent<DialogBox>();
        newDialogBox.yesButton.onClick.AddListener(yes);
        newDialogBox.yesButton.onClick.AddListener(() => isCreatedDialogBox = false);
        newDialogBox.yesButton.onClick.AddListener(() => Destroy(newDialogBox.gameObject));
        newDialogBox.noButton.onClick.AddListener(no);
        newDialogBox.noButton.onClick.AddListener(() => isCreatedDialogBox = false);
        newDialogBox.noButton.onClick.AddListener(() => Destroy(newDialogBox.gameObject));
        newDialogBox.descriptText.text = descript;
    }


    public static void CreateCheckBox(UnityAction yes, string descript)
    {
        if (checkBox == null)
            checkBox = Resources.Load<GameObject>("Prefabs/CheckBox");
        GameObject canvas = GameObject.Find("Canvas");
        DialogBox newCheckBox = Instantiate(checkBox, canvas.transform).GetComponent<DialogBox>();
        newCheckBox.yesButton.onClick.AddListener(yes);
        newCheckBox.yesButton.onClick.AddListener(() => Destroy(newCheckBox.gameObject));
        newCheckBox.descriptText.text = descript;
    }

    public static void CreateMainGameCheckBox(UnityAction yes, string descript)
    {
        if (checkBoxInMainGame == null)
            checkBoxInMainGame = Resources.Load<GameObject>("Prefabs/CheckBoxInMainGame");
        GameObject canvas = GameObject.Find("Canvas");
        DialogBox newCheckBox = Instantiate(checkBoxInMainGame, canvas.transform).GetComponent<DialogBox>();
        newCheckBox.yesButton.onClick.AddListener(yes);
        newCheckBox.yesButton.onClick.AddListener(() => Destroy(newCheckBox.gameObject));
        newCheckBox.descriptText.text = descript;
    }

    public static void CreateMiniCheckBox(UnityAction yes, string descript)
    {
        if (miniCheckBox == null)
            miniCheckBox = Resources.Load<GameObject>("Prefabs/MiniCheckBox");
        GameObject canvas = GameObject.Find("Canvas");
        DialogBox newCheckBox = Instantiate(miniCheckBox, canvas.transform).GetComponent<DialogBox>();
        newCheckBox.yesButton.onClick.AddListener(yes);
        newCheckBox.yesButton.onClick.AddListener(() => Destroy(newCheckBox.gameObject));
        newCheckBox.descriptText.text = descript;
    }

    public static Image CreateCoverPanel()
    {
        if (CoverPanel == null)
            CoverPanel = Resources.Load<GameObject>("Prefabs/CoverPanel");
        GameObject canvas = GameObject.Find("Canvas");
        Image newPanel = Instantiate(CoverPanel, canvas.transform).GetComponent<Image>();
        return newPanel;
    }
}
