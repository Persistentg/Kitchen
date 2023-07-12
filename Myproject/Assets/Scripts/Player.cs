using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,IKitchenObjectParent
{
    public static Player Instance { get;private set; }

    public event EventHandler<OnselectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnselectedCounterChangedEventArgs:EventArgs
    {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractionAction += GameInput_OnInteractionAction;
    }

    private void GameInput_OnInteractionAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this );
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if(moveDir!=Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position,lastInteractDir,out RaycastHit raycastHit,interactDistance,counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has BaseCounter
                if (baseCounter != selectedCounter)
                {
                    SetSlectedCounter(baseCounter);
                }
            }
            else
            {
                SetSlectedCounter(null);
            }
            
        }
        else
        {
            SetSlectedCounter(null);
        }
        Debug.Log(selectedCounter);
       
    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            //Cannot move towards moveDir

            //Attempt only x movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x!=0)&&(!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance));//使双键按击仍可转向
            if (canMove)
            {
                //Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                //Cannot move only on the X

                //Attempt only Z element
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove =(moveDir.z!=0)&& (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance));

                if (canMove)
                {
                    //Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //Cannot move in any direction
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
    private void SetSlectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnselectedCounterChangedEventArgs { selectedCounter = selectedCounter });
    }

    
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}