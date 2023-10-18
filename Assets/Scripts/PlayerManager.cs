using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.IO;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Fergicide;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;

    int kills;
    int deaths;
    

    void Awake()
    {
        PV = GetComponent<PhotonView>();    
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), GetSpawnPoints(), Quaternion.identity, 0, new object[] {PV.ViewID});
    }

    Vector3 GetSpawnPoints()
    {
        Vector3[] spawnpoints = new Vector3[4];

        Vector3 spawnpoint1 = new Vector3(-35, 2 ,-35);
        spawnpoints[0] = spawnpoint1;

        Vector3 spawnpoint2 = new Vector3(-35, 6, 35);
        spawnpoints[1] = spawnpoint2;

        Vector3 spawnpoint3 = new Vector3(35, 6, 35);
        spawnpoints[2] = spawnpoint3;

        Vector3 spawnpoint4 = new Vector3(35, 2, -35);
        spawnpoints[3] = spawnpoint4;

        int randNum = Random.Range(0, 4);

        return spawnpoints[randNum];
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
