using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class GPCNetworkManager : MonoBehaviour
{
	private static GPCNetworkManager _instance;

  private string playerID;

  public static GPCNetworkManager Instance
  {
		get {
			if (_instance == null)
      {
				Debug.LogError("Input manager is null");
			}
			return _instance;
		}
	}

  private void Awake()
  {
		_instance = this;

    string[] args = System.Environment.GetCommandLineArgs();
    string arg = "";
    for (int i = 0; i < args.Length; i++)
    {
      Debug.Log("ARG " + i + ": " + args[i]);
      if (args[i] == "-joinCode")
      {
        arg = args[i + 1];
      }
    }
    if (arg.Length > 0)
    {
      StartClient(arg);
      UIManager.Instance.joinCode = arg; // display joincode
    } else
    {
      StartHost();
    }
  }

  public async void StartHost()
  {
    await Authenticate();
    await StartNGOHost();

    InputManager.Instance.OnBecomeClient();
  }

  private async Task StartNGOHost()
  {
    var (ipv4address, port, allocationIdBytes, connectionData, key, joinCode) = await AllocateRelayServerAndGetJoinCode(8);
    GUIUtility.systemCopyBuffer = joinCode; // copy to clipboard
    UIManager.Instance.joinCode = joinCode; // display joincode

    NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipv4address, port, allocationIdBytes, key, connectionData, true);
    NetworkManager.Singleton.StartHost();
  }

  public async void StartClient(string joinCode)
  {
    await Authenticate();
    await StartNGOClient(joinCode);
    InputManager.Instance.OnBecomeClient();
  }

  private async Task StartNGOClient(string joinCode)
  {
    var (ipv4address, port, allocationIdBytes, connectionData, hostConnectionData, key) = await JoinRelayServerFromJoinCode(joinCode);
    NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipv4address, port, allocationIdBytes, key, connectionData, hostConnectionData, true);
    NetworkManager.Singleton.StartClient();
  }

  private async Task Authenticate()
  {
    try
    {
      await UnityServices.InitializeAsync();
      await AuthenticationService.Instance.SignInAnonymouslyAsync();
      playerID = AuthenticationService.Instance.PlayerId;
    }
    catch (Exception e)
    {
      Debug.Log(e);
    }
  }

  private static async Task<(
    string ipv4address,
    ushort port, byte[] allocationIdBytes,
    byte[] connectionData,
    byte[] key,
    string joinCode
  )> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
  {
    Allocation allocation;
    string createJoinCode;
    try
    {
      allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
    }
    catch (Exception e)
    {
      Debug.LogError($"Relay create allocation request failed {e.Message}");
      throw;
    }

    Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
    Debug.Log($"server: {allocation.AllocationId}");

    try
    {
      createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    }
    catch
    {
      Debug.LogError("Relay create join code request failed");
      throw;
    }

    var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");
    return (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.Key, createJoinCode);
  }

  public static async Task<(
    string ipv4address,
    ushort port,
    byte[] allocationIdBytes,
    byte[] connectionData,
    byte[] hostConnectionData,
    byte[] key
  )> JoinRelayServerFromJoinCode(string joinCode)
  {
    JoinAllocation allocation;
    try
    {
      allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
    }
    catch
    {
      Debug.LogError("Relay create join code request failed");
      throw;
    }

    Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
    Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
    Debug.Log($"client: {allocation.AllocationId}");

    var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");
    return (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.HostConnectionData, allocation.Key);
  }
}