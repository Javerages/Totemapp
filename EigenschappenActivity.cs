﻿using System;
using System.IO;
using System.Collections.Generic;

using SQLite;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content.PM;

namespace Totem
{
	[Activity (Label = "Totem bepalen", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EigenschappenActivity : Activity
	{
		TextView adjectief;
		List<Eigenschap> eigenschappen;
		int eigenschapCount;

		//dictionary with totem IDs as keys and frequencies as values
		Dictionary<int, int> freqs;

		Database db;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Eigenschappen);

			//setup
			db = new Database(this);
			eigenschappen = db.GetEigenschappen ();

			eigenschapCount = 1;
			freqs = new Dictionary<int, int> ();

			Button jaKnop = FindViewById<Button> (Resource.Id.jaKnop);
			Button neeKnop = FindViewById<Button> (Resource.Id.neeKnop);

			adjectief = FindViewById<TextView> (Resource.Id.eigenschap);
			Typeface face = Typeface.CreateFromAsset(Assets,"fonts/DCC - Ash.otf");
			adjectief.SetTypeface (face, 0);

			UpdateScreen ();

			jaKnop.Click += (sender, eventArgs) => PushJa();
			neeKnop.Click += (sender, eventArgs) => PushNee();
		}

		//show next eigenschap
		public void UpdateScreen() {
			if(eigenschapCount < 359) {
				adjectief.Text = eigenschappen [eigenschapCount].name;
			} else {
				ResultList ();
			}
		}

		//redirect to the result activity
		//totems and their frequencies are sorted and passed seperately as parameters
		public void ResultList() {
			var totemsActivity = new Intent(this, typeof(TotemsActivity));

			int[] sortedTotems = DictMethods.GetSortedList (freqs, true);
			int[] sortedFreqs = DictMethods.GetSortedList (freqs, false);
			totemsActivity.PutExtra ("totemIDs", sortedTotems);
			totemsActivity.PutExtra ("freqs", sortedFreqs);

			StartActivity(totemsActivity);
		}

		//adds totem IDs related to the eigenschap to freqs
		//increases eigenschap count next
		public void PushJa() {
			List<Totem_eigenschap> toevoegen = db.GetTotemsVanEigenschapsID (eigenschappen[eigenschapCount].tid);
			foreach(Totem_eigenschap totem in toevoegen) {
				int idx = Convert.ToInt32 (totem.nid);
				DictMethods.AddOrUpdateDictionaryEntry (freqs, idx) ;
			}
			eigenschapCount++;
			UpdateScreen ();
		}

		//increases eigenschap count
		public void PushNee() {
			eigenschapCount++;
			UpdateScreen ();
		}
	}
}

