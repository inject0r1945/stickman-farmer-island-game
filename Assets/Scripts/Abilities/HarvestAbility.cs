using HCGame.Control;
using HCGame.Core;
using HCGame.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Abilities
{
    public class HarvestAbility : Ability
    {
        [SerializeField] private Transform _harvestSphere;

        private bool _canHarvest;

        public void HarvestingStartCallback()
        {
            _canHarvest = true;
        }

        public void HarvestingEndCallback()
        {
            _canHarvest = false;
        }

        protected override void SubscribeEvents()
        {
            CropField.FullyHarvested += OnFullyAbilityAction;
            CropField.FullyHarvested += OnFullyHarvested;
        }

        protected override void UnsubscribeEvents()
        {
            CropField.FullyHarvested -= OnFullyAbilityAction;
        }

        protected override bool CanUseAbilitByPlayer()
        {
            return PlayerTools.IsCanHarvest;
        }

        protected override bool IsFullyAbilityOnCropField()
        {
            return CurrentCropField.IsAllTilesEmpty;
        }

        protected override bool CanUseAbilityOnCropField()
        {
            return CurrentCropField.HasWateredTiles && !CurrentCropField.HasSownTiles;
        }

        protected override void StartAbilityAnimation()
        {
            PlayerAnimator.PlayHarvestAnimation();
        }

        protected override void StopAbilityAnimation()
        {
            PlayerAnimator.StopHarvestAnimation();
        }

        protected override void DoAfterStartAbilityAnimation()
        {
            if (_canHarvest)
                CurrentCropField.Harvest(_harvestSphere);
        }

        private void OnFullyHarvested(CropField cropField) 
        {
            if (cropField == CurrentCropField)
                CurrentCropField.Reset();
        }
    }
}
