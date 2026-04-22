
using UnityEngine;
using VContainer.Unity;

public sealed class GamePresenter : IStartable, ITickable
{

    public GamePresenter()
    {
    }

    public void Tick()
    {
    }

    public void Start()
    {
        Debug.Log("GamePresenter Started");
    }
}