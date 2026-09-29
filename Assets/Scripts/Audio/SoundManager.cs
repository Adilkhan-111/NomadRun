using System.Collections.Generic;
using UnityEngine;

namespace NomadRun.Audio
{
    /// <summary>
    /// Procedural 8-bit / arcade synthesizer sound manager.
    /// Provides audio without needing external sound files.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource _sfxSource;
        private AudioSource _musicSource;
        private AudioSource _hoofSource;

        private readonly Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SetupAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SetupAudioSources()
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.volume = 0.4f;

            _hoofSource = gameObject.AddComponent<AudioSource>();
            _hoofSource.playOnAwake = false;
            _hoofSource.loop = true;
            _hoofSource.volume = 0.25f;

            StartHoofbeats();
        }

        public void PlaySfx(string sfxName)
        {
            if (!_clipCache.TryGetValue(sfxName, out var clip) || clip == null)
            {
                clip = SynthesizeClip(sfxName);
                if (clip != null) _clipCache[sfxName] = clip;
            }

            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        public void StartHoofbeats()
        {
            if (_hoofSource == null) return;
            var hoofClip = SynthesizeClip("Hoofbeats");
            _hoofSource.clip = hoofClip;
            _hoofSource.Play();
        }

        public void StopHoofbeats()
        {
            if (_hoofSource != null) _hoofSource.Stop();
        }

        public void SetHoofbeatPitch(float pitch)
        {
            if (_hoofSource != null) _hoofSource.pitch = Mathf.Clamp(pitch, 0.8f, 2.2f);
        }

        private AudioClip SynthesizeClip(string name)
        {
            int sampleRate = 44100;
            switch (name)
            {
                case "Baursak":
                    // High crisp pickup chime
                    return GenerateChime(sampleRate, 0.12f, 580f, 960f);

                case "Kumis":
                    // Energetic double powerup tone
                    return GeneratePowerup(sampleRate, 0.35f, 440f, 880f);

                case "Spit":
                    // Funny squelch / pop sound
                    return GenerateSquelch(sampleRate, 0.15f);

                case "Jump":
                    // Swooping pitch bend upward
                    return GenerateSlide(sampleRate, 0.28f, 220f, 750f);

                case "Crash":
                    // Low noise punch
                    return GenerateNoisePunch(sampleRate, 0.3f);

                case "PoliceSiren":
                    // Rapid alternating two-tone siren
                    return GenerateSiren(sampleRate, 0.6f);

                case "Blessing":
                    // Reverberant harmonic bell / gong
                    return GenerateBell(sampleRate, 0.5f, 523.25f);

                case "Win":
                    // Triumphant major arpeggio
                    return GenerateArpeggio(sampleRate, 1.2f, new float[] { 523.25f, 659.25f, 783.99f, 1046.50f });

                case "Hoofbeats":
                    // Rhythmic clip-clop galloping loop (0.45s)
                    return GenerateGallopLoop(sampleRate, 0.45f);

                default:
                    return GenerateChime(sampleRate, 0.1f, 440f, 440f);
            }
        }

        private AudioClip GenerateChime(int rate, float duration, float fStart, float fEnd)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(fStart, fEnd, t);
                float env = 1f - t;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate)) * env * 0.5f;
            }
            var clip = AudioClip.Create("Chime", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GeneratePowerup(int rate, float duration, float fStart, float fEnd)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(fStart, fEnd, t * t);
                float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate))); // retro square
                float env = 1f - t;
                data[i] = wave * env * 0.35f;
            }
            var clip = AudioClip.Create("Powerup", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateSlide(int rate, float duration, float fStart, float fEnd)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(fStart, fEnd, Mathf.Sqrt(t));
                float env = Mathf.Sin(t * Mathf.PI);
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate)) * env * 0.4f;
            }
            var clip = AudioClip.Create("Slide", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateSquelch(int rate, float duration)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(800f, 150f, t);
                float noise = (Random.value * 2f - 1f) * 0.3f;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate));
                float env = 1f - t;
                data[i] = (wave * 0.7f + noise) * env * 0.5f;
            }
            var clip = AudioClip.Create("Squelch", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateNoisePunch(int rate, float duration)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float noise = (Random.value * 2f - 1f);
                float lowSine = Mathf.Sin(2f * Mathf.PI * 65f * (i / (float)rate));
                float env = Mathf.Pow(1f - t, 2f);
                data[i] = (noise * 0.5f + lowSine * 0.5f) * env * 0.6f;
            }
            var clip = AudioClip.Create("Punch", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateSiren(int rate, float duration)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                // Wobble between 600 and 900 Hz
                float freq = (Mathf.Sin(2f * Mathf.PI * 6f * t) > 0) ? 880f : 660f;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate));
                data[i] = wave * 0.35f;
            }
            var clip = AudioClip.Create("Siren", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateBell(int rate, float duration, float freq)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float w1 = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate));
                float w2 = Mathf.Sin(2f * Mathf.PI * (freq * 2.76f) * (i / (float)rate)) * 0.4f;
                float env = Mathf.Exp(-4f * t);
                data[i] = (w1 + w2) * env * 0.5f;
            }
            var clip = AudioClip.Create("Bell", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateArpeggio(int rate, float duration, float[] notes)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            int noteCount = notes.Length;
            int samplesPerNote = samples / noteCount;

            for (int i = 0; i < samples; i++)
            {
                int noteIdx = Mathf.Clamp(i / samplesPerNote, 0, noteCount - 1);
                float freq = notes[noteIdx];
                int notePos = i % samplesPerNote;
                float tNote = (float)notePos / samplesPerNote;
                float env = 1f - tNote * 0.5f;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * (i / (float)rate));
                data[i] = wave * env * 0.4f;
            }
            var clip = AudioClip.Create("Arpeggio", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateGallopLoop(int rate, float duration)
        {
            int samples = (int)(rate * duration);
            float[] data = new float[samples];
            // Tygy-dyk rhythm: two impacts per gallop cycle
            int impact1 = (int)(samples * 0.15f);
            int impact2 = (int)(samples * 0.55f);
            int impactWidth = (int)(rate * 0.04f);

            for (int i = 0; i < samples; i++)
            {
                float d1 = Mathf.Abs(i - impact1);
                float d2 = Mathf.Abs(i - impact2);
                float val = 0f;

                if (d1 < impactWidth)
                {
                    float t = d1 / impactWidth;
                    val += (1f - t) * (Random.value * 2f - 1f) * 0.4f;
                }
                if (d2 < impactWidth)
                {
                    float t = d2 / impactWidth;
                    val += (1f - t) * (Random.value * 2f - 1f) * 0.35f;
                }
                data[i] = val;
            }
            var clip = AudioClip.Create("Gallop", samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
