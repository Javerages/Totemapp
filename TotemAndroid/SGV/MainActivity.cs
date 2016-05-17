﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace TotemAndroid {
    [Activity (Label = "Totemapp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppThemeNoAction")]
	public class MainActivity : BaseActivity {
		//Database db;
		Button totems;
		Button eigenschappen;
		Button profielen;
		Button checklist;

        Toast mToast;
        View toastView;

        protected override void OnCreate (Bundle bundle) {
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			totems = FindViewById<Button> (Resource.Id.totems);
			eigenschappen = FindViewById<Button> (Resource.Id.eigenschappen);
			profielen = FindViewById<Button> (Resource.Id.profielen);
			checklist = FindViewById<Button> (Resource.Id.checklist);

			totems.Click += (sender, eventArgs) => _appController.TotemMenuItemClicked ();
			eigenschappen.Click += (sender, eventArgs) => _appController.EigenschappenMenuItemClicked ();
			profielen.Click += (sender, eventArgs) => _appController.ProfileMenuItemClicked ();
			checklist.Click += (sender, eventArgs) => _appController.ChecklistMenuItemClicked ();

            TextView title = FindViewById<TextView>(Resource.Id.totemapp_title);
            title.LongClick += ShowEasterEgg;

            ImageButton tip = FindViewById<ImageButton>(Resource.Id.tst);
            tip.Click += (sender, e) => ShowTipDialog();

            LayoutInflater mInflater = LayoutInflater.From(this);
            toastView = mInflater.Inflate(Resource.Layout.InfoToast, null);

            //smaller font size for smaller screens
            //otherwise UI issue
            var disp = WindowManager.DefaultDisplay;
            Point size = new Point();
            disp.GetSize(size);

            if (size.X <= 480) {
                title.TextSize = 60;
            }
        }

        protected override void OnResume ()	{
			base.OnResume ();

            _appController.NavigationController.GotoTotemListEvent += gotoTotemListHandler;
			_appController.NavigationController.GotoEigenschapListEvent += gotoEigenschappenListHandler;
			_appController.NavigationController.GotoProfileListEvent += gotoProfileListHandler;
			_appController.NavigationController.GotoChecklistEvent += gotoChecklistHandler;
		}

		protected override void OnPause () {
			base.OnPause ();
            toastView.Visibility = ViewStates.Gone;

            _appController.NavigationController.GotoTotemListEvent -= gotoTotemListHandler;
			_appController.NavigationController.GotoEigenschapListEvent -= gotoEigenschappenListHandler;
			_appController.NavigationController.GotoProfileListEvent -= gotoProfileListHandler;
			_appController.NavigationController.GotoChecklistEvent -= gotoChecklistHandler;
		}

        private void ShowEasterEgg(object sender, View.LongClickEventArgs e) {
            mToast = new Toast(this);
            mToast.Duration = ToastLength.Short;
            mToast.SetGravity(GravityFlags.Center | GravityFlags.Bottom, 0, ConvertDPToPixels(10));

            toastView.Visibility = ViewStates.Visible;
            mToast.View = toastView;

            //smaller font size for smaller screens
            //otherwise UI issue
            var disp = WindowManager.DefaultDisplay;
            Point size = new Point();
            disp.GetSize(size);

            if (size.X <= 480) {
                var t = mToast.View.FindViewById<TextView>(Resource.Id.info);
                t.TextSize = 10;
            }

            mToast.Show();
        }

        private int ConvertDPToPixels(float dp) {
            float scale = Resources.DisplayMetrics.Density;
            int result = (int)(dp * scale + 0.5f);
            return result;
        }

        void GoToActivity(string activity) {
			Intent intent = null;
			switch (activity) {
			case "totems":
				intent = new Intent (this, typeof(TotemsActivity));
				break;
			case "eigenschappen":
				intent = new Intent (this, typeof(EigenschappenActivity));
				break;
			case "profielen":
				intent = new Intent (this, typeof(ProfielenActivity));
				break;
			case "checklist":
				intent = new Intent (this, typeof(TotemisatieChecklistActivity));
				break;
			}
			StartActivity (intent);
		}

		public void ShowTipDialog() {
			var dialog = TipDialog.NewInstance(this);
			RunOnUiThread (() => dialog.Show (FragmentManager, "dialog"));
		}

		public override void OnBackPressed() {
			var StartMain = new Intent (Intent.ActionMain);
			StartMain.AddCategory (Intent.CategoryHome);
			StartMain.SetFlags (ActivityFlags.NewTask);
			StartActivity (StartMain);
		}

		void gotoTotemListHandler () {
			GoToActivity ("totems");
		}

		void gotoEigenschappenListHandler () {
			GoToActivity ("eigenschappen");
		}

		void gotoProfileListHandler () {
			GoToActivity ("profielen");
		}

		void gotoChecklistHandler () {
			GoToActivity ("checklist");
		}
	}
}