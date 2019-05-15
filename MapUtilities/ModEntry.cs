using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;
using MapUtilities.Parallax;
using MapUtilities.Pseudo3D;
using MapUtilities.Trees;
using MapUtilities.Particles;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities
{
    public class ModEntry : Mod
    {

        public override void Entry(IModHelper helper)
        {
            Logger.monitor = Monitor;
            Loader.loader = helper.Content;
            Reflector.reflector = helper.Reflection;
            ParticleHandler.init();
            Pseudo3D.LevelHandler.initialize();
            TreeHandler.init();
            HarmonyInstance harmony = HarmonyInstance.Create("mabelsyrup.farmhouse");

            harmony.Patch(
                original: AccessTools.Method(typeof(Background), nameof(Background.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Background_draw_Patch), nameof(Background_draw_Patch.Prefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Background), nameof(Background.update)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Background_update_Patch), nameof(Background_update_Patch.Prefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.MovePosition)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Farmer_MovePosition_Patch), nameof(Farmer_MovePosition_Patch.Prefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Tree_draw_Patch), nameof(Tree_draw_Patch.Prefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new Type[] 
                {
                    typeof(Microsoft.Xna.Framework.Rectangle),
                    typeof(xTile.Dimensions.Rectangle),
                    typeof(bool),
                    typeof(int),
                    typeof(bool),
                    typeof(Character)
                }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_isCollidingPosition_Patch), nameof(GameLocation_isCollidingPosition_Patch.Prefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.Update)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Farmer_Update_Patch), nameof(Farmer_Update_Patch.Postfix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_draw_Patch), nameof(GameLocation_draw_Patch.Prefix)))
            );

            helper.Events.GameLoop.DayStarted += newDay;
            helper.Events.Display.RenderedWorld += drawExtraLayers;
            helper.Events.GameLoop.TimeChanged += performTenMinuteUpdate;
            helper.Events.Player.Warped += performLocationSetup;
            helper.Events.GameLoop.UpdateTicked += performTickUpdate;
        }

        public void newDay(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            BackgroundHandler.updateBackground();
            ParticleSystem testSystem = new ParticleSystem(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(32, 0, 10, 10), new Microsoft.Xna.Framework.Vector2(24, 10), 4, 0.1f, 50, 2400, 
                new Dictionary<int, float>
                {
                    {ParticleSystem.Out, 0 },
                    {ParticleSystem.North, 12 },
                    {ParticleSystem.East, 3 },
                    {ParticleSystem.Up, 0 },
                    {ParticleSystem.Right, 0 }
                },
                new Dictionary<int, float>
                {
                    {ParticleSystem.Out, 0 },
                    {ParticleSystem.North, -9.8f },
                    {ParticleSystem.East, 0 },
                    {ParticleSystem.Up, 0 },
                    {ParticleSystem.Right, 0 }
                }
            );
            ParticleHandler.systems.Add(testSystem);
        }

        public void drawExtraLayers(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (Pseudo3D.MapHandler.isPseudo3DLocation(Game1.currentLocation))
            {
                Pseudo3D.MapHandler.drawOverlays(e.SpriteBatch, Game1.currentLocation);
            }
            //e.SpriteBatch.End();
            //e.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //ParticleHandler.draw(e.SpriteBatch);
        }

        public void performTenMinuteUpdate(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            Time.TimeHandler.tenMinuteUpdate();
        }

        public void performLocationSetup(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            BackgroundHandler.updateBackground();
            Time.TimeHandler.applyAllLayersToNow(e.NewLocation);
            TreeHandler.createAllTrees(e.NewLocation);
        }

        public void performTickUpdate(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            ParticleHandler.update(Game1.currentGameTime, Game1.currentLocation);
        }
    }
}
