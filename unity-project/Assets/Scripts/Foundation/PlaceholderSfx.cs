using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Procedural placeholder SFX factory. All clips are generated once and cached.
    /// Variation comes from pitch randomization in AudioManager.PlaySfx, not from
    /// regenerating the clip data.
    /// </summary>
    public static class PlaceholderSfx
    {
        private const int SampleRate = 44100;

        // Cached clips (generated on first access)
        private static AudioClip _jump;
        private static AudioClip _land;
        private static AudioClip _roll;
        private static AudioClip _stumble;
        private static AudioClip _crash;
        private static AudioClip _pickup;
        private static AudioClip _shieldActivate;
        private static AudioClip _shieldConsume;
        private static AudioClip _win;
        private static AudioClip _lose;
        private static AudioClip[] _zoneChange = new AudioClip[5]; // one per GlucoseZone

        public static AudioClip GenerateJump()
        {
            if (_jump != null) return _jump;
            // Upward pitch sweep 200->600Hz, 80ms, sine "boing"
            const float freqStart = 200f;
            const float freqEnd = 600f;
            const float duration = 0.08f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _jump = AudioClip.Create("sfx_jump", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(freqStart, freqEnd, t);
                float env = Mathf.Sin(Mathf.PI * t); // smooth envelope
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * (float)i / SampleRate) * env * 0.5f;
            }
            _jump.SetData(data, 0);
            return _jump;
        }

        public static AudioClip GenerateLand()
        {
            if (_land != null) return _land;
            // Low thud, 60Hz square-ish, 60ms, "thump"
            const float duration = 0.06f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _land = AudioClip.Create("sfx_land", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Exp(-t * 15f); // fast decay
                float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * 60f * (float)i / SampleRate));
                wave = wave * 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * 60f * (float)i / SampleRate); // square-ish mix
                data[i] = wave * env * 0.5f;
            }
            _land.SetData(data, 0);
            return _land;
        }

        public static AudioClip GenerateRoll()
        {
            if (_roll != null) return _roll;
            // White noise burst, 150ms, "whoosh"
            const float duration = 0.15f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _roll = AudioClip.Create("sfx_roll", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            float[] noise = new float[samples];
            for (int i = 0; i < samples; i++) noise[i] = Random.Range(-1f, 1f);
            // Bandpass approximate via 3-sample moving average
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Sin(Mathf.PI * Mathf.Min(t * 2f, 1f));
                float smoothed = (i > 0 ? noise[i - 1] : 0f) * 0.15f
                              + noise[i] * 0.7f
                              + (i < samples - 1 ? noise[i + 1] : 0f) * 0.15f;
                data[i] = smoothed * env * 0.35f;
            }
            _roll.SetData(data, 0);
            return _roll;
        }

        public static AudioClip GenerateStumble()
        {
            if (_stumble != null) return _stumble;
            // Low buzzy thud + dissonance, 200ms, "uh-oh"
            const float duration = 0.2f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _stumble = AudioClip.Create("sfx_stumble", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Exp(-t * 6f);
                float buzz = Mathf.Sin(2f * Mathf.PI * 90f * (float)i / SampleRate)
                           + 0.5f * Mathf.Sin(2f * Mathf.PI * 137f * (float)i / SampleRate) // dissonant partial
                           + 0.25f * Mathf.Sin(2f * Mathf.PI * 53f * (float)i / SampleRate);
                data[i] = buzz * env * 0.35f;
            }
            _stumble.SetData(data, 0);
            return _stumble;
        }

        public static AudioClip GenerateCrash()
        {
            if (_crash != null) return _crash;
            // Noise crash + descending pitch 400->100Hz, 300ms, "boom"
            const float duration = 0.3f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _crash = AudioClip.Create("sfx_crash", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Exp(-t * 5f);
                float noise = Random.Range(-1f, 1f);
                float freq = Mathf.Lerp(400f, 100f, t);
                float tone = Mathf.Sin(2f * Mathf.PI * freq * (float)i / SampleRate);
                data[i] = (noise * 0.6f + tone * 0.4f) * env * 0.5f;
            }
            _crash.SetData(data, 0);
            return _crash;
        }

        public static AudioClip GeneratePickup()
        {
            if (_pickup != null) return _pickup;
            // Bright 2-tone bell (C5 523Hz -> E5 659Hz), 150ms, "ding"
            const float duration = 0.15f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _pickup = AudioClip.Create("sfx_pickup", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Exp(-t * 10f);
                float tone1 = Mathf.Sin(2f * Mathf.PI * 523f * (float)i / SampleRate);
                float tone2 = Mathf.Sin(2f * Mathf.PI * 659f * (float)i / SampleRate);
                data[i] = (tone1 * 0.5f + tone2 * 0.5f) * env * 0.45f;
            }
            _pickup.SetData(data, 0);
            return _pickup;
        }

        public static AudioClip GenerateShieldActivate()
        {
            if (_shieldActivate != null) return _shieldActivate;
            // Rising sweep 300->900Hz + shimmer, 200ms
            const float duration = 0.2f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _shieldActivate = AudioClip.Create("sfx_shield_activate", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Sin(Mathf.PI * t);
                float freq = Mathf.Lerp(300f, 900f, t);
                float shimmer = Mathf.PerlinNoise(t * 30f, 0f) * 0.3f;
                data[i] = (Mathf.Sin(2f * Mathf.PI * freq * (float)i / SampleRate) + shimmer) * env * 0.3f;
            }
            _shieldActivate.SetData(data, 0);
            return _shieldActivate;
        }

        public static AudioClip GenerateShieldConsume()
        {
            if (_shieldConsume != null) return _shieldConsume;
            // Quick blip + low click, 100ms
            const float duration = 0.1f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _shieldConsume = AudioClip.Create("sfx_shield_consume", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Exp(-t * 20f);
                float blip = Mathf.Sin(2f * Mathf.PI * 800f * (float)i / SampleRate);
                float click = Random.Range(-1f, 1f) * 0.5f;
                data[i] = (blip * 0.6f + click) * env * 0.4f;
            }
            _shieldConsume.SetData(data, 0);
            return _shieldConsume;
        }

        public static AudioClip GenerateWin()
        {
            if (_win != null) return _win;
            // Ascending arpeggio C-E-G-C (523-659-784-1047Hz), 400ms, "win"
            const float duration = 0.4f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _win = AudioClip.Create("sfx_win", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            float[] notes = { 523f, 659f, 784f, 1047f };
            int noteLen = samples / 4;
            for (int i = 0; i < samples; i++)
            {
                int noteIdx = Mathf.Min(i / noteLen, 3);
                float localT = (float)(i % noteLen) / noteLen;
                float env = Mathf.Sin(Mathf.PI * localT) * Mathf.Exp(-localT * 2f);
                data[i] = Mathf.Sin(2f * Mathf.PI * notes[noteIdx] * (float)i / SampleRate) * env * 0.4f;
            }
            _win.SetData(data, 0);
            return _win;
        }

        public static AudioClip GenerateLose()
        {
            if (_lose != null) return _lose;
            // Descending arpeggio + low buzz, 400ms, "lose"
            const float duration = 0.4f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _lose = AudioClip.Create("sfx_lose", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            float[] notes = { 392f, 330f, 262f, 130f }; // descending
            int noteLen = samples / 4;
            for (int i = 0; i < samples; i++)
            {
                int noteIdx = Mathf.Min(i / noteLen, 3);
                float localT = (float)(i % noteLen) / noteLen;
                float env = Mathf.Sin(Mathf.PI * localT) * Mathf.Exp(-localT * 2f);
                float tone = Mathf.Sin(2f * Mathf.PI * notes[noteIdx] * (float)i / SampleRate);
                float buzz = Mathf.Sin(2f * Mathf.PI * 65f * (float)i / SampleRate) * 0.2f;
                data[i] = (tone + buzz) * env * 0.4f;
            }
            _lose.SetData(data, 0);
            return _lose;
        }

        public static AudioClip GenerateZoneChange(int zoneIndex)
        {
            if (zoneIndex < 0 || zoneIndex >= 5) zoneIndex = 0;

            if (_zoneChange[zoneIndex] != null) return _zoneChange[zoneIndex];

            // 5 zones -> 4 pitch buckets to match the "4 zones -> 4 pitches" spec:
            // LowCrisis/LowWarning -> 200Hz, Safe -> 400Hz, HighWarning/HighCrisis -> 800Hz
            float[] zoneFreqs = { 200f, 200f, 400f, 800f, 800f };
            float freq = zoneFreqs[zoneIndex];

            const float duration = 0.1f;
            int samples = Mathf.CeilToInt(SampleRate * duration);
            _zoneChange[zoneIndex] = AudioClip.Create($"sfx_zone_{zoneIndex}", samples, 1, SampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = Mathf.Exp(-t * 15f);
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * (float)i / SampleRate) * env * 0.3f;
            }
            _zoneChange[zoneIndex].SetData(data, 0);
            return _zoneChange[zoneIndex];
        }
    }
}
