﻿using System;
using System.IO;
using System.Collections.Generic;

using SQLite;

using Android.Content;
using System.Collections;
using System.Linq;

namespace Totem
{
	public class Database
	{
		//DB params
		static string dbName = "totems.sqlite";
		string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);

		Context context;
		List<Eigenschap> eigenschappen;
		List<Totem> totems;

		public Database (Context context)
		{
			this.context = context;
			ExtractDB ();
			SetEigenschappen ();
			SetTotems ();
		}

		//load DB
		public void ExtractDB() {
			if (!File.Exists(dbPath))
			{
				using (BinaryReader br = new BinaryReader(context.Assets.Open(dbName)))
				{
					using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
					{
						byte[] buffer = new byte[2048];
						int len = 0;
						while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
						{
							bw.Write (buffer, 0, len);
						}
					}
				}
			}
		}
			
		//extract eigenschappen from DB and put them in a list
		private void SetEigenschappen() {
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "select * from eigenschap";
				eigenschappen = cmd.ExecuteQuery<Eigenschap> ();
			}
		}

		//get list of profile-objects
		public List<Profiel> GetProfielen() {
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "select distinct name from profiel";
				return cmd.ExecuteQuery<Profiel> ();
			}
		}

		//get list of profile names
		public List<string> GetProfielNamen() {
			List<string> namen = new List<string> ();
			foreach (Profiel p in this.GetProfielen()) {
				namen.Add (p.name);
			}
			return namen;
		}

		//add totem to profile in db
		public void AddTotemToProfiel(string totemID, string profielName) {
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "insert into profiel (name, nid) select '" + profielName + "'," + totemID + " WHERE NOT EXISTS ( SELECT * FROM profiel WHERE name='"+ profielName +"' AND nid=" + totemID + ");";
				cmd.ExecuteQuery<Profiel> ();
			}
		}

		//remove all profiles
		public void ClearProfiles() {
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "DELETE FROM profiel";
				cmd.ExecuteQuery<Profiel> ();
			}
		}

		//add a profile
		public void AddProfile(string name) {
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "insert into profiel (name) values ('" + name + "')";
				cmd.ExecuteQuery<Profiel> ();
			}
		}

		//retruns an array of ints with all totemIDs related to a profile
		public int [] GetTotemIDsFromProfiel(string name) {
			List<Profiel> list = new List<Profiel>();
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "select nid from profiel where name='" + name + "'";
				list = cmd.ExecuteQuery<Profiel> ();
			}
			list.Reverse ();
			int[] result = new int[395];
			int index = 0;
			foreach (Profiel p in list) {
				if(p.nid != null) 
					result.SetValue (Int32.Parse (p.nid), index);
				index++;
			}
			result.Where(x => x != 0).ToArray();

			return result;
		}

		public List<Eigenschap> GetEigenschappen() {
			return eigenschappen;
		}

		//returns List of Totem_eigenschapp related to eigenschap id
		public List<Totem_eigenschap> GetTotemsVanEigenschapsID(string id) {
			List<Totem_eigenschap> totemsVanEigenschap;
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "select nid from totem_eigenschap where tid = " + id;
				totemsVanEigenschap = cmd.ExecuteQuery<Totem_eigenschap> ();
			}
			return totemsVanEigenschap;
		}

		//extract totems from DB and put them in a list
		private void SetTotems() {
			using (var conn = new SQLite.SQLiteConnection (dbPath)) {
				var cmd = new SQLite.SQLiteCommand (conn);
				cmd.CommandText = "select * from totem";
				totems = cmd.ExecuteQuery<Totem> ();
			}
		}

		//returns array of all totem IDs
		public int[] AllTotemIDs() {
			List<Totem> list = totems;
			list.Reverse ();
			int[] result = new int[395];
			int index = 0;
			foreach(Totem t in list) {
				result.SetValue(Int32.Parse(t.nid), index);
				index++;
			}
			return result;
		}

		//returns totem-object with given id
		public Totem GetTotemOnID(int idx) {
			foreach(Totem t in totems) {
				if(t.nid.Equals(idx.ToString())) {
					return t;
				} 
			}
			return null;
		}

		//returns totem-object with given id
		public Totem GetTotemOnID(string idx) {
			return GetTotemOnID (Int32.Parse (idx));
		}

		//returns totem-object with given name
		public List<Totem> FindTotemOpNaam(string name) {
			List<Totem> result = new List<Totem> ();
			foreach(Totem t in totems) {
				if(t.title.ToLower().Contains(name.ToLower())) {
					result.Add (t);
				}
			}
			return result;
		}
	}
}

