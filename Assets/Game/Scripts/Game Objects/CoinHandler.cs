using UnityEngine;

enum PLAYER_STATE
{
    DEFAULT,
    GAINMONEY
};

public class CoinHandler : MonoBehaviour
{
    // Vars for Coin Gain
    private PLAYER_STATE state = PLAYER_STATE.DEFAULT;
    [SerializeField] private float _coinDuration = 0.125f;
    private float _coinSpawnTimer = 0f;
    private GameObject spawnPoint = null;
    private GameObject _coin = null;
    private Transform _startPos = null;
    private int _coinsToGain = 0;

    private void OnEnable()
    {
        EventBus.Subscribe<MoneyEarnedEvent>(AddMoney);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<MoneyEarnedEvent>(AddMoney);
    }

    private void AddMoney(MoneyEarnedEvent e)
    {
        _coinsToGain = e.coinAmount;
        spawnPoint = e.location;
        state = PLAYER_STATE.GAINMONEY;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PLAYER_STATE.DEFAULT:
                _coinSpawnTimer = 0f;
                return;

            case PLAYER_STATE.GAINMONEY:
                _coinSpawnTimer += Time.deltaTime;

                if (_coinSpawnTimer >= _coinDuration && _coinsToGain != 0)
                {
                    // spawn a coin
                    if (_coin == null)
                    {
                        _coin = AssetManager.Instance.Spawn("Coin", spawnPoint.transform);
                        _coin.transform.SetParent(null);
                        _coin.transform.SetAsLastSibling();
                        _startPos = _coin.transform;
                    }

                    // move coin to display
                    _coin.transform.position = Vector3.Lerp(_startPos.position, GameObject.FindWithTag("Coins").transform.position, Time.deltaTime / _coinDuration);

                    // set coin text display
                    if (Vector3.Distance(_coin.transform.position, GameObject.FindWithTag("Coins").transform.position) < 0.1f)
                    {
                        PlayerManager.Instance.Coins += 1;
                        PlayerManager.Instance.SetCoinText();
                        // remove coin
                        Destroy(_coin);
                        _coinsToGain -= 1;
                        _coinSpawnTimer = 0f;
                    }
                }

                if (_coinsToGain <= 0)
                {
                    state = PLAYER_STATE.DEFAULT;
                }
                break;
        }
    }
}
