using System;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Engine.Core.Components
{
    public struct IsoGuid
    {
        private byte[] guid;
        public byte[] Get()
        {
            return guid;
        }

        internal void Set(byte[] guid)
        {
            this.guid = guid;
        }

        public override string ToString()
        {
            string str = "";

            for(int i = 0; i < guid.Length; i++)
            {
                str += "{" + guid[i] + "}";
                if(i != guid.Length - 1)
                {
                    str += ":";
                }
            }
            
            return str;
        }
    }

    public sealed class IsoGuidProvider
    {
        private static IsoGuidProvider instance = null;
        private static readonly object padlock = new object();

        IsoGuidProvider()
        {
            _currentRandom = null;
        }

        public static IsoGuidProvider Instance
        {
            get
            {
                lock(padlock)
                {
                    if(instance == null)
                    {
                        instance = new IsoGuidProvider();
                    }
                    return instance;
                }
            }
        }

        private Random _currentRandom;

        private const int SECTIONS = 4;
        private const int SECT_LENGTH = 6;

        public IsoGuid GenerateGUIDFromSeed(int seed)
        {
            IsoGuid newGuid = new IsoGuid();
            _currentRandom = new Random(seed);

            byte[] bytesguid = new byte[SECTIONS];

            for(int s = 0; s < bytesguid.Length; s++)
            {
                _currentRandom.NextBytes(new byte[SECT_LENGTH]);
            }

            newGuid.Set(bytesguid);
            return newGuid;
        }
    }
}