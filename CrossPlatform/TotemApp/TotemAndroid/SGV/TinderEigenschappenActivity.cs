﻿using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using TotemAppCore;

namespace TotemAndroid {
	[Activity (Label = "Totem bepalen", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class TinderEigenschappenActivity : BaseActivity {
		TextView adjectief;
		List<Eigenschap> eigenschappen;
		int eigenschapCount = 1;

		protected override void OnCreate (Bundle bundle) {
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Eigenschappen);

			//Action bar
			InitializeActionBar (ActionBar);

			ActionBarTitle.Text = "Eigenschappen";
	
			eigenschappen = _appController.Eigenschappen;

			Button jaKnop = FindViewById<Button> (Resource.Id.jaKnop);
			Button neeKnop = FindViewById<Button> (Resource.Id.neeKnop);

			adjectief = FindViewById<TextView> (Resource.Id.eigenschap);

			UpdateScreen ();

			jaKnop.Click += (sender, eventArgs) => PushJa();
			neeKnop.Click += (sender, eventArgs) => PushNee();
		}

		protected override void OnResume ()	{
			base.OnResume ();

			_appController.NavigationController.GotoTotemResultEvent+= StartResultTotemsActivity;
		}

		protected override void OnPause ()	{
			base.OnPause ();

			_appController.NavigationController.GotoTotemResultEvent-= StartResultTotemsActivity;
		}

		//show next eigenschap
		public void UpdateScreen() {
			if(eigenschapCount < 324)
				adjectief.Text = eigenschappen [eigenschapCount].name;
			else
				VindTotem ();
		}

		//redirect to the result activity
		//totems and their frequencies are sorted and passed seperately as parameters
		public void VindTotem() {
			_appController.CalculateResultlist(eigenschappen);
		}

		void StartResultTotemsActivity() {
			var totemsActivity = new Intent (this, typeof(ResultTotemsActivity));
			totemsActivity.PutExtra ("GoToMain", true);
			StartActivity (totemsActivity);
		}

		//adds totem IDs related to the eigenschap to freqs
		//increases eigenschap count next
		public void PushJa() {
			eigenschappen [eigenschapCount].selected = true;
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