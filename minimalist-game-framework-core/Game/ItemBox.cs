using System;

namespace Mooyash.Modules
{
	public class ItemBox : GameObject // Equivalent of mario kart mystery box
	{
		public static string[] validItems = new string[] { "banana", "projectile" };

		bool exists;
		float radius;

		public ItemBox(Vector2 position) : base()
		{
			this.position = position;

			exists = true;
			radius = 0.5f;
		}

		public void collide(Kart k)
		{
			Random r = new Random();
			
			exists = false;
			string collect = validItems[r.Next(0, validItems.Length - 1)];
			k.itemHeld = collect;
		}
	}
}

