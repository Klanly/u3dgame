using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Yifan.Core
{
    public enum ShaderKeyword
    {
        ENABLE_MAIN_COLOR,
        ENABLE_RIM,
    }

    struct ShaderKeywords : IEnumerable<int>, IEquatable<ShaderKeywords>
    {
        private const int MaxKeywordCount = 32;
        private static readonly string[] KeywordNames =
            new string[MaxKeywordCount];

        [SerializeField]
        private int keywords;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            ShaderKeywords.SetKeywordName(
                (int)ShaderKeyword.ENABLE_MAIN_COLOR, "ENABLE_MAIN_COLOR");
            ShaderKeywords.SetKeywordName(
                (int)ShaderKeyword.ENABLE_RIM, "ENABLE_RIM");
        }

        public static void SetKeywordName(int keyword, string name)
        {
            Assert.IsTrue(keyword >= 0 && keyword < MaxKeywordCount);
            KeywordNames[keyword] = name;
        }

        public static string GetKeywordName(int keyword)
        {
            Assert.IsTrue(keyword >= 0 && keyword < MaxKeywordCount);
            return KeywordNames[keyword];
        }

        public void SetKeyword(int keyword)
        {
            Assert.IsTrue(keyword >= 0 && keyword < MaxKeywordCount);
            this.keywords |= 1 << keyword;
        }

        public void UnsetKeyword(int keyword)
        {
            Assert.IsTrue(keyword >= 0 && keyword < MaxKeywordCount);
            this.keywords &= ~(1 << keyword);
        }

        public void ToggleKeyword(int keyword)
        {
            Assert.IsTrue(keyword >= 0 && keyword < MaxKeywordCount);
            this.keywords ^= 1 << keyword;
        }

        public void Merge(ShaderKeywords keywords)
        {
            this.keywords |= keywords.keywords;
        }

        public bool HasKeyword(int keyword)
        {
            Assert.IsTrue(keyword > 0 && keyword < MaxKeywordCount);
            return (this.keywords & (1 << keyword)) != 0;
        }

        public bool Equals(ShaderKeywords other)
        {
            return this.keywords == other.keywords;
        }

        public override int GetHashCode()
        {
            return this.keywords.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new KeyworkdEnumerator(this.keywords);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new KeyworkdEnumerator(this.keywords);
        }

        private struct KeyworkdEnumerator : IEnumerator<int>
        {
            private int keywords;
            private int index;

            public KeyworkdEnumerator(int keywords)
            {
                this.keywords = keywords;
                this.index = -1;
            }

            public int Current
            {
                get
                {
                    return this.index;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.index;
                }
            }

            public bool MoveNext()
            {
                ++this.index;
                var maxCount = ShaderKeywords.MaxKeywordCount;
                for (int i = this.index; i < maxCount; ++i)
                {
                    if ((this.keywords & (1 << i)) != 0)
                    {
                        this.index = i;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                this.index = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}
