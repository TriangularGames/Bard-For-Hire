using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AudienceMemberManager : MonoBehaviour
{
    [Header("Audience Data")]
    [SerializeField] List<AudienceMemberData> memberTypes;
    [SerializeField] int numberOfAudienceMembers;

    [Header("Audience")]
    [SerializeField] List<AudienceMemberData> audienceMembers;
    [SerializeField] int totalScore;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < numberOfAudienceMembers; i++)
        {
            audienceMembers.Add(memberTypes[Random.Range(0, memberTypes.Count)]);
        }

        for (int j = 0; j < numberOfAudienceMembers; j++)
        {
            Debug.Log("Audience Member: " + audienceMembers[j].name);
            totalScore += audienceMembers[j].baseStat;
        }

        Debug.Log("Total Score: " + totalScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
