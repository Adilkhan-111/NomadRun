using System.Collections.Generic;
using UnityEngine;

namespace NomadRun.World
{
    /// <summary>
    /// Generates stylized retro 2D pixel-art sprites for Kazakh culture, monuments, obstacles, and characters.
    /// Works both in runtime and can save textures to PNG in the Editor.
    /// </summary>
    public static class NomadArtFactory
    {
        private static readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public static void ClearCache() => _cache.Clear();

        public static Sprite GetSprite(string name)
        {
            if (_cache.TryGetValue(name, out var cached) && cached != null)
                return cached;

            Sprite s = GenerateSprite(name);
            if (s != null)
            {
                s.name = name;
                _cache[name] = s;
            }
            return s;
        }

        private static Sprite GenerateSprite(string name)
        {
            switch (name)
            {
                case "Camel_0": return CreateCamelSprite(0);
                case "Camel_1": return CreateCamelSprite(1);
                case "Camel_2": return CreateCamelSprite(2);
                case "Camel_Jump": return CreateCamelJumpSprite();
                case "Baursak": return CreateBaursakSprite();
                case "Kumis": return CreateKumisSprite();
                case "Sheep_0": return CreateSheepSprite(0);
                case "Sheep_1": return CreateSheepSprite(1);
                case "PoliceCar_0": return CreatePoliceCarSprite(0);
                case "PoliceCar_1": return CreatePoliceCarSprite(1);
                case "Spit": return CreateSpitSprite();
                case "Pothole": return CreatePotholeSprite();
                case "Kazan": return CreateKazanSprite();
                case "Watermelon": return CreateWatermelonSprite();
                case "Kurgan": return CreateKurganSprite();
                case "Yurt": return CreateYurtSprite();
                case "Aksakal": return CreateAksakalSprite();
                case "ToyBillboard": return CreateToyBillboardSprite();
                case "Baiterek": return CreateBaiterekSprite();
                case "KhanShatyr": return CreateKhanShatyrSprite();
                case "YasawiMausoleum": return CreateYasawiSprite();
                case "GoldenMan": return CreateGoldenManSprite();
                case "ExpoPavilion": return CreateExpoPavilionSprite();
                case "KokTobe": return CreateKokTobeSprite();
                case "Medeu": return CreateMedeuSprite();
                case "AlatauMountains": return CreateMountainSprite();
                case "SteppeCloud": return CreateCloudSprite();
                case "RoadSign": return CreateRoadSignSprite();
                default: return CreateFallbackSprite();
            }
        }

        private static Texture2D CreateTexture(int width, int height, Color fill)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            Color[] cols = new Color[width * height];
            for (int i = 0; i < cols.Length; i++) cols[i] = fill;
            tex.SetPixels(cols);
            return tex;
        }

        private static Sprite ToSprite(Texture2D tex, Vector2? pivot = null)
        {
            tex.Apply();
            Vector2 p = pivot ?? new Vector2(0.5f, 0.5f);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), p, 32f);
        }

        // ====================================================================
        // 1. CAMEL & NOMAD RIDER — DETAILED
        // ====================================================================
        public static Sprite CreateCamelSprite(int frame)
        {
            int w = 72, h = 68;
            var tex = CreateTexture(w, h, Color.clear);

            Color fur      = new Color(0.82f, 0.62f, 0.38f);
            Color darkFur  = new Color(0.58f, 0.40f, 0.20f);
            Color fur2     = new Color(0.90f, 0.72f, 0.50f);
            Color hoofCol  = new Color(0.22f, 0.18f, 0.14f);
            Color saddle   = new Color(0.65f, 0.14f, 0.18f);
            Color gold     = new Color(0.98f, 0.82f, 0.22f);
            Color skinTone = new Color(0.92f, 0.72f, 0.55f);
            Color skinDark = new Color(0.78f, 0.55f, 0.38f);
            Color chapan   = new Color(0.82f, 0.18f, 0.22f);
            Color chapan2  = new Color(0.55f, 0.10f, 0.14f);
            Color hatFur   = new Color(0.32f, 0.20f, 0.10f);
            Color hatLight = new Color(0.55f, 0.38f, 0.22f);
            Color eyeCol   = new Color(0.08f, 0.04f, 0.02f);
            Color lipCol   = new Color(0.75f, 0.35f, 0.28f);
            Color whiteEl  = new Color(0.96f, 0.96f, 0.94f);

            // --- CAMEL BODY ---
            DrawFilledEllipse(tex, 34, 24, 20, 11, darkFur);
            DrawFilledEllipse(tex, 34, 25, 19, 10, fur);
            DrawFilledEllipse(tex, 36, 27, 14, 7, fur2);   // dorsal highlight
            // Rear hump
            DrawFilledEllipse(tex, 22, 33, 8, 9, darkFur);
            DrawFilledEllipse(tex, 22, 33, 7, 8, fur);
            DrawFilledEllipse(tex, 22, 35, 5, 5, fur2);
            // Front hump
            DrawFilledEllipse(tex, 40, 35, 8, 9, darkFur);
            DrawFilledEllipse(tex, 40, 35, 7, 8, fur);
            DrawFilledEllipse(tex, 40, 37, 5, 5, fur2);

            // Saddle cloth with gold embroidery
            DrawFilledRect(tex, 29, 29, 14, 8, saddle);
            for (int sx = 30; sx < 43; sx += 2) { tex.SetPixel(sx, 29, gold); tex.SetPixel(sx, 36, gold); }
            for (int sy = 30; sy < 37; sy += 2) { tex.SetPixel(29, sy, gold); tex.SetPixel(42, sy, gold); }

            // Neck
            DrawLine(tex, 48, 24, 56, 38, fur, 6);
            DrawLine(tex, 50, 24, 58, 38, darkFur, 2);
            // Head
            DrawFilledEllipse(tex, 61, 40, 8, 6, fur);
            DrawFilledEllipse(tex, 66, 39, 5, 4, darkFur);
            DrawFilledEllipse(tex, 68, 40, 3, 2, fur);
            tex.SetPixel(60, 43, eyeCol); tex.SetPixel(60, 44, eyeCol); tex.SetPixel(61, 43, eyeCol);
            DrawFilledEllipse(tex, 56, 46, 2, 3, darkFur); // ear
            tex.SetPixel(58, 45, eyeCol); tex.SetPixel(59, 45, eyeCol);

            // Tail
            DrawLine(tex, 14, 26, 9, 18, darkFur, 3);
            DrawLine(tex, 9, 18, 7, 14, darkFur, 2);

            // Legs - walk cycle
            int lo = (frame == 0) ? 0 : (frame == 1) ? 2 : -2;
            DrawLine(tex, 20, 16, 16 + lo, 2, darkFur, 3); DrawLine(tex, 16 + lo, 2, 16 + lo, 0, hoofCol, 3);
            DrawLine(tex, 28, 16, 32 - lo, 2, fur, 3);     DrawLine(tex, 32 - lo, 2, 32 - lo, 0, hoofCol, 3);
            DrawLine(tex, 42, 16, 38 - lo, 2, darkFur, 3); DrawLine(tex, 38 - lo, 2, 38 - lo, 0, hoofCol, 3);
            DrawLine(tex, 50, 16, 54 + lo, 2, fur, 3);     DrawLine(tex, 54 + lo, 2, 54 + lo, 0, hoofCol, 3);

            // --- RIDER ---
            int rx = 34, ry = 36;
            // Lower robe
            DrawFilledRect(tex, rx - 5, ry,      12, 14, chapan2);
            DrawFilledRect(tex, rx - 4, ry +  1, 10, 12, chapan);
            DrawLine(tex, rx - 4, ry + 12, rx + 6, ry + 12, gold, 1); // hem
            // Upper body
            DrawFilledRect(tex, rx - 4, ry + 14, 10, 10, chapan);
            DrawLine(tex, rx, ry + 14, rx, ry + 23, gold, 2);          // embroidery
            DrawFilledEllipse(tex, rx + 1, ry + 23, 4, 2, whiteEl);   // collar
            // Left arm (holding reins)
            DrawLine(tex, rx - 4, ry + 20, rx - 9, ry + 15, chapan, 3);
            DrawFilledEllipse(tex, rx - 9, ry + 14, 3, 2, skinTone);  // left hand
            // Right arm
            DrawLine(tex, rx + 6, ry + 20, rx + 12, ry + 17, chapan, 3);
            DrawFilledEllipse(tex, rx + 12, ry + 16, 3, 2, skinTone); // right hand
            // Reins
            DrawLine(tex, rx - 9, ry + 14, 54, 36, gold, 1);
            // Neck
            DrawFilledRect(tex, rx - 1, ry + 24, 4, 4, skinTone);

            // FACE - clearly detailed
            DrawFilledEllipse(tex, rx + 1, ry + 31, 7, 7, skinTone);  // face oval
            DrawFilledEllipse(tex, rx + 4, ry + 31, 4, 6, skinDark);  // right-side shadow
            DrawFilledEllipse(tex, rx + 1, ry + 31, 6, 6, skinTone);  // re-blend
            // Eyebrows
            DrawLine(tex, rx - 4, ry + 35, rx - 1, ry + 36, eyeCol, 1);
            DrawLine(tex, rx + 2, ry + 36, rx + 5, ry + 35, eyeCol, 1);
            // Eyes - whites + pupils
            DrawFilledEllipse(tex, rx - 3, ry + 33, 2, 2, whiteEl);
            DrawFilledEllipse(tex, rx + 4, ry + 33, 2, 2, whiteEl);
            tex.SetPixel(rx - 3, ry + 33, eyeCol); tex.SetPixel(rx - 3, ry + 34, eyeCol);
            tex.SetPixel(rx + 4, ry + 33, eyeCol); tex.SetPixel(rx + 4, ry + 34, eyeCol);
            // Nose
            tex.SetPixel(rx,     ry + 30, skinDark); tex.SetPixel(rx + 1, ry + 30, skinDark);
            tex.SetPixel(rx,     ry + 29, skinDark);
            // Mouth smile
            DrawLine(tex, rx - 2, ry + 27, rx,     ry + 26, lipCol, 1);
            DrawLine(tex, rx,     ry + 26, rx + 3, ry + 27, lipCol, 1);

            // MALAKHAI HAT
            DrawFilledRect(tex, rx - 6, ry + 38, 15, 7, hatFur);
            DrawFilledEllipse(tex, rx + 1, ry + 44, 7, 3, hatFur);    // dome top
            DrawFilledRect(tex, rx - 7, ry + 37, 17, 3, hatLight);    // hat brim
            DrawFilledRect(tex, rx - 7, ry + 33, 3, 5, hatFur);       // left ear flap
            DrawFilledRect(tex, rx + 11, ry + 33, 3, 5, hatFur);      // right ear flap
            tex.SetPixel(rx + 1, ry + 47, gold); tex.SetPixel(rx + 2, ry + 47, gold); // tip
            DrawLine(tex, rx - 5, ry + 40, rx + 11, ry + 40, gold, 1); // ornament stripe

            return ToSprite(tex, new Vector2(0.4f, 0.08f));
        }

        public static Sprite CreateCamelJumpSprite()
        {
            int w = 72, h = 72;
            var tex = CreateTexture(w, h, Color.clear);

            Color fur      = new Color(0.82f, 0.62f, 0.38f);
            Color darkFur  = new Color(0.58f, 0.40f, 0.20f);
            Color fur2     = new Color(0.90f, 0.72f, 0.50f);
            Color hoofCol  = new Color(0.22f, 0.18f, 0.14f);
            Color saddle   = new Color(0.65f, 0.14f, 0.18f);
            Color gold     = new Color(0.98f, 0.82f, 0.22f);
            Color skinTone = new Color(0.92f, 0.72f, 0.55f);
            Color skinDark = new Color(0.78f, 0.55f, 0.38f);
            Color chapan   = new Color(0.82f, 0.18f, 0.22f);
            Color hatFur   = new Color(0.32f, 0.20f, 0.10f);
            Color hatLight = new Color(0.55f, 0.38f, 0.22f);
            Color eyeCol   = new Color(0.08f, 0.04f, 0.02f);
            Color lipCol   = new Color(0.75f, 0.35f, 0.28f);
            Color whiteEl  = new Color(0.96f, 0.96f, 0.94f);

            // Body
            DrawFilledEllipse(tex, 34, 26, 20, 11, darkFur);
            DrawFilledEllipse(tex, 34, 27, 19, 10, fur);
            DrawFilledEllipse(tex, 36, 29, 14, 7, fur2);
            DrawFilledEllipse(tex, 22, 35, 7, 8, darkFur); DrawFilledEllipse(tex, 22, 35, 6, 7, fur);
            DrawFilledEllipse(tex, 40, 37, 7, 8, darkFur); DrawFilledEllipse(tex, 40, 37, 6, 7, fur);
            DrawFilledRect(tex, 29, 31, 14, 8, saddle);
            DrawLine(tex, 29, 31, 42, 31, gold, 1); DrawLine(tex, 29, 38, 42, 38, gold, 1);
            // Neck/Head
            DrawLine(tex, 50, 26, 60, 42, fur, 6);
            DrawFilledEllipse(tex, 63, 44, 8, 5, fur);
            DrawFilledEllipse(tex, 68, 43, 4, 3, darkFur);
            tex.SetPixel(62, 47, eyeCol); tex.SetPixel(63, 47, eyeCol);
            // Stretched leap legs
            DrawLine(tex, 18, 18, 8, 8, darkFur, 3); DrawLine(tex, 8, 8, 8, 5, hoofCol, 3);
            DrawLine(tex, 28, 18, 38, 8, fur, 3);    DrawLine(tex, 38, 8, 38, 5, hoofCol, 3);
            DrawLine(tex, 40, 18, 30, 8, darkFur, 3);DrawLine(tex, 30, 8, 30, 5, hoofCol, 3);
            DrawLine(tex, 50, 18, 60, 8, fur, 3);    DrawLine(tex, 60, 8, 60, 5, hoofCol, 3);
            DrawLine(tex, 14, 28, 7, 20, darkFur, 3);

            // Rider
            int rx = 34, ry = 38;
            DrawFilledRect(tex, rx - 4, ry, 10, 10, chapan);
            DrawLine(tex, rx, ry, rx, ry + 9, gold, 2);
            // Arms - raised
            DrawLine(tex, rx + 5, ry + 7, rx + 14, ry + 18, chapan, 3);
            DrawFilledEllipse(tex, rx + 14, ry + 18, 3, 2, skinTone); // right hand
            DrawLine(tex, rx + 14, ry + 18, rx + 18, ry + 22, gold, 1); // kamshy
            DrawLine(tex, rx - 4, ry + 6, rx - 10, ry + 12, chapan, 3);
            DrawFilledEllipse(tex, rx - 10, ry + 12, 3, 2, skinTone); // left hand
            DrawLine(tex, rx - 10, ry + 12, 52, 38, gold, 1);
            DrawFilledRect(tex, rx - 1, ry + 10, 4, 4, skinTone); // neck
            // Face
            DrawFilledEllipse(tex, rx + 1, ry + 17, 7, 7, skinTone);
            DrawFilledEllipse(tex, rx + 4, ry + 17, 4, 6, skinDark);
            DrawFilledEllipse(tex, rx + 1, ry + 17, 6, 6, skinTone);
            DrawFilledEllipse(tex, rx - 3, ry + 19, 2, 2, whiteEl);
            DrawFilledEllipse(tex, rx + 4, ry + 19, 2, 2, whiteEl);
            tex.SetPixel(rx - 3, ry + 19, eyeCol); tex.SetPixel(rx - 3, ry + 20, eyeCol);
            tex.SetPixel(rx + 4, ry + 19, eyeCol); tex.SetPixel(rx + 4, ry + 20, eyeCol);
            tex.SetPixel(rx, ry + 16, skinDark); tex.SetPixel(rx + 1, ry + 16, skinDark);
            DrawFilledEllipse(tex, rx + 1, ry + 13, 2, 1, eyeCol); // open mouth
            tex.SetPixel(rx, ry + 13, lipCol); tex.SetPixel(rx + 2, ry + 13, lipCol);
            // Hat
            DrawFilledRect(tex, rx - 6, ry + 24, 15, 7, hatFur);
            DrawFilledEllipse(tex, rx + 1, ry + 30, 7, 3, hatFur);
            DrawFilledRect(tex, rx - 7, ry + 23, 17, 3, hatLight);
            DrawFilledRect(tex, rx - 7, ry + 19, 3, 5, hatFur);
            DrawFilledRect(tex, rx + 11, ry + 19, 3, 5, hatFur);
            tex.SetPixel(rx + 1, ry + 33, gold);
            DrawLine(tex, rx - 5, ry + 26, rx + 11, ry + 26, gold, 1);

            return ToSprite(tex, new Vector2(0.4f, 0.08f));
        }

        // ====================================================================
        // 2. PICKUPS (BAURSAK & KUMIS)
        // ====================================================================
        public static Sprite CreateBaursakSprite()
        {
            int s = 24;
            var tex = CreateTexture(s, s, Color.clear);
            Color friedCrust = new Color(0.85f, 0.52f, 0.12f);
            Color goldenBody = new Color(0.98f, 0.76f, 0.25f);
            Color highlight = new Color(1f, 0.92f, 0.6f);

            // Puffy golden diamond shape
            DrawFilledDiamond(tex, 12, 12, 8, 8, friedCrust);
            DrawFilledDiamond(tex, 12, 12, 6, 6, goldenBody);
            // Delicious shine
            tex.SetPixel(11, 14, highlight);
            tex.SetPixel(12, 14, highlight);
            tex.SetPixel(11, 13, highlight);

            return ToSprite(tex);
        }

        public static Sprite CreateKumisSprite()
        {
            int w = 24, h = 32;
            var tex = CreateTexture(w, h, Color.clear);
            Color leather = new Color(0.55f, 0.32f, 0.18f);
            Color darkLeather = new Color(0.40f, 0.22f, 0.11f);
            Color stitch = new Color(0.92f, 0.78f, 0.35f);
            Color froth = new Color(0.95f, 0.97f, 1f); // White kumis foam

            // Torsoq (traditional leather flask shape)
            DrawFilledEllipse(tex, 12, 12, 9, 10, darkLeather);
            DrawFilledEllipse(tex, 12, 12, 8, 9, leather);
            // Flask neck
            DrawFilledRect(tex, 9, 21, 6, 7, darkLeather);
            DrawFilledRect(tex, 10, 21, 4, 7, leather);
            // Cap / stopper
            DrawFilledRect(tex, 8, 28, 8, 3, stitch);

            // Traditional ornamental stitches (қошқар мүйіз curve hint)
            DrawLine(tex, 7, 12, 17, 12, stitch, 1);
            DrawLine(tex, 12, 7, 12, 17, stitch, 1);

            // Froth drops
            tex.SetPixel(14, 29, froth);
            tex.SetPixel(15, 30, froth);

            return ToSprite(tex);
        }

        // ====================================================================
        // 3. OBSTACLES: SHEEP, POLICE PRIORA, POTHOLE, KAZAN, WATERMELON
        // ====================================================================
        public static Sprite CreateSheepSprite(int frame)
        {
            int w = 32, h = 26;
            var tex = CreateTexture(w, h, Color.clear);
            Color wool = new Color(0.95f, 0.95f, 0.92f);
            Color darkWool = new Color(0.78f, 0.78f, 0.74f);
            Color face = new Color(0.25f, 0.22f, 0.20f);
            Color legs = new Color(0.20f, 0.18f, 0.16f);

            // Fluffy body
            DrawFilledEllipse(tex, 15, 14, 10, 8, darkWool);
            DrawFilledEllipse(tex, 15, 14, 9, 7, wool);
            DrawFilledEllipse(tex, 18, 17, 5, 5, wool);
            DrawFilledEllipse(tex, 11, 16, 5, 4, wool);

            // Cute black sheep face
            DrawFilledEllipse(tex, 24, 15, 4, 5, face);
            tex.SetPixel(25, 17, Color.white); // eye shine
            tex.SetPixel(22, 12, face); // droopy ear

            // Legs
            int shift = (frame == 0) ? -1 : 1;
            DrawLine(tex, 10, 8, 10 + shift, 1, legs, 2);
            DrawLine(tex, 14, 8, 14 - shift, 1, legs, 2);
            DrawLine(tex, 19, 8, 19 + shift, 1, legs, 2);
            DrawLine(tex, 23, 8, 23 - shift, 1, legs, 2);

            return ToSprite(tex, new Vector2(0.5f, 0.1f));
        }

        public static Sprite CreatePoliceCarSprite(int frame)
        {
            int w = 64, h = 32;
            var tex = CreateTexture(w, h, Color.clear);
            Color white = new Color(0.92f, 0.93f, 0.95f);
            Color blueStripe = new Color(0.08f, 0.35f, 0.78f);
            Color glass = new Color(0.45f, 0.65f, 0.85f);
            Color rubber = new Color(0.12f, 0.12f, 0.14f);
            Color rim = new Color(0.75f, 0.75f, 0.75f);
            Color redFlash = (frame == 0) ? Color.red : new Color(0.4f, 0f, 0f);
            Color blueFlash = (frame == 1) ? Color.cyan : new Color(0f, 0.2f, 0.5f);

            // Car body (Sedan / Priora silhouette)
            DrawFilledRect(tex, 4, 8, 54, 11, white);
            // Cabin
            DrawFilledRect(tex, 16, 18, 28, 9, white);
            DrawFilledRect(tex, 19, 19, 23, 7, glass);

            // Blue DPS stripe along door
            DrawFilledRect(tex, 4, 11, 54, 4, blueStripe);

            // Headlights & Taillights
            DrawFilledRect(tex, 56, 12, 3, 4, Color.yellow);
            DrawFilledRect(tex, 4, 12, 2, 4, Color.red);

            // Flashing Lightbar on roof (Мигалка)
            DrawFilledRect(tex, 27, 27, 3, 4, redFlash);
            DrawFilledRect(tex, 30, 27, 3, 4, blueFlash);

            // Wheels
            DrawFilledEllipse(tex, 16, 8, 5, 5, rubber);
            DrawFilledEllipse(tex, 16, 8, 2, 2, rim);
            DrawFilledEllipse(tex, 46, 8, 5, 5, rubber);
            DrawFilledEllipse(tex, 46, 8, 2, 2, rim);

            return ToSprite(tex, new Vector2(0.5f, 0.2f));
        }

        public static Sprite CreateSpitSprite()
        {
            int s = 16;
            var tex = CreateTexture(s, s, Color.clear);
            Color spitGreen = new Color(0.45f, 0.88f, 0.32f);
            Color inner = new Color(0.85f, 1f, 0.65f);

            DrawFilledEllipse(tex, 8, 8, 6, 4, spitGreen);
            DrawFilledEllipse(tex, 8, 8, 4, 2, inner);
            DrawFilledEllipse(tex, 3, 8, 2, 2, spitGreen); // droplet trail

            return ToSprite(tex);
        }

        public static Sprite CreatePotholeSprite()
        {
            int w = 36, h = 18;
            var tex = CreateTexture(w, h, Color.clear);
            Color darkHole = new Color(0.10f, 0.10f, 0.11f);
            Color asphaltEdge = new Color(0.35f, 0.35f, 0.38f);

            DrawFilledEllipse(tex, 18, 9, 15, 7, asphaltEdge);
            DrawFilledEllipse(tex, 18, 9, 13, 5, darkHole);

            // Cracks
            DrawLine(tex, 3, 9, 1, 14, asphaltEdge, 1);
            DrawLine(tex, 33, 9, 35, 13, asphaltEdge, 1);

            return ToSprite(tex);
        }

        public static Sprite CreateKazanSprite()
        {
            int w = 32, h = 28;
            var tex = CreateTexture(w, h, Color.clear);
            Color iron = new Color(0.22f, 0.22f, 0.24f);
            Color pilaf = new Color(0.92f, 0.65f, 0.15f);
            Color steam = new Color(0.9f, 0.95f, 1f, 0.6f);

            // Tripod legs
            DrawLine(tex, 7, 2, 10, 10, iron, 2);
            DrawLine(tex, 25, 2, 22, 10, iron, 2);
            DrawLine(tex, 16, 2, 16, 9, iron, 2);

            // Heavy Cast Iron Cauldron
            DrawFilledEllipse(tex, 16, 14, 11, 7, iron);
            // Fragrant Pilaf inside
            DrawFilledEllipse(tex, 16, 17, 8, 3, pilaf);

            // Steam wisp
            DrawLine(tex, 14, 21, 12, 26, steam, 1);
            DrawLine(tex, 18, 21, 20, 26, steam, 1);

            return ToSprite(tex, new Vector2(0.5f, 0.1f));
        }

        public static Sprite CreateWatermelonSprite()
        {
            int w = 32, h = 24;
            var tex = CreateTexture(w, h, Color.clear);
            Color darkGreen = new Color(0.12f, 0.45f, 0.15f);
            Color lightGreen = new Color(0.35f, 0.82f, 0.32f);
            Color wood = new Color(0.65f, 0.45f, 0.25f);

            // Wooden crate base
            DrawFilledRect(tex, 2, 2, 28, 8, wood);

            // Pyramidal pile of round watermelons
            DrawWatermelonBall(tex, 9, 11, darkGreen, lightGreen);
            DrawWatermelonBall(tex, 16, 11, darkGreen, lightGreen);
            DrawWatermelonBall(tex, 23, 11, darkGreen, lightGreen);
            DrawWatermelonBall(tex, 13, 16, darkGreen, lightGreen);
            DrawWatermelonBall(tex, 20, 16, darkGreen, lightGreen);

            return ToSprite(tex, new Vector2(0.5f, 0.1f));
        }

        private static void DrawWatermelonBall(Texture2D tex, int cx, int cy, Color d, Color l)
        {
            DrawFilledEllipse(tex, cx, cy, 4, 4, d);
            DrawLine(tex, cx - 2, cy - 3, cx + 2, cy + 3, l, 1);
            DrawLine(tex, cx + 2, cy - 3, cx - 2, cy + 3, l, 1);
        }

        // ====================================================================
        // 4. ROADSIDE / SHOULDER OBJECTS: KURGAN RAMP, YURT, AKSAKAL, BILLBOARDS
        // ====================================================================
        public static Sprite CreateKurganSprite()
        {
            int w = 54, h = 26;
            var tex = CreateTexture(w, h, Color.clear);
            Color sand = new Color(0.85f, 0.72f, 0.45f);
            Color darkSand = new Color(0.68f, 0.55f, 0.32f);
            Color steppeGrass = new Color(0.55f, 0.65f, 0.28f);

            // Mound / Ramp profile (gradual slope from left, steep drop on right or rounded crest)
            for (int x = 2; x < w - 2; x++)
            {
                float t = (float)x / (w - 4);
                // Ramp shape: rising up to 20 px then curving down
                int yTop = Mathf.RoundToInt(Mathf.Sin(t * Mathf.PI) * 20);
                for (int y = 0; y <= yTop; y++)
                {
                    Color c = (y > yTop - 3) ? steppeGrass : (y > yTop - 8 ? sand : darkSand);
                    tex.SetPixel(x, y, c);
                }
            }

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateYurtSprite()
        {
            int w = 64, h = 48;
            var tex = CreateTexture(w, h, Color.clear);
            Color feltWhite = new Color(0.94f, 0.93f, 0.88f);
            Color darkFelt = new Color(0.80f, 0.78f, 0.72f);
            Color koshkarRed = new Color(0.82f, 0.18f, 0.20f);
            Color gold = new Color(0.96f, 0.78f, 0.22f);
            Color doorWood = new Color(0.48f, 0.28f, 0.15f);

            // Yurt Wall (cylindrical lower body)
            DrawFilledRect(tex, 6, 4, 52, 16, feltWhite);

            // Yurt Dome (semi-ellipse top)
            DrawFilledEllipse(tex, 32, 20, 26, 18, feltWhite);

            // Shanyrak crown at apex
            DrawFilledEllipse(tex, 32, 38, 8, 4, gold);
            DrawFilledEllipse(tex, 32, 38, 5, 2, doorWood);

            // Red Kazakh ornamental ribbon band around wall
            DrawFilledRect(tex, 6, 16, 52, 4, koshkarRed);
            for (int x = 8; x < 56; x += 6)
            {
                tex.SetPixel(x, 18, gold);
                tex.SetPixel(x + 1, 18, gold);
            }

            // Wooden Door
            DrawFilledRect(tex, 28, 4, 8, 12, doorWood);
            DrawFilledRect(tex, 29, 6, 6, 9, koshkarRed);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateAksakalSprite()
        {
            int w = 32, h = 40;
            var tex = CreateTexture(w, h, Color.clear);
            Color chapanBlue = new Color(0.12f, 0.35f, 0.75f);
            Color gold = new Color(0.96f, 0.82f, 0.25f);
            Color beard = new Color(0.95f, 0.95f, 0.95f);
            Color skin = new Color(0.92f, 0.74f, 0.58f);
            Color staff = new Color(0.45f, 0.28f, 0.14f);

            // Robe / Chapan
            DrawFilledRect(tex, 10, 4, 12, 22, chapanBlue);
            DrawFilledRect(tex, 15, 4, 2, 22, gold); // golden lining

            // Head & Wise White Beard
            DrawFilledEllipse(tex, 16, 28, 4, 4, skin);
            DrawFilledEllipse(tex, 16, 25, 4, 4, beard); // long beard
            tex.SetPixel(15, 29, Color.black);
            tex.SetPixel(17, 29, Color.black);

            // Kazakh Kalpak (traditional tall hat with curved brim)
            DrawFilledRect(tex, 12, 32, 8, 2, Color.black);
            DrawFilledEllipse(tex, 16, 35, 3, 4, Color.white);
            tex.SetPixel(16, 38, gold);

            // Asa-tayak (Staff)
            DrawLine(tex, 24, 2, 24, 34, staff, 2);
            DrawFilledEllipse(tex, 24, 35, 3, 3, gold);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateToyBillboardSprite()
        {
            int w = 64, h = 44;
            var tex = CreateTexture(w, h, Color.clear);
            Color wood = new Color(0.42f, 0.28f, 0.16f);
            Color banner = new Color(0.85f, 0.15f, 0.18f);
            Color gold = new Color(0.98f, 0.85f, 0.25f);

            // Wooden posts
            DrawLine(tex, 12, 2, 12, 22, wood, 3);
            DrawLine(tex, 52, 2, 52, 22, wood, 3);

            // Billboard board
            DrawFilledRect(tex, 4, 20, 56, 22, banner);
            DrawRectOutline(tex, 4, 20, 56, 22, gold);

            // Text / ornament hint "ТОЙ БӘКЕ"
            DrawLine(tex, 8, 36, 56, 36, gold, 1);
            DrawLine(tex, 8, 26, 56, 26, gold, 1);
            // Smiley / Heart / Shanyrak icon
            DrawFilledEllipse(tex, 32, 31, 5, 4, gold);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateRoadSignSprite()
        {
            int w = 36, h = 40;
            var tex = CreateTexture(w, h, Color.clear);
            Color pole = new Color(0.6f, 0.6f, 0.65f);
            Color blueSign = new Color(0.08f, 0.35f, 0.82f);
            Color white = Color.white;

            DrawLine(tex, 18, 2, 18, 24, pole, 2);
            DrawFilledRect(tex, 4, 22, 28, 16, blueSign);
            DrawRectOutline(tex, 4, 22, 28, 16, white);
            // Arrow right ->
            DrawLine(tex, 10, 30, 26, 30, white, 2);
            DrawLine(tex, 22, 33, 26, 30, white, 2);
            DrawLine(tex, 22, 27, 26, 30, white, 2);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        // ====================================================================
        // 5. KAZAKH MONUMENTS (PARALLAX BACKGROUND)
        // ====================================================================
        public static Sprite CreateBaiterekSprite()
        {
            int w = 48, h = 120;
            var tex = CreateTexture(w, h, Color.clear);
            Color whiteStructure = new Color(0.92f, 0.94f, 0.98f);
            Color darkMetal = new Color(0.65f, 0.70f, 0.78f);
            Color goldenEgg = new Color(0.98f, 0.84f, 0.25f); // Samruk's Golden Egg!
            Color eggGlow = new Color(1f, 0.95f, 0.55f);

            // Tower Base
            DrawFilledRect(tex, 16, 2, 16, 12, darkMetal);

            // Flaring curved trunk
            for (int y = 14; y < 85; y++)
            {
                float t = (float)(y - 14) / (85 - 14);
                // Curves outward at the top
                int halfWidth = Mathf.RoundToInt(Mathf.Lerp(4f, 16f, t * t));
                DrawLine(tex, 24 - halfWidth, y, 24 + halfWidth, y, whiteStructure, 1);
                // Tree branches (petals)
                if (y > 60 && (y % 4 == 0))
                {
                    tex.SetPixel(24 - halfWidth, y, darkMetal);
                    tex.SetPixel(24 + halfWidth, y, darkMetal);
                }
            }

            // Giant Golden Sphere at the top (Egg of sacred bird Samruk)
            DrawFilledEllipse(tex, 24, 98, 14, 14, goldenEgg);
            DrawFilledEllipse(tex, 22, 102, 6, 6, eggGlow); // specular shine

            // Spire on top
            DrawLine(tex, 24, 112, 24, 118, goldenEgg, 2);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateKhanShatyrSprite()
        {
            int w = 110, h = 90;
            var tex = CreateTexture(w, h, Color.clear);
            Color tentTranslucent = new Color(0.75f, 0.88f, 0.95f, 0.85f);
            Color spireSteel = new Color(0.85f, 0.88f, 0.92f);
            Color cable = new Color(0.45f, 0.60f, 0.72f);
            Color baseArch = new Color(0.52f, 0.58f, 0.65f);

            // Conical asymmetrical tent
            int apexX = 45, apexY = 82;

            for (int y = 4; y <= apexY; y++)
            {
                float t = (float)(y - 4) / (apexY - 4);
                int leftX = Mathf.RoundToInt(Mathf.Lerp(8f, apexX, t));
                int rightX = Mathf.RoundToInt(Mathf.Lerp(102f, apexX, t));
                for (int x = leftX; x <= rightX; x++)
                {
                    Color c = ((x - leftX) % 8 == 0) ? cable : tentTranslucent;
                    tex.SetPixel(x, y, c);
                }
            }

            // Central inclined spire
            DrawLine(tex, apexX, apexY, apexX - 6, apexY + 7, spireSteel, 3);
            DrawFilledRect(tex, 6, 2, 98, 4, baseArch);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateYasawiSprite()
        {
            int w = 96, h = 72;
            var tex = CreateTexture(w, h, Color.clear);
            Color bakedBrick = new Color(0.82f, 0.58f, 0.38f);
            Color darkBrick = new Color(0.68f, 0.44f, 0.25f);
            Color turquoiseDome = new Color(0.12f, 0.72f, 0.75f); // Famous azure turquoise dome!
            Color turquoiseGlaze = new Color(0.20f, 0.85f, 0.88f);
            Color shadow = new Color(0.15f, 0.12f, 0.10f);

            // Main brick monumental portal (Peshtak)
            DrawFilledRect(tex, 16, 2, 64, 46, bakedBrick);

            // Corner Minaret Towers
            DrawFilledRect(tex, 8, 2, 10, 52, darkBrick);
            DrawFilledRect(tex, 78, 2, 10, 52, darkBrick);

            // Grand Archway (Iwan)
            DrawFilledEllipse(tex, 48, 30, 14, 16, shadow);
            DrawFilledRect(tex, 34, 2, 28, 28, shadow);

            // Ribbed Turquoise Dome behind portal
            DrawFilledEllipse(tex, 48, 54, 18, 15, turquoiseDome);
            for (int dx = -14; dx <= 14; dx += 4)
                DrawLine(tex, 48 + dx, 50, 48 + dx / 2, 68, turquoiseGlaze, 1);

            // Majolica tiles band
            DrawFilledRect(tex, 18, 44, 60, 4, turquoiseDome);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateGoldenManSprite()
        {
            int w = 48, h = 100;
            var tex = CreateTexture(w, h, Color.clear);
            Color marble = new Color(0.88f, 0.90f, 0.92f);
            Color gold = new Color(0.98f, 0.82f, 0.22f);
            Color darkGold = new Color(0.78f, 0.60f, 0.12f);

            // Tall Stele column
            DrawFilledRect(tex, 20, 2, 8, 70, marble);
            DrawFilledRect(tex, 14, 2, 20, 6, marble);

            // Winged Snow Leopard (Барс) atop stele
            DrawFilledRect(tex, 16, 72, 16, 8, darkGold);
            // Wings
            DrawLine(tex, 16, 76, 8, 86, gold, 3);
            DrawLine(tex, 32, 76, 40, 86, gold, 3);

            // Golden Warrior standing proudly on leopard
            DrawFilledRect(tex, 22, 80, 4, 12, gold);
            // Tall conical golden headdress
            DrawLine(tex, 24, 92, 24, 99, gold, 2);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        // ====================================================================
        // 5b. NEW KAZAKH LANDMARKS
        // ====================================================================

        /// <summary>EXPO 2017 Nur-Sultan - the iconic futuristic Nur Alem sphere pavilion.</summary>
        public static Sprite CreateExpoPavilionSprite()
        {
            int w = 140, h = 130;
            var tex = CreateTexture(w, h, Color.clear);
            Color steelBlue  = new Color(0.22f, 0.45f, 0.75f);
            Color steelLight = new Color(0.45f, 0.68f, 0.92f);
            Color glass      = new Color(0.62f, 0.82f, 1.00f, 0.75f);
            Color glassShine = new Color(0.88f, 0.96f, 1.00f, 0.90f);
            Color structGray = new Color(0.52f, 0.56f, 0.64f);
            Color gold       = new Color(0.98f, 0.82f, 0.22f);
            Color dark       = new Color(0.12f, 0.18f, 0.28f);

            // --- Base platform ---
            DrawFilledRect(tex, 30, 2, 80, 8, structGray);
            DrawFilledRect(tex, 50, 8, 40, 16, structGray);

            // --- Main sphere (Nur Alem globe) ---
            DrawFilledEllipse(tex, 70, 72, 50, 50, dark);
            DrawFilledEllipse(tex, 70, 72, 48, 48, steelBlue);
            DrawFilledEllipse(tex, 70, 72, 45, 45, glass);

            // Latitude rings
            for (int lat = -30; lat <= 30; lat += 15)
            {
                int ry2 = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Max(0, 45f * 45f - (float)lat * lat / 50f * 50f)));
                if (ry2 < 2) continue;
                DrawLine(tex, 70 - ry2, 72 + lat, 70 + ry2, 72 + lat, steelLight, 1);
            }
            // Longitude arcs (simplified as vertical ellipses)
            for (int lon = -3; lon <= 3; lon += 1)
            {
                int lx = 70 + lon * 14;
                if (lx < 25 || lx > 115) continue;
                DrawLine(tex, lx, 28, lx, 116, steelLight, 1);
            }

            // Shine on upper-left of sphere
            DrawFilledEllipse(tex, 48, 92, 14, 10, glassShine);
            DrawFilledEllipse(tex, 45, 94, 8, 6, new Color(1f, 1f, 1f, 0.7f));

            // Gold top ornament
            DrawLine(tex, 70, 120, 70, 128, gold, 3);
            DrawFilledEllipse(tex, 70, 128, 4, 4, gold);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        /// <summary>Kok-Tobe - Almaty TV tower on a hill with gondola cable car.</summary>
        public static Sprite CreateKokTobeSprite()
        {
            int w = 96, h = 160;
            var tex = CreateTexture(w, h, Color.clear);
            Color hillGreen  = new Color(0.25f, 0.52f, 0.22f);
            Color hillDark   = new Color(0.18f, 0.38f, 0.16f);
            Color hillLight  = new Color(0.42f, 0.70f, 0.35f);
            Color towerGray  = new Color(0.55f, 0.58f, 0.65f);
            Color towerDark  = new Color(0.32f, 0.34f, 0.40f);
            Color redBand    = new Color(0.85f, 0.12f, 0.12f);
            Color antWhite   = new Color(0.92f, 0.94f, 0.96f);
            Color gondolaRed = new Color(0.80f, 0.15f, 0.15f);
            Color cableCol   = new Color(0.45f, 0.45f, 0.50f);
            Color gold       = new Color(0.98f, 0.82f, 0.22f);

            // Hill silhouette
            for (int x = 0; x < w; x++)
            {
                float t = (float)x / w;
                int hillH = Mathf.RoundToInt(30f + 22f * Mathf.Sin(t * Mathf.PI));
                for (int y = 0; y <= hillH; y++)
                {
                    Color hc = (y > hillH - 4) ? hillLight : (y > hillH - 14 ? hillGreen : hillDark);
                    tex.SetPixel(x, y, hc);
                }
            }

            // TV Tower base - tapering trunk
            for (int y = 30; y < 100; y++)
            {
                float t = (float)(y - 30) / 70f;
                int halfW = Mathf.RoundToInt(Mathf.Lerp(7f, 3f, t));
                DrawLine(tex, 48 - halfW, y, 48 + halfW, y, towerGray, 1);
                if (y % 8 == 0) DrawLine(tex, 48 - halfW - 2, y, 48 + halfW + 2, y, towerDark, 1); // ledge ring
            }

            // Observation deck disc at 100
            DrawFilledEllipse(tex, 48, 100, 14, 5, towerDark);
            DrawFilledEllipse(tex, 48, 101, 12, 4, towerGray);
            DrawFilledRect(tex, 39, 100, 18, 8, towerGray); // windows band
            for (int wx = 41; wx < 57; wx += 3) DrawFilledRect(tex, wx, 101, 2, 5, new Color(0.65f, 0.82f, 1f)); // window glass

            // Red accent bands
            DrawLine(tex, 38, 108, 58, 108, redBand, 2);

            // Needle/spire
            DrawLine(tex, 48, 105, 48, 155, antWhite, 3);
            DrawLine(tex, 47, 105, 47, 150, towerDark, 1);
            // Red banding on spire
            DrawLine(tex, 46, 125, 51, 125, redBand, 2);
            DrawLine(tex, 46, 140, 51, 140, redBand, 2);
            // Gold tip
            tex.SetPixel(48, 155, gold); tex.SetPixel(48, 156, gold);

            // Cable car wire (diagonal)
            DrawLine(tex, 0, 22, 80, 38, cableCol, 1);
            // Gondola capsule
            DrawFilledRect(tex, 15, 26, 10, 7, gondolaRed);
            DrawFilledRect(tex, 16, 27, 8, 5, new Color(0.65f, 0.82f, 1f, 0.9f)); // window
            DrawLine(tex, 20, 33, 20, 37, cableCol, 1); // hanger

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        /// <summary>Medeu - iconic high-altitude skating rink outside Almaty in a mountain valley.</summary>
        public static Sprite CreateMedeuSprite()
        {
            int w = 140, h = 100;
            var tex = CreateTexture(w, h, Color.clear);
            Color mtnRock   = new Color(0.38f, 0.40f, 0.50f);
            Color mtnDark   = new Color(0.24f, 0.26f, 0.35f);
            Color snow      = new Color(0.94f, 0.96f, 1.00f);
            Color iceBlue   = new Color(0.72f, 0.88f, 0.98f);
            Color iceShine  = new Color(0.90f, 0.96f, 1.00f);
            Color concrete  = new Color(0.62f, 0.62f, 0.64f);
            Color concDark  = new Color(0.44f, 0.44f, 0.48f);
            Color treeGreen = new Color(0.20f, 0.45f, 0.18f);
            Color treeDark  = new Color(0.14f, 0.32f, 0.12f);
            Color flagRed   = new Color(0.82f, 0.12f, 0.12f);

            // Background mountain peaks
            int[] peaks2 = { 28, 70, 115 };
            int[] heights2 = { 88, 98, 82 };
            for (int p = 0; p < peaks2.Length; p++)
            {
                int cx = peaks2[p], ht = heights2[p];
                for (int y = 2; y <= ht; y++)
                {
                    float t = (float)y / ht;
                    int spread = Mathf.RoundToInt((1f - t) * 32);
                    for (int x = cx - spread; x <= cx + spread; x++)
                    {
                        if (x < 0 || x >= w) continue;
                        Color c = (y > ht - 16) ? snow : (x < cx ? mtnRock : mtnDark);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            // Trees on slopes
            for (int i = 0; i < 18; i++)
            {
                int tx2 = 8 + i * 7, ty2 = 18 + (int)(Mathf.Sin(i * 1.3f) * 5);
                DrawLine(tex, tx2, ty2, tx2, ty2 + 10, treeDark, 2);
                DrawFilledEllipse(tex, tx2, ty2 + 12, 4, 5, treeGreen);
            }

            // Dam / embankment - tiered concrete terraces
            DrawFilledRect(tex, 20, 4, 100, 6, concrete);
            DrawFilledRect(tex, 24, 10, 92, 5, concDark);
            DrawFilledRect(tex, 28, 15, 84, 5, concrete);

            // Ice rink oval
            DrawFilledEllipse(tex, 70, 12, 38, 8, iceBlue);
            DrawFilledEllipse(tex, 70, 12, 35, 6, iceShine);
            // Rink lines
            DrawLine(tex, 70, 6, 70, 18, iceBlue, 1);
            DrawLine(tex, 50, 12, 90, 12, iceBlue, 1);

            // Stands / stadium walls
            DrawFilledRect(tex, 30, 18, 80, 4, concDark);

            // Flag poles
            DrawLine(tex, 55, 20, 55, 30, concrete, 1);
            DrawFilledRect(tex, 55, 28, 8, 4, flagRed);
            DrawLine(tex, 85, 20, 85, 30, concrete, 1);
            DrawFilledRect(tex, 85, 28, 8, 4, flagRed);

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateMountainSprite()
        {
            int w = 160, h = 70;
            var tex = CreateTexture(w, h, Color.clear);
            Color rock = new Color(0.35f, 0.38f, 0.48f);
            Color darkRock = new Color(0.24f, 0.26f, 0.35f);
            Color snow = new Color(0.95f, 0.97f, 1f);

            int[] peaks = new int[] { 25, 70, 120 };
            int[] heights = new int[] { 58, 68, 52 };

            for (int p = 0; p < peaks.Length; p++)
            {
                int cx = peaks[p], hTop = heights[p];
                for (int y = 0; y <= hTop; y++)
                {
                    float t = (float)y / hTop;
                    int spread = Mathf.RoundToInt((1f - t) * 36);
                    for (int x = cx - spread; x <= cx + spread; x++)
                    {
                        if (x < 0 || x >= w) continue;
                        Color c = (y > hTop - 14) ? snow : ((x < cx) ? rock : darkRock);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            return ToSprite(tex, new Vector2(0.5f, 0f));
        }

        public static Sprite CreateCloudSprite()
        {
            int w = 64, h = 24;
            var tex = CreateTexture(w, h, Color.clear);
            Color cloud = new Color(1f, 1f, 1f, 0.8f);

            DrawFilledEllipse(tex, 24, 10, 16, 8, cloud);
            DrawFilledEllipse(tex, 40, 11, 14, 9, cloud);
            DrawFilledEllipse(tex, 15, 8, 10, 6, cloud);

            return ToSprite(tex);
        }

        private static Sprite CreateFallbackSprite()
        {
            var tex = CreateTexture(16, 16, Color.magenta);
            return ToSprite(tex);
        }

        // ====================================================================
        // PRIMITIVE DRAWING HELPERS
        // ====================================================================
        private static void DrawFilledRect(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            for (int px = x; px < x + w; px++)
                for (int py = y; py < y + h; py++)
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, c);
        }

        private static void DrawRectOutline(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            for (int px = x; px < x + w; px++)
            {
                if (px >= 0 && px < tex.width)
                {
                    if (y >= 0 && y < tex.height) tex.SetPixel(px, y, c);
                    if (y + h - 1 >= 0 && y + h - 1 < tex.height) tex.SetPixel(px, y + h - 1, c);
                }
            }
            for (int py = y; py < y + h; py++)
            {
                if (py >= 0 && py < tex.height)
                {
                    if (x >= 0 && x < tex.width) tex.SetPixel(x, py, c);
                    if (x + w - 1 >= 0 && x + w - 1 < tex.width) tex.SetPixel(x + w - 1, py, c);
                }
            }
        }

        private static void DrawFilledEllipse(Texture2D tex, int cx, int cy, int rx, int ry, Color c)
        {
            for (int x = cx - rx; x <= cx + rx; x++)
            {
                for (int y = cy - ry; y <= cy + ry; y++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                    float dx = (float)(x - cx) / rx;
                    float dy = (float)(y - cy) / ry;
                    if (dx * dx + dy * dy <= 1f)
                        tex.SetPixel(x, y, c);
                }
            }
        }

        private static void DrawFilledDiamond(Texture2D tex, int cx, int cy, int rx, int ry, Color c)
        {
            for (int x = cx - rx; x <= cx + rx; x++)
            {
                for (int y = cy - ry; y <= cy + ry; y++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                    float d = Mathf.Abs((float)(x - cx) / rx) + Mathf.Abs((float)(y - cy) / ry);
                    if (d <= 1f)
                        tex.SetPixel(x, y, c);
                }
            }
        }

        private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color c, int thickness = 1)
        {
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                for (int tx = -thickness / 2; tx <= thickness / 2; tx++)
                {
                    for (int ty = -thickness / 2; ty <= thickness / 2; ty++)
                    {
                        int px = x0 + tx, py = y0 + ty;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                            tex.SetPixel(px, py, c);
                    }
                }
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
