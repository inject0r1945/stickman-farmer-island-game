mergeInto(LibraryManager.library, {

  RateGameSDK: function () {
      ysdk.feedback.canReview()
        .then(({ value, reason }) => {
            if (value) {
                ysdk.feedback.requestReview()
                    .then(({ feedbackSent }) => {
                        console.log(feedbackSent);
                        myGameInstance.SendMessage("YandexSDK", "OnUserRateGame");
                    })
            } else {
                console.log(reason);
            }
        })
  },
  
  SendSaveToYandexCloud: function (state) {
    var stateString = UTF8ToString(state);
    var stateJson = JSON.parse(stateString);
    player.setData(stateJson);
  },
  
  SendRequestToLoadSaveFromYandexCloud: function () {
    player.getData().then(_state => {
        const stateJson = JSON.stringify(_state);
        myGameInstance.SendMessage("YandexSaveDataReceiver", "Receive", stateJson);
    });
  },
  
  ShowConsoleMessage: function (message) {
    console.log(UTF8ToString(message));
  },
  
  GetLanguageFromYandexSDK: function () {
    var lang = ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(lang) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(lang, buffer, bufferSize);
    return buffer;
  },
  
  ShowInterstitialAdvertisementSDK: function () {
    ysdk.adv.showFullscreenAdv({
        callbacks: {
            onClose: function(wasShown) {
                myGameInstance.SendMessage("YandexInterstitialAdvertisement", "OnClose");
            },
            onError: function(error) {
                myGameInstance.SendMessage("YandexInterstitialAdvertisement", "OnError");
            }
        }
    })
  },
  
  ShowRewardedAdvertisementSDK: function () {
      ysdk.adv.showRewardedVideo({
        callbacks: {
            onOpen: () => {
            },
            onRewarded: () => {
              myGameInstance.SendMessage("YandexAdvertisimentsHandler", "OnRewardedAdvertisement");
            },
            onClose: () => {
              myGameInstance.SendMessage("YandexAdvertisimentsHandler", "OnCloseAdvertisement");
            }, 
            onError: (e) => {
              myGameInstance.SendMessage("YandexAdvertisimentsHandler", "OnErrorAdvertisement");
            }
        }
    })
  },
});

