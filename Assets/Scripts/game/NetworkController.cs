using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : NetworkBehaviour
{
    public const int maxPlayers = 4;
    public string RelayJoinCode;
    
    public string JoinCode;

    private UIController uiController;
    private Board board;
    private Score score;
    private OtherPlayers otherPlayers;
    
    private async void Awake()
    {
        Application.targetFrameRate = 60;
        
        DontDestroyOnLoad(gameObject);
        
        uiController = FindObjectOfType<UIController>();
        
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        var plr = AuthenticationService.Instance.PlayerId;
        Debug.Log(plr);
    }

    public async Task CreateRelay()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        RelayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        var serverData = new RelayServerData(allocation, "dtls");
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoaded;
        
        CountClientRpc(NetworkManager.Singleton.ConnectedClients.Count, RelayJoinCode);
    }

    public void SetJoinCode(string text)
    {
        JoinCode = text;
    }
    
    public async Task JoinRelay()
    {
        var allocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);
        var serverData = new RelayServerData(allocation, "dtls");
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
        NetworkManager.Singleton.StartClient();
    }
    
    private void OnClientConnected(ulong clientId)
    {
        if (SceneManager.GetActiveScene().name == "TitleMulti")
        {
            CountClientRpc(NetworkManager.Singleton.ConnectedClients.Count, RelayJoinCode);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (SceneManager.GetActiveScene().name == "TitleMulti")
        {
            CountClientRpc(NetworkManager.Singleton.ConnectedClients.Count, RelayJoinCode);
        }
    }

    public void LoadGameScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Tetris", LoadSceneMode.Single);
    }

    private int count;
    
    private void OnLoaded(ulong id,string sceneName,LoadSceneMode mode)
    {
        if (sceneName == "Tetris")
        {
            count++;
            
            Debug.Log(count);
            if (count == NetworkManager.Singleton.ConnectedClients.Count)
            {
                StartGameClientRpc(NetworkManager.Singleton.ServerTime.TimeAsFloat + 3f, NetworkManager.Singleton.ConnectedClientsIds.ToArray(), DateTime.UnixEpoch.Millisecond);
            }
        }
    }
    
    public void LeaveGame()
    {
        NetworkManager.Singleton.Shutdown();
    }

    [ClientRpc]
    public void CountClientRpc(int count, string code, ClientRpcParams clientRpcParams = default)
    {
        uiController.SetCount(count);
        uiController.SetText(code);
    }
    
    [ClientRpc]
    public void StartGameClientRpc(float time, ulong[] ConnectedClientsIds, int seed, ClientRpcParams clientRpcParams = default)
    {
        board = FindObjectOfType<Board>();
        score = FindObjectOfType<Score>();
        otherPlayers = FindObjectOfType<OtherPlayers>();
        
        board.StartGameInit(time, seed);
        otherPlayers.StartGameInit(ConnectedClientsIds);
    }

    [ServerRpc (RequireOwnership = false)]
    public void OnAttackServerRpc(int dmg, ulong id)
    {
        var clients = NetworkManager.Singleton.ConnectedClientsIds.ToArray();
        clients = clients.Where(x => x != id).ToArray();
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = clients
            }
        };
        OnAttackClientRpc(dmg,clientRpcParams);
    }
    
    [ClientRpc]
    private void OnAttackClientRpc(int dmg, ClientRpcParams clientRpcParams = default)
    {
        score.OnSavage(dmg);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void SetTileServerRpc(short[] tilemap, ulong id, ServerRpcParams serverRpcParams = default)
    {
        var clients = NetworkManager.Singleton.ConnectedClientsIds.ToArray();
        clients = clients.Where(x => x != id).ToArray();
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = clients
            }
        };
        
        SetTileClientRpc(tilemap, id, clientRpcParams);
    }
    
    [ClientRpc]
    public void SetTileClientRpc(short[] tilemap, ulong id, ClientRpcParams clientRpcParams = default)
    {
        otherPlayers.SetTile(tilemap,id);
    }
}
