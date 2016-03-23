using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nc1Ex1Server
{
	class Nc1Ex1ServerMainAm2
	{
		//dll import: NclibCpc4Dll.dll, Jc1Cs.dll, Jc1Dn2_0.dll, Jg1Cs.dll, Jg1CsDn2_0.dll

		public class Sv : NccpcDll.NccpcNw1Sv
		{
			public NccpcDll.NccpcMemmgr2Mgr mMm;

			public Sv()
				: base()
			{
				mMm = new NccpcDll.NccpcMemmgr2Mgr();
			}

			public bool create()
			{
				if (!mMm.create()) { return false; }

				var co = new NccpcDll.NccpcNw1Sv.CreateOptions(mMm, "7777");

				if (!base.create(co)) { return false; }

				return true;
			}

			new public void release()
			{
				base.release();

				mMm.release();
			}

			public void qv(string s1) { System.Console.WriteLine(s1); }
			public override void onNccpcNwLog(string s1) { qv(s1); }
			//public override void onNccpcNwErr(string s1) { qv("Err " + s1); }
			public override void onNccpcNwEnter(int cti, string peer) { qv("Dbg NwEnter ct:" + cti + " Peer:" + peer); }
			//public override NccpcMemmgr2Obj1 onNccpcNwEncode(int cti, int out desclen, NccpcMemmgr2Obj1 srcobj, int srclen, unsigned char cft) { return null; }
			//public override NccpcMemmgr2Obj1 onNccpcNwDecode(int cti, int out desclen, NccpcNw1StreamWar1 srcsw, unsigned char cft) { return null; }
			public override void onNccpcNwRecv(int cti, NccpcDll.NccpcNw1Pk2 ncpk) { qv("Dbg NwRecv Type:" + ncpk.getType() + " Len:" + ncpk.getDataLen()); }
			public override void onNccpcNwLeave(int cti) { qv("Dbg NwLeave ct:" + cti); }

		}


		static void Main(string[] args)
		{
			NccpcDll.NccpcNw1Cmn.stWsaStartup();

			var sv = new Sv();

			Console.WriteLine("server starting");

			if (!sv.create()) { return; }

			Console.WriteLine("server started");

			bool bWhile = true;
			while (bWhile)
			{
				sv.framemove();

				System.Threading.Thread.Sleep(1000);
			}

			sv.release();

			NccpcDll.NccpcNw1Cmn.stWsaCleanup();
		}
	}
}
