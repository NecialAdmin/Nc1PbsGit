using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nc1Ex1Client
{	
	class Nc1Ex1ClientMain
	{
		//dll import: Jc1Dn2_0.dll, JcCtUnity1Dll.dll

		class Client : JcCtUnity1.JcCtUnity1
		{
			public Client() : base(Encoding.Default) { }
			public void qv(string s1) { innLogOutput(s1); }
			
			//JcCtUnity1.JcCtUnity1
			protected override void innLogOutput(string s1) { Console.WriteLine(s1); }
			protected override void onConnect(JcCtUnity1.NwRst1 rst1, Exception ex = null)
			{
				qv("Dbg on connect: " + rst1);
				int pkt = 1111;
				using (JcCtUnity1.PkWriter1Nm pkw = new JcCtUnity1.PkWriter1Nm(pkt))
				{
					pkw.wInt32u((UInt32)2222);
					this.send(pkw);
				}
				qv("Dbg send packet Type:" + pkt);
			}
			protected override void onDisconnect()
			{ qv("Dbg on disconnect"); }
			protected override bool onRecvTake(Jc1Dn2_0.PkReader1 pkrd)
			{ qv("Dbg on recv: " + pkrd.getPkt()/* + pkrd.ReadString()*/ ); return true; }
		}

		static void Main(string[] args)
		{
			Client ct = new Client();

			Console.WriteLine("Dbg client start");

			if (!ct.connect("127.0.0.1", 7777)) { Console.WriteLine("Dbg connect fail"); return; }
			
			bool bWhile = true;
			while (bWhile)
			{
				//if(ct.isConnected()) { string s1 = "send"+System.DateTime.Now.Ticks; JcCtUnity1.PkWriter1Nm pkw = new JcCtUnity1.PkWriter1Nm(11); pkw.wStr1(s1); ct.send(pkw); Console.WriteLine(s1); }

				ct.framemove();
				System.Threading.Thread.Sleep(1000);
			}

			ct.disconnect();
		}
	}
}
