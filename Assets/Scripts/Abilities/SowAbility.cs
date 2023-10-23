using HCGame.Control;
using HCGame.Core;
using HCGame.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Abilities
{
    public class SowAbility : Ability
    {
        protected override void SubscribeEvents()
        {
            CropField.FullySown += OnFullyAbilityAction;
        }

        protected override void UnsubscribeEvents()
        {
            CropField.FullySown -= OnFullyAbilityAction;
        }

        protected override bool CanUseAbilitByPlayer()
        {
            return PlayerTools.IsCanSow;
        }

        protected override bool IsFullyAbilityOnCropField()
        {
            return !CurrentCropField.HasEmptyTiles;
        }

        protected override bool CanUseAbilityOnCropField()
        {
            return CurrentCropField.HasEmptyTiles && !CurrentCropField.HasWateredTiles;
        }

        protected override void StartAbilityAnimation()
        {
            PlayerAnimator.PlaySowAnimation();
        }

        protected override void StopAbilityAnimation()
        {
            PlayerAnimator.StopSowAnimation();
        }

        protected override void AbilityParticlesColliderHandler(Vector3[] particlesPositions)
        {
            CurrentCropField.SeedCollidedCallback(particlesPositions);
        }
    }
}
