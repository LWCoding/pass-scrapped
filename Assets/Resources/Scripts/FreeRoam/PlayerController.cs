using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDirection
{
    Up, Left, Down, Right
}

/*
    A queue of these are stored for each individual following
    character, so that they can mimic the leading player's
    direction and animation.
*/
public struct PlayerPosition
{

    public PlayerDirection direction;
    public string animationName;

    public PlayerPosition(PlayerDirection d, string anim)
    {
        direction = d;
        animationName = anim;
    }

}

public class PlayerController : MonoBehaviour
{

    private List<GameObject> playerObjects = new List<GameObject>();
    private List<Animator> playerAnimators = new List<Animator>();
    private BoxCollider2D boxCollider;
    private const float PLAYER_INTERACT_DISTANCE = 2.5f;
    private const float PLAYER_FOLLOW_THRESHOLD = 180; // How many recorded movements until moving
    private float playerSpeedIncrement;
    public List<PlayerDirection> playerMovements = new List<PlayerDirection>();
    [HideInInspector] public List<GameObject> interactableObjects = new List<GameObject>();
    // The struct inside of the Queue contains the current player's movement (PlayerDirection)
    // and active animation (string).
    private List<Queue<PlayerPosition>> recordedPositions = new List<Queue<PlayerPosition>>();
    [Header("Object Assignments")]
    public GameObject jackObject;
    public GameObject renoObject;
    public GameObject ryanObject;
    [Header("Variable Assignments")]
    public float playerBaseSpeed;
    public float playerRunIncrement;
    public static PlayerController instance;

    /* 
        This function should be called on all player gameObjects. The boolean
        in the second parameter determines whether or not the Update function
        will consider their movements and animations. Set to `false` if they
        are not in the current party.
    */
    private void RegisterPlayer(GameObject obj, bool isInParty)
    {
        if (!isInParty)
        {
            obj.SetActive(false);
            return;
        }
        playerObjects.Add(obj);
        playerAnimators.Add(obj.GetComponent<PlayerData>().playerAnimator);
        // Destroy non-main player's PlayerData script
        // and register new RecordedPositions queue
        if (playerObjects.Count != 1)
        {
            Destroy(obj.GetComponent<PlayerData>());
            recordedPositions.Add(new Queue<PlayerPosition>());
        }
    }

    /*
        This function changes the direction of the BoxCollider2D on the parent
        object for player objects. This makes it so the player can interact 
        depending on what direction they are facing.
    */
    private void ModifyDirection(PlayerDirection dir, bool isMoving = true)
    {
        playerAnimators[0].SetBool("isMoving", isMoving);
        AnimatorStateInfo asi = playerAnimators[0].GetCurrentAnimatorStateInfo(0);
        // if (asi.IsName("MoveBackwards") || asi.IsName("BackwardIdle"))
        // {
        //     boxCollider.offset = new Vector3(0, PLAYER_INTERACT_DISTANCE);
        // }
        // else if (asi.IsName("MoveForwards") || asi.IsName("ForwardIdle"))
        // {
        //     boxCollider.offset = new Vector3(0, -1 * PLAYER_INTERACT_DISTANCE);
        // }
        if (asi.IsName("MoveLeft") || asi.IsName("LeftIdle"))
        {
            boxCollider.offset = new Vector3(-0.5f * PLAYER_INTERACT_DISTANCE, -0.5f * PLAYER_INTERACT_DISTANCE);
        }
        else if (asi.IsName("MoveRight") || asi.IsName("RightIdle"))
        {
            boxCollider.offset = new Vector3(0.5f * PLAYER_INTERACT_DISTANCE, -0.5f * PLAYER_INTERACT_DISTANCE);
        }
    }

    /*
        When the main character enters a trigger (i.e. interaction),
        this should handle what happens when the user presses the
        interact keycode.

        The InteractWith function that may be called in this function
        will wait until the user lifts up the interact keycode.

        HOWEVER, any interactions with a ChangeScene NPC will be done
        automatically.
    */
    public void HandleCollision(GameObject obj)
    {
        InteractableController control = obj.GetComponent<InteractableController>();
        if (control.makePlayerLookBackwards)
        {
            playerAnimators[0].Play("BackwardIdle");
        }
        switch (control.interactableType)
        {
            case InteractableType.NPC:
                StartCoroutine(control.InteractWith(playerObjects[0].transform.position));
                break;
        }
    }

    private void Awake()
    {
        instance = this.GetComponent<PlayerController>();
        // The first in playerAnimators is always the "leader"!
        RegisterPlayer(jackObject, true);
        RegisterPlayer(renoObject, false);
        RegisterPlayer(ryanObject, false);
        Camera.main.transform.SetParent(playerObjects[0].transform);
        // Make all characters face forward by default
        foreach (Animator a in playerAnimators)
        {
            a.Play("ForwardIdle");
        }
    }

    private void Start()
    {
        // Set BoxCollider2D of leader
        boxCollider = playerObjects[0].GetComponent<BoxCollider2D>();
    }

    private float GetSpeed()
    {
        return playerBaseSpeed + playerSpeedIncrement;
    }

    private void Update()
    {
        if (!Globals.canPlayerMove) { return; }
        // Handle interactions (always handle the most recent one first)
        // (first frame on GetKeyDown())
        if (Input.GetKeyDown(Globals.keybinds.interactKeycode) && interactableObjects.Count > 0)
        {
            HandleCollision(interactableObjects[interactableObjects.Count - 1]);
        }
        // Handle running toggle
        // (first and last frame on GetKeyUp() and GetKeyDown())
        if (Input.GetKeyDown(Globals.keybinds.runToggleKeycode))
        {
            playerSpeedIncrement = playerRunIncrement;
            for (int i = 0; i < playerAnimators.Count; i++)
            {
                Animator playerAnimator = playerAnimators[i];
                playerAnimator.speed = 1.4f;
            }
        }
        if (Input.GetKeyUp(Globals.keybinds.runToggleKeycode))
        {
            playerSpeedIncrement = 0;
            for (int i = 0; i < playerAnimators.Count; i++)
            {
                Animator playerAnimator = playerAnimators[i];
                playerAnimator.speed = 1f;
            }
        }
        // Handle directional changes 
        // (first frame on GetKeyDown())
        if (Input.GetKeyDown(Globals.keybinds.leftMoveKeycode) || Input.GetKeyDown(Globals.keybinds.altLeftMoveKeycode))
        {
            ModifyDirection(PlayerDirection.Left);
            playerMovements.Add(PlayerDirection.Left);
        }
        if (Input.GetKeyDown(Globals.keybinds.rightMoveKeycode) || Input.GetKeyDown(Globals.keybinds.altRightMoveKeycode))
        {
            ModifyDirection(PlayerDirection.Right);
            playerMovements.Add(PlayerDirection.Right);
        }
        // if (Input.GetKeyDown(Globals.keybinds.upMoveKeycode) || Input.GetKeyDown(Globals.keybinds.altUpMoveKeycode))
        // {
        //     ModifyDirection(PlayerDirection.Up);
        //     playerMovements.Add(PlayerDirection.Up);
        // }
        // if (Input.GetKeyDown(Globals.keybinds.downMoveKeycode) || Input.GetKeyDown(Globals.keybinds.altDownMoveKeycode))
        // {
        //     ModifyDirection(PlayerDirection.Down);
        //     playerMovements.Add(PlayerDirection.Down);
        // }
        // Handle continuous movement changes 
        // (multiple frames on GetKey())
        if (playerMovements.Count > 0)
        {
            GameObject playerObject = playerObjects[0];
            Animator playerAnimator = playerAnimators[0];
            string animatorName = GetAnimName(playerMovements[0]);
            if (playerMovements.Contains(PlayerDirection.Left))
            {
                MoveMainPlayer(PlayerDirection.Left, animatorName);
            }
            if (playerMovements.Contains(PlayerDirection.Right))
            {
                MoveMainPlayer(PlayerDirection.Right, animatorName);
            }
            // if (playerMovements.Contains(PlayerDirection.Up))
            // {
            //     MoveMainPlayer(PlayerDirection.Up, animatorName);
            // }
            // if (playerMovements.Contains(PlayerDirection.Down))
            // {
            //     MoveMainPlayer(PlayerDirection.Down, animatorName);
            // }
            playerAnimator.Play(animatorName);
            MoveAllies();
        }
        // Handle end of directional changes
        // (last frame on GetKeyUp())
        if (Input.GetKeyUp(Globals.keybinds.leftMoveKeycode) || Input.GetKeyUp(Globals.keybinds.altLeftMoveKeycode))
        {
            Animator playerAnimator = playerAnimators[0];
            playerMovements.Remove(PlayerDirection.Left);
            if (playerMovements.Count == 0)
            {
                StopAnimations();
                ModifyDirection(PlayerDirection.Left, false);
            }
        }
        if (Input.GetKeyUp(Globals.keybinds.rightMoveKeycode) || Input.GetKeyUp(Globals.keybinds.altRightMoveKeycode))
        {
            Animator playerAnimator = playerAnimators[0];
            playerMovements.Remove(PlayerDirection.Right);
            if (playerMovements.Count == 0)
            {
                StopAnimations();
                ModifyDirection(PlayerDirection.Right, false);
            }
        }
        // if (Input.GetKeyUp(Globals.keybinds.upMoveKeycode) || Input.GetKeyUp(Globals.keybinds.altUpMoveKeycode))
        // {
        //     Animator playerAnimator = playerAnimators[0];
        //     playerMovements.Remove(PlayerDirection.Up);
        //     if (playerMovements.Count == 0)
        //     {
        //         StopAnimations();
        //         ModifyDirection(PlayerDirection.Up, false);
        //     }
        // }
        // if (Input.GetKeyUp(Globals.keybinds.downMoveKeycode) || Input.GetKeyUp(Globals.keybinds.altDownMoveKeycode))
        // {
        //     Animator playerAnimator = playerAnimators[0];
        //     playerMovements.Remove(PlayerDirection.Down);
        //     if (playerMovements.Count == 0)
        //     {
        //         StopAnimations();
        //         ModifyDirection(PlayerDirection.Down, false);
        //     }
        // }
    }

    /*
        This function is called when the player is forced to stop moving.
        This may be when dialogue is being displayed (handled by Globals.cs),
        or some other external reason.
    */
    public void ForceStopMoving()
    {
        if (playerMovements.Contains(PlayerDirection.Left))
        {
            Animator playerAnimator = playerAnimators[0];
            playerMovements.Remove(PlayerDirection.Left);
            if (playerMovements.Count == 0) StopAnimations();
        }
        if (playerMovements.Contains(PlayerDirection.Right))
        {
            Animator playerAnimator = playerAnimators[0];
            playerMovements.Remove(PlayerDirection.Right);
            if (playerMovements.Count == 0) StopAnimations();
        }
        // if (playerMovements.Contains(PlayerDirection.Up))
        // {
        //     Animator playerAnimator = playerAnimators[0];
        //     playerMovements.Remove(PlayerDirection.Up);
        //     if (playerMovements.Count == 0) StopAnimations();
        // }
        // if (playerMovements.Contains(PlayerDirection.Down))
        // {
        //     Animator playerAnimator = playerAnimators[0];
        //     playerMovements.Remove(PlayerDirection.Down);
        //     if (playerMovements.Count == 0) StopAnimations();
        // }
    }

    /*
        This function moves the main player and enqueues positions when
        necessary.
    */
    private void MoveMainPlayer(PlayerDirection dir, string animName)
    {
        playerObjects[0].transform.Translate(GetMovement(dir));
        for (int i = 0; i < recordedPositions.Count; i++)
        {
            recordedPositions[i].Enqueue(new PlayerPosition(dir, animName));
        }
    }

    /*
        This function moves all of the following allies a certain direction
        by utilizing the recordedPositions list.
    */
    private void MoveAllies()
    {
        for (int i = 1; i < playerObjects.Count; i++)
        {
            Queue<PlayerPosition> recordedPos = recordedPositions[i - 1];
            // Take the most recent recorded position and get the
            // animation name, and use that as the main animation.
            if (recordedPos.Count > PLAYER_FOLLOW_THRESHOLD * i)
            {
                PlayerPosition pp = recordedPos.Peek();
                playerAnimators[i].Play(pp.animationName);
                playerAnimators[i].SetBool("isMoving", true);
                // While we have over `threshold` movements recorded,
                // dequeue a movement and move the following players
                // in that direction.
                while (recordedPos.Count > PLAYER_FOLLOW_THRESHOLD * i)
                {
                    PlayerPosition pp2 = recordedPos.Dequeue();
                    Vector3 movement = GetMovement(pp2.direction);
                    playerObjects[i].transform.position += movement;
                }
            }
        }
    }

    /*
        This function returns a Vector3 representing the movement offset
        when given a PlayerDirection.
    */
    private Vector3 GetMovement(PlayerDirection dir)
    {
        switch (dir)
        {
            case PlayerDirection.Left:
                return new Vector3(-1 * GetSpeed() * Time.deltaTime, 0, 0);
            case PlayerDirection.Right:
                return new Vector3(GetSpeed() * Time.deltaTime, 0, 0);
                // case PlayerDirection.Up:
                //     return new Vector3(0, GetSpeed() * Time.deltaTime, 0);
                // case PlayerDirection.Down:
                //     return new Vector3(0, -1 * GetSpeed() * Time.deltaTime, 0);
        }
        return new Vector3();
    }

    /*
        This function returns a string representing which animation to
        play when a player is moving a certain direction.
    */
    private string GetAnimName(PlayerDirection dir)
    {
        switch (dir)
        {
            case PlayerDirection.Left:
                return "MoveLeft";
            case PlayerDirection.Right:
                return "MoveRight";
            case PlayerDirection.Up:
                return "MoveBackwards";
            case PlayerDirection.Down:
                return "MoveForwards";
        }
        return "";
    }

    /*
        Stops the animations of the main ally and all allies behind
        them.
    */
    private void StopAnimations()
    {
        for (int i = 0; i < playerAnimators.Count; i++)
        {
            playerAnimators[i].SetBool("isMoving", false);
        }
    }

    private bool IsPlayingAnimation(Animator a)
    {
        return a.GetCurrentAnimatorStateInfo(0).length > a.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name);
    }

}
