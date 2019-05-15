using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Trees
{
    public class LeafCluster : TreePart
    {
        public LeafCluster(TreeRenderer renderer)
        {
            children = new List<TreePart>();
            spriteSheet = TreeHandler.treeImage;
            this.renderer = renderer;
            sprite = new Microsoft.Xna.Framework.Rectangle(32, 0, 48, 32);
            rotation = 0f;
            depth = -1;
        }

        public override void draw(SpriteBatch b, Vector2 treePos, float x, float y, float currentRotation, float depthOffset = 0f)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(treePos.X * 64, treePos.Y * 64));
            Vector2 farmerLocalPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(Game1.player.getTileX() * 64, Game1.player.getTileY() * 64));
            Color foliageFade = Color.White;
            float distanceToPlayer = (float)(Math.Min(Math.Sqrt(Math.Pow((local.X + x) - farmerLocalPosition.X, 2) + Math.Pow((local.Y + y) - farmerLocalPosition.Y, 2)),(64 * 7f)));
            float distanceMult = (distanceToPlayer / (64 * 7f));
            //foliageFade.R = (byte)(255 * distanceMult);
            //foliageFade.B = (byte)(255 * distanceMult);
            //foliageFade.G = (byte)(128 * distanceMult + 128);
            foliageFade *= Math.Min(distanceMult, renderer.transparency.A / 255f);
            b.Draw(spriteSheet, new Vector2(x + local.X, y + local.Y), sprite, foliageFade, 0f, new Vector2(sprite.Width / 2, sprite.Height / 2), 4f, SpriteEffects.None, (treePos.Y * 64) / 10000 + ((treePos.X * 64) % 9 - depth * 10) / 10000 + depthOffset + 0.0004f);
            
        }
    }
}
