using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class AbilityTagManager : Singleton<AbilityTagManager>
{
    private string AbilityTagPath = UnityEngine.Application.dataPath + "/Configs/AbilityTag.ini";

    public Dictionary<string, FAbilityTagContainer> TagContainersMap;
    public Dictionary<uint, FAbilityTag> TagsMap;

    public override void Initialize()
    {
        TagContainersMap = new Dictionary<string, FAbilityTagContainer>();
        TagsMap = new Dictionary<uint, FAbilityTag>();

        if (File.Exists(AbilityTagPath))
        {
            using (FileStream file = File.OpenRead(AbilityTagPath))
            {
                using (StreamReader tReader = new StreamReader(file))
                {
                    while (!tReader.EndOfStream)
                    {
                        string tStr = Regex.Replace(tReader.ReadLine(), @"\s", "");
                        AddTag(tStr);
                    }
                }
            }
        }
    }
    public bool GetTag(string inTagName,out FAbilityTag abilityTag)
    {
        if (TagsMap != null && TagsMap.TryGetValue(CRC32.GetCRC32(inTagName), out FAbilityTag tempTag))
        {
            abilityTag = tempTag;
            return true;
        }
        abilityTag = new FAbilityTag();
        return false;
    }
    public bool GetTagContainer(string inTagName,out FAbilityTagContainer tagContainer)
    {
        if (TagContainersMap != null && TagContainersMap.TryGetValue(inTagName, out FAbilityTagContainer tempContainer))
        {
            tagContainer = tempContainer;
            return true;
        }
        tagContainer = new FAbilityTagContainer();
        return false;
    }

    void AddTag(string inStr)
    {
        if (string.IsNullOrEmpty(inStr) || string.IsNullOrWhiteSpace(inStr)) return;

        string[] strs = inStr.Split('.');

        FAbilityTagContainer[] containers = new FAbilityTagContainer[strs.Length];

        FAbilityTag rootTag = new FAbilityTag(strs[0]);
        if(!TagsMap.ContainsKey(rootTag.TagId))
            TagsMap.Add(rootTag.TagId, rootTag);

        for (int i = 0; i < containers.Length; i++)
        {
            containers[i].AddAbilityTag(rootTag);
        }
        if(!TagContainersMap.ContainsKey(rootTag.TagName))
            TagContainersMap.Add(rootTag.TagName, containers[0]);

        FAbilityTag parentTag = rootTag;
        for (int i = 1; i < strs.Length; i++)
        {
            parentTag = new FAbilityTag(strs[i], parentTag);
            if(!TagsMap.ContainsKey(parentTag.TagId))
                TagsMap.Add(parentTag.TagId, parentTag);

            for (int j = i; j < containers.Length; j++)
            {
                containers[j].AddAbilityTag(parentTag);
            }
            if (!TagContainersMap.ContainsKey(parentTag.TagName))
                TagContainersMap.Add(parentTag.TagName, containers[i]);
        }

    }
}