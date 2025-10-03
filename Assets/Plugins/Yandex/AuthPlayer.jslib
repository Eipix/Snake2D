mergeInto(LibraryManager.library, {

  GetPlayerData: function ()
  {
    gameInstance.SendMessage("Yandex","SetName", player.getName());
    gameInstance.SendMessage("Yandex","SetPhoto", player.getPhoto());
  },

  GetCurrencyIcon: function()
  {
    ysdk.getPayments({ signed: true }).then(payments =>
    {
      payments.getCatalog().then(products =>
      {
        gameInstance.SendMessage("Markups", "SetIcon", products[0].getPriceCurrencyImage("medium"));
      });
    });
  },

  GetPurchasePrice: function(id, objectPath, methodName)
  {
    var ID = UTF8ToString(id);
    var stringObjectPath = UTF8ToString(objectPath);
    var stringMethodName = UTF8ToString(methodName);

    ysdk.getPayments({ signed: true }).then(payments =>
    {
      payments.getCatalog().then(products =>
      {
          for(product in products)
          {
            if(product.id === ID)
              gameInstance.SendMessage(stringObjectPath, stringMethodName, product.priceValue);
          }
      });
    });

  },

  InitSDK: function()
  {
    YaGames.init().then(ysdk =>
    {
       console.log('Yandex SDK initialized');
       window.ysdk = ysdk;
       initLib();
       initPlayer();
       initPayments();
       gameInstance.SendMessage("Markups", "OnSDKInit");
    });
  },

  SaveExtern: function(json)
  {
    var dateString = UTF8ToString(json);
    var obj = JSON.parse(dateString);
    player.setData(obj);
  },

  LoadExtern: function()
  {
    ysdk.getPlayer().then(player => 
    {
      player.getData().then(_date =>
      {
         const json = JSON.stringify(_date);
         gameInstance.SendMessage("SaveSerial","LoadGame", json);
      });
    });
  },

  SetToLeaderboard: function(value){
      lb.setLeaderboardScore('Apples', value);
  },

  GetLang: function()
  {
      var lang = ysdk.environment.i18n.lang;
      var bufferSize = lengthBytesUTF8(lang) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(lang, buffer, bufferSize);
      return buffer;
  },

  GameReady: function()
  {
      ysdk.features.LoadingAPI.ready()
  },

  GetServerTimeExtern: function(){
    var time = ysdk.serverTime().toString();
    var bufferSize = lengthBytesUTF8(time) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(time, buffer, bufferSize);
    return buffer;
  },

  ShowFullScreenAd: function()
  {
      ysdk.adv.showFullscreenAdv({
      callbacks: {
        onOpen: () => {
            gameInstance.SendMessage("Volumes", "Pause", 1);
          },
        onClose: function(wasShown) {
            gameInstance.SendMessage("Volumes", "Resume", 1);
            gameInstance.SendMessage("Markups", "OnFullScreenAdClose");
          },
        onError: function(error) {
            gameInstance.SendMessage("Volumes", "Resume", 1);
            gameInstance.SendMessage("Markups", "OnFullScreenAdClose");
          }
        }
      })
  },

  RewardedAds: function(objectPath, methodName, onCloseMethod){
    var stringObjectPath = UTF8ToString(objectPath);
    var stringMethodName = UTF8ToString(methodName);
    var stringOnCloseMethod = UTF8ToString(onCloseMethod);
    var isRewarded = false;

    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: () => {
          console.log('Video ad open.');
          gameInstance.SendMessage("Volumes", "Pause", 1);
        },
        onRewarded: () => {
          console.log('Video ad rewarded.');
          gameInstance.SendMessage(stringObjectPath, stringMethodName);
          isRewarded = true;
        },
        onClose: () => {
          console.log('Video ad closed.');
          if(isRewarded)
          {
            gameInstance.SendMessage(stringObjectPath, stringOnCloseMethod);
          }
          gameInstance.SendMessage("Volumes", "Resume", 1);
        },
        onError: (e) => {
          console.log('Error while open video ad:', e);
          gameInstance.SendMessage("Volumes", "Resume", 1);
        }
      }
    })
  },

  BuyProduct: function(ID, path, methodName){
    var strID = UTF8ToString(ID);
    var strPath = UTF8ToString(path);
    var strMethodName = UTF8ToString(methodName);

    payments.purchase({ id: strID }).then(purchase => {
        console.log(`Purchase success: ${strID}`);
        gameInstance.SendMessage(strPath, strMethodName);
        payments.consumePurchase(purchase.purchaseToken);
    }).catch(err => {
        // Покупка не удалась: в консоли разработчика не добавлен товар с таким id,
        // пользователь не авторизовался, передумал и закрыл окно оплаты,
        // истекло отведенное на покупку время, не хватило денег и т. д.
    })
  },

  GetPurchase: function(ID, path, methodName){
        var strID = UTF8ToString(ID);
        var strPath = UTF8ToString(path);
        var strMethodName = UTF8ToString(methodName);

        payments.getPurchases().then(purchases => {
        if (purchases.some(purchase => purchase.productID === strID)) {
          console.log(strPath);
          gameInstance.SendMessage(strPath, strMethodName);
        }
    }).catch(err => {
        // Выбрасывает исключение USER_NOT_AUTHORIZED для неавторизованных пользователей.
    })
  },

  CheckPurchase: function(ID)
  {
      var strID = UTF8ToString(ID);

      ysdk.getPayments({ signed: true }).then(_payments =>
      {
        payments.getPurchases().then(purchases => purchases.forEach(consumePurchase));

        function consumePurchase(purchase)
        {
          if (purchase.productID === strID)
          {
            console.log(`Purchase success: ${strID}`);
            gameInstance.SendMessage("Markups","ConsumePurchase", strID);
            payments.consumePurchase(purchase.purchaseToken);
          }
        }
      });
  }
});