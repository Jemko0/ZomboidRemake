using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace Iso.Engine.Core.Converters
{
    public class IsoConvert
    {
        public static Vector2 ParseVector2(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return Vector2.Zero;
            }

            string[] parts = v.Split(',');

            if (parts.Length != 2)
                throw new Exception($"Invalid Vector2 format: '{v}'");

            return new Vector2(
                float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture)
            );
        }
    }
}