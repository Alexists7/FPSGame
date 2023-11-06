using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedManager : MonoBehaviour
{
    [SerializeField] GameObject killFeed;
    [SerializeField] Transform spawn;

    [SerializeField] Sprite[] deathTypes;

    public void KillFeedShown(int deathType, string killer, string enemy)
    {
        GameObject kill = Instantiate(killFeed, spawn);
        kill.transform.SetParent(spawn);

        TMP_Text playerName;
        TMP_Text enemyName;

        Image dImage;

        foreach(Transform child in kill.transform)
        {
            if(child.name == "PName")
            {
                playerName = child.GetComponent<TMP_Text>();
                playerName.text = killer;
            } 
            else if(child.name == "EName")
            {
                enemyName = child.GetComponent<TMP_Text>();
                enemyName.text = enemy;
            }
            else if (child.name == "DImage")
            {
                dImage = child.GetComponent<Image>();
                dImage.sprite = deathTypes[deathType];
            }
        }

        Destroy(kill, 7f);
    }
}
