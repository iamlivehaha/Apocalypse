using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController;
using Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon;
using UnityEngine;

public class Catch : IWeapon
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void ShowAttackEffect()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack(GameObject theTarget)
    {
        throw new System.NotImplementedException();
    }
}
