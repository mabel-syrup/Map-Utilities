using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MapUtilities.Particles
{
    public static class ParticleHandler
    {
        public static List<ParticleSystem> systems;

        public static void init()
        {
            systems = new List<ParticleSystem>();
        }

        public static void draw(SpriteBatch b)
        {
            foreach(ParticleSystem system in systems)
            {
                system.draw(b);
            }
        }

        public static void update(GameTime time, GameLocation location)
        {
            foreach (ParticleSystem system in systems)
            {
                system.update(time, location);
            }
        }
    }
}
