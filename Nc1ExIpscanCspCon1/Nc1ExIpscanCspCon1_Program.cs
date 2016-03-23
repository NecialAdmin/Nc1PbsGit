using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NccpcDll;

namespace Nc1ExIpscanCspCon1
{
	class Nc1ExIpscanCspCon1_Program
	{
		static NccpcSflMt1Simple1 gFlog = new NccpcSflMt1Simple1("Nc1ExIpscanCspCon1", 0xffff);

		static public void qv(string s1) { Console.WriteLine(s1); } //gFlog.write(s1); }

		static object gLockobj = new object();
		static Int64 gHostBeginInt, gHostEndInt,
			gHostCur = 1, gTdCnt = 0;
		static string gHostBeginStr, gHostEndStr,
			gHostRst = null, gServRst = null;

		public class TtCt : NccpcDll.NccpcNw1Ct
		{
			public TtCt()
				: base(0xfff, 0xfff)
			{

			}

			public override void onNccpcNwLog(string s1) { Nc1ExIpscanCspCon1_Program.qv(s1); }
			public void qv(string s1) { onNccpcNwLog(s1); }


			public void find1(object obj1)
			{
				string host, serv = gServRst;

				while (true)
				{
					lock (gLockobj)
					{
						if (gHostRst != null || gHostCur > gHostEndInt)
						{ goto Goto_Find1_End; }

						host = CnvInt64ToIpv4(gHostCur);
						gHostCur++;
					}

					if (!this.create(host, serv, false))
					{ qv("Dbg ct create fail host:" + host + " serv:" + serv); }
					else
					{
						qv("Dbg ct create success host:" + host + " serv:" + serv);
						gHostRst = host;
						goto Goto_Find1_End;
					}
				}

			Goto_Find1_End:
				lock (gLockobj)
				{
					gTdCnt--;
				}
				return;
			}
		}

		static Int64 CnvIpv4ToInt64(string ipv4s)
		{
			Int64 rv = 0;
			var ss = ipv4s.Split('.');
			for (int i = 0; i < ss.Length; i++)
			{
				rv += Int64.Parse(ss[i]) << ((ss.Length - i - 1) * 8);
			}
			return rv;
		}
		static string CnvInt64ToIpv4(Int64 ipv4i)
		{
			string rs = "";
			while (true)
			{
				if (ipv4i <= 0) { break; }
				if (rs.Length > 0) { rs = "." + rs; }
				rs = (ipv4i & 0xff).ToString() + rs;
				ipv4i >>= 8;
			}
			return rs;
		}

		static void Main(string[] args)
		{
			qv("Dbg " + (new System.Diagnostics.StackFrame(0, true)).GetFileName());
			NccpcNw1Cmn.stWsaStartup();

			string
				//hostbegin = Nc1ExPortscanCspCon1.Properties.Settings.Default.HostBegin,
				//hostend = Nc1ExPortscanCspCon1.Properties.Settings.Default.HostEnd,
				serv = Nc1ExIpscanCspCon1.Properties.Settings.Default.Serv;
			int findtdc = Nc1ExIpscanCspCon1.Properties.Settings.Default.FindThread;

			gHostBeginStr = Nc1ExIpscanCspCon1.Properties.Settings.Default.HostBegin;
			gHostEndStr = Nc1ExIpscanCspCon1.Properties.Settings.Default.HostEnd;
			gServRst = serv;

			gHostBeginInt = CnvIpv4ToInt64(gHostBeginStr);
			var t1 = CnvInt64ToIpv4(gHostBeginInt);
			gHostEndInt = CnvIpv4ToInt64(gHostEndStr);

			gHostCur = gHostBeginInt;

			//var mm = new NccpcMemmgr2Mgr();
			//var tm = new NccpcTdMgr2();
			//var ct1i = DateTime.Now.Ticks % uromax;
			var cts = new List<TtCt>();

			//if (!mm.create()) { qv("mm create fail"); return; }
			//if (!tm.create()) { qv("tm create fail"); return; }

			qv("Dbg Ctm1 startup h:" + gHostBeginStr + " s:" + serv);
			qv("Dbg key: Q = Quit, ");

			
			for(int tc=0; tc<findtdc; tc++)
			{
				var ct = new TtCt();
				cts.Add(ct);

				lock (gLockobj) { gTdCnt++; }

				Jc1.Tdut.exec(ct.find1);
			}


			qv("Dbg ct created");


			bool bWhile = true;
			while (bWhile)
			{
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo k = Console.ReadKey(false);

					switch (k.Key)
					{
						case ConsoleKey.Q:
							bWhile = false;
							qv("Dbg quit");
							break;
						case ConsoleKey.M:
							{
							}
							break;
					}
				}
				else
				{
					System.Threading.Thread.Sleep(100);
				}

				//ct1.framemove(100);

				lock (gLockobj)
				{
					if (gTdCnt == 0)
					{
						qv("");
						qv("***** WorkEnd ***** ");
						if (gHostRst != null && gServRst != null)
						{ qv("Dbg host:" + gHostRst + " serv:" + gServRst); }
						else { qv("Dbg fail"); }
						qv("");

						bWhile = false;
					}
				}
			}

			foreach (var ct in cts)
			{
				ct.release();
			}

			NccpcNw1Cmn.stWsaCleanup();
		}
	}
}
