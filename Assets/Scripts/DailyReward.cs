using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{

    public bool initialized;
    public long rewardGivingTicks;
    public GameObject rewardMenu;
    public Text remainigTimeText;

    public void InitializedDailyReward()
    {
        PlayerPrefs.SetString("lastDailyReward", (System.DateTime.Now.Ticks - 864000000000 + 10 * 10000000).ToString());
        if (PlayerPrefs.HasKey("lastDailyReward"))
        {
            rewardGivingTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
            long cuurentTime = System.DateTime.Now.Ticks;
            if(cuurentTime >=rewardGivingTicks)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward();
        }
        initialized = true;
    }


    public void GiveReward()
    {
        LevelController.Current.GiveMoneyToPlayer(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString());
        rewardGivingTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
    }

    void Update()
    {
        if(initialized)
        {
            if (LevelController.Current.startMenu.activeInHierarchy)
            {
                long cuurentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTicks - cuurentTime;
                if(remainingTime <= 0) 
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);
                    remainigTimeText.text = string.Format("{0}:{1}:{2}",timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
                }
            }
        }
    }

    public void TapToReturnButton()
    {
        rewardMenu.SetActive(false);
    }
}
