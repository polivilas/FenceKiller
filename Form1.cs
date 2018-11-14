using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using zlib;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using System.Management;


namespace FenceKiller
{
	public partial class Form1 : Form
	{
		private static readonly HttpClient client = new HttpClient();
		private string DeviceID = "INVALID"; // Requires your HWID
		private string HardwareCode = "1337";
		private string MajorV = "0.9.0.1582"; // FIXME update for current v
		private string MinorV = "bgkidft87ddd"; // FIXME update for current v
		private string MoneyClass = "5449016a4bdc2d6f028b456f";
		//RUBLES - 5449016a4bdc2d6f028b456f
		//DOLLAR - 5696686a4bdc2da3298b456a
		private string Trader = "579dc571d53a0658a154fbec";
		//FENCE  - 579dc571d53a0658a154fbec
		//PCEKPR - 5935c25fb3acc3127c3d8cd9
		Dictionary<string, int> MoneyKeeper = new Dictionary<string,int>();
		List<string> WantedItems = new List<string>();
		List<string> InstaSell = new List<string>();
		List<string> SingleBuy = new List<string>();
		List<string> TraderIDs = new List<string>();
		List<string> TraderNames = new List<string>();
		BackgroundWorker bw = new BackgroundWorker();

		public Form1()
		{
			CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();
			client.BaseAddress = new Uri("http://prod.escapefromtarkov.com");
			TraderIDs.Add("54cb50c76803fa8b248b4571");
			TraderNames.Add("Prapor");
			TraderIDs.Add("54cb57776803fa99248b456e");
			TraderNames.Add("Therapist");
			TraderIDs.Add("579dc571d53a0658a154fbec");
			TraderNames.Add("Fence");
			TraderIDs.Add("58330581ace78e27b8b10cee");
			TraderNames.Add("Skier");
			TraderIDs.Add("5935c25fb3acc3127c3d8cd9");
			TraderNames.Add("Peacekeeper");
			TraderIDs.Add("5a7c2eca46aef81a7ca2145d");
			TraderNames.Add("Mechanic");
			TraderIDs.Add("5ac3b934156ae10c4430e83c");
			TraderNames.Add("Ragman");
			//WantedItems.Add("5ad5d49886f77455f9731921"); - Power utility key
			/*GOOD ITEMS - 
				59faff1d86f7746c51718c9c - BTC
				59faf7ca86f7740dbe19f6c2 - Rolex
				59f9ddae86f77407ab46e047 - Flash drive
				5734758f24597738025ee253 - GChain

				5857a8b324597729ab0a0e7d - Beta container
				5857a8bc2459772bad15db29 - Gamma container
				59db794186f77448bc595262 - Epsilon container
				567143bf4bdc2d1a0f8b4567 - Pistol case

				57347d7224597744596b4e72 - Tushonka
				5ab8f20c86f7745cdb629fb2 - Shmaska
				5aa2b9ede5b5b000137b758b - Cowboy hat

				5448ba0b4bdc2d02308b456c - Factory key
				5780cf7f2459777de4559322 - Marked key
				5a0eec9686f77402ac5c39f2 - 310 key
				5ad5d7d286f77450166e0a89 - KIBA key
				5ad7217186f7746744498875 - OliCashReg
				5ad7242b86f7740a6a3abd43 - IdeaCashReg
				5ad7247386f7747487619dc3 - GoshanCashReg

				573476d324597737da2adc13 - Malboro cigs
				573476f124597737e04bf328 - Wilston cigs
				5734770f24597738025ee254 - Strike cigs
				57347ca924597744596b4e71 - GPU

			*/
			int counter = 0;
			string line = null;
			System.IO.StreamReader file = new System.IO.StreamReader("singlebuy");
			while ((line = file.ReadLine()) != null)
			{
				SingleBuy.Add(line);
				richTextBox1.AppendText(line + " - Added as singlebuy\n");
				counter++;
			}
			file.Close();
			richTextBox1.AppendText("Total singlebuys:" + counter + "\n");
			LoginJSON LoginTXT = new LoginJSON();
			LoginTXT.email = " login NAME (not password/email) "; // FIXME: YOUR LOGIN NAME
			LoginTXT.pass = " md5 hash of your password"; // FIXME: YOUR MD5(PASSWORD)
			//http://www.md5.cz/
			LoginTXT.version.major = MajorV;
			LoginTXT.version.minor = MinorV;
			LoginTXT.version.game = MinorV;
			LoginTXT.version.backend = 6; // make sure its not outdated...
			LoginTXT.version.taxonomy = 266; // make sure its not outdated...
			LoginTXT.device_id = DeviceID;
			LoginTXT.develop = true;
			// ***WARNING*** BAD - Need to login over HTTPS!!!
			POSTZlib("/client/game/login", RemoveWhitespace(JsonConvert.SerializeObject(LoginTXT)), false, out dynamic test);

			if (test.err == 209) // "GameClient::login - Received new hardware code. Need confirm"
			{
				POSTZlib("/client/game/hardwareCode/activate", RemoveWhitespace("{\"email\":\""+LoginTXT.email+ "\", \"device_id\":\""+ DeviceID + "\", \"activateCode\":\""+HardwareCode+ "\"}"), false, out dynamic devnull);
				richTextBox1.AppendText("Check your email for hardware code then update this.\n");
				return;
			}
			POSTZlib("/client/game/keepalive", "{}", false, out dynamic useless);
			dynamic GetPMC = JsonConvert.DeserializeObject("{\"_id\":\"" + 0 + "\"}");
			POSTZlib("/client/game/profile/list", "{}", false, out useless);
			foreach (var item in useless.data)
			{
				string id = item._id;
				if (item.Info.LastTimePlayedAsSavage == 0)
				{
					GetPMC = item;
					Debug.Print("PMC! {0}\n", id);
				}
				else
				{
					Debug.Print("Scav! {0}\n", id);
				}
			}
			foreach (var item in GetPMC.Inventory.items)
			{
				if (item._tpl == MoneyClass)
				{
					string itemID = item._id;
					int Count = item.upd.StackObjectsCount;
					MoneyKeeper.Add(itemID, Count);
				}
			}
			POSTZlib("/client/game/profile/select", RemoveWhitespace(JsonConvert.SerializeObject(JsonConvert.DeserializeObject("{\"uid\":\"" + GetPMC._id + "\"}"))), false, out useless);
			
			foreach (var item in GetPMC.Inventory.items)
			{
				if (item._tpl == MoneyClass)
				{
					string itemID = item._id;
					int Count = item.upd.StackObjectsCount;
					if (!MoneyKeeper.ContainsKey(itemID))
						MoneyKeeper.Add(itemID, Count);
				}
			}


			Task.Run(() =>
			{
				while (true)
				{
					try
					{
						POSTZlib("/client/game/keepalive", "{}", false, out dynamic ravioli);
					}
					catch { }
					Thread.Sleep(30000);
				}
			});
			Task.Run(() =>
			{
				dynamic Input2;
				while (true)
				{
					try
					{
						if (!POSTZlib("/client/trading/api/getTraderAssort/" + Trader, "{}", false, out dynamic Input1))
						{
							Thread.Sleep(1000);
							continue;
						}
						foreach (var item in Input1.data.items)
						{
							string template = item._tpl;
							if (WantedItems.Contains(template))
							{
								int price = 1;
								string id = item._id;
								string InitialPurchase = "{\"data\": [{\"Action\": \"TradingConfirm\",\"type\": \"buy_from_trader\",\"tid\": \"" + Trader + "\",\"item_id\": \"" + id + "\",\"count\": 1,\"scheme_id\": 0,\"scheme_items\": [";
								foreach (var barter in Input1.data.barter_scheme)
								{
									if (barter.Name == id)
									{
										price = barter.First.First.First.count;
										break;
									}
								}
								bool kablelis = false;
								foreach (var Moneys in MoneyKeeper)
								{
									if (price <= 0)
										break;
									if (kablelis)
										InitialPurchase = InitialPurchase + ",";
									else
										kablelis = true;
									int MnyVal = Moneys.Value;
									if (price <= Moneys.Value)
									{
										InitialPurchase = InitialPurchase + "{\"id\": \"" + Moneys.Key + "\",\"count\": " + price + "}";
										price -= price;
									}
									else
									{
										InitialPurchase = InitialPurchase + "{\"id\": \"" + Moneys.Key + "\",\"count\": " + Moneys.Value + "}";
										price -= Moneys.Value;
									}
								}
								InitialPurchase = InitialPurchase + "]}]}";
								POSTZlib("/client/game/profile/items/moving", RemoveWhitespace(InitialPurchase), false, out Input2);
								foreach (var thing in Input2.data.items)
								{
									if (thing.Name == "new")
									{
										string TmpTmpl = thing.First.First._tpl;
										string TmpID = thing.First.First._id;
										//response on item bought? currently its silent
									}

									if (thing.Name == "del")
									{
										string tmpID = thing.First.First._id;
										if (MoneyKeeper.ContainsKey(tmpID))
											MoneyKeeper.Remove(tmpID);
										Debug.Print("Deleting money.");
									}
									if (thing.Name == "change")
									{
										string tmpID = thing.First.First._id;
										if (MoneyKeeper.ContainsKey(tmpID))
										{
											int tmpCnt = thing.First.First.upd.StackObjectsCount;
											MoneyKeeper[tmpID] = tmpCnt;
										}
										Debug.Print("Changing.");
									}
								}
							}
							if (SingleBuy.Contains(template))
							{
								int price = 1;
								string id = item._id;
								string InitialPurchase = "{\"data\": [{\"Action\": \"TradingConfirm\",\"type\": \"buy_from_trader\",\"tid\": \"" + Trader + "\",\"item_id\": \"" + id + "\",\"count\": 1,\"scheme_id\": 0,\"scheme_items\": [";
								foreach (var barter in Input1.data.barter_scheme)
								{
									if (barter.Name == id)
									{
										price = barter.First.count;
										break;
									}
								}
								bool kablelis = false;
								foreach (var Moneys in MoneyKeeper)
								{
									if (price <= 0)
										break;
									if (kablelis)
										InitialPurchase = InitialPurchase + ",";
									else
										kablelis = true;
									int MnyVal = Moneys.Value;
									if (price <= Moneys.Value)
									{
										InitialPurchase = InitialPurchase + "{\"id\": \"" + Moneys.Key + "\",\"count\": " + price + "}";
										price -= price;
									}
									else
									{
										InitialPurchase = InitialPurchase + "{\"id\": \"" + Moneys.Key + "\",\"count\": " + Moneys.Value + "}";
										price -= Moneys.Value;
									}
								}
								InitialPurchase = InitialPurchase + "]}]}";
								POSTZlib("/client/game/profile/items/moving", RemoveWhitespace(InitialPurchase), false, out Input2);
								foreach (var thing in Input2.data.items)
								{
									if (thing.Name == "new")
									{
										string TmpTmpl = thing.First.First._tpl;
										string TmpID = thing.First.First._id;
										if (SingleBuy.Contains(TmpTmpl))
										{
											richTextBox1.AppendText("!!SINGLE BUY SNIPED!! - " + TmpTmpl + "\n");

											SingleBuy.Remove(TmpTmpl);
											File.WriteAllLines("singlebuy", SingleBuy);
										}
									}

									if (thing.Name == "del")
									{
										string tmpID = thing.First.First._id;
										if (MoneyKeeper.ContainsKey(tmpID))
											MoneyKeeper.Remove(tmpID);
										Debug.Print("Deleting money.");
									}
									if (thing.Name == "change")
									{
										string tmpID = thing.First.First._id;
										if (MoneyKeeper.ContainsKey(tmpID))
										{
											int tmpCnt = thing.First.First.upd.StackObjectsCount;
											MoneyKeeper[tmpID] = tmpCnt;
										}
										Debug.Print("Changing.");
									}
								}
							}
						}
						Thread.Sleep(100);
					}
					catch { }
				}

			});
		}
		public static void CompressData(byte[] inData, out byte[] outData)
		{
			using (MemoryStream outMemoryStream = new MemoryStream())
			using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
			using (Stream inMemoryStream = new MemoryStream(inData))
			{
				CopyStream(inMemoryStream, outZStream);
				outZStream.finish();
				outData = outMemoryStream.ToArray();
			}
		}

		public static void DecompressData(byte[] inData, out byte[] outData)
		{
			using (MemoryStream outMemoryStream = new MemoryStream())
			using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
			using (Stream inMemoryStream = new MemoryStream(inData))
			{
				CopyStream(inMemoryStream, outZStream);
				outZStream.finish();
				outData = outMemoryStream.ToArray();
			}
		}
		public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
		{
			byte[] buffer = new byte[2000];
			int len;
			while ((len = input.Read(buffer, 0, 2000)) > 0)
			{
				output.Write(buffer, 0, len);
			}
			output.Flush();
		}
		public static string RemoveWhitespace(string input)
		{
			return new string(input.ToCharArray()
				.Where(c => !Char.IsWhiteSpace(c))
				.ToArray());
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}
		
		public bool POSTZlib(string page, string args, bool loud, out dynamic jsonOut)
		{
			if (loud)
				richTextBox1.AppendText(args + "\n");
			CompressData(System.Text.Encoding.UTF8.GetBytes(args), out byte[] bytes);

			HttpContent bytesContent = new ByteArrayContent(bytes);
			var result = client.PostAsync(page, bytesContent).Result;
			jsonOut = 1;
			if (result.IsSuccessStatusCode)
			{
				var responsetext = result.Content;
				byte[] enc = responsetext.ReadAsByteArrayAsync().Result;
				byte[] dec;
				try
				{
					DecompressData(enc, out dec);
				}
				catch {
					return false;
				}
				jsonOut = JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(dec));
				if (loud)
					richTextBox1.AppendText(System.Text.Encoding.UTF8.GetString(dec) + "\n");
			}
			return true;
		}

		public struct LoginInner
		{
			public string major;
			public string minor;
			public string game;
			public int backend;
			public int taxonomy;
		}
		public struct LoginJSON
		{
			public string email;
			public string pass;
			public LoginInner version;
			public string device_id;
			public bool develop;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			if (ItemID.Text == "")
				return;
			int i = 0;
			bool NoDice = true;
			foreach (string trader in TraderIDs) {
				if (POSTZlib("/client/trading/api/getTraderAssort/" + trader, "{}", false, out dynamic TraderOutput))
				{
					foreach (var item in TraderOutput.data.items)
					{
						string template = item._tpl;
						if (ItemID.Text == template)
						{
							NoDice = false;
							int price = 1;
							int priceActual = 1;
							string id = item._id;
							string InitialPurchase = "{\"data\": [{\"Action\": \"TradingConfirm\",\"type\": \"buy_from_trader\",\"tid\": \"" + trader + "\",\"item_id\": \"" + id + "\",\"count\": 1,\"scheme_id\": 0,\"scheme_items\": [";
							foreach (var barter in TraderOutput.data.barter_scheme)
							{
								if (barter.Name == id)
								{
									price = barter.First.First.First.count;
									priceActual = barter.First.First.First.count;
									break;
								}
							}
							bool kablelis = false;
							foreach (var Moneys in MoneyKeeper)
							{
								if (price <= 0)
									break;
								if (kablelis)
									InitialPurchase = InitialPurchase + ",";
								else
									kablelis = true;
								int MnyVal = Moneys.Value;
								if (price <= Moneys.Value)
								{
									InitialPurchase = InitialPurchase + "{\"id\": \"" + Moneys.Key + "\",\"count\": " + price + "}";
									price -= price;
								}
								else
								{
									InitialPurchase = InitialPurchase + "{\"id\": \"" + Moneys.Key + "\",\"count\": " + Moneys.Value + "}";
									price -= Moneys.Value;
								}
							}
							InitialPurchase = InitialPurchase + "]}]}";
							richTextBox1.AppendText("Item found at " + TraderNames[i] + " with a cost of " + priceActual + "\n");
							if (TraderYesNo.Text == "y")
							{
								POSTZlib("/client/game/profile/items/moving", RemoveWhitespace(InitialPurchase), false, out dynamic Output2);
								foreach (var thing in Output2.data.items)
								{
									if (thing.Name == "new")
									{
										string TmpTmpl = thing.First.First._tpl;
										string TmpID = thing.First.First._id;
										richTextBox1.AppendText("Purchased item from " + TraderNames[i] + "\n");
									}

									if (thing.Name == "del")
									{
										string tmpID = thing.First.First._id;
										if (MoneyKeeper.ContainsKey(tmpID))
											MoneyKeeper.Remove(tmpID);
										Debug.Print("Deleting money.");
									}
									if (thing.Name == "change")
									{
										string tmpID = thing.First.First._id;
										if (MoneyKeeper.ContainsKey(tmpID))
										{
											int tmpCnt = thing.First.First.upd.StackObjectsCount;
											MoneyKeeper[tmpID] = tmpCnt;
										}
										Debug.Print("Changing.");
									}
								}
							}
						}
					}
				}
				i++;
			};
			if (NoDice)
				richTextBox1.AppendText("No traders sell it.\n");
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			
		}
	}
}
