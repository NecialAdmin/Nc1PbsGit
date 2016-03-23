using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nc1Ex1Server
{
	class Nc1Ex1ServerMainAm1
	{
		class Sct : Jc1.NwSct
		{
			protected override void onConnect(Jc1.NwRst1 rst)
			{ Console.WriteLine("on connect: " + rst); }
			protected override void onDisconnect()
			{ Console.WriteLine("on disconnect"); }
			protected override bool onRecvTake(Jc1Dn2_0.PkReader1 pkrd)
			{ Console.WriteLine("on ct: " + this.getCti() + ", recv: " + pkrd.getPkt()); return true; }
		}

		static void Main_1(string[] args)
		{
			Jc1.NwSv sv = new Jc1.NwSv();

			for (int i = 0; i < 100; i++)
			{ sv.ctpooladd(new Sct()); }

			Console.WriteLine("server start");

			sv.create(5557);


			bool bWhile = true;
			while (bWhile)
			{
				sv.framemove();

				System.Threading.Thread.Sleep(1000);
				Console.WriteLine("cnt:" + sv.scConnected().Count);
			}

			sv.release();
		}
	}
}
