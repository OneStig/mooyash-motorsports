using System;

namespace Mooyash.Modules
{
	public class ItemBox : GameObject // Equivalent of mario kart mystery box
	{
		public static string[] validItems = new string[] { "banana", "projectile", "speed" };

		public float radius;

		public ItemBox(Vector2 position) : base()
		{
			this.position = position;
			textures = new Texture[1];
			textures[0] = Engine.LoadTexture("mystery_box.png");

			sizes = new Vector2[1] { new Vector2(500, 500) };

			radius = 50f;
		}

		public override void collide(Kart k)
		{
			if (exists && k.itemHeld == 0)
			{
                exists = false;
				k.rollItemTime = 0;
				k.itemHeld = -1;
            }
		}
	}
}

