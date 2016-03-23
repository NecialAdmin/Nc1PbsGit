using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NccpcDll;

namespace Nc1ExNaoStress1
{
	class Nc1ExNaoStress1_Program
	{
		public static NccpcSflMt1Simple1 gFlog = null; //new NccpcSflMt1Simple1("Nc1ExNaoStress1", 0xffff);
		public static void qv(string s1) { Console.WriteLine(s1); if (gFlog != null) { gFlog.write(s1); } }
		public static string
				gHost = Nc1ExNaoStress1.Properties.Settings.Default.Host,
				gServ = Nc1ExNaoStress1.Properties.Settings.Default.Serv;
		public static int gCtmax = Nc1ExNaoStress1.Properties.Settings.Default.CtMax;
		public static int gPkBfMax = Nc1ExNaoStress1.Properties.Settings.Default.PkBfMax;
		public static int gNwBfMax = gPkBfMax * 10;
		public static volatile bool gbWhile = true;
		public static Int64
			gHostCur = 1, gTdCnt = 0;

		public class TtSv : NccpcNw1Sv
		{
			enum Nit
			{
				None,
				Send,
			};

			public class To
			{
				public NiSend mO;
				public byte[] mB;

				public To(NiSend o) { mO = o; mB = new byte[0xfff]; mB[0] = 1; }
			}

			public class NiSend : NccpcNao1IptBase
			{
				public NccpcNw1Pk2 mPk;
				public int mCti;

				public NiSend(int cti, NccpcNw1Pk2 pk)
					: base()
				{
					mTypeid = (long)Nit.Send;
					mTypeobj = new To(this);

					mPk = pk;
					mCti = cti;
				}

				//~NiSend()
				//{
				//	mPk = null;
				//}
			}
			
			public class No : NccpcNao1ObjBase
			{
				public TtSv mNw;

				public No(TtSv nw, NccpcNao1MgrBase nm)
					: base(nm, true)
				{
					mNw = nw;
				}
				
				public override void nairIptbase(NccpcNao1IptBase iptb)
				{
					switch ((Nit)iptb.mTypeid)
					{
					case Nit.Send:
						{
							var ni = (NiSend)iptb;

							using (var pkw = ni.mPk)
							{
								//ni.mPk = null;
								mNw.send(ni.mCti, pkw);
							}
						}
						break;
					default:
						break;
					}
				}
			}

			public bool mbCreate = false;
			NccpcMemmgr2Mgr mMm;
			public long mLogTransSize = 0, mLogCtCnt = 0;
			public No mNo;

			public TtSv(NccpcMemmgr2Mgr mm, NccpcNao1MgrBase nm)
			//: base(0xfff, 0xfff)
			{
				mMm = mm;
				mNo = new No(this, nm);
			}

			public override void onNccpcNwLog(string s1) { Nc1ExNaoStress1_Program.qv(s1); }
			public void qv(string s1) { onNccpcNwLog(s1); }

			public bool create()
			{
				if (!mNo.create()) { return false; }
				{
					NccpcNw1Sv.CreateOptions co = new CreateOptions(mMm, gServ);
					co.mHost = gHost;
					//co.mCtmax = (uint)gCtmax + 1;
					co.mTransbuffersize = (uint)gNwBfMax;
					if (!base.create(co)) { return false; }
				}

				mbCreate = true;

				return true;
			}
			public new void release() { if (!mbCreate) { return; } mbCreate = false; base.release(); mNo.release(); }

			public new void framemove()
			{

				if (!mbCreate) { return; }

				var ts = System.Threading.Interlocked.Read(ref mLogTransSize);
				System.Threading.Interlocked.Exchange(ref mLogTransSize, 0);
				var cc = System.Threading.Interlocked.Read(ref mLogCtCnt);

				qv("Dbg SvTransSize:" + ts + " cc:" + cc);

				base.framemove();
			}

			public override void onNccpcNwEnter(int cti, string peer_clr)
			{
				System.Threading.Interlocked.Add(ref mLogCtCnt, +1);
				//qv("Dbg SvCtEt cc:" + System.Threading.Interlocked.Read(ref mLogCtCnt) + " ct:" + cti + " peer:" + peer_clr);
			}
			public override void onNccpcNwLeave(int cti)
			{
				System.Threading.Interlocked.Add(ref mLogCtCnt, -1);
				//qv("Dbg SvCtLv cc:" + System.Threading.Interlocked.Read(ref mLogCtCnt) + " ct:" + cti);
			}
			public override void onNccpcNwRecv(int cti, NccpcNw1Pk2 pkr)
			{
				var pklen = (long)pkr.getDataLen();
				System.Threading.Interlocked.Add(ref mLogTransSize, pklen);

				if (false)
				{
					using (var pkw = pkr.copyDeep())
					{
						send(cti, pkw);
					}
				}
				else
				{
					mNo.naiwIptbase(new NiSend(cti, pkr.copyDeep()));
				}
				
				//base.onNccpcNwRecv(cti, ncpk);
			}
		}

		public class TtCt : NccpcNw1Ct
		{
			enum Nit
			{
				None,
				Send,
			};

			public class NiSend : NccpcNao1IptBase
			{
				public NccpcNw1Pk2 mPk;
				public int mCti;

				public NiSend(int cti, NccpcNw1Pk2 pk)
					: base()
				{
					mTypeid = (long)Nit.Send;

					mPk = pk;
					mCti = cti;
				}
			}

			public class No : NccpcNao1ObjBase
			{
				public TtCt mNw;

				public No(TtCt nw, NccpcNao1MgrBase nm)
					: base(nm, true)
				{
					mNw = nw;
				}

				public override void nairIptbase(NccpcNao1IptBase iptb)
				{
					switch ((Nit)iptb.mTypeid)
					{
						case Nit.Send:
							{
								var ni = (NiSend)iptb;

								using (var pkw = ni.mPk)
								{
									ni.mPk = null;
									mNw.send(pkw);
								}
							}
							break;
						default:
							break;
					}
				}
			}

			public static long gCtcc = 0;
			bool mbCreate = false;
			public No mNo;

			public TtCt(NccpcNao1MgrBase nm)
				: base(gNwBfMax, gNwBfMax)
			{
				mNo = new No(this, nm);
			}

			public override void onNccpcNwLog(string s1) { Nc1ExNaoStress1_Program.qv(s1); }
			public void qv(string s1) { onNccpcNwLog(s1); }

			public bool create()
			{
				if (!mNo.create()) { return false; }
				if (!base.create(gHost, gServ, false)) { return false; }
				System.Threading.Interlocked.Add(ref gCtcc, 1);
				mbCreate = true;
				return true;
			}
			public new void release()
			{
				if (!mbCreate) { return; }
				System.Threading.Interlocked.Add(ref gCtcc, -1);
				mbCreate = false;
				base.release();
				mNo.release();
			}

			//public new void framemove() { if (!mbCreate) { return; } base.framemove();  }
			//public override void onNccpcNwLeave(...)
			public override void onNccpcNwRecv(NccpcNw1Pk2 pkr)
			{
				//qv("Dbg CtRv l:" + pkr.getDataLen());

				if (false)
				{
					using (var pkw = pkr.copyDeep())
					{
						send(pkw);
					}
				}
				else
				{
					mNo.naiwIptbase(new NiSend(0, pkr.copyDeep()));
				}
				
				//base.onNccpcNwRecv(cti, ncpk);
			}
		}

		static void Main(string[] args)
		{
			qv("Dbg " + (new System.Diagnostics.StackFrame(0, true)).GetFileName());
			NccpcNw1Cmn.stWsaStartup();

			var mm = new NccpcMemmgr2Mgr();
			//var tm = new NccpcTdMgr2();
			var nm = new NccpcNao1MgrBase();
			
			//long ctcc = 0; //conn cnt;
						
			if (!mm.create()) { qv("Err mm create fail"); return; }
			if (!nm.create()) { qv("Err nm create fail"); return; }
			//if (!tm.create()) { qv("Err tm create fail"); return; }
			//if (!sv.create()) { qv("Err sv create fail"); return; }

			var cts = new List<TtCt>();
			var sv = new TtSv(mm, nm);


			qv("Dbg Ctm1 startup h:" + gHost + " s:" + gServ);
			qv("Dbg key: Q = Quit, ");


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
						case ConsoleKey.S:
							{
								if (!sv.create()) { qv("Err sv.create fail"); break; }
							}
							break;
						case ConsoleKey.C:
							{
								for (int tc = 0; tc < gCtmax; tc++)
								{
									var ct = new TtCt(nm);
									if (!ct.create()) { qv("Err Ct.create() fail"); return; }

									using (var pkw = NccpcNw1Pk2.stAlloc(gPkBfMax))
									{
										var pkwlen = pkw.getWritableLen();
										for (int i = 0; i < pkwlen; i++)
										{ pkw.wInt8s((sbyte)i); }

										ct.send(pkw);
									}

									cts.Add(ct);

									//lock (gLockobj) { gTdCnt++; }

									//Jc1.Tdut.exec(ct.exec);
								}
							}
							break;
						case ConsoleKey.L:
							{
								if (gFlog != null) { qv("Dbg Log off"); gFlog = null; }
								else { gFlog = new NccpcSflMt1Simple1("Nc1ExNaoStress1", 0xffff); qv("Dbg Log on"); }
							}
							break;
					}
				}
				else
				{
					sv.framemove();
					//if (cts.Count != 0) { qv("Dbg Cts:" + cts.Count + " Ctcc:" + System.Threading.Interlocked.Read(ref TtCt.gCtcc)); }
					System.Threading.Thread.Sleep(1000);
				}

			}

			foreach (var ct in cts)
			{
				ct.release();
			}

			sv.release();

			nm.release();
			mm.release();

			NccpcNw1Cmn.stWsaCleanup();
		}
	}
}
