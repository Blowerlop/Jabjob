using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject scoreBoard;
    [SerializeField] GameObject playerStatsList;
    [SerializeField] GameObject playerStatsPrefab;
    [SerializeField] int nbPlayer = 1;

    private void Awake()
    {
        for (int i = 0; i < nbPlayer; i++)
        {
            Instantiate(playerStatsPrefab, playerStatsList.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }
}
