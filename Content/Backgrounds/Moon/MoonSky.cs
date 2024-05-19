﻿using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Macrocosm.Content.Backgrounds.Moon
{
    public class MoonSky : CustomSky, ILoadable
    {
        public bool Active;
        public float Intensity;

        private readonly Stars starsDay;
        private readonly Stars starsNight;

        private readonly CelestialBody earth;
        private readonly CelestialBody sun;

        private readonly Asset<Texture2D> skyTexture;

        private readonly Asset<Texture2D> sunTexture;

        private readonly Asset<Texture2D> earthBody;
        private readonly Asset<Texture2D> earthBodyDrunk;
        private readonly Asset<Texture2D> earthBodyFlat;
        private readonly Asset<Texture2D> earthAtmo;

        private readonly Asset<Texture2D> nebulaYellow;
        private readonly Asset<Texture2D> nebulaRinged;
        private readonly Asset<Texture2D> nebulaMythril;
        private readonly Asset<Texture2D> nebulaBlue;
        private readonly Asset<Texture2D> nebulaGreen;
        private readonly Asset<Texture2D> nebulaPink;
        private readonly Asset<Texture2D> nebulaOrange;
        private readonly Asset<Texture2D> nebulaPurple;
        private readonly Asset<Texture2D> nebulaNormal;

        const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
        const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten

        const string AssetPath = "Macrocosm/Content/Backgrounds/Moon/";

        public MoonSky()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            skyTexture = ModContent.Request<Texture2D>(AssetPath + "MoonSky", mode);

            sunTexture = ModContent.Request<Texture2D>(AssetPath + "Sun", mode);
            earthBody = ModContent.Request<Texture2D>(AssetPath + "Earth", mode);
            earthBodyDrunk = ModContent.Request<Texture2D>(AssetPath + "EarthDrunk", mode);
            earthBodyFlat = ModContent.Request<Texture2D>(AssetPath + "EarthFlat", mode);

            earthAtmo = ModContent.Request<Texture2D>(AssetPath + "EarthAtmo", mode);

            starsDay = new();
            starsNight = new();

            sun = new CelestialBody(sunTexture);
            earth = new CelestialBody(earthBody, earthAtmo, scale: 0.9f);

            sun.SetupSkyRotation(CelestialBody.SkyRotationMode.Day);

            earth.SetParallax(0.01f, 0.12f, new Vector2(0f, -200f));

            earth.SetLighting(sun);
            earth.ConfigureBackRadialShader = ConfigureEarthAtmoShader;
            earth.ConfigureBodySphericalShader = ConfigureEarthBodyShader;

            nebulaYellow = ModContent.Request<Texture2D>(AssetPath + "NebulaYellow");
            nebulaRinged = ModContent.Request<Texture2D>(AssetPath + "NebulaRinged");
            nebulaMythril = ModContent.Request<Texture2D>(AssetPath + "NebulaMythril");
            nebulaBlue = ModContent.Request<Texture2D>(AssetPath + "NebulaBlue");
            nebulaGreen = ModContent.Request<Texture2D>(AssetPath + "NebulaGreen");
            nebulaPink = ModContent.Request<Texture2D>(AssetPath + "NebulaPink");
            nebulaOrange = ModContent.Request<Texture2D>(AssetPath + "NebulaOrange");
            nebulaPurple = ModContent.Request<Texture2D>(AssetPath + "NebulaPurple");
            nebulaNormal = ModContent.Request<Texture2D>(AssetPath + "NebulaNormal");
        }

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            SkyManager.Instance["Macrocosm:MoonSky"] = new MoonSky();
        }

        public void Unload() { }

        public override void Activate(Vector2 position, params object[] args)
        {
            starsDay.SpawnStars(100, 130, baseScale: 1.4f, twinkleFactor: 0.05f);
            starsNight.SpawnStars(600, 700, baseScale: 0.8f, twinkleFactor: 0.05f);

            MacrocosmStar mars = starsDay.RandStar(); // :) 
            mars.OverrideColor(new Color(224, 137, 8, 220));
            mars.Scale *= 1.4f;

            Intensity = 0.002f;
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            Intensity = 0f;
            starsDay.Clear();
            starsNight.Clear();
            Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SubworldSystem.IsActive<Subworlds.Moon>() && maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Main.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Draw(skyTexture.Value, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * Intensity);

                float nebulaBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.17f, 0.45f);
                float nightStarBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.1f, 0.8f);

                DrawMoonNebula(nebulaBrightness);

                starsDay.Draw(spriteBatch);
                starsNight.Draw(spriteBatch, nightStarBrightness);

                sun.Draw(spriteBatch);
                earth.Draw(spriteBatch);
            }
        }

        private void DrawMoonNebula(float brightness)
        {
            Texture2D nebula = Main.moonType switch
            {
                1 => nebulaYellow.Value,
                2 => nebulaRinged.Value,
                3 => nebulaMythril.Value,
                4 => nebulaBlue.Value,
                5 => nebulaGreen.Value,
                6 => nebulaPink.Value,
                7 => nebulaOrange.Value,
                8 => nebulaPurple.Value,
                _ => nebulaNormal.Value
            };

            Color nebulaColor = (Color.White * brightness).WithOpacity(0f);

            Main.spriteBatch.Draw(nebula, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), nebulaColor);
        }

        public override void Update(GameTime gameTime)
        {
            if (!SubworldSystem.IsActive<Subworlds.Moon>())
                Active = false;

            Intensity = Active ? Math.Min(1f, Intensity + 0.01f) : Math.Max(0f, Intensity - 0.01f);
            SetEarthTextures();
        }

        private void SetEarthTextures()
        {
            if (Utility.IsAprilFools())
            {
                earth.SetLighting(null);
                earth.SetTextures(earthBodyFlat);
            }
            else
            {
                earth.SetLighting(sun);

                if (Main.drunkWorld)
                    earth.SetTextures(earthBodyDrunk, earthAtmo);
                else
                    earth.SetTextures(earthBody, earthAtmo);
            }
        }

        private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
        {
            float brightness;

            float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

            if (Main.dayTime)
            {
                if (Main.time <= fadeOutTimeDawn)
                    brightness = (maxBrightnessDay + ((1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor));
                else if (Main.time >= fadeInTimeDusk)
                    brightness = (maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor);
                else
                    brightness = maxBrightnessDay;
            }
            else
            {
                brightness = maxBrightnessNigt;
            }

            return brightness;
        }

        private void ConfigureEarthBodyShader(CelestialBody celestialBody, CelestialBody lightSource, out Vector3 lightPosition, out float radius, out int pixelSize) 
        {
            float distanceFactor;
            float depth;
            if (Main.dayTime)
            {
                distanceFactor = MathHelper.Clamp(Vector2.Distance(celestialBody.Center, lightSource.Center) / Math.Max(celestialBody.Width * 2, celestialBody.Height * 2), 0, 1);
                depth = MathHelper.Lerp(-60, 400, Utility.QuadraticEaseIn(distanceFactor));
                lightPosition = new Vector3(Utility.ClampOutsideCircle(lightSource.Center, celestialBody.Center, earth.Width / 2 * 2), depth);
                radius = 0.1f;
            }
            else
            {
                distanceFactor = MathHelper.Clamp(Vector2.Distance(celestialBody.Center, lightSource.Center) / Math.Max(celestialBody.Width * 2, celestialBody.Height * 2), 0, 1);
                depth = MathHelper.Lerp(400, 5000, 1f - Utility.QuadraticEaseOut(distanceFactor));
                lightPosition = new Vector3(Utility.ClampOutsideCircle(lightSource.Center, celestialBody.Center, earth.Width / 2 * 2), depth);
                radius = 0.01f;
            }

            pixelSize = 1;
        }

        private void ConfigureEarthAtmoShader(CelestialBody earth, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution)
        {
            Vector2 screenSize = Main.ScreenSize.ToVector2();
            float distance = Vector2.Distance(earth.Center / screenSize, earth.LightSource.Center / screenSize);
            float offsetRadius = MathHelper.Lerp(0.12f, 0.56f, 1 - distance);

            if (!Main.dayTime)
            {
                offsetRadius = MathHelper.Lerp(0.56f, 0.01f, 1 - distance);
            }
            else
            {
                if (distance < 0.1f)
                {
                    float proximityFactor = 1 - (distance / 0.1f);
                    offsetRadius += 0.8f * proximityFactor;
                }
            }

            offset = Utility.PolarVector(offsetRadius, rotation) * 0.65f;
            intensity = 0.96f;
            shadeResolution /= 2;
            radius = 1f;
        }

        public override Color OnTileColor(Color inColor)
        {
            Color darkColor = new Color(35, 35, 35);
            Color earthshineBlue = Color.Lerp(new Color(39, 87, 155), darkColor, 0.6f);

            if (Main.dayTime)
            {
                if (Main.time < MacrocosmSubworld.CurrentDayLength * 0.1)
                    return Color.Lerp(darkColor, Color.White, (float)(Main.time / (MacrocosmSubworld.CurrentDayLength * 0.1)));
                else if (Main.time > MacrocosmSubworld.CurrentDayLength * 0.9)
                    return Color.Lerp(darkColor, Color.White, (float)((MacrocosmSubworld.CurrentDayLength - Main.time) / (MacrocosmSubworld.CurrentDayLength - MacrocosmSubworld.CurrentDayLength * 0.9)));
                else
                    return Color.White;
            }
            else
            {
                if (Main.time < MacrocosmSubworld.CurrentNightLength * 0.2)
                    return Color.Lerp(darkColor, earthshineBlue, (float)(Main.time / (MacrocosmSubworld.CurrentNightLength * 0.2)));
                else if (Main.time > MacrocosmSubworld.CurrentNightLength * 0.8)
                    return Color.Lerp(darkColor, earthshineBlue, (float)((MacrocosmSubworld.CurrentNightLength - Main.time) / (MacrocosmSubworld.CurrentNightLength - MacrocosmSubworld.CurrentNightLength * 0.8)));
                else
                    return earthshineBlue;
            }
        }

        public override float GetCloudAlpha() => 0f;

        public override void Reset()
        {
            starsDay.Clear();
            starsNight.Clear();
            Active = false;
        }

        public override bool IsActive()
        {
            return Active;
        }
    }
}