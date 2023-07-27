using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter :BaseCounter,IKitchenObjectParent
{
    public event EventHandler OnplayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //Player is not carring anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            OnplayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
   
}

