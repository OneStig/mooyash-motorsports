using System;

namespace Mooyash.Modules
{
	public class ItemBox : GameObject // Equivalent of mario kart mystery box
	{
		public static string[] validItems = new string[] { "banana", "green_shell", "mushroom" };

		public float radius;

		public ItemBox(Vector2 position) : base()
		{
			this.position = position;
			texture = Engine.LoadTexture("mystery_box.png");
			size = new Vector2(500, 500);
			resolution = new Vector2(32, 32);

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

