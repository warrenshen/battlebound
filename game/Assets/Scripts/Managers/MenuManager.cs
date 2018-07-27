using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public void LoadBattle()
    {
        Application.LoadLevel("Battle");
    }

    public void LoadCollection()
    {
        Application.LoadLevel("Collection");
    }

    public void LoadMarketplace()
    {
        Application.LoadLevel("Marketplace");
    }
}
