using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Particles
{
    public class ParticleSystem
    {
        public const int Out = 0;
        public const int North = 1;
        public const int East = 2;
        public const int Up = 3;
        public const int Right = 4;

        public Texture2D particleSheet;
        public Rectangle particleRect;

        public Vector2 tileLocation;

        public float scale;

        public float range;

        public int count;

        public int longetivity;

        public List<Particle> particles;

        public Dictionary<int, float> velocities;

        public Dictionary<int, float> accelerations;

        public ParticleSystem(Texture2D spriteSheet, Rectangle sprite, Vector2 location, float scale = 4f, float range = 2f, int count = 4, int longetivity = 1200, Dictionary<int, float> velocities = null, Dictionary<int, float> accelerations = null)
        {
            this.particleSheet = spriteSheet;
            this.particleRect = sprite;
            this.tileLocation = location;
            this.scale = scale;
            this.range = range;
            this.count = count;
            this.longetivity = longetivity;
            if (velocities == null)
            {
                this.velocities = new Dictionary<int, float>();
                this.velocities[Out] = 0;
                this.velocities[North] = 0;
                this.velocities[East] = 0;
                this.velocities[Up] = 0;
                this.velocities[Right] = 0;
            }
            else
            {
                this.velocities = velocities;
                if (!this.velocities.ContainsKey(Out))
                    this.velocities[Out] = 0;
                if (!this.velocities.ContainsKey(North))
                    this.velocities[North] = 0;
                if (!this.velocities.ContainsKey(East))
                    this.velocities[East] = 0;
                if (!this.velocities.ContainsKey(Up))
                    this.velocities[Up] = 0;
                if (!this.velocities.ContainsKey(Right))
                    this.velocities[Right] = 0;
            }

            if (accelerations == null)
                this.accelerations = new Dictionary<int, float>();
            else
                this.accelerations = accelerations;

            if (!this.accelerations.ContainsKey(Out))
                this.accelerations[Out] = 0;
            if (!this.accelerations.ContainsKey(North))
                this.accelerations[North] = 0;
            if (!this.accelerations.ContainsKey(East))
                this.accelerations[East] = 0;
            if (!this.accelerations.ContainsKey(Up))
                this.accelerations[Up] = 0;
            if (!this.accelerations.ContainsKey(Right))
                this.accelerations[Right] = 0;
            this.particles = new List<Particle>();
        }

        public void update(GameTime time, GameLocation location)
        {
            while(particles.Count < count)
            {
                //Logger.log("Adding particle.");
                spawnParticle(time);
            }
            List<int> particlesToDespawn = new List<int>();
            for(int i = 0; i < particles.Count; i++)
            {
                if (particles[i].lifetime >= particles[i].lifeDuration)
                {
                    //Logger.log("Killing particle.");
                    particlesToDespawn.Add(i);
                }
                else
                {
                    particles[i].lifetime += time.ElapsedGameTime.Milliseconds;
                    //Logger.log("Particle lifetime: " + particles[i].lifetime + ", longetivity: " + longetivity + ", game time: " + time.ElapsedGameTime.Milliseconds + ", difference: " + (time.ElapsedGameTime.Milliseconds - (particles[i].lifetime + longetivity)));
                }
            }
            particlesToDespawn.Reverse();
            foreach(int index in particlesToDespawn)
            {
                particles.RemoveAt(index);
            }

            updateParticlePositions();
        }

        public void updateParticlePositions()
        {
            foreach (Particle particle in particles)
            {
                float deltaX = 0f;
                float deltaY = 0f;

                if (velocities.ContainsKey(Out))
                {
                    float unitMultiplier = 1f / (float)(Math.Sqrt(Math.Pow(particle.position.X, 2) + Math.Pow(particle.position.Y, 2)));
                    float xMult = particle.position.X * unitMultiplier;
                    float yMult = particle.position.Y * unitMultiplier;
                    deltaX += xMult * velocities[Out] + (accelerations[Out] * (particle.lifetime / 1000f) * xMult);
                    deltaY += yMult * velocities[Out] + (accelerations[Out] * (particle.lifetime / 1000f) * yMult);
                }
                if (velocities.ContainsKey(North))
                {
                    deltaY -= velocities[North] + (accelerations[North] * (particle.lifetime / 1000f));
                }
                if (velocities.ContainsKey(East))
                {
                    deltaX += velocities[East] + (accelerations[East] * (particle.lifetime / 1000f));
                }

                particle.position.X += deltaX;
                particle.position.Y += deltaY;
            }
        }

        public void draw(SpriteBatch b)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));
            foreach(Particle particle in particles)
            {
                particle.draw(b, particleSheet, particleRect, (tileLocation.Y * 64f) / 10000f, local);
            }
        }

        public virtual void spawnParticle(GameTime time)
        {
            particles.Add(new Particle(particleSpawnPoint(), scale, 0f, 0, longetivity / 2 + Game1.random.Next(longetivity)));
        }

        public virtual Vector2 particleSpawnPoint()
        {
            double distance = Game1.random.NextDouble() * range * 64 * 2f - range;
            double rotation = Game1.random.NextDouble() * Math.PI * 2;

            float x = (float)(distance * Math.Sin(rotation) * -1);
            float y = (float)(distance * Math.Cos(rotation));

            return new Vector2(x, y);
        }

    }
}
