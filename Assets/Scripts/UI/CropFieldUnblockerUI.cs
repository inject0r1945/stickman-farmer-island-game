using HCGame.Advertisements;
using HCGame.Environment;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HCGame.UI
{
    [RequireComponent(typeof(UniversalAdvertisementShower))]
    public class CropFieldUnblockerUI : MonoBehaviour
    {
        [SerializeField] private CropField _cropField;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private CanvasGroup _advertisimentError;
        [SerializeField] private float _advertisimentErrorShowTime = 2;
        [SerializeField] private Canvas _cropFieldBlockCanvas;

        private UniversalAdvertisementShower _advertisementShower;
        private Coroutine _advertisimentErrorCoroutine;

        private void Awake()
        {
            _advertisementShower = GetComponent<UniversalAdvertisementShower>();
        }

        private void OnEnable()
        {
            _unlockButton.onClick.AddListener(OnUnlockButtonClick);

            if (_advertisementShower != null)
            {
                _advertisementShower.AdvertisementShowFailed += OnAdvertisementShowFailed;
                _advertisementShower.AdvertisementShowSuccess += Unblock;
            }
        }

        private void OnDisable()
        {
            _unlockButton.onClick.RemoveListener(OnUnlockButtonClick);

            if (_advertisementShower != null)
            {
                _advertisementShower.AdvertisementShowFailed -= OnAdvertisementShowFailed;
                _advertisementShower.AdvertisementShowSuccess -= Unblock;
            }

            _advertisimentError.gameObject.SetActive(false);
        }

        private void Start()
        {
            _cropFieldBlockCanvas.worldCamera = Camera.main;
            _advertisimentError.gameObject.SetActive(false);
        }

        private void OnAdvertisementShowFailed()
        {
            if (_advertisimentErrorCoroutine != null)
            {
                StopCoroutine(_advertisimentErrorCoroutine);
            }

            _advertisimentErrorCoroutine = StartCoroutine(ShowAdvertisimentError());
        }

        private IEnumerator ShowAdvertisimentError()
        {
            _advertisimentError.gameObject.SetActive(true);
            float maxAlphaValue = 1;
            _advertisimentError.alpha = maxAlphaValue;

            float timer = 0f;

            while (timer < _advertisimentErrorShowTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            while (_advertisimentError.alpha > 0)
            {
                _advertisimentError.alpha = Mathf.Max(_advertisimentError.alpha - Time.deltaTime, 0);
            }

            _advertisimentError.gameObject.SetActive(false);
        }

        private void Unblock()
        {
            _cropField.Unblock();
        }

        private void OnUnlockButtonClick()
        {
            ShowAdvertisiment();
        }

        private void ShowAdvertisiment()
        {
            if (_advertisementShower == null)
                return;

            if (!_advertisementShower.CanShowAdvertisement())
            {
                OnAdvertisementShowFailed();
                return;
            }

            _advertisementShower.TryShowAdvertisement();
        }
    }
}
