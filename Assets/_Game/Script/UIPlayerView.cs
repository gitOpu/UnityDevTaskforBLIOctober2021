using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIPlayerView : MonoBehaviour
{
    public static UIPlayerView instance;
    [Header("Player Table View")]
    public GameObject rowTemplate;
    public GameObject tableContainer;
    public GameObject addPlayerPopup;
    [Header("Add Player")]
    public new TMP_InputField name;
    public TMP_InputField age;
    public TMPro.TMP_Dropdown type;
    public Button addPlayerButton;

    [Header("Search")]
    public TMP_InputField search;
    public GameObject emptyWarning;

    public Button searchButton;

    [Header("Private Filed")]
    private GameObject item;
    private int count;
    public List<Player> availablePlayers;
    private Animator popupFormAnimatior;
    private string fileName = "cricket.json";
    private bool searching;
    public List<Player> players = new List<Player>();

    private void Awake()
    {
        if (instance == null)
            instance = this;

        addPlayerPopup.SetActive(false);
        rowTemplate.SetActive(false);
        popupFormAnimatior = addPlayerPopup.GetComponentInChildren<Animator>();
        addPlayerButton.onClick.AddListener(AddPlayer);
    }

    void Start()
    {
        ReloadPlayersDB();
        LoadPlayersInAvailablePlayers();
        search.onValueChanged.AddListener(delegate { SearchFiledValueChanged(search.text.ToLower()); }); // change value
        searchButton.onClick.AddListener(delegate { SearchFiledValueChanged(search.text.ToLower()); }); // Button Click
         // search.onEndEdit.AddListener(delegate { SearchFiledEndEditing(); }); // Enter
    }
    /* private void SearchFiledEndEditing()
     {
         if(search.text.Length >= 2) searching = true;
          else searching = false;
         LoadPlayersInAvailablePlayers();
     }*/
    private void SearchFiledValueChanged(string searchText)
    {
         
        //if (!string.IsNullOrEmpty(searchText))
        if (searchText.Length >= 2)
        {
            searching = true;
        }
        else if (searchText.Length == 1)
        {
            // restore table
            if (searching)
            {
                searching = false;
            }
        }
        LoadPlayersInAvailablePlayers();
    }
    private void ReloadPlayersDB()
    {
        players = FileHandler.ReadFromJson<Player>(fileName);
    }
    private void LoadPlayersInAvailablePlayers()
    {
        availablePlayers.Clear();
        count = players.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (searching)
                {
                    if (players[i].name.ToLower().Contains(search.text.ToLower()))
                    {
                        availablePlayers.Add(players[i]);
                    }
                }
                else
                {
                    availablePlayers.Add(players[i]);
                }

            }
        }

        LoadAvailablePlayersInTable();
    }

    private void LoadAvailablePlayersInTable()
    {
        ClearExistanceCell();
        int playersCount = availablePlayers.Count;
        int numberOfEmptyCell = 4 - playersCount;
        if (playersCount > 0)
            for (int i = 0; i < playersCount; i++)
            {
                if (rowTemplate != null)
                {
                    item = Instantiate(rowTemplate, tableContainer.transform);
                    item.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = availablePlayers[i].name.ToString(); // Name
                    item.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = availablePlayers[i].age.ToString() + " Years"; // Age
                    item.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = availablePlayers[i].type.ToString().Replace("_", "-"); // Type

                    /*last cell border will be created if cell count is less than 5 */
                    if (i >= 4 && i == playersCount - 1)
                    {
                        item.transform.GetChild(1).gameObject.SetActive(numberOfEmptyCell > 0 ? true : false);
                    }
                    else
                    {
                        item.transform.GetChild(1).gameObject.SetActive(true);
                    }


                    item.SetActive(true);
                }
            }
        emptyWarning.SetActive(playersCount == 0);
        if (numberOfEmptyCell > 0)
        {
            CreateEmptyCell(numberOfEmptyCell);
        }

    }
    private void ClearExistanceCell()
    {
        int rowCount = tableContainer.transform.childCount;
        if (rowCount > 1)
        {
            for (int i = 1; i < rowCount; i++)
            {
                Destroy(tableContainer.transform.GetChild(i).gameObject);
            }
        }
    }
    private void CreateEmptyCell(int numberOfEmptyCell)
    {
        int i = 0;
        while (i < numberOfEmptyCell)
        {
            if (rowTemplate != null)
            {
                i++;
                item = Instantiate(rowTemplate, tableContainer.transform);
                item.transform.GetChild(0).gameObject.SetActive(false);
                item.transform.GetChild(1).gameObject.SetActive(true);
                item.SetActive(true);
            }
        }
    }
    public void AddPlayer()
    {
        bool isAge = int.TryParse(age.text, out int ageValue);
        if (name.text == null || name.text.Length < 2 || IsContainsNumber(name.text))
        {
            Debug.Log($"Name : {name.text}; Type : {type.value.ToString()}->{type.options[type.value].text}");
            popupFormAnimatior.SetTrigger("name_warning");
            return;
        }
        else if (age.text == null || !isAge || ageValue < 9 || ageValue >= 100)
        {
            Debug.Log($"Age : {age.text}; ageValue : {ageValue}; Type : {type.value.ToString()}->{type.options[type.value].text}");
            popupFormAnimatior.SetTrigger("age_warning");
            return;
        }

        players.Add(new Player(name.text, ageValue, (PlayerType)type.value));
        FileHandler.SaveToJson<Player>(players, fileName, (response, isSuccess) =>
        {
            if (isSuccess)
            {
                popupFormAnimatior.SetTrigger("on_success");
                ResetPopupForm();

            }
        });
        ReloadPlayersDB();
        LoadPlayersInAvailablePlayers();

    }
    public void ResetPopupForm() // used by popup cancel button
    {
        name.text = "";
        age.text = "";
        type.value = 0;

    }
    private bool IsContainsNumber(string str)
    {
        foreach (char c in str)
        {
            if (int.TryParse(c.ToString(), out int x)) return true;
        }
        return false;
    }
}
