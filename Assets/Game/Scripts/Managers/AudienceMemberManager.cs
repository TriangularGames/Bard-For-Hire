using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudienceMemberManager : MonoBehaviour
{
    [Header("Audience Data")]
    /// <summary>
    /// Audience Member types available for this Performance
    /// </summary>
    [SerializeField] List<AudienceMemberData> memberTypes;

    /// <summary>
    /// Number of Members in the Audience
    /// </summary>
    [SerializeField] int numberOfAudienceMembers;

    [Header("Audience")]
    /// <summary>
    /// Members in the current active Audience
    /// </summary>
    [SerializeField] List<AudienceMemberData> audienceMembers;

    /// <summary>
    /// Total Score value for this Audience
    /// </summary>
    [SerializeField] int totalScore;

    [Header("UI")]
    [SerializeField] TMP_Text scoreToBeat;


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
        scoreToBeat.text = totalScore.ToString();
    }

    /// <summary>
    /// Takes a Value to either add or remove from Total Score
    /// </summary>
    /// <param name="val">Value to add or remove</param>
    public void EditScore(int val)
    {
        totalScore += val;
    }
}
