using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Parallax
{
    public static class BackgroundHandler
    {
        public static void updateBackground()
        {
            GameLocation location = Game1.currentLocation;
            if(location != null && location.map != null && location.map.Properties.ContainsKey("Background"))
            {
                Logger.log("Found Background property, " + location.map.Properties["Background"].ToString());
                try
                {
                    List<ParallaxLayer> layers = new List<ParallaxLayer>();

                    Dictionary<string, string> backgroundSource = Loader.loader.Load<Dictionary<string, string>>("Content/Parallax/" + location.map.Properties["Background"].ToString() + "", ContentSource.ModFolder);
                    Logger.log("Found background source...");
                    foreach (string layerString in backgroundSource.Keys)
                    {
                        Logger.log("Adding layer " + layerString + ": " + backgroundSource[layerString]);
                        string[] layerDefs = backgroundSource[layerString].Split('/');
                        ParallaxLayer layer = new ParallaxLayer(Loader.loader.Load<Texture2D>("Content/Parallax/" + layerDefs[0] + ".png", ContentSource.ModFolder), Convert.ToSingle(layerDefs[1]));
                        layer.zoomScale = Convert.ToInt32(layerDefs[2]);

                        layers.Add(layer);
                    }

                    ParallaxBackground bg = new ParallaxBackground(layers);

                    Game1.background = bg;
                }
                catch( Microsoft.Xna.Framework.Content.ContentLoadException)
                {

                }

                //layers.Add(new ParallaxLayer(Loader.loader.Load<Texture2D>("Content/Parallax/realtest_back.png", ContentSource.ModFolder), 0f));
                //layers.Add(new ParallaxLayer(Loader.loader.Load<Texture2D>("Content/Parallax/realtest_vista.png", ContentSource.ModFolder), 0.1f));
                //layers.Add(new ParallaxLayer(Loader.loader.Load<Texture2D>("Content/Parallax/realtest_trees_back.png", ContentSource.ModFolder), 0.85f));
                //layers.Add(new ParallaxLayer(Loader.loader.Load<Texture2D>("Content/Parallax/realtest_trees_front.png", ContentSource.ModFolder), 0.9f));

                
            }
            else
            {
                Logger.log("Location did not have a Background property");
                Game1.background = null;
            }
        }
    }
}
