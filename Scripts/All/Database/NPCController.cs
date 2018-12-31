using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NPCController : MonoBehaviour {

    private Quest quest;
    
    public Text QuestNameText;
    public Text QuestDescriptionText;
    public Text QuestTaskText;
    public Text QuestRewardText;

    public Transform info;


    private void Start()
    {
        SetQuestExample();
        ShowQuestInfo();
        print(JsonUtility.ToJson(quest));
    }

    void SetQuestExample()
    {
        quest = new Quest();
        quest.questName = "Mummies Everywhere";
        quest.description = "These Mummies want to kill you!";
        quest.reward = new Quest.Reward();
        quest.reward.exp = 400;
        quest.reward.money = 50;
        quest.task = new Quest.Task();
        quest.task.kills = new Quest.QuestKill[2];
        quest.task.kills[0] = new Quest.QuestKill();
        quest.task.kills[0].id = 0;
        quest.task.kills[0].amount = 10;
        quest.task.kills[1] = new Quest.QuestKill();
        quest.task.kills[1].id = 1;
        quest.task.kills[1].amount = 55;
    }

    public void ShowQuestInfo()
    {
        QuestNameText.text = quest.questName;
        QuestDescriptionText.text = quest.description;

        string taskString = "任务：\n";
        if(quest.task.kills != null)
        {
            foreach(Quest.QuestKill qk in quest.task.kills)
            {
                taskString += "击杀 " + qk.amount + " 只" + EnemyDatabase.Enemies[qk.id] + "。\n";
            }
        }
        if(quest.task.items != null)
        {
            foreach(Quest.QuestItem qi in quest.task.items)
            {
                taskString += "收集 " + qi.amount + " 个" + ItemDatabase.items[qi.id] + "。\n";
            }
        }
        if(quest.task.talkTo != null)
        {
            foreach(int id in quest.task.talkTo)
            {
                taskString += "找到 " + NPCDatabase.Npcs[id] + "。\n";
            }
        }
        QuestTaskText.text = taskString;

        string rewardString = "奖励：\n";
        if(quest.reward.items != null)
        {
            foreach(Quest.QuestItem qi in quest.reward.items)
            {
                rewardString += ItemDatabase.items[qi.id] + " *" + qi.amount + "。\n";
            }
        }
        if (quest.reward.exp > 0) rewardString += quest.reward.exp + " 经验值。\n";
        if (quest.reward.money > 0) rewardString += quest.reward.money + " 金币。\n";
        QuestRewardText.text = rewardString;

    }
}
