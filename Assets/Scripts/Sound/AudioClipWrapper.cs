using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    [Serializable]
    public class AudioClipWrapper
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private AudioClip audioClip;

        public string Name => name;
        public AudioClip AudioClip => audioClip;
    }
}
