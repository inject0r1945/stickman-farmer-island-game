using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Advertisements
{
    public interface IAdvertised
    {
        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

        public bool CanShowAdvertisement();

        public bool TryShowAdvertisement();
    }
}
