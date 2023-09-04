using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveObject
{
    
    public Keybinds keybinds;
    public Inventory inventory;
    public List<Ally> partyMembers;
    public Wallet wallet;
    public DictStringInt dialogueSaves;

}
