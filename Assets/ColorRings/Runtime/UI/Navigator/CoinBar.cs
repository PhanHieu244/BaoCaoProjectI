using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Text))]
public class CoinBar : MonoBehaviour {
    private Text _textCoin;

    private void Awake() {
        _textCoin = GetComponent<Text>();
    }

    private void Start() {
        ChangeCoin(GameDataManager.Coin);
        GameDataManager.OnCoinChange += ChangeCoin;
    }

    private void ChangeCoin(int coin) {
        _textCoin.text = coin.ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
    }

    private void OnDestroy() {
        GameDataManager.OnCoinChange -= ChangeCoin;
    }
}