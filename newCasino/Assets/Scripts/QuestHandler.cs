using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestHandler : MonoBehaviour
{
    public Quest displayedQuest;
    private Text questTitleText;
    private Text questDescriptionText;
    private void Start()
    {
        this.GetComponent<Image>().sprite = displayedQuest.icon;
        questTitleText = transform.GetChild(0).GetComponent<Text>();
        questTitleText.text = displayedQuest.title;
        questDescriptionText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        questDescriptionText.text = displayedQuest.description;
    }


    public void ActivateDescription()
    {
        StartCoroutine(HideDescription());
    }

    private IEnumerator HideDescription()
    {
        yield return new WaitForSeconds(2f);
        questDescriptionText.transform.parent.gameObject.SetActive(false);
        GetComponent<Button>().interactable = true;
    }
}
