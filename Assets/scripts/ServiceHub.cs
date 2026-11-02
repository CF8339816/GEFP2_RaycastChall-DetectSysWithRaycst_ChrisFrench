using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServiceHub : MonoBehaviour
{

    public static ServiceHub Instance { get; private set; }


    [Header("System References")]
    public playercontroler playerController;
 

    private void Awake()
    {
        #region Singleton Pattern


        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        #endregion
    }


}
