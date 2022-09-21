﻿namespace TempFileStorage;

public static class TempFileKeyGenerator
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private static readonly Random Random = new Random(DateTime.Now.Millisecond);

    public static string Generate()
    {
        var key = "";

        for (var i = 0; i < 10; i++)
        {
            var rnd = Random.Next(0, Characters.Length);
            key += Characters[rnd];
        }

        return key;
    }
}