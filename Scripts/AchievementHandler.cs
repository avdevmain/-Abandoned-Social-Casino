using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHandler : MonoBehaviour
{
    public Achievement displayedAchievement;
    private Text achievementTitleText;
    private Text achievementDescriptionText;
    private void Start()
    {
        this.GetComponent<Image>().sprite = displayedAchievement.icon;
        achievementTitleText = transform.GetChild(0).GetComponent<Text>();
        achievementTitleText.text = displayedAchievement.title;
        achievementDescriptionText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        achievementDescriptionText.text = displayedAchievement.description;
    }


    public void ActivateDescription()
    {
        StartCoroutine(HideDescription());
    }

    private IEnumerator HideDescription()
    {
        yield return new WaitForSeconds(2f);
        achievementDescriptionText.transform.parent.gameObject.SetActive(false);
        GetComponent<Button>().interactable = true;
    }
}
