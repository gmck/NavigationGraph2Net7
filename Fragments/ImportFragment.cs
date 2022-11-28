using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.navigationgraph2net7.Fragments
{
    public  class ImportFragment : Fragment
    {
        private NavFragmentOnBackPressedCallback? onBackPressedCallback;
        public ImportFragment() { }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View? view = inflater.Inflate(Resource.Layout.fragment_import, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_import);             //null forgiving operator makes the expression not-null even if it was maybe-null without the ! applied.
            textView!.Text = "This is Import fragment";
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            onBackPressedCallback = new NavFragmentOnBackPressedCallback(this, true);
            //// Android docs:  Strongly recommended to use the ViewLifecycleOwner.This ensures that the OnBackPressedCallback is only added when the LifecycleOwner is Lifecycle.State.STARTED.
            //// The activity also removes registered callbacks when their associated LifecycleOwner is destroyed, which prevents memory leaks and makes it suitable for use in fragments or other lifecycle owners
            //// that have a shorter lifetime than the activity.
            //// Note: this rule out using OnAttach(Context context) as the view hasn't been created yet.
            RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback);
        }

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleBackPressed
        public void HandleBackPressed()
        {
            onBackPressedCallback!.Enabled = false;

            // Had to add this for Android 12 devices becausue MainActivity's OnDestroy wasn't being called.
            // and therefore our Service plus its Notification wasn't being closed.
            RequireActivity().Finish();
        }
        #endregion
    }
}