using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GameStatus { Preparing, Game, GameOver};

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform mainCanvas;
    public int cardsInHandCount = 6;

    private List<CardStruct> cardsInHand = new List<CardStruct>();

    public static GameManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            this.enabled = false;
        }
        else
        {
            instance = this;
        }
    }

    private GameStatus gameStatus;
    public GameStatus GameStatus
    {
        get
        {
            return gameStatus;
        }

        set
        {
            gameStatus = value;
            switch(gameStatus)
            {
                case GameStatus.Preparing:
                    break;

                case GameStatus.Game:
                    StartGame();
                    break;

                case GameStatus.GameOver:
                    break;
            }
        }
    }

    private void CreateCards()
    {
        GameObject go;
        Card card;
        for(int i = 0; i < cardsInHandCount; i++)
        {
            go = Instantiate(cardPrefab, mainCanvas);
            card = go.GetComponent<Card>();
            card.Init(Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7), "Card " + (i + 1), "Description " + (i + 1));
            cardsInHand.Add(new CardStruct(card, card.GetComponent<RectTransform>()));
        }
    }

    IEnumerator DealCards()
    {
        Vector3 endCardPos;
        float animationDuration = 0.5f;
        WaitForSeconds waitBetweenCards = new WaitForSeconds(0.2f);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            endCardPos = CalculateStartCardPos(cardsInHand.Count, i);
            cardsInHand[i].rectTransform.DOLocalMove(endCardPos, animationDuration).SetEase(Ease.OutBack);
            cardsInHand[i].rectTransform.DORotateQuaternion(CalculateCardRotation(endCardPos.x), animationDuration);
            yield return waitBetweenCards;
        }
    }

    private Vector3 CalculateStartCardPos(int cardsCount, int cardNumber)
    {
        float cardInterval = 50;
        float x = -(cardsCount / 2f - 0.5f) * cardInterval + cardNumber * cardInterval;
        float y = -280 - Mathf.Abs(x) * 0.2f;
        return new Vector3(x, y, 0);
    }

    private Quaternion CalculateCardRotation(float xPos)
    {
        float coeff = -0.1f;
        float zRotation = coeff * xPos;
        return Quaternion.Euler(0, 0 , zRotation);
    }

    private void StartGame()
    {
        CreateCards();
        StartCoroutine(DealCards());
    }

    private int lastChangedCardInd = -1;
    public void ChangeRandomCardValue()
    {
        lastChangedCardInd++;
        if(lastChangedCardInd >= cardsInHand.Count)
        {
            lastChangedCardInd = 0;
        }

        int valueToChange = Random.Range(0, 3);
        int amountToChange = Random.Range(-2, 10);

        switch (valueToChange)
        {
            case 0:
                cardsInHand[lastChangedCardInd].card.Attack += amountToChange;
                break;

            case 1:
                cardsInHand[lastChangedCardInd].card.HP += amountToChange;
                break;

            case 2:
                cardsInHand[lastChangedCardInd].card.Mana += amountToChange;
                break;
        }
        cardsInHand[lastChangedCardInd].rectTransform.SetAsLastSibling();
    }

    private struct CardStruct
    {
        public Card card;
        public RectTransform rectTransform;
        public CardStruct(Card card, RectTransform rectTransform)
        {
            this.card = card;
            this.rectTransform = rectTransform;
        }
    }
}
