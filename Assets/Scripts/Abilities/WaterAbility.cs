using HCGame.Control;
using HCGame.Core;
using HCGame.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Abilities
{
    public class WaterAbility : Ability
    {
        protected override void SubscribeEvents()
        {
            CropField.FullyWatered += OnFullyAbilityAction;
        }

        protected override void UnsubscribeEvents()
        {
            CropField.FullyWatered -= OnFullyAbilityAction;
        }

        protected override bool CanUseAbilitByPlayer()
        {
            return PlayerTools.IsCanWater;
        }

        protected override bool IsFullyAbilityOnCropField()
        {
            return !CurrentCropField.HasSownTiles;
        }

        protected override bool CanUseAbilityOnCropField()
        {
            return CurrentCropField.HasSownTiles && !CurrentCropField.HasEmptyTiles;
        }

        protected override void StartAbilityAnimation()
        {
            PlayerAnimator.PlayWaterAnimation();
        }

        protected override void StopAbilityAnimation()
        {
            PlayerAnimator.StopWaterAnimation();
        }

        protected override void AbilityParticlesColliderHandler(Vector3[] particlesPositions)
        {
            CurrentCropField.WaterCollidedCallback(particlesPositions);
        }
    }
}
