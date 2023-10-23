using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCGame.Environment;
using HSVStudio.Tutorial;

namespace HCGame.Tutorials
{
    public class FarmingStagesTutorialHandler : MonoBehaviour
    {
        [SerializeField] private HSVTutorialManager _previousStageTutorialManager;
        [SerializeField] private HSVTutorialManager _nextStageTutorialManager;
        [SerializeField] private FarmingStage _triggerStage;

        private enum FarmingStage { Sow, Water, Harvest }

        private void OnEnable()
        {
            CropField.FullySown += x => OnFullySown();
            CropField.FullyWatered += x => OnFullyWatered();
            CropField.FullyHarvested += x => OnFullyHarvested();
        }

        private void OnDisable()
        {
            CropField.FullySown -= x => OnFullySown();
            CropField.FullyWatered -= x => OnFullyWatered();
            CropField.FullyHarvested -= x => OnFullyHarvested();
        }

        private void OnFullySown()
        {
            if (_triggerStage == FarmingStage.Sow)
                StartCoroutine(StartTutorial());
        }

        private void OnFullyWatered()
        {
            if (_triggerStage == FarmingStage.Water)
                StartCoroutine(StartTutorial());
        }

        private void OnFullyHarvested()
        {
            if (_triggerStage == FarmingStage.Harvest)
                StartCoroutine(StartTutorial());
        }

        private IEnumerator StartTutorial()
        {
            if (_previousStageTutorialManager != null)
            {
                Destroy(_previousStageTutorialManager.gameObject);

                while (_previousStageTutorialManager != null)
                {
                    yield return null;
                }
            }

            if (_nextStageTutorialManager != null)
            {
                _nextStageTutorialManager.gameObject.SetActive(true);
                _nextStageTutorialManager.InitializeTutorialManager();
                _nextStageTutorialManager.AdvanceTutorial();
                gameObject.SetActive(false);
            }
        }
    }
}

