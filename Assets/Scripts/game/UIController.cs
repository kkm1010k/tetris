using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIController : NetworkBehaviour
{
    private Button[] buttons;
    private NetworkController networkController;

    private Transform[] Panels;
    
    [SerializeField] private GameObject startButton;
    private void Awake()
    {
        Panels = transform.Cast<Transform>().ToArray();
        
        networkController = FindObjectOfType<NetworkController>();
        buttons = Panels[0].GetComponentsInChildren<Button>();
        
        
        ChangePanel(0);
    }
    
    public void SetActive(bool active)
    {
        foreach (var button in buttons)
        {
            button.interactable = active;
        }
    }

    public void SetCode()
    {
        networkController.SetJoinCode(Panels[0].GetComponentInChildren<TMP_InputField>().text);
    }

    public void SetText(string code)
    {
        Panels[1].GetComponentInChildren<TMP_Text>().text = $"Join Code : {code}";

        if (IsHost)
        {
            startButton.SetActive(true);
        }
    }
    
    public async void CreateRelayBtn()
    {
        SetActive(false);
        
        await networkController.CreateRelay();
        
        ChangePanel(1);
    }
    
    public async void JoinRelayBtn()
    {
        SetActive(false);
        
        await networkController.JoinRelay();
        
        ChangePanel(1);
    }

    private void ChangePanel(int index)
    {
        foreach (var panel in Panels)
        {
            panel.gameObject.SetActive(false);
        }
        Panels[index].gameObject.SetActive(true);
    }
    
    public void LoadScene()
    {
        networkController.LoadGameScene();
    }

    public void SetCount(int count)
    {
        Panels[1].GetComponentsInChildren<TMP_Text>()[1].text = $"Players : {count} / {NetworkController.maxPlayers}";
    }
}
