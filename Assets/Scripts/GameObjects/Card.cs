using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text description;
    public Image art;
    public TMP_Text attackText;
    public TMP_Text hpText;
    public TMP_Text manaText;

    private int attack;
    private int hp;
    private int mana;

    private Coroutine attackCounterAnim;
    private Coroutine hpCounterAnim;
    private Coroutine manaCounterAnim;

    public int Attack { 
        get
        {
            return attack;
        }

        set
        {
            attack = value;
            if(attack < 0)
            {
                attack = 0;
            }
            if (attackCounterAnim != null)
                StopCoroutine(attackCounterAnim);
            attackCounterAnim = StartCoroutine(CounterAnimation(attackText, attack));
        }
    }

    public int HP
    {
        get
        {
            return hp;
        }

        set
        {
            hp = value;
            if (hp < 0)
            {
                hp = 0;
            }
            if (hpCounterAnim != null)
                StopCoroutine(hpCounterAnim);
            hpCounterAnim = StartCoroutine(CounterAnimation(hpText, hp));
        }
    }

    public int Mana
    {
        get
        {
            return mana;
        }

        set
        {
            mana = value;
            if (mana < 0)
            {
                mana = 0;
            }
            if (manaCounterAnim != null)
                StopCoroutine(manaCounterAnim);
            manaCounterAnim = StartCoroutine(CounterAnimation(manaText, mana));
        }
    }

    public void Init(int attack, int hp, int mana)
    {
        this.attack = attack;
        this.hp = hp;
        this.mana = mana;
        attackText.text = attack.ToString();
        hpText.text = hp.ToString();
        manaText.text = mana.ToString();
        ArtLoader.instance.SetImage(art);
    }

    public void Init(int attack, int hp, int mana, string title, string description)
    {
        Init(attack, hp, mana);
        SetTitle(title);
        SetDescription(description);
    }

    public void SetTitle(string title)
    {
        this.title.text = title;
    }

    public void SetDescription(string description)
    {
        this.description.text = description;
    }

    private WaitForSeconds counterWait = new WaitForSeconds(0.1f);
    IEnumerator CounterAnimation(TMP_Text textField, int newValue)
    {
        int value = int.Parse(textField.text);
        while (value != newValue)
        {
            value += newValue > value ? 1 : -1;
            textField.text = value.ToString();
            yield return counterWait;
        }
    }
}
