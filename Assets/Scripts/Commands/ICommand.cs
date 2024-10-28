
using UnityEngine;

public interface ICommand
{
    // Executes the skill command by spawning the skill prefab and applying damage to the target.
    void Execute();
}
