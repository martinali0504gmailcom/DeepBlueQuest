using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Config")]
    public LevelsConfig levelsConfig; //Scripitable object listing tutorial level indexes

    [Header("UI References")]
    public GameObject levelButtonPrefab;

    [Header("Main Levels")]
    public Transform mainListParent;

    [Header("Tutorial Levels")]
    public Transform tutorialListParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveManager.LoadGame();
        BuildTutorialList();
        BuildMainList();
    }

    void BuildTutorialList()
    {
        //clear old children in parent
        foreach (Transform child in tutorialListParent)
        {
            Destroy(child.gameObject);
        }

        //for each tutorial index in levelsConfig
        foreach (int tutorialIndex in levelsConfig.tutorialLevels)
        {
            //check if index = 0 or if out of range
            if (tutorialIndex < 1 || tutorialIndex >= SceneManager.sceneCountInBuildSettings)
                continue;

            CreateLevelButton(tutorialListParent, tutorialIndex, true);
        }
    }
    void BuildMainList()
    {
        //clear old children for parent
        foreach (Transform child in mainListParent)
        {
            Destroy(child.gameObject);
        }

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int highestLevelUnlocked = SaveManager.currentData.highestUnlockedLevel;

        //assume that index 0 is the main menu, and skip any indexes that are tutorials
        for (int i = 1; i < sceneCount; i++)
        {
            //if i is a tutorial level, skip it
            if (IsTutorialLevel(i))
                continue;

            bool isUnlocked = (i <= highestLevelUnlocked);
            CreateLevelButton(mainListParent, i, isUnlocked);
        }

    }

    void CreateLevelButton(Transform parent, int buildIndex, bool unlocked)
    {
        GameObject btnObj = Instantiate(levelButtonPrefab, parent);
        Button btn = btnObj.GetComponent<Button>();
        TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.text = "Level " + buildIndex;

        //if locked, disable
        btn.interactable = unlocked;

        if (unlocked)
        {
            int localIndex = buildIndex;
            btn.onClick.AddListener(() => LoadLevel(localIndex));
        } 
    }

    bool IsTutorialLevel(int index)
    {
        foreach (int t in levelsConfig.tutorialLevels)
        {
            if (t == index) return true;
        }
        return false;
    }

    public void LoadLevel (int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
