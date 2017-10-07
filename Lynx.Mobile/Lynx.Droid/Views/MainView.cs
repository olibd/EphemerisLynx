using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.Core;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Lynx.Core.Interactions;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for MainViewModel")]
    class MainView : MvxActivity
    {
        private IMvxInteraction<MnemonicCheckInteraction> _createButtons;
        private List<Button> verificationButtons = new List<Button>();

        public IMvxInteraction<MnemonicCheckInteraction> CreateButtons
        {
            get => _createButtons;
            set
            {
                if (_createButtons != null)
                    _createButtons.Requested -= CreateMnemonicVerificationButtons;

                _createButtons = value;
                _createButtons.Requested += CreateMnemonicVerificationButtons;
            }
        }

        private void CreateMnemonicVerificationButtons(object sender, MvxValueEventArgs<MnemonicCheckInteraction> e)
        {
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.mainView);

            //Clear out the buttons 
            if(verificationButtons.Count != 0)
                foreach(Button btn in verificationButtons)
                    layout.RemoveView(btn);

            if (e.Value.buttons == null) return;

            foreach (string word in e.Value.buttons)
            {
                Button btn = new Button(this);
                btn.Text = word;
                btn.Click += (o, a) => e.Value.onButtonClick(word);
                verificationButtons.Add(btn);
                layout.AddView(btn);
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainView);
            CreateBindings();
        }


        private void CreateBindings()
        {
            var set = this.CreateBindingSet<MainView, MainViewModel>();
            set.Bind(this).For(view => view.CreateButtons).To(viewModel => viewModel.CreateButtons).OneWay();
            set.Apply();
        }
    }
}