using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using AndroidX.Preference;
using Google.Android.Material.Navigation;
using System.Runtime.Versioning;

namespace com.companyname.navigationgraph2net7
{
    [Activity(Label = "@string/app_name", /*Theme = "@style/AppTheme.NoActionBar",*/ MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavController.IOnDestinationChangedListener, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppBarConfiguration? appBarConfiguration;
        private NavController? navController;
        private DrawerLayout? drawerLayout;
        private NavigationView? navigationView;
        private bool animateFragments;

        #region OnCreate
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            Toolbar? toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // navigationView for NavigationUI and drawerLayout for the AppBarConfiguration and NavigationUI
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            // The new stuff
            // NavHostFragment so we can get a NavController 
            NavHostFragment? navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
            navController = navHostFragment!.NavController;

            // Top Level Destinations - for the NavigationView
            int[] topLevelDestinationIds = new int[] { Resource.Id.import_fragment, Resource.Id.gallery_fragment, Resource.Id.slideshow_fragment, Resource.Id.tools_fragment };
            appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).SetOpenableLayout(drawerLayout).Build();

            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);

            //NavigationUI.SetupWithNavController(navigationView, navController); // Commented out compared to Navigation1
            // SetNavigationItemSelectedListener replaces it, so now we are responsible for the code in OnNavigationItemSelected
            navigationView!.SetNavigationItemSelectedListener(this);

            // Add the DestinationChanged listener
            navController.AddOnDestinationChangedListener(this);
        }
        #endregion

        #region OnSupportNavigationUp
        public override bool OnSupportNavigateUp()
        {
            return NavigationUI.NavigateUp(navController, appBarConfiguration) || base.OnSupportNavigateUp();
        }
        #endregion

        #region OnDestinationChanged
        //[SupportedOSPlatformAttribute("android28.0")]
        //public void OnDestinationChanged(NavController navController, NavDestination navDestination, Bundle bundle)
        //{
        //    CheckForPreferenceChanges();

        //    // The first menu item is not checked by default, so we need to check it to show it is selected on the startDestination fragment
        //    navigationView!.Menu.FindItem(Resource.Id.import_fragment)!.SetChecked(navDestination.Id == Resource.Id.import_fragment);

        //    // This nullable stuff can be a pain - Long way around to figure out the nullable stuff for the above line
        //    //IMenu menu = navigationView!.Menu;
        //    //IMenuItem? menuItem = menu.FindItem(Resource.Id.import_fragment);
        //    //menuItem!.SetChecked(navDestination.Id == Resource.Id.import_fragment);


        //    // Required  LayoutInDisplayCutoutMode.Default to correct distorted view in landscape after including the Splash Screen API - See docs for an image of what the landscape view looked like.
        //    // Remove all the Splash Screen API stuff reverting back to original in NavigationGraph2 (which didn't include Xamarin.AndroidX.Core.SplashScreen because it wasn't available at that time) restores the correct behaviour.

        //    // Include this if using the Splash Screen API
        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
        //    {
        //        // Don't understand why we get this warning - when we are already checking for >= BuildVersionCodes.P. uncomment the pragma lines to see the SupportedOSPlatformVersion warning.
                
        //        //#pragma warning disable CA1416 // Validate platform compatibility
        //        Window!.Attributes!.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
        //        //#pragma warning restore CA1416 // Validate platform compatibility
        //    }

        //}
        #endregion


        #region OnDestinationChanged
        [SupportedOSPlatformAttribute("android28.0")]
        public void OnDestinationChanged(NavController navController, NavDestination navDestination, Bundle bundle)
        {
            CheckForPreferenceChanges();

            // The first menu item is not checked by default, so we need to check it to show it is selected on the startDestination fragment
            navigationView!.Menu.FindItem(Resource.Id.import_fragment)!.SetChecked(navDestination.Id == Resource.Id.import_fragment);

            // Required  LayoutInDisplayCutoutMode.Default to correct distorted view in landscape after including the Splash Screen API - See docs for an image of what the landscape view looked like.
            // Remove all the Splash Screen API stuff reverting back to original in NavigationGraph2 (which didn't include Xamarin.AndroidX.Core.SplashScreen because it wasn't available at that time) restores the correct behaviour.

            // Include this if using the Splash Screen API
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                Window!.Attributes!.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
        }
        #endregion


        #region OnCreateOptionsMenu
        public override bool OnCreateOptionsMenu(IMenu? menu)
        {
            base.OnCreateOptionsMenu(menu);
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return true;
        }
        #endregion

        #region OnOptionsItemSelected
        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.action_settings:
                    navController!.Navigate(Resource.Id.settings_fragment);
                    break;
            }
            return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(this, Resource.Id.nav_host)) || base.OnOptionsItemSelected(menuItem);
        }
        #endregion

        #region OnNavigationItemSelected
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            // Using Fader2 as the default as animateFragment is false by default - check AnimationResource.cs for different animations
            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();

            bool proceed = false;

            switch (menuItem.ItemId)
            {
                // These are all topLevel fragments
                // Add fragment classes and fragment layouts as we add to the codebase as per the NavigationView items. 
                // If any classes and layouts are missing, then the NavigationView will not update the item selected.
                // The menuitem highlight will stay on the current item and the current fragment will remain displayed, nor will the app crash.
                case Resource.Id.import_fragment:
                case Resource.Id.gallery_fragment:
                case Resource.Id.slideshow_fragment:
                case Resource.Id.tools_fragment:
                    proceed = true;
                    break;

                default:
                    break;
            }
            // We have the option here of animating our toplevel destinations. If we don't want animation comment out the NavOptions or just rely on NavigationUI.OnNavDestinationSelected.
            bool handled = false;
            if (proceed)
            {
                navController!.Navigate(menuItem.ItemId, null, navOptions);
                handled = true;
            }

            if (!handled)
                //Default behavior as if we didn't use OnNavigationItemSelected
                handled = NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(this, Resource.Id.nav_host)) || base.OnOptionsItemSelected(menuItem);

            if (drawerLayout!.IsDrawerOpen(GravityCompat.Start))
                drawerLayout.CloseDrawer(GravityCompat.Start);

            return handled;
        }
        #endregion

        #region CheckForPreferenceChanges
        private void CheckForPreferenceChanges()
        {
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            animateFragments = sharedPreferences.GetBoolean("use_animations", false);
        }
        #endregion
    }
}

