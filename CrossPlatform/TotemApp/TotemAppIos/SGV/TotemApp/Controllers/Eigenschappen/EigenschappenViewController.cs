﻿using System;

using UIKit;

using TotemAppCore;
using CoreGraphics;
using System.Threading;

namespace TotemAppIos {
	public partial class EigenschappenViewController : UIViewController	{
		#region delegates

		#endregion

		#region variables
		AppController _appController = AppController.Instance;
		bool isSearching;
		bool isShowingSelected;
		#endregion

		#region constructor
		public EigenschappenViewController () : base ("EigenschappenViewController", null) {}
		#endregion

		#region properties

		#endregion

		#region public methods

		#region overrided methods
		public override void DidReceiveMemoryWarning () {
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override UIStatusBarStyle PreferredStatusBarStyle () {
			return UIStatusBarStyle.LightContent;
		}

		#region viewlifecycle
		public override void ViewDidLoad () {
			base.ViewDidLoad ();
			setData ();
			NavigationController.NavigationBarHidden = true;
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewDidAppear (bool animated) {
			base.ViewDidAppear (animated);
			btnReturn.TouchUpInside+= btnReturnTouchUpInside;
			btnMore.TouchUpInside+= btnMoreTouchUpInside;
			btnSearch.TouchUpInside+= btnSearchTouchUpInside;
			txtSearch.EditingChanged+= TxtSearchValueChangedHandler;
			btnVind.TouchUpInside += btnVindTouchUpInside;

			_appController.UpdateCounter += updateCounter;
			_appController.NavigationController.GotoTotemResultEvent += gotoResultListHandler;
		}

		public override void ViewWillDisappear (bool animated) {
			base.ViewWillDisappear (animated);
			btnReturn.TouchUpInside-= btnReturnTouchUpInside;
			btnMore.TouchUpInside-= btnMoreTouchUpInside;
			btnSearch.TouchUpInside -= btnSearchTouchUpInside;
			txtSearch.EditingChanged -= TxtSearchValueChangedHandler;
			btnVind.TouchUpInside -= btnVindTouchUpInside;

			_appController.UpdateCounter -= updateCounter;
			_appController.NavigationController.GotoTotemResultEvent -= gotoResultListHandler;
		}
		#endregion

		#endregion

		#endregion

		#region private methods

		private void setData() {
			lblTitle.Text = "Eigenschappen";

			imgReturn.Image = UIImage.FromBundle ("SharedAssets/arrow_back_white");
			imgSearch.Image = UIImage.FromBundle ("SharedAssets/search_white");
			imgMore.Image = UIImage.FromBundle ("SharedAssets/more_vert_white");
			imgVind.Image = UIImage.FromBundle ("SharedAssets/arrow_forward_white");

			bottomBarHeight.Constant = 0;

			txtSearch.Hidden=true;
			txtSearch.TintColor = UIColor.White;
			txtSearch.ReturnKeyType = UIReturnKeyType.Search;
			txtSearch.ShouldReturn = ((UITextField textfield) => {
				textfield.ResignFirstResponder ();
				return true;
			});
			UIColor color = UIColor.White;
			txtSearch.AttributedPlaceholder = new Foundation.NSAttributedString("Zoek eigenschap",foregroundColor: color);
			tblEigenschappen.Source = new EigenschappenTableViewSource (_appController.Eigenschappen);
		}

		void btnReturnTouchUpInside (object sender, EventArgs e) {
			resetSelections ();
			NavigationController.PopViewController (true);
		}

		void btnMoreTouchUpInside (object sender, EventArgs e) {
			// Create a new Alert Controller
			UIAlertController actionSheetAlert = UIAlertController.Create(null,null,UIAlertControllerStyle.ActionSheet);

			// Add Actions
			actionSheetAlert.AddAction(UIAlertAction.Create("Reset selectie",UIAlertActionStyle.Default, (action) => resetSelections ()));

			actionSheetAlert.AddAction(UIAlertAction.Create(isShowingSelected?"Toon volledige lijst":"Toon enkel selectie",UIAlertActionStyle.Default, (action) => toggleShowSelected()));

			actionSheetAlert.AddAction(UIAlertAction.Create("Annuleer",UIAlertActionStyle.Cancel, null));

			// Required for iPad - You must specify a source for the Action Sheet since it is
			// displayed as a popover
			UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
			if (presentationPopover!=null) {
				presentationPopover.SourceView = this.View;
				presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}

			// Display the alert
			this.PresentViewController(actionSheetAlert,true,null);
		}

		void btnSearchTouchUpInside (object sender, EventArgs e) {
			if (isSearching) {
				txtSearch.Hidden = true;
				btnReturn.Hidden = false;
				lblTitle.Hidden = false;
				txtSearch.Text = "";
				TxtSearchValueChangedHandler (txtSearch,null);
				txtSearch.ResignFirstResponder ();
				imgSearch.Image = UIImage.FromBundle ("SharedAssets/search_white");
			} else {
				txtSearch.Hidden = false;
				btnReturn.Hidden = true;
				lblTitle.Hidden = true;
				imgSearch.Image = UIImage.FromBundle ("SharedAssets/close_white");
				txtSearch.BecomeFirstResponder ();
			}
			isSearching = !isSearching;
		}

		void TxtSearchValueChangedHandler (object sender, EventArgs e) {
			(tblEigenschappen.Source as EigenschappenTableViewSource).Eigenschappen = _appController.FindEigenschapOpNaam ((sender as UITextField).Text);
			tblEigenschappen.ReloadSections (new Foundation.NSIndexSet (0), UITableViewRowAnimation.Automatic);
			tblEigenschappen.ScrollRectToVisible (new CGRect(0,0,1,1), false);
			isShowingSelected = false;
		}

		void gotoResultListHandler () {
			NavigationController.PushViewController (new TotemsResultViewController(),true);
		}

		void resetSelections() {
			foreach (var eigenschap in _appController.Eigenschappen) 
				eigenschap.selected = false;
			
			txtSearch.Text = "";
			TxtSearchValueChangedHandler (txtSearch,null);
			txtSearch.ResignFirstResponder ();
			tblEigenschappen.ScrollRectToVisible (new CGRect(0,0,1,1), true);
			isShowingSelected = false;
			_appController.FireUpdateEvent ();
		}

		void toggleShowSelected() {
			if (isShowingSelected) {
				(tblEigenschappen.Source as EigenschappenTableViewSource).Eigenschappen = _appController.FindEigenschapOpNaam (txtSearch.Text);
				tblEigenschappen.ReloadSections (new Foundation.NSIndexSet (0), UITableViewRowAnimation.Automatic);
				tblEigenschappen.ScrollRectToVisible (new CGRect(0,0,1,1), false);
				isShowingSelected = !isShowingSelected;
			} else if ((_appController.FindEigenschapOpNaam (txtSearch.Text).FindAll (x=>x.selected)).Count != 0) {
				(tblEigenschappen.Source as EigenschappenTableViewSource).Eigenschappen = _appController.FindEigenschapOpNaam (txtSearch.Text).FindAll (x => x.selected);
				tblEigenschappen.ReloadSections (new Foundation.NSIndexSet (0), UITableViewRowAnimation.Automatic);
				isShowingSelected = !isShowingSelected;
				tblEigenschappen.ScrollRectToVisible (new CGRect(0,0,1,1), false);
			}
		}

		void updateCounter() {
			int count = _appController.Eigenschappen.FindAll (x => x.selected).Count;
			lblNumberSelected.Text = count + " geselecteerd";
			if(count > 0)
				bottomBarHeight.Constant = 50;
			else
				bottomBarHeight.Constant = 0;
		}

		void btnVindTouchUpInside (object sender, EventArgs e) {
			_appController.CalculateResultlist(_appController.Eigenschappen);
		}
		#endregion
	}
}