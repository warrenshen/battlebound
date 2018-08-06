using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class GalleryIdle : AnimateIdle
{
    [SerializeField]
    private string cardName;

    public Card card;
    private static System.Random random = new System.Random();

    protected override void Awake()
    {
        this.card = Card.CreateByNameAndLevel(RandomString(5), this.cardName, 0);
        base.Awake();
    }

    protected override void Start()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Animate()
    {
        base.Animate();
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
